﻿// MUX Reference NavigationView.idl, commit 52e7fc4

namespace Microsoft.UI.Xaml.Controls
{
	/// <summary>
	/// Defines constants that specify how the pane is shown in a NavigationView.
	/// </summary>
	public enum NavigationViewDisplayMode
    {
		/// <summary>
		/// Only the menu button remains fixed. The pane shows and hides as needed.
		/// </summary>
		Minimal = 0,

		/// <summary>
		/// The pane always shows as a narrow sliver which can be opened to full width.
		/// </summary>
		Compact = 1,

		/// <summary>
		/// The pane stays open alongside the content.
		/// </summary>
		Expanded = 2,
    }
}
