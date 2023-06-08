namespace MediaImporter.Views
{
    using io.ecn.Encoder;
    using io.ecn.Importer;
    using io.ecn.Importer.Model;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using System;
    using System.Collections.Generic;
    using Windows.Storage.Pickers;

    public sealed partial class ImportPage : Page
    {
        private readonly EncodeManager encodeManager;
        List<Device> devices = new();
        List<Item> importItems;
        ImportManager importManager;

        #region UI

        public ImportPage()
        {
            this.devices = Utility.Get();
            this.InitializeComponent();

            if (this.devices != null && this.devices.Count > 0 )
            {
                this.DeviceComboBox.SelectedIndex = 0;
            }

            this.encodeManager = new EncodeManager();
        }

        private bool IsValid()
        {
            if (this.DeviceComboBox.SelectedIndex == -1)
            {
                return false;
            }

            if (this.ImportDestinationTextBox.Text == "")
            {
                return false;
            }

            return true;
        }

        private void SetScanButtonEnabled()
        {
            this.ScanButton.IsEnabled = IsValid();
        }

        private void DeviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetScanButtonEnabled();
        }

        private void RefreshDeviceButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.loadDevices();
            SetScanButtonEnabled();
        }

        private async void BrowseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            };

            var handle = WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App)?.Window as MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, handle);

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                ImportDestinationTextBox.Text = folder.Path;
            }

            SetScanButtonEnabled();
        }

        private void ScanButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.ScanButton.IsEnabled = false;
            this.ScanButtonDefaultLabel.Visibility = Visibility.Collapsed;
            this.ScanButtonPendingLabel.Visibility = Visibility.Visible;
            this.importManager = new ImportManager(this.ImportDestinationTextBox.Text, this.devices[this.DeviceComboBox.SelectedIndex]);
            this.ScanMedia();
        }

        private void ImportButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.ImportButton.IsEnabled = false;
            this.ImportMedia();
        }

        #endregion

        private void loadDevices()
        {
            this.devices = Utility.Get();
            if (this.devices == null || this.devices.Count == 0)
            {
                this.DeviceComboBox.SelectedIndex = -1;
            }
        }

        private async void ScanMedia()
        {
            var items = await this.importManager.FindMedia();
            this.importItems = items;
            (Application.Current as App)?.Window.DispatcherQueue.TryEnqueue(() =>
            {
                this.ScanButtonPendingLabel.Visibility = Visibility.Collapsed;
                this.ScanButtonFinishedLabel.Visibility = Visibility.Visible;
                this.ScanButtonFinishedLabelText.Text = $"Found {this.importItems.Count} items";
                this.ImportButton.IsEnabled = true;
            });
        }

        private async void ImportMedia()
        {
            var progress = new Progress<int>();
            progress.ProgressChanged += (object sender, int e) =>
            {
                (Application.Current as App)?.Window.DispatcherQueue.TryEnqueue(() =>
                {
                    this.ImportProgressRing.Value = (e / this.importItems.Count) * 100;
                    this.ImportProgressStatus.Text = $"Importing ({e}/{this.importItems.Count})";
                });
            };

            List<string> paths = await this.importManager.ImportMedia(this.importItems, progress);

            if (!Preferences.ConvertHeic)
            {
                (Application.Current as App)?.Window.DispatcherQueue.TryEnqueue(() =>
                {
                    Finished();
                });
                return;
            }

            this.EncodeMedia(paths);
        }

        private async void EncodeMedia(List<string> paths)
        {
            var progress = new Progress<int>();
            progress.ProgressChanged += (object sender, int e) =>
            {
                (Application.Current as App)?.Window.DispatcherQueue.TryEnqueue(() =>
                {
                    this.ImportProgressRing.Value = (e / this.importItems.Count) * 100;
                    this.ImportProgressStatus.Text = $"Encoding ({e}/{this.importItems.Count})";
                });
            };

            await this.encodeManager.EncodeMedia(paths, progress);
            (Application.Current as App)?.Window.DispatcherQueue.TryEnqueue(() =>
            {
                Finished();
            });
        }

        private void Finished()
        {
            this.ImportButtonDefaultLabel.Visibility = Visibility.Collapsed;
            this.ImportButtonFinishedLabel.Visibility = Visibility.Visible;
            this.ImportButtonFinishedText.Text = $"Imported {this.importItems.Count} items";
            this.ImportProgress.Visibility = Visibility.Collapsed;
        }
    }
}
