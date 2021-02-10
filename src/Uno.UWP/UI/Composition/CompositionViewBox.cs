#nullable enable

using System.Numerics;

namespace Windows.UI.Composition
{
	public partial class CompositionViewBox : CompositionObject
	{
		public float VerticalAlignmentRatio { get; set; }
		public CompositionStretch Stretch { get; set; }
		public Vector2 Size { get; set; }
		public Vector2 Offset { get; set; }
		public float HorizontalAlignmentRatio { get; set; }
	}
}
