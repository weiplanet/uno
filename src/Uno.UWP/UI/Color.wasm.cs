﻿using System;
using System.Globalization;
using System.Text;

namespace Windows.UI
{
	partial struct Color
	{
		/// <summary>
		/// Get color value in CSS format "rgba(r, g, b, a)"
		/// </summary>
		public string ToCssString() => 
			"rgba(" 
			+ R.ToString(CultureInfo.InvariantCulture) + "," 
			+ G.ToString(CultureInfo.InvariantCulture) + "," 
			+ B.ToString(CultureInfo.InvariantCulture) + ","
			+ (A / 255.0).ToString(CultureInfo.InvariantCulture)
			+ ")";

		internal string ToHexString()
		{
			var builder = new StringBuilder(10);

			builder.Append("#");
			builder.Append(R.ToString("X2", CultureInfo.InvariantCulture));
			builder.Append(G.ToString("X2", CultureInfo.InvariantCulture));
			builder.Append(B.ToString("X2", CultureInfo.InvariantCulture));

			if (A != 255)
			{
				// Include alpha chanel only when required.
				builder.Append(A.ToString("X2", CultureInfo.InvariantCulture));
			}

			return builder.ToString();
		}
	}
}
