using Windows.UI.Xaml.Media.Imaging;

namespace CustomWallpaper.Domain.Models
{
    public class ImageItem
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }
}
