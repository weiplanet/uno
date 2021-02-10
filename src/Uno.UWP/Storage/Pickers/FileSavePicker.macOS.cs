﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using Windows.Foundation;

namespace Windows.Storage.Pickers
{
	public partial class FileSavePicker
	{
		public PickerLocationId SuggestedStartLocation { get; set; }
		public IAsyncOperation<StorageFile> PickSaveFileAsync() => PickFileTaskAsync().AsAsyncOperation();


		private async Task<StorageFile> PickFileTaskAsync()
		{
			var savePicker = new NSSavePanel();
			savePicker.DirectoryUrl = new NSUrl(GetStartPath(), true);
			savePicker.AllowedFileTypes = GetFileTypes();
			if (SuggestedFileName != null)
			{
				savePicker.NameFieldStringValue = SuggestedFileName;
			}
			if (savePicker.RunModal() == 1)
			{
				return await StorageFile.GetFileFromPathAsync(savePicker.Url.Path);
			}
			else
			{
				return null;
			}
		}

		private string GetStartPath()
		{
			var specialFolder = SuggestedStartLocation switch
			{
				PickerLocationId.DocumentsLibrary =>  NSSearchPathDirectory.DocumentDirectory,
				PickerLocationId.Desktop => NSSearchPathDirectory.DesktopDirectory,
				PickerLocationId.MusicLibrary => NSSearchPathDirectory.MusicDirectory,
				PickerLocationId.PicturesLibrary => NSSearchPathDirectory.PicturesDirectory,
				PickerLocationId.VideosLibrary => NSSearchPathDirectory.MoviesDirectory,
				_ => NSSearchPathDirectory.UserDirectory
			};

			return NSFileManager.DefaultManager.GetUrls(specialFolder, NSSearchPathDomain.User)[0].AbsoluteString;
		}

		private string[] GetFileTypes() => FileTypeChoices.SelectMany(x => x.Value.Select(val => val.TrimStart(new[] { '.' }))).ToArray();
	}
}
