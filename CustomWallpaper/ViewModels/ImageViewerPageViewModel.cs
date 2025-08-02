using CustomWallpaper.Domain.Models;
using CustomWallpaper.Domain.Services;
using CustomWallpaper.Navigation;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;

namespace CustomWallpaper.ViewModels
{
    public class PropertyItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ImageViewerPageViewModel : ViewModelBaseEx
    {
        private ImageItem _image;
        private readonly IImageService _imageService;

        public ImageItem Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public ObservableCollection<PropertyItem> ImageProperties { get; } = new ObservableCollection<PropertyItem>();

        public ImageViewerPageViewModel(IImageService imageService)
        {
            _imageService = imageService;
        }

        public override async Task OnShowAsync(object parameter = null)
        {
            if (parameter is ImageItem image)
            {
                var imageEntity = await _imageService.GetByPathAsync(image.Path);

                var file = await StorageFile.GetFileFromPathAsync(image.Path);

                var imageProps = await file.Properties.GetImagePropertiesAsync();
                var basicProps = await file.GetBasicPropertiesAsync();

                var extraProps = await file.Properties.RetrievePropertiesAsync(new[] {
                    "System.Photo.Orientation",
                    "System.Photo.CameraModel",
                    "System.Photo.CameraManufacturer",
                    "System.Photo.DateTaken",
                    "System.GPS.Latitude",
                    "System.GPS.Longitude",
                    "System.Image.HorizontalSize",
                    "System.Image.VerticalSize",
                    "System.Image.HorizontalResolution",
                    "System.Image.VerticalResolution",
                    "System.Image.BitDepth"
                });

                Image = new ImageItem
                {
                    Name = image.Name,
                    Path = image.Path,
                    Thumbnail = image.Thumbnail,

                    Id = imageEntity?.Id,
                    FileExtension = imageEntity?.FileExtension,
                    FileSizeInBytes = imageEntity?.FileSizeInBytes ?? 0,
                    DateCreated = imageEntity?.DateCreated,
                    DateModified = imageEntity?.DateModified,
                    Width = imageEntity?.Width ?? 0,
                    Height = imageEntity?.Height ?? 0,
                    Hash = imageEntity?.Hash,
                    IsFavorite = imageEntity?.IsFavorite ?? false,

                    Orientation = GetOrientation(extraProps, "System.Photo.Orientation"),
                    CameraModel = GetPropertyValue<string>(extraProps, "System.Photo.CameraModel"),
                    CameraManufacturer = GetPropertyValue<string>(extraProps, "System.Photo.CameraManufacturer"),
                    DateTaken = GetDateTimeOffsetString(extraProps, "System.Photo.DateTaken"),
                    Latitude = GetPropertyValue<double?>(extraProps, "System.GPS.Latitude"),
                    Longitude = GetPropertyValue<double?>(extraProps, "System.GPS.Longitude"),
                    DpiX = GetPropertyValue<double?>(extraProps, "System.Image.HorizontalResolution"),
                    DpiY = GetPropertyValue<double?>(extraProps, "System.Image.VerticalResolution"),
                    BitDepth = GetPropertyValue<uint?>(extraProps, "System.Image.BitDepth")
                };

                LoadPropertiesFromImageItem(Image);
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            
            if (e.Parameter is ImageItem image)
            {

                Image = image;
            }
        }

        private T GetPropertyValue<T>(IDictionary<string, object> props, string key, T defaultValue = default)
        {
            if (props.TryGetValue(key, out object value) && value is T typedValue)
                return typedValue;
            return defaultValue;
        }

        private string GetDateTimeOffsetString(IDictionary<string, object> props, string key, string format = "dd/MM/yyyy HH:mm")
        {
            if (props.TryGetValue(key, out object value) && value is DateTimeOffset dto)
                return dto.ToString(format);
            return null;
        }

        private ushort GetOrientation(IDictionary<string, object> props, string key)
        {
            if (props.TryGetValue(key, out object value) && value is int i)
                return (ushort)i;
            return 0;
        }

        public void LoadPropertiesFromImageItem(ImageItem image)
        {
            ImageProperties.Clear();

            if (image == null)
                return;

            var props = typeof(ImageItem).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                try
                {
                    object value = prop.GetValue(image);
                    string stringValue = value?.ToString() ?? "(null)";

                    if(prop.Name != nameof(ImageItem.Thumbnail))
                    {
                        ImageProperties.Add(new PropertyItem
                        {
                            Key = prop.Name,
                            Value = stringValue
                        });
                    }
                }
                catch
                {
                    ImageProperties.Add(new PropertyItem
                    {
                        Key = prop.Name,
                        Value = "[Error reading value]"
                    });
                }
            }
        }

    }
}
