﻿using CustomWallpaper.CrossCutting.Events;
using CustomWallpaper.CrossCutting.Services;
using CustomWallpaper.Domain.Services;
using CustomWallpaper.Navigation;
using CustomWallpaper.Services.BackgroundTasks;
using CustomWallpaper.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace CustomWallpaper.ViewModels
{
    public class AppShellViewModel : ViewModelBase
    {
        private readonly INavigationServiceEx _navigationServiceEx;
        private readonly ILoggerService _loggerService;

        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly ISmartEngineService _smartEngineService;

        private readonly IEventAggregator _eventAggregator;

        // Commands
        public DelegateCommand ImportFolderCommand { get; }
        public DelegateCommand ExportHistoryCommand { get; }
        public DelegateCommand ExportLogsCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand RunManualAnalysisCommand { get; }
        public DelegateCommand StartDbStressCommand { get; }
        public DelegateCommand StopDbStressCommand { get; }
        public DelegateCommand StartSmartEngineCommand { get; }
        public DelegateCommand StopSmartEngineCommand { get; }

        public DelegateCommand<NavigationViewItemInvokedEventArgs> NavigateCommand { get; }

        // Crash
        public DelegateCommand ForceCrashCommand { get; }

        public AppShellViewModel()
        {
                
        }

        public AppShellViewModel(
            INavigationServiceEx navigationServiceEx,
            ILoggerService loggerService,
            IBackgroundTaskService backgroundTaskService,
            ISmartEngineService smartEngineService,
            IEventAggregator eventAggregator)
        {
            _navigationServiceEx = navigationServiceEx;
            _backgroundTaskService = backgroundTaskService;
            _smartEngineService = smartEngineService;
            _loggerService = loggerService;

            _eventAggregator = eventAggregator;

            ImportFolderCommand = new DelegateCommand(ImportFolder);
            ExportHistoryCommand = new DelegateCommand(ExportHistory);
            ExportLogsCommand = new DelegateCommand(ExportLogs);
            ExitCommand = new DelegateCommand(ExitApplication);
            RunManualAnalysisCommand = new DelegateCommand(RunManualAnalysis);
            StartDbStressCommand = new DelegateCommand(StartDbStressAsync);
            StopDbStressCommand = new DelegateCommand(StopDbStressAsync);
            StartSmartEngineCommand = new DelegateCommand(StartSmartEngineAsync);
            StopSmartEngineCommand = new DelegateCommand(StopSmartEngine);

            // Crash
            ForceCrashCommand = new DelegateCommand(ForceCrash);

            // Navigate
            NavigateCommand = new DelegateCommand<NavigationViewItemInvokedEventArgs>(OnNavigateAsync);
        }

        private async void OnNavigateAsync(NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                string key = item.Tag?.ToString();

                if (!string.IsNullOrEmpty(key))
                {
                    Type pageType = ResolvePageTypeFromKey(key);
                    var navItem = new NavigationItem(key, pageType);
                    await _navigationServiceEx.NavigateAsync(navItem);
                }
            }
        }

        private Type ResolvePageTypeFromKey(string key)
        {
            switch (key)
            {
                case nameof(PicturesGridPage):
                    return typeof(PicturesGridPage);
                case nameof(SavedImagesListPage):
                    return typeof(SavedImagesListPage);
                case nameof(WallpaperHistoryPage):
                    return typeof(WallpaperHistoryPage);
                case nameof(MainPage):
                    return typeof(MainPage);
                default:
                    throw new ArgumentException($"Unknown navigation key: {key}");
            }
        }


        private async void ImportFolder()
        {
            try
            {
                _loggerService.Info(nameof(AppShellViewModel), "Folder import process has started.");

                FolderPicker picker = new FolderPicker
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add("*");

                StorageFolder folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                    _eventAggregator.GetEvent<FolderImportedEvent>().Publish(folder);

                    _loggerService.Info(nameof(AppShellViewModel), "Folder successfully selected and published.");
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error(nameof(AppShellViewModel), ex, "Failed to import folder.");
            }
        }

        private void ExportHistory()
        {
            try
            {
                // TODO: Implement history export logic
                _loggerService.Info(nameof(AppShellViewModel), "History export process has started.");
            }
            catch (Exception ex)
            {
                _loggerService.Error("Something went wrong while exporting history.", ex);
            }
        }

        private async void ExportLogs()
        {
            try
            {
                _loggerService.Info(nameof(AppShellViewModel), "Starting log export process...");

                var localFolder = ApplicationData.Current.LocalFolder;
                var logFiles = await localFolder.GetFilesAsync();

                var destinationFolder = await KnownFolders.DocumentsLibrary.CreateFolderAsync("CustomWallpaper_Logs", CreationCollisionOption.OpenIfExists);

                foreach (var file in logFiles)
                {
                    if (IsCustomLogFile(file))
                    {
                        await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
                        _loggerService.Info(nameof(AppShellViewModel), $"Log file '{file.Name}' successfully exported.");
                    }
                }

                _loggerService.Info(nameof(AppShellViewModel), "Log export process completed successfully.");

                var dialog = new ContentDialog
                {
                    Title = "Logs Exported",
                    Content = $"Log files were successfully exported to:\n{destinationFolder.Path}",
                    PrimaryButtonText = "Copy Path",
                    CloseButtonText = "Close",
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                    dataPackage.SetText(destinationFolder.Path);
                    Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                    _loggerService.Info(nameof(AppShellViewModel), "Log path copied to clipboard.");
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error(nameof(AppShellViewModel), ex, "An error occurred while exporting logs.");
            }
        }

        private bool IsCustomLogFile(StorageFile file)
        {
            var name = file.Name;
            return (name.StartsWith("CustomWallpaper_") || name.StartsWith("CustomWallpaperBgTask_"))
                   && name.EndsWith(".log", StringComparison.OrdinalIgnoreCase);
        }

        private void ExitApplication()
        {
            try
            {
                _loggerService.Info(nameof(AppShellViewModel), "Application exit requested.");
                Process.GetCurrentProcess().Kill();
                // App.Current.Exit();
            }
            catch (Exception ex)
            {
                _loggerService.Error("Failed to close the application properly.", ex);
            }
        }

        private async void RunManualAnalysis()
        {
            try
            {
                _loggerService.Info(nameof(AppShellViewModel), "Manual analysis requested — starting now.");
                await _smartEngineService.RunAnalysisAsync();
                _loggerService.Info(nameof(AppShellViewModel), "Manual analysis has started successfully.");
            }
            catch (Exception ex)
            {
                _loggerService.Error("Something went wrong during manual analysis execution.", ex);
            }
        }

        private async void StartDbStressAsync()
        {
            try
            {
                _loggerService.Info(nameof(AppShellViewModel), "Requesting start of database stress routine...");
                await _backgroundTaskService.RegisterStressTaskAsync();
                _loggerService.Info(nameof(AppShellViewModel), "Database stress routine started successfully.");
            }
            catch (Exception ex)
            {
                _loggerService.Error("Failed to start database stress routine.", ex);
            }
        }

        private async void StopDbStressAsync()
        {
            try
            {
                _loggerService.Info(nameof(AppShellViewModel), "Requesting stop of database stress routine...");
                await _backgroundTaskService.UnregisterStressTaskAsync();
                _loggerService.Info(nameof(AppShellViewModel), "Database stress routine stopped.");
            }
            catch (Exception ex)
            {
                _loggerService.Error("Failed to stop database stress routine.", ex);
            }
        }

        private async void StartSmartEngineAsync()
        {
            try
            {
                _loggerService.Info(nameof(AppShellViewModel), "Requesting start of Smart Engine routine...");
                await _backgroundTaskService.RegisterSmartEngineTaskAsync();
                _loggerService.Info(nameof(AppShellViewModel), "Smart Engine started successfully.");
            }
            catch (Exception ex)
            {
                _loggerService.Error("Failed to start Smart Engine.", ex);
            }
        }

        private async void StopSmartEngine()
        {
            try
            {
                _loggerService.Info(nameof(AppShellViewModel), "Requesting stop of Smart Engine routine...");
                await _backgroundTaskService.UnregisterSmartEngineTaskAsync();
                _loggerService.Info(nameof(AppShellViewModel), "Smart Engine stopped.");
            }
            catch (Exception ex)
            {
                _loggerService.Error("Failed to stop Smart Engine.", ex);
            }
        }

        private void ForceCrash()
        {
            _loggerService?.Fatal(nameof(AppShellViewModel), 
                new InvalidOperationException("This is a deliberate crash for testing purposes."), 
                "Forced crash initiated by user.");

            throw new InvalidOperationException("This is a deliberate crash for testing purposes.");
        }
    }
}
