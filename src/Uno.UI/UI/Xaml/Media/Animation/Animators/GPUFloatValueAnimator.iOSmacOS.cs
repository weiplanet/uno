﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Windows.Foundation;
using Uno.UI.DataBinding;
#if __IOS__
using UIKit;
using _View = UIKit.UIView;
#else
using AppKit;
using _View = AppKit.NSView;
#endif
using CoreGraphics;
using Foundation;
using CoreAnimation;
using Uno.Extensions;
using Uno.Logging;
using Windows.UI.Composition;
using Uno.UI;
using Uno.UI.Extensions;

namespace Windows.UI.Xaml.Media.Animation
{
	/// <summary>
	/// Animates a float property using a native <see cref="CoreAnimation"/>.
	/// </summary>
	internal class GPUFloatValueAnimator : IValueAnimator
	{
		private static readonly string __notSupportedProperty = "This transform is not supported by GPU enabled animations.";

		private float _to;
		private float _from;
		private long _duration;
		private IEnumerable<IBindingItem> _bindingPath;
		private FloatValueAnimator _valueAnimator;
		private UnoCoreAnimation _coreAnimation;
		private IEasingFunction _easingFunction;
		private bool _isDisposed;

#region PropertyNameConstants
		private const string TranslateTransformX = "TranslateTransform.X";
		private const string TranslateTransformXWithNamespace = "Windows.UI.Xaml.Media:TranslateTransform.X";
		private const string TranslateTransformY = "TranslateTransform.Y";
		private const string TranslateTransformYWithNamespace = "Windows.UI.Xaml.Media:TranslateTransform.Y";
		private const string RotateTransformAngle = "RotateTransform.Angle";
		private const string RotateTransformAngleWithNamespace = "Windows.UI.Xaml.Media:RotateTransform.Angle";
		private const string ScaleTransformX = "ScaleTransform.ScaleX";
		private const string ScaleTransformXWithNamespace = "Windows.UI.Xaml.Media:ScaleTransform.ScaleX";
		private const string ScaleTransformY = "ScaleTransform.ScaleY";
		private const string ScaleTransformYWithNamespace = "Windows.UI.Xaml.Media:ScaleTransform.ScaleY";
		private const string SkewTransformAngleX = "SkewTransform.AngleX";
		private const string SkewTransformAngleXWithNamespace = "Windows.UI.Xaml.Media:SkewTransform.AngleX";
		private const string SkewTransformAngleY = "SkewTransform.AngleY";
		private const string SkewTransformAngleYWithNamespace = "Windows.UI.Xaml.Media:SkewTransform.AngleY";
		private const string CompositeTransformCenterX = "CompositeTransform.CenterX";
		private const string CompositeTransformCenterXWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.CenterX";
		private const string CompositeTransformCenterY = "CompositeTransform.CenterY";
		private const string CompositeTransformCenterYWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.CenterY";
		private const string CompositeTransformTranslateX = "CompositeTransform.TranslateX";
		private const string CompositeTransformTranslateXWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.TranslateX";
		private const string CompositeTransformTranslateY = "CompositeTransform.TranslateY";
		private const string CompositeTransformTranslateYWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.TranslateY";
		private const string CompositeTransformRotation = "CompositeTransform.Rotation";
		private const string CompositeTransformRotationWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.Rotation";
		private const string CompositeTransformScaleX = "CompositeTransform.ScaleX";
		private const string CompositeTransformScaleXWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.ScaleX";
		private const string CompositeTransformScaleY = "CompositeTransform.ScaleY";
		private const string CompositeTransformScaleYWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.ScaleY";
		private const string CompositeTransformSkewX = "CompositeTransform.SkewX";
		private const string CompositeTransformSkewXWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.SkewX";
		private const string CompositeTransformSkewY = "CompositeTransform.SkewY";
		private const string CompositeTransformSkewYWithNamespace = "Windows.UI.Xaml.Media:CompositeTransform.SkewY";
#endregion

		internal static Point GetAnchorForAnimation(Transform transform, Point relativeOrigin, Size viewSize)
		{
			switch (transform)
			{
				case RotateTransform rotate:
					return GetAnimationAnchor(relativeOrigin, viewSize, rotate.CenterX, rotate.CenterY);

				case ScaleTransform scale:
					return GetAnimationAnchor(relativeOrigin, viewSize, scale.CenterX, scale.CenterY);

				case CompositeTransform composite:
					return GetAnimationAnchor(relativeOrigin, viewSize, composite.CenterX, composite.CenterY);

				default:
					return relativeOrigin;
			}
		}

