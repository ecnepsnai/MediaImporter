namespace MediaImporter.Views
{
    using Microsoft.UI.Xaml.Controls;
    using System.Reflection;

    public sealed partial class SettingsPage : Page
    {
        private readonly UIHelper uihelper;
        private string DirectoryFormat;
        private string FileFormat;
        private bool ConvertHeic;

        public SettingsPage()
        {
            this.InitializeComponent();
            this.uihelper = new UIHelper(this.Content);
            Load();
        }

        private void Load()
        {
            this.DirectoryFormat = Preferences.DirectoryFormat;
            this.FileFormat = Preferences.FileFormat;
            this.ConvertHeic = Preferences.ConvertHeic;
            this.DirFormatTextBox.Text = DirectoryFormat;
            this.FileFormatTextBox.Text = FileFormat;
            this.ConvertHeicCheckbox.IsChecked = ConvertHeic;
        }

        private void DirFormatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DirectoryFormat = this.DirFormatTextBox.Text;
            SaveButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
        }

        private void FileFormatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FileFormat = this.FileFormatTextBox.Text;
            SaveButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
        }

        private void ConvertHeicCheckbox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ConvertHeic = this.ConvertHeicCheckbox.IsChecked ?? false;
            SaveButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
        }

        private async void SaveButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DirectoryFormat.Length == 0)
            {
                await this.uihelper.ShowMessageBox("Invalid Entry", "Directory format is required");
                return;
            }

            if (FileFormat.Length == 0)
            {
                await this.uihelper.ShowMessageBox("Invalid Entry", "File format is required");
                return;
            }

            Preferences.DirectoryFormat = DirectoryFormat;
            Preferences.FileFormat = FileFormat;
            Preferences.ConvertHeic = ConvertHeic;
            SaveButton.IsEnabled = false;
        }

        private void ResetButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Preferences.Reset();
            Load();
            ResetButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
        }

        private async void AboutButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await this.uihelper.ShowMessageBox("About Media Importer", $"Media Importer v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}\r\n\r\nCreated by Ian Spence. Copyright © 2023. Media Importer is free & open source software released under the terms of the Mozilla Public License (MPL).\r\n\r\nMedia Importer includes ImageMagick. ImageMagick is Copyright © 1999 ImageMagick Studio LLC.");
        }
    }
}
