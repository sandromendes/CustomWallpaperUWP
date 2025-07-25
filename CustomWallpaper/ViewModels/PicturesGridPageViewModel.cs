using CustomWallpaper.Domain.Models;
using CustomWallpaper.Services.Images;
using CustomWallpaper.Services.States;
using CustomWallpaper.Services.Wallpapers;
using Prism.Windows.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using Prism.Commands;
using CustomWallpaper.Core.Events;
using Prism.Events;
using Prism.Windows.Navigation;
using System.Diagnostics;
using CustomWallpaper.Core.Services;

namespace CustomWallpaper.ViewModels
{
    public class PicturesGridPageViewModel : ViewModelBase
    {
        private readonly IWallpaperService _wallpaperService;
        private readonly IImageService _imageService;
        private readonly IPageStateService _pageStateService;
        private readonly INavigationService _navigationService;
        private readonly ILoggerService _loggerService;

        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<ImageItem> Images { get; set; } = new ObservableCollection<ImageItem>();

        private ImageItem _selectedItem;
        public ImageItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public DelegateCommand<ImageItem> CopyCommand { get; }
        public DelegateCommand<ImageItem> SetAsWallpaperCommand { get; }

        public PicturesGridPageViewModel(
            IImageService imageService,
            IWallpaperService wallpaperService,
            IPageStateService pageStateService,
            INavigationService navigationService,
            ILoggerService loggerService,
            IEventAggregator eventAggregator)
        {
            _imageService = imageService;
            _wallpaperService = wallpaperService;
            _pageStateService = pageStateService;
            _navigationService = navigationService;
            _loggerService = loggerService;

            _eventAggregator = eventAggregator;

            CopyCommand = new DelegateCommand<ImageItem>(CopyImage);
            SetAsWallpaperCommand = new DelegateCommand<ImageItem>(SetAsWallpaper);

            _eventAggregator.GetEvent<FolderImportedEvent>().Subscribe(async folder =>
            {
                await LoadImagesFromFolderAsync(folder);
            });
        }

        public async Task LoadImagesFromFolderAsync(StorageFolder folder)
        {
            if (folder == null)
                return;

            StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

            Images.Clear();

            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                var ext = file.FileType.ToLowerInvariant();
                if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".bmp")
                {
                    await _imageService.AddOrUpdateFromFileAsync(file);

                    var thumb = await LoadThumbnailAsync(file);
                    Images.Add(new ImageItem
                    {
                        Name = file.Name,
                        Path = file.Path,
                        Thumbnail = thumb
                    });
                }
            }

            if (Images.Any())
                _pageStateService.Set("ImageThumbnails", Images.ToList());
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

        private async void CopyImage(ImageItem item)
        {
            if (item != null)
                await _imageService.CopyToClipboardAsync(item.Path);
        }

        private async void SetAsWallpaper(ImageItem item)
        {
            if (item != null)
            {
                var file = await StorageFile.GetFileFromPathAsync(item.Path);
                await _wallpaperService.SetWallpaperAsync(file);
            }
        }

        public void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ImageItem image)
            {
                var result = _navigationService.Navigate("ImageViewer", image);
                _loggerService.Info(nameof(PicturesGridPageViewModel),$"Navigation to ImageViewerPage: {result}");
            }
        }
    }
}

