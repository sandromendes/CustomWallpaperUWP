using Prism.Events;
using Windows.Storage;

namespace CustomWallpaper.CrossCutting.Events
{
    public class FolderImportedEvent : PubSubEvent<StorageFolder> { }
}
