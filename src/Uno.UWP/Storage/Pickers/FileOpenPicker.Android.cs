﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Uno.UI;

namespace Windows.Storage.Pickers
{
    public partial class FileOpenPicker
    {
		internal const int RequestCode = 6002;
		private static TaskCompletionSource<Intent?>? _currentFileOpenPickerRequest;

		internal static bool TryHandleIntent(Intent intent, Result resultCode)
		{
			if (_currentFileOpenPickerRequest == null)
			{
				return false;
			}
			if (resultCode == Result.Canceled)
			{
				_currentFileOpenPickerRequest.SetResult(null);
			}
			else
			{
				_currentFileOpenPickerRequest.SetResult(intent);
			}
			return true;
		}

		private async Task<StorageFile?> PickSingleFileTaskAsync(CancellationToken token)
		{
			var files = await PickFilesAsync(false, token);
			return files.FirstOrDefault();
		}

		private async Task<IReadOnlyList<StorageFile>> PickMultipleFilesTaskAsync(CancellationToken token)
		{
			return await PickFilesAsync(true, token);
		}

		private async Task<FilePickerSelectedFilesArray> PickFilesAsync(bool multiple, CancellationToken token)
		{
			if (!(ContextHelper.Current is Activity appActivity))
			{
				throw new InvalidOperationException("Application activity is not yet set, API called too early.");
			}

			var action = Intent.ActionOpenDocument;

			var intent = new Intent(action);
			intent.PutExtra(Intent.ExtraAllowMultiple, multiple);
			intent.SetType("*/*");

			var mimeTypes = GetMimeTypes();
			intent.PutExtra(Intent.ExtraMimeTypes, mimeTypes);

			_currentFileOpenPickerRequest = new TaskCompletionSource<Intent?>();

			appActivity.StartActivityForResult(intent, RequestCode);

			var resultIntent = await _currentFileOpenPickerRequest.Task;
			_currentFileOpenPickerRequest = null;

			if (resultIntent?.ClipData != null)
			{
				List<StorageFile> files = new List<StorageFile>();
				for (var i = 0; i < resultIntent.ClipData.ItemCount; i++)
				{
					var item = resultIntent.ClipData.GetItemAt(i);
					if (item?.Uri == null)
					{
						continue;
					}
					var file = StorageFile.GetFromSafUri(item.Uri);
					files.Add(file);
				}
				return new FilePickerSelectedFilesArray(files.ToArray());
			}
			else if (resultIntent?.Data != null)
			{
				var file = StorageFile.GetFromSafUri(resultIntent.Data);
				return new FilePickerSelectedFilesArray(new[] { file });
			}

			return FilePickerSelectedFilesArray.Empty;
		}

		private string[] GetMimeTypes()
		{
			if (FileTypeFilter.Contains("*"))
			{
				return new[] { "*/*" };
			}

			return FileTypeFilter
				.Select(extension => MimeTypeService.GetFromExtension(extension))
				.Distinct()
				.ToArray();
		}
	}
}