		private static Point GetAnimationAnchor(Point origin, Size size, double centerX, double centerY)
			=> new Point(
				size.Width == 0
					? origin.X
					: centerX / size.Width + origin.X,
				size.Height == 0
					? origin.Y
					: centerY / size.Height + origin.Y);

		public GPUFloatValueAnimator(float from, float to, IEnumerable<IBindingItem> bindingPath)
		{
			_to = to;
			_from = from;
			_bindingPath = bindingPath;

			_valueAnimator = new FloatValueAnimator(from, to);
			_valueAnimator.Update += OnInnerAnimatorUpdate;

			_coreAnimation = null;
		}

		private void OnInnerAnimatorUpdate(object sender, EventArgs e)
		{
			Update?.Invoke(this, e);
		}

		public void Start()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			InitializeCoreAnimation();

			if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
			{
				this.Log().DebugFormat("Starting GPU Float value animator on property {0}.", _bindingPath.LastOrDefault().PropertyName);
			}

			_valueAnimator?.Start();
			_coreAnimation?.Start();
		}

		private void InitializeCoreAnimation()
		{
			var animatedItem = _bindingPath.LastOrDefault();
			switch (animatedItem.DataContext)
			{
				case _View view when animatedItem.PropertyName.EndsWith("Opacity"):
					_coreAnimation = InitializeOpacityCoreAnimation(view);
					return;

				case TranslateTransform translate:
					_coreAnimation = InitializeTranslateCoreAnimation(translate, animatedItem);
					return;

				case RotateTransform rotate:
					_coreAnimation = InitializeRotateCoreAnimation(rotate, animatedItem);
					return;

				case ScaleTransform scale:
					_coreAnimation = InitializeScaleCoreAnimation(scale, animatedItem);
					return;

				case SkewTransform skew:
					_coreAnimation = InitializeSkewCoreAnimation(skew, animatedItem);
					return;

				case CompositeTransform composite:
					_coreAnimation = InitializeCompositeCoreAnimation(composite, animatedItem);
					return;

				// case TransformGroup group:
				//  ==> No needs to validate the TransformGroup: there is no animatable property on it.
				//		If a anmiation is declared on it (e.g. "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"),
				//		the _bindingPath should resolve the target child Transform, and animatedItem.DataContext should be the ScaleTransform.


				default:
					throw new NotSupportedException(__notSupportedProperty);
			}
		}

		public void Pause()
		{
			var pausedTime = _valueAnimator?.CurrentPlayTime;
			var pausedValue = (float?)_valueAnimator?.AnimatedValue;

			_valueAnimator?.Pause();
			_coreAnimation?.Pause(pausedTime, pausedValue);

			AnimationPause?.Invoke(this, EventArgs.Empty);
		}

		public void Resume()
		{
			_valueAnimator?.Resume();
			_coreAnimation?.Resume();
		}

		public void Cancel()
		{
			_valueAnimator?.Cancel();
			_coreAnimation?.Cancel();
			AnimationCancel?.Invoke(this, EventArgs.Empty);

			ReleaseCoreAnimation();
		}

		public bool IsRunning => _valueAnimator.IsRunning;

		public long StartDelay
		{
			get => _valueAnimator.StartDelay;

			set => _valueAnimator.StartDelay = value;
		}

		public object AnimatedValue => _valueAnimator.AnimatedValue;

		public long CurrentPlayTime
		{
			get => _valueAnimator.CurrentPlayTime;
			set => _valueAnimator.CurrentPlayTime = value;
		}

		public long Duration => _duration;

		public event EventHandler Update;

		public event EventHandler AnimationPause;

		public event EventHandler AnimationEnd;

		public event EventHandler AnimationCancel;

		public event EventHandler AnimationFailed;

		public void SetDuration(long duration)
		{
			_duration = duration;
			_valueAnimator.SetDuration(duration);
		}

