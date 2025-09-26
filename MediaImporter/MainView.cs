namespace io.ecn.MediaImporter
{
    using io.ecn.Encoder;
    using io.ecn.Importer;
    using io.ecn.Importer.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    public partial class MainView : Form
    {
        private readonly static Logger logger = new Logger(typeof(MainView));
        private readonly BackgroundWorker importWorker = new BackgroundWorker();
        private readonly EncodeManager encodeManager = new EncodeManager();
        private readonly ImportManager importManager = new ImportManager();
        List<Device> devices = new();
        List<Item> itemsToImport = new();
        Device? importDevice;
        string? importDestination;
        bool firstLoadComplete = false;

        public MainView()
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            InitializeComponent();

            if (!Preferences.TipShown)
            {
                MessageBox.Show($"For best results, disable automatic conversion on iOS.\r\n\r\n1. Open the \"Settings\" app.\r\n2. Go to \"Apps\" then \"Photos\"\r\n3. Set \"Transfer to Mac or PC\" to \"Keep Originals\".\r\nMedia Importer will manage conversions automatically.", "Media Importer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Preferences.TipShown = true;
            }

            importWorker.DoWork += new DoWorkEventHandler(ImportItems);
            importWorker.ProgressChanged += new ProgressChangedEventHandler(ImportProgress);
            importWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ImportComplete);
            importWorker.WorkerReportsProgress = true;

            this.destinationTextBox.Text = Preferences.DefaultDirectory;
        }

        private async void DeviceReloadButtonClick(object? sender, EventArgs e)
        {
            await this.LoadDevices();
        }

        private void DeviceComboBoxSelectedIndexChanged(object? sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void DestinationBrowseButtonClick(object? sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Imported Media Destination";
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            this.destinationTextBox.Text = dialog.SelectedPath;
            ValidateInputs();
        }

        private void DestinationTextBoxTextChanged(object? sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void ValidateInputs()
        {
            if (this.deviceComboBox.SelectedIndex == -1)
            {
                this.scanButton.Enabled = false;
                return;
            }
            if (this.destinationTextBox.Text == "")
            {
                this.scanButton.Enabled = false;
                return;
            }
            this.scanButton.Enabled = true;
        }

        private async void ScanButtonClick(object? sender, EventArgs e)
        {
            this.importDevice = this.devices[this.deviceComboBox.SelectedIndex];
            this.importDestination = this.destinationTextBox.Text;

            this.setControlsEnabled(false);

            var items = await this.ScanMedia();
            this.itemsToImport = items;

            this.setControlsEnabled(true);

            if (items.Count == 0)
            {
                MessageBox.Show("No media items found on selected device", "Media Importer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Found {items.Count} items. Start import?", "Media Importer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            Preferences.DefaultDirectory = this.destinationTextBox.Text;

            this.progressBar.Visible = true;
            this.progressBar.Value = 0;
            this.progressLabel.Visible = true;
            this.scanButton.Visible = false;
            this.optionsButton.Visible = false;

            importWorker.RunWorkerAsync();
        }

        private void OptionsButtonClick(object? sender, EventArgs e)
        {
            var view = new PreferencesView();
            view.ShowDialog();
        }

        void ImportProgress(object? sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            string? message = (string?)e.UserState;
            if (message != null && message.Length > 0)
            {
                this.progressLabel.Text = message;
            }
        }

        void ImportComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar.Visible = false;
            this.progressLabel.Visible = false;
            this.scanButton.Visible = true;
            this.optionsButton.Visible = true;

            MessageBox.Show($"Imported {this.itemsToImport.Count} items", "Media Importer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task LoadDevices()
        {
            this.deviceComboBox.Enabled = false;
            this.deviceReloadButton.Enabled = false;
            this.deviceReloadButton.Text = "Loading...";
            this.Cursor = Cursors.WaitCursor;

            List<Device>? devices;
            try
            {
                devices = await Task.Run(Utility.Get);
                this.devices = devices;
            }
            catch (Exception ex)
            {
                this.deviceComboBox.Enabled = true;
                this.deviceReloadButton.Enabled = true;
                this.deviceReloadButton.Text = "Reload";
                this.Cursor = Cursors.Default;

                logger.Error($"Error getting devices: {ex}");
                MessageBox.Show($"Error loading devices:\r\n{ex}", "Media Importer");
                return;
            }

            this.deviceComboBox.Items.Clear();
            if (devices == null || devices.Count == 0)
            {
                this.deviceComboBox.SelectedIndex = -1;
            }
            else
            {
                foreach (Device device in devices)
                {
                    var key = $"{device.Label} - {device.Guid}";
                    if (!this.deviceComboBox.Items.Contains(key))
                    {
                        this.deviceComboBox.Items.Add(key);
                    }
                }
                for (int i = this.deviceComboBox.Items.Count - 1; i >= 0; i--)
                {
                    string? item = this.deviceComboBox.Items[i] as string;
                    bool found = false;
                    foreach (Device device in devices)
                    {
                        var key = $"{device.Label} - {device.Guid}";
                        if (key == item)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        deviceComboBox.Items.RemoveAt(i);
                    }
                }

                if (this.deviceComboBox.SelectedIndex == -1)
                {
                    this.deviceComboBox.SelectedIndex = 0;
                    ValidateInputs();
                }
            }

            this.deviceComboBox.Enabled = true;
            this.deviceReloadButton.Enabled = true;
            this.deviceReloadButton.Text = "Reload";
            this.Cursor = Cursors.Default;
        }

        private async Task<List<Item>> ScanMedia()
        {
            return await Task.Run(() => this.importManager.FindMedia(this.importDevice!));
        }

        private void ImportItems(object? sender, DoWorkEventArgs e)
        {
            int count = this.itemsToImport.Count;

            var importProgress = new Progress<int>();
            importProgress.ProgressChanged += (object? sender, int e) =>
            {
                int percentage = Convert.ToInt32(((double)e / (double)count) * 100.0);
                importWorker.ReportProgress(percentage, $"Importing ({e}/{count})");
            };

            importWorker.ReportProgress(0, $"Importing (0/{count})");

            var importMediaTask = this.importManager.ImportMedia(this.importDevice!, this.importDestination!, this.itemsToImport, importProgress);
            importMediaTask.Wait();
            List<string> paths = importMediaTask.Result;

            if (!Preferences.ConvertHeic)
            {
                logger.Info("Skipping encoding");
                return;
            }

            importWorker.ReportProgress(0, $"Encoding (0/{count})");

            var encodeProgress = new Progress<int>();
            encodeProgress.ProgressChanged += (object? sender, int e) =>
            {
                int percentage = Convert.ToInt32(((double)e / (double)count) * 100.0);
                importWorker.ReportProgress(percentage, $"Encoding ({e}/{count})");
            };

            this.encodeManager.EncodeMedia(paths, encodeProgress).Wait();
        }

        private void setControlsEnabled(bool enabled)
        {
            this.optionsButton.Enabled = enabled;
            this.scanButton.Enabled = enabled;
            this.deviceComboBox.Enabled = enabled;
            this.destinationTextBox.Enabled = enabled;
            this.destinationBrowseButton.Enabled = enabled;
            this.deviceReloadButton.Enabled = enabled;
        }

        private async void MainView_Activated(object sender, EventArgs e)
        {
            if (!this.firstLoadComplete)
            {
                this.firstLoadComplete = true;
                await this.LoadDevices();
            }
        }
    }
}
