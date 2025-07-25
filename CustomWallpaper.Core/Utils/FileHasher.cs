using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Storage;

namespace CustomWallpaper.Core.Utils
{
    public static class FileHasher
    {
        public static async Task<string> ComputeHashAsync(StorageFile file)
        {
            using (var stream = await file.OpenStreamForReadAsync())
            {
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }

}