		public void SetEasingFunction(IEasingFunction easingFunction)
		{
			_easingFunction = easingFunction;
			_valueAnimator.SetEasingFunction(easingFunction);
		}

#region coreAnimationInitializers
		private UnoCoreAnimation InitializeOpacityCoreAnimation(_View view)
		{
			return CreateCoreAnimation(view, "opacity", value => new NSNumber(value));
		}
		private UnoCoreAnimation InitializeTranslateCoreAnimation(TranslateTransform transform, IBindingItem animatedItem)
		{
			if (animatedItem.PropertyName.Equals("X")
				|| animatedItem.PropertyName.Equals(TranslateTransformX)
				|| animatedItem.PropertyName.Equals(TranslateTransformXWithNamespace))
			{
				return CreateCoreAnimation(transform, "transform.translation.x", value => new NSNumber(value));
			}
			else if (animatedItem.PropertyName.Equals("Y")
				|| animatedItem.PropertyName.Equals(TranslateTransformY)
				|| animatedItem.PropertyName.Equals(TranslateTransformYWithNamespace))
			{
				return CreateCoreAnimation(transform, "transform.translation.y", value => new NSNumber(value));
			}
			else
			{
				throw new NotSupportedException(__notSupportedProperty);
			}
		}

		private UnoCoreAnimation InitializeRotateCoreAnimation(RotateTransform transform, IBindingItem animatedItem)
		{
			if (animatedItem.PropertyName.Equals("Angle")
				|| animatedItem.PropertyName.Equals(RotateTransformAngle)
				|| animatedItem.PropertyName.Equals(RotateTransformAngleWithNamespace))
			{
				return CreateCoreAnimation(transform, "transform.rotation", value => new NSNumber(MathEx.ToRadians(value)));
			}
			else
			{
				throw new NotSupportedException(__notSupportedProperty);
			}
		}

		private UnoCoreAnimation InitializeScaleCoreAnimation(ScaleTransform transform, IBindingItem animatedItem)
		{
			if (animatedItem.PropertyName.Equals("ScaleX")
				|| animatedItem.PropertyName.Equals(ScaleTransformX)
				|| animatedItem.PropertyName.Equals(ScaleTransformXWithNamespace))
			{
				return CreateCoreAnimation(transform, "transform.scale.x", value => new NSNumber(value));
			}
			else if (animatedItem.PropertyName.Equals("ScaleY")
				|| animatedItem.PropertyName.Equals(ScaleTransformY)
				|| animatedItem.PropertyName.Equals(ScaleTransformYWithNamespace))
			{
				return CreateCoreAnimation(transform, "transform.scale.y", value => new NSNumber(value));
			}
			else
			{
				throw new NotSupportedException(__notSupportedProperty);
			}
		}

		private UnoCoreAnimation InitializeSkewCoreAnimation(SkewTransform transform, IBindingItem animatedItem)
		{
			// We need to review this.  This won't play along if other transforms are happening at the same time since we are animating the whole transform
			_View view = transform.View;

			if (animatedItem.PropertyName.Equals("AngleX")
				|| animatedItem.PropertyName.Equals(SkewTransformAngleX)
				|| animatedItem.PropertyName.Equals(SkewTransformAngleXWithNamespace))
			{
				return CreateCoreAnimation(view, "transform", value => ToCASkewTransform(value, 0));
			}
			else if (animatedItem.PropertyName.Equals("AngleY")
				|| animatedItem.PropertyName.Equals(SkewTransformAngleY)
				|| animatedItem.PropertyName.Equals(SkewTransformAngleYWithNamespace))
			{
				return CreateCoreAnimation(view, "transform", value => ToCASkewTransform(0, value));
			}
			else
			{
				throw new NotSupportedException(__notSupportedProperty);
			}
		}

