namespace CustomWallpaper.Domain.Entities
{
    public class Image
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
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
