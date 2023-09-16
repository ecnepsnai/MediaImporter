namespace io.ecn.MediaImporter
{
    partial class MainView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            deviceComboBox = new ComboBox();
            deviceReloadButton = new Button();
            label1 = new Label();
            label2 = new Label();
            destinationTextBox = new TextBox();
            destinationBrowseButton = new Button();
            scanButton = new Button();
            optionsButton = new Button();
            progressBar = new ProgressBar();
            progressLabel = new Label();
            SuspendLayout();
            // 
            // deviceComboBox
            // 
            deviceComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            deviceComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            deviceComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            deviceComboBox.FormattingEnabled = true;
            deviceComboBox.Location = new Point(12, 37);
            deviceComboBox.Name = "deviceComboBox";
            deviceComboBox.Size = new Size(372, 23);
            deviceComboBox.TabIndex = 0;
            deviceComboBox.SelectedIndexChanged += DeviceComboBoxSelectedIndexChanged;
            // 
            // deviceReloadButton
            // 
            deviceReloadButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            deviceReloadButton.Location = new Point(390, 37);
            deviceReloadButton.Name = "deviceReloadButton";
            deviceReloadButton.Size = new Size(75, 23);
            deviceReloadButton.TabIndex = 1;
            deviceReloadButton.Text = "Reload";
            deviceReloadButton.UseVisualStyleBackColor = true;
            deviceReloadButton.Click += DeviceReloadButtonClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 17);
            label1.Name = "label1";
            label1.Size = new Size(112, 15);
            label1.TabIndex = 2;
            label1.Text = "Import From Device";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 74);
            label2.Name = "label2";
            label2.Size = new Size(97, 15);
            label2.TabIndex = 3;
            label2.Text = "Save To Directory";
            // 
            // destinationTextBox
            // 
            destinationTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            destinationTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            destinationTextBox.AutoCompleteSource = AutoCompleteSource.FileSystem;
            destinationTextBox.Location = new Point(12, 94);
            destinationTextBox.Name = "destinationTextBox";
            destinationTextBox.Size = new Size(372, 23);
            destinationTextBox.TabIndex = 4;
            destinationTextBox.TextChanged += DestinationTextBoxTextChanged;
            // 
            // destinationBrowseButton
            // 
            destinationBrowseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            destinationBrowseButton.Location = new Point(390, 94);
            destinationBrowseButton.Name = "destinationBrowseButton";
            destinationBrowseButton.Size = new Size(75, 23);
            destinationBrowseButton.TabIndex = 5;
            destinationBrowseButton.Text = "Browse";
            destinationBrowseButton.UseVisualStyleBackColor = true;
            destinationBrowseButton.Click += DestinationBrowseButtonClick;
            // 
            // scanButton
            // 
            scanButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            scanButton.Enabled = false;
            scanButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            scanButton.Location = new Point(390, 176);
            scanButton.Name = "scanButton";
            scanButton.Size = new Size(75, 23);
            scanButton.TabIndex = 6;
            scanButton.Text = "Scan";
            scanButton.UseVisualStyleBackColor = true;
            scanButton.Click += ScanButtonClick;
            // 
            // optionsButton
            // 
            optionsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            optionsButton.Location = new Point(309, 176);
            optionsButton.Name = "optionsButton";
            optionsButton.Size = new Size(75, 23);
            optionsButton.TabIndex = 7;
            optionsButton.Text = "Options";
            optionsButton.UseVisualStyleBackColor = true;
            optionsButton.Click += OptionsButtonClick;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(12, 176);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(291, 23);
            progressBar.Step = 1;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 8;
            progressBar.Visible = false;
            // 
            // progressLabel
            // 
            progressLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            progressLabel.AutoSize = true;
            progressLabel.Location = new Point(309, 181);
            progressLabel.Name = "progressLabel";
            progressLabel.Size = new Size(14, 15);
            progressLabel.TabIndex = 9;
            progressLabel.Text = "P";
            progressLabel.Visible = false;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(477, 211);
            Controls.Add(progressLabel);
            Controls.Add(progressBar);
            Controls.Add(optionsButton);
            Controls.Add(scanButton);
            Controls.Add(destinationBrowseButton);
            Controls.Add(destinationTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(deviceReloadButton);
            Controls.Add(deviceComboBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainView";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Media Importer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox deviceComboBox;
        private Button deviceReloadButton;
        private Label label1;
        private Label label2;
        private TextBox destinationTextBox;
        private Button destinationBrowseButton;
        private Button scanButton;
        private Button optionsButton;
        private ProgressBar progressBar;
        private Label progressLabel;
    }
}