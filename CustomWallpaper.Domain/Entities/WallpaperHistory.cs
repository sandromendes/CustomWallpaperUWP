namespace CustomWallpaper.Domain.Entities
{
    public class WallpaperHistory
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public string FileName { get; set; }
        public string AppliedAt { get; set; }
        public string Source { get; set; }
    }

}
