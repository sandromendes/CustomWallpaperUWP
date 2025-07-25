using CustomWallpaper.Domain.Models;

namespace CustomWallpaper.Services.Selection
{
    public class ImageSelectionService : IImageSelectionService
    {
        public ImageItem SelectedImage { get; set; }
    }
}
