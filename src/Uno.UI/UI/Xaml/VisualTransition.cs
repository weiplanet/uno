﻿using Uno.Extensions;
using Windows.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Markup;

namespace Windows.UI.Xaml
{
	[ContentProperty(Name = "Storyboard")]
	public partial class VisualTransition : DependencyObject
	{
		internal Action LazyBuilder { get; set; }

		public VisualTransition()
		{
			IsAutoPropertyInheritanceEnabled = false;
			InitializeBinder();
		}

		public string From { get; set; }

		public string To { get; set; }


		#region Storyboard DependencyProperty

		public Storyboard Storyboard
		{
			get
			{
				EnsureMaterialized();
				return (Storyboard)this.GetValue(StoryboardProperty);
			}
			set { this.SetValue(StoryboardProperty, value); }
		}

		private void EnsureMaterialized()
		{
			if (LazyBuilder != null)
			{
				var builder = LazyBuilder;
				LazyBuilder = null;
				builder.Invoke();

				if (Storyboard is IDependencyObjectStoreProvider storyboardProvider)
				{
					// Set the theme changed flag on so the update processes
					// the children.
					storyboardProvider.Store.UpdateResourceBindings(true);
				}
			}
		}

		public static DependencyProperty StoryboardProperty { get ; } =
			DependencyProperty.Register(
				"Storyboard",
				typeof(Storyboard),
				typeof(VisualTransition),
				new FrameworkPropertyMetadata(
					defaultValue: null,
					options: FrameworkPropertyMetadataOptions.LogicalChild
				)
			);

		#endregion
	}
}
