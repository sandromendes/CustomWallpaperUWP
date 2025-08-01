using CustomWallpaper.Domain.Models;
using CustomWallpaper.Domain.Services;
using CustomWallpaper.Navigation;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CustomWallpaper.ViewModels
{
    public class WallpaperHistoryPageViewModel : ViewModelBaseEx
    {
        private readonly IWallpaperHistoryService _historyService;

        public ObservableCollection<WallpaperHistoryDto> History { get; } = new ObservableCollection<WallpaperHistoryDto>();
        public DelegateCommand RefreshCommand { get; }

        public WallpaperHistoryPageViewModel(IWallpaperHistoryService historyService)
        {
            _historyService = historyService;
            RefreshCommand = new DelegateCommand(async () => await LoadAsync());
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            History.Clear();
            var items = await _historyService.GetHistoryWithImageNamesAsync();
            foreach (var h in items)
                History.Add(h);
        }
    }
}
