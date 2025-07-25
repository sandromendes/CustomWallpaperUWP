namespace CustomWallpaper.Domain.Models
{
    public class WallpaperHistoryDto
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public string FileName { get; set; }
        public string AppliedAt { get; set; }
        public string Source { get; set; }
    }
}
