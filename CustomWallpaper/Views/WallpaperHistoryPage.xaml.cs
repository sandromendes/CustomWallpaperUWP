﻿using CustomWallpaper.ViewModels;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace CustomWallpaper.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class WallpaperHistoryPage : Page
    {
        public WallpaperHistoryPageViewModel ViewModel => (WallpaperHistoryPageViewModel)DataContext;

        public WallpaperHistoryPage()
        {
            this.InitializeComponent();
        }
    }
}
