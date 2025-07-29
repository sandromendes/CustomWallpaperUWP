using CustomWallpaper.Domain.Models;
using CustomWallpaper.ViewModels;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace CustomWallpaper.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class PicturesGridPage : Page
    {
        public PicturesGridPageViewModel ViewModel => (PicturesGridPageViewModel)DataContext;

        public PicturesGridPage()
        {
            InitializeComponent();
        }

        private void GridViewElementName_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataContext is PicturesGridPageViewModel vm && e.ClickedItem is ImageItem image)
            {
                vm.NavigateToImageViewerCommand.Execute(image);
            }
        }
    }
}
