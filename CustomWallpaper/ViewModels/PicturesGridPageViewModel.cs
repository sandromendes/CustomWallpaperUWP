using CustomWallpaper.Domain.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using Prism.Commands;
using CustomWallpaper.CrossCutting.Events;
using Prism.Events;
using Prism.Windows.Navigation;
using CustomWallpaper.CrossCutting.Services;
using System.Threading;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using CustomWallpaper.Navigation;
using CustomWallpaper.CrossCutting.Utils;
using System.Windows.Input;
using CustomWallpaper.Views;
using CustomWallpaper.Domain.Services;
using CustomWallpaper.Infrastructure.Services;
using Windows.Storage.FileProperties;

namespace CustomWallpaper.ViewModels
{
    public class PicturesGridPageViewModel : ViewModelBaseEx
    {
        private readonly CoreDispatcher _dispatcher;

        private readonly IImageService _imageService;
        private readonly IFolderService _folderService;
        private readonly IWallpaperService _wallpaperService;
        private readonly IWallpaperHistoryService _wallpaperHistoryService;

        private readonly INavigationServiceEx _navigationService;
        private readonly ILoggerService _loggerService;

        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<ImageItem> Images { get; set; } = new ObservableCollection<ImageItem>();

        public DelegateCommand<ImageItem> SetAsWallpaperCommand { get; }
        public DelegateCommand<ImageItem> SetAsLockScreenCommand { get; }
        public ICommand NavigateToImageViewerCommand { get; }

        public PicturesGridPageViewModel(
            IImageService imageService,
            IFolderService folderService,
            IWallpaperService wallpaperService,
            IWallpaperHistoryService wallpaperHistoryService,
            INavigationServiceEx navigationService,
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
            NavigateToImageViewerCommand = new DelegateCommand<ImageItem>(NavigateToImageViewer);

            _eventAggregator
                .GetEvent<FolderImportedEvent>()
                .Subscribe(async storageFolder => await HandleFolderImportedAsync(storageFolder));

            _dispatcher = Window.Current.Dispatcher;
        }

        private async Task HandleFolderImportedAsync(StorageFolder storageFolder)
        {
            try
            {
                var token = $"Folder_{Guid.NewGuid()}";
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, storageFolder);

                await RegisterAllMediaAsync(storageFolder.Path, token).ConfigureAwait(false);
                await LoadRegisteredFoldersAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _loggerService?.Error(nameof(PicturesGridPageViewModel), ex, $"Error while importing folder: {storageFolder?.Path}");
            }
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

        public async Task RegisterAllMediaAsync(string rootFolder, string token)
        {
            var allFolders = await _folderService.RegisterAllFoldersRecursivelyAsync(rootFolder, token);
            await _imageService.RegisterImagesInFoldersAsync(allFolders, token);
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

        private async void NavigateToImageViewer(ImageItem image)
        {
            if (image == null) return;

            await _navigationService.NavigateAsync(new NavigationItem(
                key: nameof(ImageViewerPage),
                pageType: typeof(ImageViewerPage),
                parameter: image));
        }

        private readonly SemaphoreSlim _loadImagesSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Loads all image files from the given folder into the Images collection, creating thumbnails asynchronously.
        /// Prevents reentrancy using a semaphore and ensures BitmapImage creation occurs on the UI thread.
        /// </summary>
        public async Task LoadImagesFromFolderAsync(StorageFolder folder)
        {
            if (folder == null)
                return;

            if (!await _loadImagesSemaphore.WaitAsync(0))
                return;

            try
            {
                var files = await folder.GetFilesAsync();
                var imageFiles = files.Where(f =>
                {
                    var ext = f.FileType.ToLowerInvariant();
                    return ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".bmp";
                }).ToList();

                var tasks = imageFiles.Select(async file =>
                {
                    var thumbnail = await CreateThumbnailAsync(file);

                    await _dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                    {
                        Images.Add(new ImageItem
                        {
                            Name = file.Name,
                            Path = file.Path,
                            Thumbnail = thumbnail
                        });
                    });
                });

                await Task.WhenAll(tasks);
            }
            finally
            {
                _loadImagesSemaphore.Release();
            }
        }

        private async Task<BitmapImage> CreateThumbnailAsync(StorageFile file)
        {
            var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView, 150, ThumbnailOptions.UseCurrentScale);
            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(thumbnail);
            return bitmapImage;
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

