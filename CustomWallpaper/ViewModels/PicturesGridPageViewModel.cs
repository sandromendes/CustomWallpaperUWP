using CustomWallpaper.Domain.Models;
using CustomWallpaper.Services.Images;
using CustomWallpaper.Services.Wallpapers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using Prism.Commands;
using CustomWallpaper.Core.Events;
using Prism.Events;
using Prism.Windows.Navigation;
using CustomWallpaper.Core.Services;
using System.Threading;
using CustomWallpaper.Services.Folders;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using CustomWallpaper.Navigation;
using CustomWallpaper.Core.Utils;
using CustomWallpaper.Services.WallpaperHistories;

namespace CustomWallpaper.ViewModels
{
    public class PicturesGridPageViewModel : ViewModelBaseEx
    {
        private readonly CoreDispatcher _dispatcher;

        private readonly IImageService _imageService;
        private readonly IFolderService _folderService;
        private readonly IWallpaperService _wallpaperService;
        private readonly IWallpaperHistoryService _wallpaperHistoryService;

        private readonly INavigationService _navigationService;
        private readonly ILoggerService _loggerService;

        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<ImageItem> Images { get; set; } = new ObservableCollection<ImageItem>();

        public DelegateCommand<ImageItem> SetAsWallpaperCommand { get; }
        public DelegateCommand<ImageItem> SetAsLockScreenCommand { get; }

        public PicturesGridPageViewModel(
            IImageService imageService,
            IFolderService folderService,
            IWallpaperService wallpaperService,
            IWallpaperHistoryService wallpaperHistoryService,
            INavigationService navigationService,
            ILoggerService loggerService,
            IEventAggregator eventAggregator)
        {
            _imageService = imageService;
            _folderService = folderService;
            _wallpaperService = wallpaperService;
            _wallpaperHistoryService = wallpaperHistoryService;
            _navigationService = navigationService;
            _loggerService = loggerService;

            _eventAggregator = eventAggregator;

            SetAsWallpaperCommand = new DelegateCommand<ImageItem>(SetAsWallpaper);
            SetAsLockScreenCommand = new DelegateCommand<ImageItem>(SetAsLockscreen);

            _eventAggregator.GetEvent<FolderImportedEvent>().Subscribe(async storageFolder =>
            {
                await RegisterFolderAsync(storageFolder);

                await LoadRegisteredFoldersAsync();
            });

            _dispatcher = Window.Current.Dispatcher;
        }

        public override async Task OnShowAsync(object parameter = null)
        {
            await LoadRegisteredFoldersAsync();
        }

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            await LoadAsyncSafely();
            base.OnNavigatedTo(e, viewModelState);
        }

        private async Task LoadAsyncSafely()
        {
            try
            {
                await LoadRegisteredFoldersAsync();
            }
            catch (Exception ex)
            {
                _loggerService.Error(nameof(PicturesGridPageViewModel), ex, $"Failed to load registered folders.");
            }
        }

        private async Task RegisterFolderAsync(StorageFolder storageFolder)
        {
            try
            {
                var existingFolders = await _folderService.GetAllFoldersAsync();
                
                if (!existingFolders.Any(f => f.FolderPath == storageFolder.Path))
                {
                    string token = Guid.NewGuid().ToString();
                    await _folderService.AddFolderAsync(storageFolder.Path, token);

                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, storageFolder);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error(nameof(PicturesGridPageViewModel), ex, $"Failed to register folder {storageFolder.Path}");
            }
        }

        public async Task LoadRegisteredFoldersAsync()
        {
            var folders = await _folderService.GetAllFoldersAsync();

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Images.Clear());

            foreach (var folder in folders)
            {
                try
                {
                    var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder.FolderPath);
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(folder.AccessToken, storageFolder);
                    await LoadImagesFromFolderAsync(storageFolder);
                }
                catch (Exception ex)
                {
                    _loggerService.Error(nameof(PicturesGridPageViewModel), ex, $"Failed to load folder {folder.FolderPath}");
                }
            }
        }

        private readonly SemaphoreSlim _loadImagesSemaphore = new SemaphoreSlim(1, 1);

        public async Task LoadImagesFromFolderAsync(StorageFolder folder)
        {
            if (folder == null)
                return;

            if (!await _loadImagesSemaphore.WaitAsync(0))
                return; // evita reentrância

            try
            {
                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                {
                    var ext = file.FileType.ToLowerInvariant();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".bmp")
                    {
                        await _imageService.AddOrUpdateFromFileAsync(file);

                        var thumb = await LoadThumbnailAsync(file);

                        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            Images.Add(new ImageItem
                            {
                                Name = file.Name,
                                Path = file.Path,
                                Thumbnail = thumb
                            });
                        });
                    }
                }
            }
            finally
            {
                _loadImagesSemaphore.Release();
            }
        }

        private async Task<BitmapImage> LoadThumbnailAsync(StorageFile file)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream);
                return bitmap;
            }
        }

        private async void SetAsWallpaper(ImageItem item)
        {
            if (item != null)
            {
                var file = await StorageFile.GetFileFromPathAsync(item.Path);
                await _wallpaperService.SetWallpaperAsync(file);

                var image = await _imageService.GetByHashAsync(await FileHasher.ComputeHashAsync(file));
                await _wallpaperHistoryService.AddAsync(image.Id, "Wallpaper - Manual Selection");
            }
        }

        private async void SetAsLockscreen(ImageItem item)
        {
            if (item != null)
            {
                var file = await StorageFile.GetFileFromPathAsync(item.Path);
                await _wallpaperService.SetLockscreenAsync(file);

                var image = await _imageService.GetByHashAsync(await FileHasher.ComputeHashAsync(file));
                await _wallpaperHistoryService.AddAsync(image.Id, "Lockscreen - Manual Selection");
            }
        }
    }
}

