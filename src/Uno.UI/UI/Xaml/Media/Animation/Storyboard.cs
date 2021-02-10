﻿using Uno.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml.Data;
using Uno.Diagnostics.Eventing;
using Windows.UI.Xaml.Markup;
using System.Threading;
using Windows.UI.Core;
using Uno.Disposables;

namespace Windows.UI.Xaml.Media.Animation
{
	[ContentProperty(Name = "Children")]
	public sealed partial class Storyboard : Timeline, ITimeline, IAdditionalChildrenProvider, IThemeChangeAware
	{
		private static readonly IEventProvider _trace = Tracing.Get(TraceProvider.Id);
		private EventActivity _traceActivity;

		public static class TraceProvider
		{
			public static readonly Guid Id = Guid.Parse("{57A7F5D4-8AA9-453F-A2D5-9F9DCA48BF54}");

			public const int StoryBoard_Start = 1;
			public const int StoryBoard_Stop = 2;
			public const int StoryBoard_Pause = 3;
			public const int StoryBoard_Resume = 4;
		}

		private DateTimeOffset _lastBeginTime;
		private int _replayCount = 1;
		private int _runningChildren = 0;
		private bool _hasFillingChildren = false;
		private Dictionary<ITimeline, IDisposable> _childrenSubscriptions = new Dictionary<ITimeline, IDisposable>();

		public Storyboard()
		{
			Children = new TimelineCollection(owner: this, isAutoPropertyInheritanceEnabled: false);
		}

		public TimelineCollection Children { get; }

		#region TargetName Attached Property
		public static string GetTargetName(Timeline timeline) => (string)timeline.GetValue(TargetNameProperty);

		public static void SetTargetName(Timeline timeline, string value) => timeline.SetValue(TargetNameProperty, value);

		// Using a DependencyProperty as the backing store for TargetName.  This enables animation, styling, binding, etc...
		public static DependencyProperty TargetNameProperty { get ; } =
			DependencyProperty.RegisterAttached("TargetName", typeof(string), typeof(Storyboard), new FrameworkPropertyMetadata(null));
		#endregion

		#region TargetProperty Attached Property
		public static string GetTargetProperty(Timeline timeline) => (string)timeline.GetValue(TargetPropertyProperty);

		public static void SetTargetProperty(Timeline timeline, string value) => timeline.SetValue(TargetPropertyProperty, value);

		// Using a DependencyProperty as the backing store for TargetProperty.  This enables animation, styling, binding, etc...
		public static DependencyProperty TargetPropertyProperty { get ; } =
			DependencyProperty.RegisterAttached("TargetProperty", typeof(string), typeof(Storyboard), new FrameworkPropertyMetadata(null));
		#endregion

		public static void SetTarget(Timeline timeline, DependencyObject target) => timeline.Target = target;

		/// <summary>
		/// Explicitly sets the target using an ElementNameSubject, in case of lazy 
		/// evaluation of the target element.
		/// </summary>
		public static void SetTarget(Timeline timeline, ElementNameSubject target) => timeline.SetElementNameTarget(target);

		/// <summary>
		/// Disposes event subscriptions for an <see cref="ITimeline"/>
		/// </summary>
		/// <param name="child"></param>
		private void DisposeChildRegistrations(ITimeline child)
		{
			if(_childrenSubscriptions.TryGetValue(child, out var disposable))
			{
				disposable.Dispose();
				_childrenSubscriptions.Remove(child);
			}
		}

		/// <summary>
		/// Replay this animation.
		/// </summary>
		private void Replay()
		{
			_replayCount++;

			Play();
		}

		public void Begin()
		{
			if (_trace.IsEnabled)
			{
				_traceActivity = _trace.WriteEventActivity(
					TraceProvider.StoryBoard_Start,
					EventOpcode.Start,
					payload: new[] {
						this.GetParent()?.GetType().Name,
						this.GetParent()?.GetDependencyObjectId().ToString(),
					}
				);
			}

			State = TimelineState.Active;
			_hasFillingChildren = false;
			_replayCount = 1;
			_lastBeginTime = DateTimeOffset.Now;

			Play();
		}

