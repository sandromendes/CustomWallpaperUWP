using Windows.UI.Xaml.Media.Imaging;

namespace CustomWallpaper.Domain.Models
{
    public class ImageItem
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public BitmapImage Thumbnail { get; set; }

        public string Id { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Hash { get; set; }
        public bool IsFavorite { get; set; }
    }

}
