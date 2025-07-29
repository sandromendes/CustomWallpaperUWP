using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CustomWallpaper.Navigation
{
    public class NavigationServiceEx : INavigationServiceEx
    {
        private Frame _frame;

        public NavigationServiceEx()
        {
        }

        public void SetFrame(Frame frame)
        {
            _frame = frame;
        }

        public async Task NavigateAsync(NavigationItem item)
        {
            if (_frame.CurrentSourcePageType == item.PageType)
                return;

            bool navigated = _frame.Navigate(item.PageType, item.Parameter);

            if (!navigated)
                throw new Exception($"Navigation to {item.PageType.Name} failed.");

            await Task.Delay(50);

            var page = _frame.Content as Page;
            if (page?.DataContext is INavigationAwareEx aware)
            {
                await aware.OnShowAsync(item.Parameter);
            }
        }
    }
}
