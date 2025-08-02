using Windows.UI.Xaml.Media.Imaging;

namespace CustomWallpaper.Domain.Models
{
    public class ImageItem
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public BitmapImage Thumbnail { get; set; }

        //Database
        public string Id { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Proportion { get => $"{Width} x {Height}"; }
        public string Hash { get; set; }
        public bool IsFavorite { get; set; }

        //EXIF
        public ushort Orientation { get; set; }
        public string CameraModel { get; set; }
        public string CameraManufacturer { get; set; }
        public string DateTaken { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        public double? DpiX { get; set; }
        public double? DpiY { get; set; }
        public uint? BitDepth { get; set; }
    }

}
