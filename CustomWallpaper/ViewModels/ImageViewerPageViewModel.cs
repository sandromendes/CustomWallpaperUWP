using CustomWallpaper.Domain.Models;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System.Collections.Generic;

namespace CustomWallpaper.ViewModels
{
    public class ImageViewerPageViewModel : ViewModelBase
    {
        private ImageItem _image;
        public ImageItem Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
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
