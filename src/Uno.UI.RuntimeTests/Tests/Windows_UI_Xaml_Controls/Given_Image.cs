﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Private.Infrastructure;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Uno.UI.RuntimeTests.Tests.Windows_UI_Xaml_Controls
{
	[TestClass]
	public class Given_Image
	{
#if !__IOS__ // Currently fails on iOS
		[TestMethod]
#endif
		[RunsOnUIThread]
		public async Task When_Fixed_Height_And_Stretch_Uniform()
		{
			var imageLoaded = new TaskCompletionSource<bool>();

			var image = new Image { Height = 30, Stretch = Stretch.Uniform, Source = new BitmapImage(new Uri("ms-appx:///Assets/storelogo.png")) };
			image.Loaded += (s, e) => imageLoaded.TrySetResult(true);

			var innerGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
			var outerGrid = new Grid { Height = 750, Width = 430 };
			innerGrid.Children.Add(image);
			outerGrid.Children.Add(innerGrid);

			TestServices.WindowHelper.WindowContent = outerGrid;
			await TestServices.WindowHelper.WaitForIdle();

			await imageLoaded.Task;

			image.InvalidateMeasure();

			await TestServices.WindowHelper.WaitForIdle();
			await TestServices.WindowHelper.WaitForIdle();

			outerGrid.Measure(new Size(1000, 1000));
			var desiredContainer = innerGrid.DesiredSize;

			Assert.AreEqual(30, Math.Round(desiredContainer.Width));
			Assert.AreEqual(30, Math.Round(desiredContainer.Height));

			TestServices.WindowHelper.WindowContent = null;
		}

#if __WASM__
		[TestMethod]
		[RunsOnUIThread]
		public async Task When_Resource_Has_Scale_Qualifier()
		{
			var scales = new List<ResolutionScale>()
			{
				ResolutionScale.Scale100Percent,
				ResolutionScale.Scale150Percent,
				ResolutionScale.Scale200Percent,
				ResolutionScale.Scale300Percent,
				ResolutionScale.Scale400Percent,
				ResolutionScale.Scale500Percent,
			};

			try
			{
				foreach (var scale in scales)
				{
					var imageOpened = new TaskCompletionSource<bool>();

					var source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/FluentIcon_Medium.png"));
					source.ScaleOverride = scale;

					var image = new Image { Height = 24, Width = 24, Stretch = Stretch.Uniform, Source = source };
					image.ImageOpened += (s, e) => imageOpened.TrySetResult(true);
					image.ImageFailed += (s, e) => imageOpened.TrySetResult(false);

					TestServices.WindowHelper.WindowContent = image;

					await TestServices.WindowHelper.WaitForIdle();

					var result = await imageOpened.Task;

					Assert.IsTrue(result);
				}
			}
			finally
			{
				TestServices.WindowHelper.WindowContent = null;
			}
		}
#endif
	}
}