		private void Play()
		{
			if (Children != null && Children.Count > 0)
			{
				foreach (ITimeline child in Children)
				{
					DisposeChildRegistrations(child);

					_runningChildren++;
					child.Completed += Child_Completed;
					child.Failed += Child_Failed;

					_childrenSubscriptions.Add(
						child,
						Disposable.Create(() => {
							child.Completed -= Child_Completed;
							child.Failed -= Child_Failed;
						})
					);

					child.Begin();
				}
			}
			else
			{
				Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					// No children, so we complete immediately
					State = TimelineState.Stopped;
					OnCompleted();
				});
			}
		}

		public void Stop()
		{
			if (_trace.IsEnabled)
			{
				_trace.WriteEventActivity(
					eventId: TraceProvider.StoryBoard_Stop,
					opCode: EventOpcode.Stop, 
					activity: _traceActivity,
					payload: new object[] { Target?.GetType().ToString(), PropertyInfo?.Path }
				);
			}

			State = TimelineState.Stopped;
			_hasFillingChildren = false;

			if (Children != null)
			{
				foreach (ITimeline child in Children)
				{
					child.Stop();
					DisposeChildRegistrations(child);
				}
			}
			_runningChildren = 0;
		}

		public void Resume()
		{
			if (_trace.IsEnabled)
			{
				_trace.WriteEventActivity(
					eventId: TraceProvider.StoryBoard_Resume,
					opCode: EventOpcode.Send,
					activity: _traceActivity,
					payload: new object[] { Target?.GetType().ToString(), PropertyInfo?.Path }
				);
			}

			if (Children != null && Children.Count > 0)
			{
				State = TimelineState.Active;

				foreach (ITimeline child in Children)
				{
					child.Resume();
				}
			}
		}

		public void Pause()
		{
			if (_trace.IsEnabled)
			{
				_trace.WriteEventActivity(
					eventId: TraceProvider.StoryBoard_Pause,
					opCode: EventOpcode.Send,
					activity: _traceActivity,
					payload: new object[] { Target?.GetType().ToString(), PropertyInfo?.Path }
				);
			}

			State = TimelineState.Paused;

			if (Children != null)
			{
				foreach (ITimeline child in Children)
				{
					child.Pause();
				}
			}
		}

		public void Seek(TimeSpan offset)
		{
			if (Children != null)
			{
				foreach (ITimeline child in Children)
				{
					child.Seek(offset);
				}
			}
		}

		public void SeekAlignedToLastTick(TimeSpan offset)
		{
			if (Children != null)
			{
				foreach (ITimeline child in Children)
				{
					child.SeekAlignedToLastTick(offset);
				}
			}
		}
		public void SkipToFill()
		{
			if (Children != null)
			{
				foreach (ITimeline child in Children)
				{
					child.SkipToFill();
				}
			}
		}

		internal void Deactivate()
		{
			State = TimelineState.Stopped;
			_hasFillingChildren = false;

			if (Children != null)
			{
				foreach (ITimeline child in Children)
				{
					child.Deactivate();
					DisposeChildRegistrations(child);
				}

				_runningChildren = 0;
			}
		}

		internal void TurnOverAnimationsTo(Storyboard storyboard)
		{
			var affectedProperties = storyboard.Children.TargetedProperties;

			foreach (var child in Children)
			{
				var id = child.GetTimelineTargetFullName();

				if (affectedProperties.Contains(id))
				{
					((ITimeline)child).Deactivate();
				}
				else
				{
					((ITimeline)child).Stop();
				}
			}
			State = TimelineState.Stopped;
		}

		public ClockState GetCurrentState()
		{
			switch (State)
			{
				case TimelineState.Filling:
					return ClockState.Filling;
				case TimelineState.Stopped:
					return ClockState.Stopped;
				default:
				case TimelineState.Active:
				case TimelineState.Paused:
					return ClockState.Active;
			}
		}

		public TimeSpan GetCurrentTime()
		{
			throw new NotImplementedException();
		}

		private void Child_Failed(object sender, object e)
		{
			if (!(sender is Timeline child))
			{
				return;
			}

			DisposeChildRegistrations(child);

			Interlocked.Decrement(ref _runningChildren);

			// OnFailed is not called here because it relates to an individual
			// child, where completed relates to all children being completed.
		}

		private void Child_Completed(object sender, object e)
		{
			if (!(sender is Timeline child))
			{
				return;
			}

			DisposeChildRegistrations(child);

			Interlocked.Decrement(ref _runningChildren);
			_hasFillingChildren |= (child.FillBehavior != FillBehavior.Stop);

			if (_runningChildren == 0)
			{
				if (NeedsRepeat(_lastBeginTime, _replayCount))
				{
					Replay(); // replay the animation
					return;
				}
				if (State == TimelineState.Active)
				{
					State = _hasFillingChildren ? TimelineState.Filling : TimelineState.Stopped;
				}

				OnCompleted();
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (Children != null)
			{
				foreach (var child in Children)
				{
					child.Dispose();
				}
			}
		}

		IEnumerable<DependencyObject> IAdditionalChildrenProvider.GetAdditionalChildObjects() => Children;

		void IThemeChangeAware.OnThemeChanged()
		{
			if (State == TimelineState.Filling)
			{
				// If we're filling, reapply the fill to reapply values that may have changed with the app theme
				SkipToFill();
			}
		}
	}
}
