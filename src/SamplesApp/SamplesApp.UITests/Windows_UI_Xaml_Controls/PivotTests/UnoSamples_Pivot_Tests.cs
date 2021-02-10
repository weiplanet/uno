﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SamplesApp.UITests.TestFramework;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace SamplesApp.UITests.Windows_UI_Xaml_Controls.PivotTests
{
	public class UnoSamples_Pivot_Tests : SampleControlUITestBase
	{
		[Test]
		[AutoRetry()]
		public void Pivot_Non_PivotItem_Items()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.Pivot.Pivot_CustomContent_Automated");

			var selectedItemTitle = _app.Marked("selectedItemTitle");
			var selectedItemContent = _app.Marked("selectedItemContent");

			_app.WaitForElement(_app.Marked("selectedItemTitle"));

			_app.WaitForDependencyPropertyValue(selectedItemTitle, "Text", "item 1");
			_app.WaitForDependencyPropertyValue(selectedItemContent, "Text", "My Item 1 Content");
		}

		[Test]
		[AutoRetry()]
		public void Pivot_Non_PivotItemChange_Validation()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.Pivot.Pivot_CustomContent_Automated");

			var selectedItemTitle = _app.Marked("selectedItemTitle");
			var selectedItemContent = _app.Marked("selectedItemContent");
			var firstPivotItemHeader = _app.Marked("myPivot").Descendant().Marked("item 1");
			var secondPivotItemHeader = _app.Marked("myPivot").Descendant().Marked("item 2");

			// Select header 2 and assert
			_app.FastTap(secondPivotItemHeader);
			_app.WaitForElement(_app.Marked("selectedItemTitle"));
			_app.WaitForDependencyPropertyValue(selectedItemTitle, "Text", "item 2");
			_app.WaitForDependencyPropertyValue(selectedItemContent, "Text", "My Item 2 Content");

			// Select header 1 and assert
			_app.FastTap(firstPivotItemHeader);
			_app.WaitForElement(_app.Marked("selectedItemTitle"));
			_app.WaitForDependencyPropertyValue(selectedItemTitle, "Text", "item 1");
			_app.WaitForDependencyPropertyValue(selectedItemContent, "Text", "My Item 1 Content");
		}		
	}
}
