#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uno.Extensions;
using Windows.Foundation;

#if __IOS__
using UIKit;
using Foundation;
#endif

namespace Windows.Storage
{
	public partial class StorageFolder : IStorageFolder, IStorageItem, IStorageItem2
	{
		public string Path { get; private set; }
		public string Name { get; private set; }

		internal StorageFolder(string fullPath)
		{
			Path = fullPath;
			Name = global::System.IO.Path.GetFileName(fullPath);
		}

		internal StorageFolder(string name, string path)
		{
			Path = path;
			Name = name;
		}

#if !__WASM__
		private static async Task TryInitializeStorage() { }
#endif

		public static global::Windows.Foundation.IAsyncOperation<global::Windows.Storage.StorageFolder> GetFolderFromPathAsync(string path) =>
			AsyncOperation.FromTask(async ct =>
			{
				await TryInitializeStorage();

				if (Directory.Exists(path))
				{
					return new StorageFolder(path);
				}
				else
				{
					throw new DirectoryNotFoundException($"The folder {path} does not exist");
				}
			}
		);

		public IAsyncOperation<StorageFolder> CreateFolderAsync(string folderName) => CreateFolderAsync(folderName, CreationCollisionOption.FailIfExists);

		public IAsyncOperation<StorageFolder> CreateFolderAsync(string folderName, CreationCollisionOption option) =>
			AsyncOperation.FromTask(async ct =>
			{
				await TryInitializeStorage();

				var path = global::System.IO.Path.Combine(Path, folderName);
				DirectoryInfo di;
				switch (option)
				{
					case CreationCollisionOption.ReplaceExisting:
						if (Directory.Exists(path))
						{
							Directory.Delete(path, true);
						}

						di = Directory.CreateDirectory(path);
						return new StorageFolder(di.Name, path);

					case CreationCollisionOption.FailIfExists:
						if (Directory.Exists(path))
						{
							throw new UnauthorizedAccessException();
						}

						di = Directory.CreateDirectory(path);
						return new StorageFolder(di.Name, path);

					case CreationCollisionOption.OpenIfExists:
						if (Directory.Exists(path))
						{
							return new StorageFolder(folderName, path);
						}

						di = Directory.CreateDirectory(path);
						return new StorageFolder(di.Name, path);

					case CreationCollisionOption.GenerateUniqueName:
						if (Directory.Exists(path))
						{
							di = Directory.CreateDirectory(path += Guid.NewGuid().ToStringInvariant());
							return new StorageFolder(di.Name, path);
						}
						else
						{
							di = Directory.CreateDirectory(path);
							return new StorageFolder(di.Name, path);
						}
				}

				return null;
			});

		/// <summary>
		/// WARNING This method should not be used because it doesn't match the StorageFile API
		/// </summary>
		public IAsyncOperation<StorageFile> SafeGetFileAsync(string path) =>
			AsyncOperation.FromTask(async ct =>
			{
				await TryInitializeStorage();

				return await StorageFile.GetFileFromPathAsync(global::System.IO.Path.Combine(Path, path));
			});

		public IAsyncOperation<StorageFile> GetFileAsync(string path) =>
			AsyncOperation.FromTask(async ct =>
			{
				await TryInitializeStorage();

				var filePath = global::System.IO.Path.Combine(Path, path);

				if (!File.Exists(filePath))
				{
					throw new FileNotFoundException(filePath);
				}

				return StorageFile.GetFileFromPath(filePath);
			});

		public IAsyncOperation<global::Windows.Storage.IStorageItem> GetItemAsync(string name) =>
			AsyncOperation.FromTask(async ct =>
			{
				await TryInitializeStorage();

				var itemPath = global::System.IO.Path.Combine(Path, name);

				var fileExists = File.Exists(itemPath);
				var directoryExists = Directory.Exists(itemPath);

				if (!fileExists && !directoryExists)
				{
					throw new FileNotFoundException(itemPath);
				}

				if (fileExists)
				{
					return (IStorageItem)await StorageFile.GetFileFromPathAsync(itemPath);
				}
				else
				{
					return (IStorageItem)await StorageFolder.GetFolderFromPathAsync(itemPath);
				}
			});

