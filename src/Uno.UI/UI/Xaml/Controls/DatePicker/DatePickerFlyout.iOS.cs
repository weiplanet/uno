#if XAMARIN_IOS
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Uno.Disposables;
using Uno.Extensions;
using Uno.UI;
using Uno.UI.Common;
using Uno.UI.DataBinding;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Core;

namespace Windows.UI.Xaml.Controls
{
	//TODO#2780: support Day/Month/YearVisible
	public partial class DatePickerFlyout : PickerFlyoutBase
	{
		#region Template Parts
		public const string AcceptButtonPartName = "AcceptButton";
		public const string DismissButtonPartName = "DismissButton";
		#endregion

		public event TypedEventHandler<DatePickerFlyout, DatePickedEventArgs> DatePicked;

		private readonly SerialDisposable _presenterCommandsDisposable = new SerialDisposable();
		private readonly SerialDisposable _presenterLoadedDisposable = new SerialDisposable();
		private readonly SerialDisposable _presenterUnloadedDisposable = new SerialDisposable();
		private bool _isInitialized;
		private DatePickerFlyoutPresenter _presenter;
		private DatePickerSelector _selector;

		public DatePickerFlyout()
		{
			Opening += DatePickerFlyout_Opening;
			Closed += DatePickerFlyout_Closed;
		}

		protected override void InitializePopupPanel()
		{
			_popup.PopupPanel = new PickerFlyoutPopupPanel(this)
			{
				Visibility = Visibility.Collapsed,
				Background = SolidColorBrushHelper.Transparent
			};
		}

		/// <summary>
		/// This method sets the Content property of the Flyout.
		/// </summary>
		/// <remarks>
		/// Note that for performance reasons, we don't call it in the constructor. Instead, we wait for the popup to be opening.
		/// The native UIDatePicker contained in the DatePickerSelector is known for being slow in general (https://bugzilla.xamarin.com/show_bug.cgi?id=49469).
		/// Using this strategy means that a page containing a DatePicker will no longer be slowed down by this initialization during the page creation.
		/// Instead, you'll see the delay when opening the DatePickerFlyout for the first time.
		/// This is most notable on pages containing multiple DatePicker controls.
		/// </remarks>
		private void InitializeContent()
		{
			if (_isInitialized)
			{
				return;
			}

			_isInitialized = true;

			Content = _selector = new DatePickerSelector()
			{
				MinYear = MinYear,
				MaxYear = MaxYear
			};

			BindToContent("MinYear");
			BindToContent("MaxYear");
		}

		protected override Control CreatePresenter()
		{
			_presenter = new DatePickerFlyoutPresenter() { Content = _selector };

			_presenterLoadedDisposable.Disposable = Disposable.Create(() => _presenter.Loaded -= OnPresenterLoaded);
			_presenterUnloadedDisposable.Disposable = Disposable.Create(() => _presenter.Unloaded -= OnPresenterUnloaded);

			_presenter.Loaded += OnPresenterLoaded;
			_presenter.Unloaded += OnPresenterUnloaded;

			return _presenter;
		}

		#region Content DependencyProperty
		internal IFrameworkElement Content
		{
			get { return (IFrameworkElement)this.GetValue(ContentProperty); }
			set { this.SetValue(ContentProperty, value); }
		}

		internal static DependencyProperty ContentProperty { get ; } =
			DependencyProperty.Register(
				"Content",
				typeof(IFrameworkElement),
				typeof(DatePickerFlyout),
				new FrameworkPropertyMetadata(default(IFrameworkElement), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.ValueInheritsDataContext, OnContentChanged));

		private static void OnContentChanged(object dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			var flyout = dependencyObject as DatePickerFlyout;

			if (flyout._presenter != null)
			{
				if (args.NewValue is IDependencyObjectStoreProvider binder)
				{
					binder.Store.SetValue(binder.Store.TemplatedParentProperty, flyout.TemplatedParent, DependencyPropertyValuePrecedences.Local);
					binder.Store.SetValue(binder.Store.DataContextProperty, flyout.DataContext, DependencyPropertyValuePrecedences.Local);
				}

				flyout._presenter.Content = args.NewValue;
			}
		}
		#endregion

		private void DatePickerFlyout_Opening(object sender, EventArgs e)
		{
			InitializeContent();
			UpdateSelectorDate(Date);
		}

		private void DatePickerFlyout_Closed(object sender, EventArgs e)
		{
			_selector.Cancel();
		}

		private void OnPresenterLoaded(object sender, RoutedEventArgs e)
		{
			var disposables = new CompositeDisposable();
			_presenterCommandsDisposable.Disposable = disposables;

			AttachFlyoutCommand(AcceptButtonPartName, x => x.Accept()).DisposeWith(disposables);
			AttachFlyoutCommand(DismissButtonPartName, x => x.Dismiss()).DisposeWith(disposables);

			_presenterLoadedDisposable.Disposable = null;
		}

		private void OnPresenterUnloaded(object sender, RoutedEventArgs e)
		{
			_presenterCommandsDisposable.Disposable = null;
			_presenterLoadedDisposable.Disposable = null;
			_presenterUnloadedDisposable.Disposable = null;
		}

		partial void OnDateChangedPartialNative(DateTimeOffset oldDate, DateTimeOffset newDate)
		{
			UpdateSelectorDate(newDate);
		}

		private void Accept()
		{
			_selector.SaveValue();
			Hide(false);

			var oldDate = Date;
			Date = _selector.Date;

			DatePicked?.Invoke(this, new DatePickedEventArgs(Date, oldDate));
		}

		private void Dismiss()
		{
			_selector.Cancel();
			Hide(false);
		}

		private void UpdateSelectorDate(DateTimeOffset value)
		{
			if (!_isInitialized) return;

			// The date coerced by UIDatePicker doesn't propagate back to DatePickerSelector (#137137)
			// When the user selected an invalid date, a `ValueChanged` will be raised after coercion to propagate the coerced date.
			// However, when the `Date` is set below, there no `ValueChanged` to propagate the coerced date.
			// To address this, we clamp the date between the valid range.
			var validDate = value;
			validDate = validDate > MaxYear ? MaxYear : validDate;
			validDate = validDate < MinYear ? MinYear : validDate;

			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				_selector.Date = validDate;
			}
			else
			{
				// This is a workaround for the datepicker crashing on iOS 9 with this error:
				// NSInternalInconsistencyException Reason: UITableView dataSource is not set.
				this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
				{
					await Task.Delay(100);
					_selector.Date = validDate;
				});
			}
		}

		private IDisposable AttachFlyoutCommand(string targetButtonName, Action<DatePickerFlyout> action)
		{
			if (_presenter.FindName(targetButtonName) is Button button)
			{
				if (button.Command == null)
				{
					var self = WeakReferencePool.RentSelfWeakReference(this);
					button.Command = new DelegateCommand(() => (self.Target as DatePickerFlyout)?.Apply(action));

					return Disposable.Create(() =>
					{
						button.Command = null;
						self.Dispose();
					});
				}
			}

			return Disposable.Empty;
		}

		private void BindToContent(string propertyName)
		{
			this.Binding(propertyName, propertyName, Content, BindingMode.TwoWay);
		}
	}
}

#endif
