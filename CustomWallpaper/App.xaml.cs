using Prism.Unity.Windows;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel;
using Microsoft.Practices.Unity;
using CustomWallpaper.Core.Services;
using CustomWallpaper.Services.Wallpapers;
using CustomWallpaper.Services.BackgroundTasks;
using CustomWallpaper.Services.WallpaperHistories;
using CustomWallpaper.Services.Images;
using CustomWallpaper.Infrastructure.Repositories;
using CustomWallpaper.Infrastructure;
using CustomWallpaper.Domain.Application;
using Prism.Mvvm;
using CustomWallpaper.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using CustomWallpaper.Services.SmartEngine;
using CustomWallpaper.Services.States;
using Prism.Events;

namespace CustomWallpaper
{
    /// <summary>
    ///Fornece o comportamento específico do aplicativo para complementar a classe Application padrão.
    /// </summary>
    sealed partial class App : PrismUnityApplication
    {
        private ILoggerService _logger;

        /// <summary>
        /// Inicializa o objeto singleton do aplicativo. Essa é a primeira linha do código criado
        /// executado e, por isso, é o equivalente lógico de main() ou WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Suspending += OnSuspending;
            Resuming += OnResuming;
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            try
            {
                _logger.Info(nameof(App), "Application suspending.");

                var deferral = e.SuspendingOperation.GetDeferral();

                deferral.Complete();

                _logger.Info(nameof(App), "Application suspended successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(App), ex, "Error during application suspending.");
            }
        }

        private void OnResuming(object sender, object e)
        {
            try
            {
                _logger.Info(nameof(App), "Application resuming.");

                _logger.Info(nameof(App), "Application resumed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(App), ex, "Error during application resuming.");
            }
        }

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            try
            {
                _logger = Container.Resolve<ILoggerService>();
                _logger.Info(nameof(App), "Application initialized.");

                SQLitePCL.Batteries.Init();
                await DatabaseBootstrapper.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logger initialization failed: {ex.Message}");
            }

            NavigationService.Navigate("PicturesGrid", null);
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Windows.UI.Xaml.Application.Current.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            return base.OnInitializeAsync(args);
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Log exception
            _logger?.Fatal(nameof(App), e.Exception, "Unhandled UI exception");
            //e.Handled = true; // avoid crash
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Log background exception
            _logger?.Fatal(nameof(App), e.Exception, "Unobserved task exception");
            e.SetObserved();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<MainPage>();
            Container.RegisterType<SavedImagesListPage>();
            Container.RegisterType<WallpaperHistoryPage>();
            Container.RegisterType<ImageViewerPage>();

            RegisterTypeIfMissing(typeof(IWallpaperService), typeof(WallpaperService), false);
            RegisterTypeIfMissing(typeof(ILoggerService), typeof(LoggerService), false);
            RegisterTypeIfMissing(typeof(IPageStateService), typeof(PageStateService), true); //singleton
            RegisterTypeIfMissing(typeof(IBackgroundTaskService), typeof(BackgroundTaskService), false);
            RegisterTypeIfMissing(typeof(ISmartEngineService), typeof(SmartEngineService), false);
            RegisterTypeIfMissing(typeof(IWallpaperHistoryService), typeof(WallpaperHistoryService), false);
            RegisterTypeIfMissing(typeof(IImageService), typeof(ImageService), false);

            RegisterTypeIfMissing(typeof(IImageRepository), typeof(ImageRepository),false);
            RegisterTypeIfMissing(typeof(IWallpaperHistoryRepository), typeof(WallpaperHistoryRepository),false);

            RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true); //singleton
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<AppShell, ViewModels.AppShellViewModel>();
            ViewModelLocationProvider.Register<SavedImagesListPage, ViewModels.SavedImagesListPageViewModel>();
            ViewModelLocationProvider.Register<WallpaperHistoryPage, ViewModels.WallpaperHistoryPageViewModel>();
            ViewModelLocationProvider.Register<PicturesGridPage, ViewModels.PicturesGridPageViewModel>();
            ViewModelLocationProvider.Register<ImageViewerPage, ViewModels.ImageViewerPageViewModel>();

        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.Resolve<AppShell>();
            shell.NavigationFrame.Navigate(typeof(PicturesGridPage));
            return shell;
        }
    }
}
