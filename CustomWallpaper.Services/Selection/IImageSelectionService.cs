using CustomWallpaper.Domain.Models;

namespace CustomWallpaper.Services.Selection
{
    public interface IImageSelectionService
    {
        ImageItem SelectedImage { get; set; }
    }
}
