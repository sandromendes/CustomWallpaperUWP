using CustomWallpaper.Core.Events;
using CustomWallpaper.Core.Services;
using CustomWallpaper.Core.Utils;
using CustomWallpaper.Domain.Models;
using CustomWallpaper.Services.BackgroundTasks;
using CustomWallpaper.Services.Images;
using CustomWallpaper.Services.SmartEngine;
using CustomWallpaper.Services.States;
using CustomWallpaper.Services.WallpaperHistories;
using CustomWallpaper.Services.Wallpapers;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CustomWallpaper.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IWallpaperService _wallpaperService;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly ISmartEngineService _smartEngineService;
        private readonly IImageService _imageService;
        private readonly IWallpaperHistoryService _wallpaperHistoryService;
        private readonly ILoggerService _loggerService;
        private readonly IPageStateService _pageStateService;

        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<ImageItem> Thumbnails { get; } = new ObservableCollection<ImageItem>();
       
        public ICommand LoadFolderCommand { get; }
        public DelegateCommand<ImageItem> SelectImageCommand { get; }
        public DelegateCommand SetWallpaperCommand { get; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private StorageFile _selectedFile;

        public MainPageViewModel(IWallpaperService wallpaperService, 
            IBackgroundTaskService backgroundTaskService,
            ISmartEngineService smartEngineService,
            IImageService imageService,
            IWallpaperHistoryService wallpaperHistoryService,
            ILoggerService loggerService,
            IPageStateService pageStateService,
            IEventAggregator eventAggregator)
        {
            _wallpaperService = wallpaperService;
            _backgroundTaskService = backgroundTaskService;
            _smartEngineService = smartEngineService;
            _imageService = imageService;
            _wallpaperHistoryService = wallpaperHistoryService;
            _loggerService = loggerService;
            _pageStateService = pageStateService;
            _eventAggregator = eventAggregator;

            SelectImageCommand = new DelegateCommand<ImageItem>(OnSelectImage);
            SetWallpaperCommand = new DelegateCommand(async () => await SetWallpaperAsync());
            LoadFolderCommand = new DelegateCommand(async () => await LoadImagesFromFolderAsync());

            _ = RestoreStateAsync();

            _eventAggregator.GetEvent<FolderImportedEvent>().Subscribe(async folder =>
            {
                await LoadImagesFromFolderAsync(folder);
            });
        }

        private async Task RestoreStateAsync()
        {
            var selectedPath = _pageStateService.Get<string>("SelectedImagePath");

            if (!string.IsNullOrEmpty(selectedPath))
            {
                try
                {
                    var file = await StorageFile.GetFileFromPathAsync(selectedPath);
                    var bitmapImage = new BitmapImage();

                    using (var stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }

                    _selectedFile = file;
                    ImageSource = bitmapImage;
                    ImagePath = selectedPath;

                    _loggerService?.Info(nameof(MainPageViewModel), $"Restored selected image: {selectedPath}");
                }
                catch (Exception ex)
                {
                    _loggerService?.Error(nameof(MainPageViewModel), ex, "Failed to restore selected image.");
                }
            }

            var thumbnails = _pageStateService.Get<List<ImageItem>>("ImageThumbnails");

            if (thumbnails != null)
            {
                try
                {
                    foreach (var thumb in thumbnails)
                    {
                        var bitmap = new BitmapImage(new Uri(thumb.Path));

                        Thumbnails.Add(new ImageItem { Path = thumb.Path, Name = thumb.Name, Thumbnail = thumb.Thumbnail });
                        _loggerService?.Info(nameof(MainPageViewModel), $"Restored Thumbnail: {thumb.Path}");

                        await Task.Delay(100);
                    }
                }
                catch (Exception ex)
                {
                    _loggerService?.Error(nameof(MainPageViewModel), ex, "Failed to restore thumbnails.");
                }
            }
        }

        private async Task LoadImagesFromFolderAsync()
        {
            try
            {
                FolderPicker picker = new FolderPicker
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add("*");

                StorageFolder folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                    ImagePath = folder.Path;
                    Thumbnails.Clear();

                    var files = await folder.GetFilesAsync();
                    foreach (var file in files)
                    {
                        var ext = file.FileType.ToLowerInvariant();

                        if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".bmp")
                        {
                            await _imageService.AddOrUpdateFromFileAsync(file);

                            var thumb = await LoadThumbnailAsync(file);
                            Thumbnails.Add(new ImageItem { Path = file.Path, Thumbnail = thumb });
                        }
                    }

                    if (Thumbnails.Any())
                        _pageStateService.Set("ImageThumbnails", Thumbnails.ToList());
                }
            }
            catch (Exception ex)
            {
                _loggerService?.Error(nameof(MainPageViewModel), ex, "Failed to load folder images.");
            }
        }

        private async Task LoadImagesFromFolderAsync(StorageFolder folder)
        {
            try
            {
                ImagePath = folder.Path;
                Thumbnails.Clear();

                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                {
                    var ext = file.FileType.ToLowerInvariant();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".bmp")
                    {
                        await _imageService.AddOrUpdateFromFileAsync(file);

                        var thumb = await LoadThumbnailAsync(file);
                        Thumbnails.Add(new ImageItem { Path = file.Path, Thumbnail = thumb });
                    }
                }

                if (Thumbnails.Any())
                    _pageStateService.Set("ImageThumbnails", Thumbnails.ToList());
            }
            catch (Exception ex)
            {
                _loggerService?.Error(nameof(MainPageViewModel), ex, "Failed to load folder images.");
            }
        }

        private async Task<BitmapImage> LoadThumbnailAsync(StorageFile file)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                var bmp = new BitmapImage();
                await bmp.SetSourceAsync(stream);
                return bmp;
            }
        }

        private async void OnSelectImage(ImageItem selectedItem)
        {
            if (selectedItem == null) return;

            _selectedFile = StorageFile.GetFileFromPathAsync(selectedItem.Path).GetAwaiter().GetResult();

            var bitmapImage = new BitmapImage();
            using (var stream = await _selectedFile.OpenAsync(FileAccessMode.Read))
            {
                await bitmapImage.SetSourceAsync(stream);
            }

            ImageSource = bitmapImage;

            ImagePath = selectedItem.Path;

            _pageStateService.Set("SelectedImagePath", selectedItem.Path);

            _loggerService?.Info(nameof(MainPageViewModel), $"Image selected: {selectedItem.Path}");
        }

        private async Task SetWallpaperAsync()
        {
            if (_selectedFile == null)
            {
                _loggerService?.Info(nameof(MainPageViewModel), "No image selected. Wallpaper not set.");
                return;
            }

            if (!string.IsNullOrEmpty(ImagePath))
            {
                await _wallpaperService.SetWallpaperAsync(_selectedFile);

                var image = await _imageService.GetByHashAsync(await FileHasher.ComputeHashAsync(_selectedFile));
                await _wallpaperHistoryService.AddAsync(image.Id, "Manual Selection");
            }
        }
    }
}
