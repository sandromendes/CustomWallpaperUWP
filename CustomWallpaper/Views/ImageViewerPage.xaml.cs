using CustomWallpaper.ViewModels;
using System;
using System.Linq;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace CustomWallpaper.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class ImageViewerPage : Page
    {
        public ImageViewerPageViewModel ViewModel => (ImageViewerPageViewModel)DataContext;

        public ImageViewerPage()
        {
            InitializeComponent();
        }

        private void CopyValueButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is string valueToCopy)
                {
                    var dataPackage = new DataPackage();
                    dataPackage.SetText(valueToCopy);
                    Clipboard.SetContent(dataPackage);
                    Clipboard.Flush();

                    ShowToast("Copied", "Value copied to clipboard");
                }
            }
            catch (Exception ex)
            {
                ShowToast("Error copying value", ex.Message);
            }
        }

        private void ExportToJsonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.ImageProperties != null)
                {
                    var dict = ViewModel.ImageProperties.ToDictionary(item => item.Key, item => item.Value);

                    var json = JsonSerializer.Serialize(dict, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    var dataPackage = new DataPackage();
                    dataPackage.SetText(json);
                    Clipboard.SetContent(dataPackage);
                    Clipboard.Flush();

                    ShowToast("Exported as JSON", "The content has been copied to the clipboard.");
                }
            }
            catch (Exception ex)
            {
                ShowToast("Error exporting JSON", ex.Message);
            }
        }

        private void ShowToast(string title, string message)
        {
            try
            {
                var xmlToastTemplate = $@"
                        <toast activationType='foreground'>
                            <visual>
                                <binding template='ToastGeneric'>
                                    <text>{title}</text>
                                    <text>{message}</text>
                                </binding>
                            </visual>
                        </toast>";

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlToastTemplate);

                var toast = new ToastNotification(xmlDoc);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch
            {
                // Fail silently if toast display fails
            }
        }
    }
}
