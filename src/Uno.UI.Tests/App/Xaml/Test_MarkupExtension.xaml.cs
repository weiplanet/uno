﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.UI.Tests.App.Xaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Test_MarkupExtension : UserControl
    {
		public TextBlock TestText1 => Text1;
		public TextBlock TestText2 => Text2;
		public TextBlock TestText3 => Text3;
		public TextBlock TestText4 => Text4;
		public TextBlock TestText5 => Text5;

		public Test_MarkupExtension()
        {
            this.InitializeComponent();
        }
    }
}