		public global::Windows.Foundation.IAsyncOperation<global::Windows.Storage.StorageFolder> GetFolderAsync(string name) =>
			AsyncOperation.FromTask(async ct =>
			{
				await TryInitializeStorage();

				var itemPath = global::System.IO.Path.Combine(Path, name);

				var directoryExists = Directory.Exists(itemPath);

				if (!directoryExists)
				{
					throw new FileNotFoundException(itemPath);
				}

				return await StorageFolder.GetFolderFromPathAsync(itemPath);
			});

		public IAsyncOperation<IStorageItem> TryGetItemAsync(string path) =>
				AsyncOperation.FromTask(async ct =>
				{
					await TryInitializeStorage();

					var filePath = global::System.IO.Path.Combine(Path, path);

					var result = File.Exists(filePath)
						? await StorageFile.GetFileFromPathAsync(filePath)
						: default(StorageFile);

					return (IStorageItem)result;
				});

		public IAsyncOperation<StorageFile> CreateFileAsync(string desiredName) => CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);

		public IAsyncOperation<StorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options) =>
			AsyncOperation.FromTask(async ct =>
			{
				await TryInitializeStorage();

				if (File.Exists(global::System.IO.Path.Combine(Path, desiredName)))
				{
					switch (options)
					{
						case CreationCollisionOption.FailIfExists:
							throw new Exception("Cannot create a file when that file already exists.");
						case CreationCollisionOption.OpenIfExists:
							break;
						case CreationCollisionOption.ReplaceExisting:
							File.Create(global::System.IO.Path.Combine(Path, desiredName)).Close();
							break;
						case CreationCollisionOption.GenerateUniqueName:

							var pathExtension = global::System.IO.Path.GetExtension(desiredName);
							if (!string.IsNullOrEmpty(pathExtension))
							{
								desiredName = desiredName.Replace(pathExtension, "_" + Guid.NewGuid().ToStringInvariant().Replace("-", "") + pathExtension);
							}
							else
							{
								desiredName = desiredName + "_" + Guid.NewGuid();
							}

							File.Create(global::System.IO.Path.Combine(Path, desiredName)).Close();
							break;
						default:
							throw new ArgumentOutOfRangeException(nameof(options));
					}
				}
				else
				{
					File.Create(global::System.IO.Path.Combine(Path, desiredName)).Close();
				}

				return await StorageFile.GetFileFromPathAsync(global::System.IO.Path.Combine(Path, desiredName));
			});

		public IAsyncAction DeleteAsync() =>
			AsyncAction.FromTask(async ct =>
			{
				await TryInitializeStorage();

				Directory.Delete(this.Path, true);
            });

		internal async Task<IReadOnlyList<IStorageItem>> GetItemsTask(CancellationToken ct)
		{
			var items = new List<IStorageItem>();

			foreach (var folder in Directory.EnumerateDirectories(this.Path))
			{
				items.Add(await StorageFolder.GetFolderFromPathAsync(folder).AsTask(ct));
			}

			foreach (var folder in Directory.EnumerateFiles(this.Path))
			{
				items.Add(await StorageFile.GetFileFromPathAsync(folder).AsTask(ct));
			}

			return items.AsReadOnly();
		}

		public IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync() => AsyncOperation.FromTask(ct => GetItemsTask(ct));

		internal async Task<IReadOnlyList<StorageFile>> GetFilesTask(CancellationToken ct)
		{
			var items = new List<StorageFile>();

			foreach (var folder in Directory.EnumerateFiles(this.Path))
			{
				items.Add(await StorageFile.GetFileFromPathAsync(folder).AsTask(ct));
			}
			return items.AsReadOnly();
		}

		public IAsyncOperation<IReadOnlyList<StorageFile>> GetFilesAsync() => AsyncOperation.FromTask(ct => GetFilesTask(ct));

		internal async Task<IReadOnlyList<StorageFolder>> GetFoldersTask(CancellationToken ct)
		{
			var items = new List<StorageFolder>();

			foreach (var folder in Directory.EnumerateDirectories(this.Path))
			{
				items.Add(await StorageFolder.GetFolderFromPathAsync(folder).AsTask(ct));
			}

			return items.AsReadOnly();
		}

		public IAsyncOperation<IReadOnlyList<StorageFolder>> GetFoldersAsync() => AsyncOperation.FromTask(ct => GetFoldersTask(ct));
	}
}
