using CustomWallpaper.Domain.Entities;
using CustomWallpaper.Domain.Services;
using CustomWallpaper.Navigation;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CustomWallpaper.ViewModels
{
    public class SavedImagesListPageViewModel : ViewModelBaseEx
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

        public override async Task OnShowAsync(object parameter = null)
        {
            await LoadAsync();
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
