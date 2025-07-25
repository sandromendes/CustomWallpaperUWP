using CustomWallpaper.Domain.Entities;
using CustomWallpaper.Services.Images;
using Prism.Commands;
using Prism.Windows.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CustomWallpaper.ViewModels
{
    public class SavedImagesListPageViewModel : ViewModelBase
    {
        private readonly IImageService _imageService;

        public ObservableCollection<Image> Images { get; } = new ObservableCollection<Image>();
        public DelegateCommand RefreshCommand { get; }

        public SavedImagesListPageViewModel()
        {
        }

        public SavedImagesListPageViewModel(IImageService imageService)
        {
            _imageService = imageService;
            RefreshCommand = new DelegateCommand(async () => await LoadAsync());
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            Images.Clear();
            var items = await _imageService.GetAllAsync();
            foreach (var img in items)
                Images.Add(img);
        }
    }
}