		private UnoCoreAnimation InitializeCompositeCoreAnimation(CompositeTransform transform, IBindingItem animatedItem)
		{
			switch (animatedItem.PropertyName)
			{
				case CompositeTransformCenterX:
				case CompositeTransformCenterXWithNamespace:
				case "CenterX"://This animation is a Lie. transform.position.x doesn't exist, the real animator is the CPU bound one
					return CreateCoreAnimation(transform, "transform.position.x", value => new NSNumber(value));
				case CompositeTransformCenterY:
				case CompositeTransformCenterYWithNamespace:
				case "CenterY"://This animation is a Lie. transform.position.x doesn't exist, the real animator is the CPU bound one
					return CreateCoreAnimation(transform, "transform.position.y", value => new NSNumber(value));
				case CompositeTransformTranslateX:
				case CompositeTransformTranslateXWithNamespace:
				case "TranslateX":
					return CreateCoreAnimation(transform, "transform.translation.x", value => new NSNumber(value));
				case CompositeTransformTranslateY:
				case CompositeTransformTranslateYWithNamespace:
				case "TranslateY":
					return CreateCoreAnimation(transform, "transform.translation.y", value => new NSNumber(value));
				case CompositeTransformRotation:
				case CompositeTransformRotationWithNamespace:
				case "Rotation":
					return CreateCoreAnimation(transform, "transform.rotation", value => new NSNumber(MathEx.ToRadians(value)));
				case CompositeTransformScaleX:
				case CompositeTransformScaleXWithNamespace:
				case "ScaleX":
					return CreateCoreAnimation(transform, "transform.scale.x", value => new NSNumber(value));
				case CompositeTransformScaleY:
				case CompositeTransformScaleYWithNamespace:
				case "ScaleY":
					return CreateCoreAnimation(transform, "transform.scale.y", value => new NSNumber(value));

				//Again, we need to review how we handle SkewTransforms
				case CompositeTransformSkewX:
				case CompositeTransformSkewXWithNamespace:
				case "SkewX":
					return CreateCoreAnimation(transform.View, "transform", value => ToCASkewTransform(value, 0));
				case CompositeTransformSkewY:
				case CompositeTransformSkewYWithNamespace:
				case "SkewY":
					return CreateCoreAnimation(transform.View, "transform", value => ToCASkewTransform(0, value));
				default:
					throw new NotSupportedException(__notSupportedProperty);
			}
		}
#endregion

		private UnoCoreAnimation CreateCoreAnimation(
			Transform transform,
			string property,
			Func<float, NSValue> nsValueConversion)
			=> CreateCoreAnimation(transform.View, property, nsValueConversion, transform.StartAnimation, transform.EndAnimation);
		
		private UnoCoreAnimation CreateCoreAnimation(
			_View view,
			string property,
			Func<float, NSValue> nsValueConversion,
			Action prepareAnimation = null,
			Action endAnimation = null)
		{
			var timingFunction = _easingFunction == null ?
				CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear) :
				_easingFunction.GetTimingFunction();

			var isDiscrete = _easingFunction is DiscreteDoubleKeyFrame.DiscreteDoubleKeyFrameEasingFunction;

			return prepareAnimation == null || endAnimation == null
				? new UnoCoreAnimation(view.Layer, property, _from, _to, StartDelay, _duration, timingFunction, nsValueConversion, FinalizeAnimation, isDiscrete)
				: new UnoCoreAnimation(view.Layer, property, _from, _to, StartDelay, _duration, timingFunction, nsValueConversion, FinalizeAnimation, isDiscrete, prepareAnimation, endAnimation);

		}

		private NSValue ToCASkewTransform(float angleX, float angleY)
		{
			var matrix = CGAffineTransform.MakeIdentity();
			matrix.yx = (float)Math.Tan(MathEx.ToRadians(angleY));
			matrix.xy = (float)Math.Tan(MathEx.ToRadians(angleX));

			return NSValue.FromCATransform3D(CATransform3D.MakeFromAffine(matrix));
		}

		private void FinalizeAnimation(UnoCoreAnimation.CompletedInfo completedInfo)
		{
			if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
			{
				this.Log().DebugFormat("Finalizing animation for GPU Float value animator on property {0}.", _bindingPath.LastOrDefault().PropertyName);
			}

			if (_valueAnimator?.IsRunning ?? false)
			{
				_valueAnimator.Cancel();
			}

			switch(completedInfo)
			{
				case UnoCoreAnimation.CompletedInfo.Sucesss: AnimationEnd?.Invoke(this, EventArgs.Empty); break;
				case UnoCoreAnimation.CompletedInfo.Error: AnimationFailed?.Invoke(this, EventArgs.Empty); break;
				default: throw new NotSupportedException($"{completedInfo} is not supported");
			};

			ReleaseCoreAnimation();
		}

		private void ReleaseCoreAnimation()
		{
			_coreAnimation?.Dispose();
			_coreAnimation = null;
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_bindingPath = null;

				_valueAnimator.Dispose();
				_valueAnimator = null;

				_coreAnimation?.Dispose();
				_coreAnimation = null;

				this.Update = null;
				this.AnimationEnd = null;
				this.AnimationCancel = null;
			}

			_isDisposed = true;
		}
	}
}
