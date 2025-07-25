using CustomWallpaper.ViewModels;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace CustomWallpaper.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public AppShellViewModel ViewModel => (AppShellViewModel)DataContext;
        public Frame NavigationFrame => ContentFrame;

        public AppShell()
        {
            InitializeComponent();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                // TODO: Menu de configurações
                // ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                var pageTag = selectedItem.Tag.ToString();

                switch (pageTag)
                {
                    case nameof(MainPage):
                        ContentFrame.Navigate(typeof(MainPage));
                        break;
                    case nameof(PicturesGridPage):
                        ContentFrame.Navigate(typeof(PicturesGridPage));
                        break;
                    case nameof(SavedImagesListPage):
                        ContentFrame.Navigate(typeof(SavedImagesListPage));
                        break;
                    case nameof(WallpaperHistoryPage):
                        ContentFrame.Navigate(typeof(WallpaperHistoryPage));
                        break;
                }
            }
        }
    }
}
