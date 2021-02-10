﻿#nullable enable

using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Uno.Extensions;
using Uno.Logging;
using Windows.Graphics;

namespace Windows.UI.Composition
{
	internal partial class SkiaCompositionSurface : ICompositionSurface
	{
		private SKImage? _image;

		public SKImage? Image { get => _image; }

		internal void LoadFromBytes(byte[] image)
		{
		}

		internal (bool success, object nativeResult) LoadFromStream(int? targetWidth, int? targetHeight, Stream imageStream)
		{
			using var stream = new SKManagedStream(imageStream);

			if (targetWidth is int actualTargetWidth && targetHeight is int actualTargetHeight)
			{
				using var codec = SKCodec.Create(stream);

				var info = codec.Info;

				var bitmap = new SKBitmap(actualTargetWidth, actualTargetHeight, info.ColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);

				var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());

				if (this.Log().IsEnabled(LogLevel.Debug))
				{
					this.Log().Debug($"Image load result {result}");
				}

				return (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput, result);
			}
			else
			{
				try
				{
					_image = SKImage.FromEncodedData(stream);
					return _image is null
						? (false, "Failed to decode image")
						: (true, "Success");
				}
				catch (Exception e)
				{
					return (true, e.Message);
				}
			}
		}

		internal unsafe void SetPixels(int pixelWidth, int pixelHeight, byte[] data)
		{
			var info = new SKImageInfo(pixelWidth, pixelHeight, SKColorType.Rgba8888, SKAlphaType.Premul);

			fixed (byte* pData = data)
			{
				_image = SKImage.FromPixels(info, (IntPtr)pData, pixelWidth * 4);
			}
		}
	}
}
