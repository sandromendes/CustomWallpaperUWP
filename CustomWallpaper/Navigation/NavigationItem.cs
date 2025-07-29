using System;

namespace CustomWallpaper.Navigation
{
    public class NavigationItem
    {
        public string Key { get; }
        public Type PageType { get; }
        public object Parameter { get; }

        public NavigationItem(string key, Type pageType, object parameter = null)
        {
            Key = key;
            PageType = pageType;
            Parameter = parameter;
        }
    }
}
