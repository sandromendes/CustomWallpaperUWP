using CustomWallpaper.Domain.Application;
using CustomWallpaper.Domain.Models;
using CustomWallpaper.Services.WallpaperHistories;
using Prism.Commands;
using Prism.Windows.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CustomWallpaper.ViewModels
{
    public class WallpaperHistoryPageViewModel : ViewModelBase
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
