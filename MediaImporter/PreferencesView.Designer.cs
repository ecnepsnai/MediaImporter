namespace io.ecn.MediaImporter
{
    partial class PreferencesView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesView));
            label1 = new Label();
            directoryFormatTextBox = new TextBox();
            fileFormatTextBox = new TextBox();
            label2 = new Label();
            label3 = new Label();
            convertCheckbox = new CheckBox();
            saveButton = new Button();
            resetButton = new Button();
            aboutButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(90, 15);
            label1.TabIndex = 0;
            label1.Text = "Directory Name";
            // 
            // directoryFormatTextBox
            // 
            directoryFormatTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            directoryFormatTextBox.Location = new Point(12, 29);
            directoryFormatTextBox.Name = "directoryFormatTextBox";
            directoryFormatTextBox.Size = new Size(460, 23);
            directoryFormatTextBox.TabIndex = 1;
            // 
            // fileFormatTextBox
            // 
            fileFormatTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fileFormatTextBox.Location = new Point(12, 85);
            fileFormatTextBox.Name = "fileFormatTextBox";
            fileFormatTextBox.Size = new Size(460, 23);
            fileFormatTextBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 65);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 2;
            label2.Text = "File Name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(12, 113);
            label3.Name = "label3";
            label3.Size = new Size(192, 135);
            label3.TabIndex = 4;
            label3.Text = "%y - Year (2004)\r\n%M - Month (06)\r\n%MMM - Month short (Jun)\r\n%MMMM - Month full (June)\r\n%d - Day of month (30)\r\n%dddd - Day of week (Wednesday)\r\n%H - Hour (01)\r\n%m - Minute (45)\r\n%s - Second (59)";
            // 
            // convertCheckbox
            // 
            convertCheckbox.AutoSize = true;
            convertCheckbox.Location = new Point(12, 260);
            convertCheckbox.Name = "convertCheckbox";
            convertCheckbox.Size = new Size(230, 19);
            convertCheckbox.TabIndex = 5;
            convertCheckbox.Text = "Convert HEIC and HEIF Images to JPEG";
            convertCheckbox.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            saveButton.Location = new Point(397, 326);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(75, 23);
            saveButton.TabIndex = 6;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // resetButton
            // 
            resetButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            resetButton.Location = new Point(316, 326);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(75, 23);
            resetButton.TabIndex = 7;
            resetButton.Text = "Reset";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            // 
            // aboutButton
            // 
            aboutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            aboutButton.Location = new Point(12, 326);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(149, 23);
            aboutButton.TabIndex = 8;
            aboutButton.Text = "About Media Importer";
            aboutButton.UseVisualStyleBackColor = true;
            aboutButton.Click += aboutButton_Click;
            // 
            // PreferencesView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 361);
            Controls.Add(aboutButton);
            Controls.Add(resetButton);
            Controls.Add(saveButton);
            Controls.Add(convertCheckbox);
            Controls.Add(label3);
            Controls.Add(fileFormatTextBox);
            Controls.Add(label2);
            Controls.Add(directoryFormatTextBox);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PreferencesView";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Preferences";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox directoryFormatTextBox;
        private TextBox fileFormatTextBox;
        private Label label2;
        private Label label3;
        private CheckBox convertCheckbox;
        private Button saveButton;
        private Button resetButton;
        private Button aboutButton;
    }
}