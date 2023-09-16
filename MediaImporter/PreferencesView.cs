 namespace io.ecn.MediaImporter
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    public partial class PreferencesView : Form
    {
        public PreferencesView()
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            InitializeComponent();
            this.directoryFormatTextBox.Text = Preferences.DirectoryFormat;
            this.fileFormatTextBox.Text = Preferences.FileFormat;
            this.convertCheckbox.Checked = Preferences.ConvertHeic;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Preferences.DirectoryFormat = this.directoryFormatTextBox.Text;
            Preferences.FileFormat = this.fileFormatTextBox.Text;
            Preferences.ConvertHeic = this.convertCheckbox.Checked;
            Preferences.Save();
            this.Close();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            Preferences.Reset();
            Preferences.Save();
            this.Close();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Media Importer v{Assembly.GetExecutingAssembly().GetName().Version!}\r\n\r\nCreated by Ian Spence. Copyright © 2023. Media Importer is free & open source software released under the terms of the Mozilla Public License (MPL).\r\n\r\nMedia Importer includes ImageMagick. ImageMagick is Copyright © 1999 ImageMagick Studio LLC.", "About Media Importer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
