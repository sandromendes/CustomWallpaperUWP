using System;

namespace CustomWallpaper.Domain.Entities
{
    public class Folder
    {
        public string Id { get; set; }
        public string FolderPath { get; set; }
        public string FolderName { get; set; }
        public DateTime DateAdded { get; set; }
        public string AccessToken { get; set; }
    }
}
