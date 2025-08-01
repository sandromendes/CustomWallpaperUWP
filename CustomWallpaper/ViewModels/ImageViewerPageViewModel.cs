using CustomWallpaper.Domain.Models;
using CustomWallpaper.Domain.Services;
using CustomWallpaper.Navigation;
using Prism.Windows.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace CustomWallpaper.ViewModels
{
    public class ImageViewerPageViewModel : ViewModelBaseEx
    {
        private ImageItem _image;
        private readonly IImageService _imageService;

        public ImageItem Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public ImageViewerPageViewModel(IImageService imageService)
        {
            _imageService = imageService;
        }

        public override async Task OnShowAsync(object parameter = null)
        {
            if (parameter is ImageItem image)
            {
                var imageEntity = await _imageService.GetByPathAsync(image.Path);

                Image = new ImageItem
                {
                    Name = image.Name,
                    Path = image.Path,
                    Thumbnail = image.Thumbnail,

                    Id = imageEntity?.Id,
                    FileExtension = imageEntity?.FileExtension,
                    FileSizeInBytes = imageEntity?.FileSizeInBytes ?? 0,
                    DateCreated = imageEntity?.DateCreated,
                    DateModified = imageEntity?.DateModified,
                    Width = imageEntity?.Width ?? 0,
                    Height = imageEntity?.Height ?? 0,
                    Hash = imageEntity?.Hash,
                    IsFavorite = imageEntity?.IsFavorite ?? false
                };
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            
            if (e.Parameter is ImageItem image)
            {

                Image = image;
            }
        }
    }
}
