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

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ViewModel.NavigateCommand.Execute(args);
        }
    }
}
