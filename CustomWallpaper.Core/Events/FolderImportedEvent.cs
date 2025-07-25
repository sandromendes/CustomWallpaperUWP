using Prism.Events;
using Windows.Storage;

namespace CustomWallpaper.Core.Events
{
    public class FolderImportedEvent : PubSubEvent<StorageFolder> { }
}
