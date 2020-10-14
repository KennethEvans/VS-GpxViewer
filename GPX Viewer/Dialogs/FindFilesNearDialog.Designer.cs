namespace GPXViewer.Dialogs {
    partial class FindFilesNearDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelDirectory = new System.Windows.Forms.TableLayoutPanel();
            this.labelDirectory = new System.Windows.Forms.Label();
            this.textBoxDirectory = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.tableLayoutPanelParameters = new System.Windows.Forms.TableLayoutPanel();
            this.labelLatitiude = new System.Windows.Forms.Label();
            this.textBoxLatitude = new System.Windows.Forms.TextBox();
            this.labelLongitude = new System.Windows.Forms.Label();
            this.textBoxLongitude = new System.Windows.Forms.TextBox();
            this.labelRadius = new System.Windows.Forms.Label();
            this.textBoxRadius = new System.Windows.Forms.TextBox();
            this.comboBoxUnits = new System.Windows.Forms.ComboBox();
            this.labelFileFilter = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.checkBoxWaypoints = new System.Windows.Forms.CheckBox();
            this.checkBoxTracks = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanelPlacemark = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonPastePlacemark = new System.Windows.Forms.Button();
            this.buttonCopyPlacemark = new System.Windows.Forms.Button();
            this.buttonCopyCircle = new System.Windows.Forms.Button();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.tableLayoutPanelTop.SuspendLayout();
            this.tableLayoutPanelDirectory.SuspendLayout();
            this.tableLayoutPanelParameters.SuspendLayout();
            this.flowLayoutPanelPlacemark.SuspendLayout();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelTop
            // 
            this.tableLayoutPanelTop.AutoSize = true;
            this.tableLayoutPanelTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelTop.ColumnCount = 1;
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.Controls.Add(this.tableLayoutPanelDirectory, 0, 0);
            this.tableLayoutPanelTop.Controls.Add(this.tableLayoutPanelParameters, 0, 1);
            this.tableLayoutPanelTop.Controls.Add(this.flowLayoutPanelButtons, 0, 2);
            this.tableLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelTop.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            this.tableLayoutPanelTop.RowCount = 3;
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(1039, 376);
            this.tableLayoutPanelTop.TabIndex = 1;
            // 
            // tableLayoutPanelDirectory
            // 
            this.tableLayoutPanelDirectory.AutoSize = true;
            this.tableLayoutPanelDirectory.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelDirectory.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanelDirectory.ColumnCount = 3;
            this.tableLayoutPanelDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDirectory.Controls.Add(this.labelDirectory, 0, 0);
            this.tableLayoutPanelDirectory.Controls.Add(this.textBoxDirectory, 1, 0);
            this.tableLayoutPanelDirectory.Controls.Add(this.buttonBrowse, 2, 0);
            this.tableLayoutPanelDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDirectory.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelDirectory.Name = "tableLayoutPanelDirectory";
            this.tableLayoutPanelDirectory.RowCount = 1;
            this.tableLayoutPanelDirectory.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDirectory.Size = new System.Drawing.Size(1033, 48);
            this.tableLayoutPanelDirectory.TabIndex = 2;
            // 
            // labelDirectory
            // 
            this.labelDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDirectory.AutoSize = true;
            this.labelDirectory.BackColor = System.Drawing.SystemColors.Control;
            this.labelDirectory.Location = new System.Drawing.Point(3, 0);
            this.labelDirectory.Name = "labelDirectory";
            this.labelDirectory.Size = new System.Drawing.Size(136, 48);
            this.labelDirectory.TabIndex = 0;
            this.labelDirectory.Text = "Directory:";
            this.labelDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDirectory
            // 
            this.textBoxDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDirectory.Location = new System.Drawing.Point(145, 3);
            this.textBoxDirectory.Name = "textBoxDirectory";
            this.textBoxDirectory.Size = new System.Drawing.Size(760, 38);
            this.textBoxDirectory.TabIndex = 1;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.buttonBrowse.AutoSize = true;
            this.buttonBrowse.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonBrowse.BackColor = System.Drawing.SystemColors.Control;
            this.buttonBrowse.Location = new System.Drawing.Point(911, 3);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(119, 42);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = false;
            this.buttonBrowse.Click += new System.EventHandler(this.OnDirectoryBrowseClick);
            // 
            // tableLayoutPanelParameters
            // 
            this.tableLayoutPanelParameters.AutoSize = true;
            this.tableLayoutPanelParameters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelParameters.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanelParameters.ColumnCount = 4;
            this.tableLayoutPanelParameters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelParameters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelParameters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelParameters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelParameters.Controls.Add(this.labelLatitiude, 0, 0);
            this.tableLayoutPanelParameters.Controls.Add(this.textBoxLatitude, 1, 0);
            this.tableLayoutPanelParameters.Controls.Add(this.labelLongitude, 2, 0);
            this.tableLayoutPanelParameters.Controls.Add(this.textBoxLongitude, 3, 0);
            this.tableLayoutPanelParameters.Controls.Add(this.labelRadius, 0, 1);
            this.tableLayoutPanelParameters.Controls.Add(this.textBoxRadius, 1, 1);
            this.tableLayoutPanelParameters.Controls.Add(this.comboBoxUnits, 2, 1);
            this.tableLayoutPanelParameters.Controls.Add(this.labelFileFilter, 0, 2);
            this.tableLayoutPanelParameters.Controls.Add(this.textBoxFilter, 1, 2);
            this.tableLayoutPanelParameters.Controls.Add(this.checkBoxWaypoints, 0, 3);
            this.tableLayoutPanelParameters.Controls.Add(this.checkBoxTracks, 2, 3);
            this.tableLayoutPanelParameters.Controls.Add(this.flowLayoutPanelPlacemark, 0, 4);
            this.tableLayoutPanelParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelParameters.Location = new System.Drawing.Point(3, 57);
            this.tableLayoutPanelParameters.Name = "tableLayoutPanelParameters";
            this.tableLayoutPanelParameters.RowCount = 5;
            this.tableLayoutPanelParameters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelParameters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelParameters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelParameters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelParameters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelParameters.Size = new System.Drawing.Size(1033, 249);
            this.tableLayoutPanelParameters.TabIndex = 0;
            // 
            // labelLatitiude
            // 
            this.labelLatitiude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLatitiude.AutoSize = true;
            this.labelLatitiude.BackColor = System.Drawing.SystemColors.Control;
            this.labelLatitiude.Location = new System.Drawing.Point(3, 0);
            this.labelLatitiude.Name = "labelLatitiude";
            this.labelLatitiude.Size = new System.Drawing.Size(126, 44);
            this.labelLatitiude.TabIndex = 0;
            this.labelLatitiude.Text = "Latitude:";
            this.labelLatitiude.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxLatitude
            // 
            this.textBoxLatitude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLatitude.Location = new System.Drawing.Point(209, 3);
            this.textBoxLatitude.Name = "textBoxLatitude";
            this.textBoxLatitude.Size = new System.Drawing.Size(303, 38);
            this.textBoxLatitude.TabIndex = 1;
            // 
            // labelLongitude
            // 
            this.labelLongitude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLongitude.AutoSize = true;
            this.labelLongitude.BackColor = System.Drawing.SystemColors.Control;
            this.labelLongitude.Location = new System.Drawing.Point(518, 0);
            this.labelLongitude.Name = "labelLongitude";
            this.labelLongitude.Size = new System.Drawing.Size(150, 44);
            this.labelLongitude.TabIndex = 3;
            this.labelLongitude.Text = "Longitude:";
            this.labelLongitude.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxLongitude
            // 
            this.textBoxLongitude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLongitude.Location = new System.Drawing.Point(724, 3);
            this.textBoxLongitude.Name = "textBoxLongitude";
            this.textBoxLongitude.Size = new System.Drawing.Size(306, 38);
            this.textBoxLongitude.TabIndex = 4;
            // 
            // labelRadius
            // 
            this.labelRadius.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRadius.AutoSize = true;
            this.labelRadius.BackColor = System.Drawing.SystemColors.Control;
            this.labelRadius.Location = new System.Drawing.Point(3, 44);
            this.labelRadius.Name = "labelRadius";
            this.labelRadius.Size = new System.Drawing.Size(112, 45);
            this.labelRadius.TabIndex = 5;
            this.labelRadius.Text = "Radius:";
            this.labelRadius.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxRadius
            // 
            this.textBoxRadius.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRadius.Location = new System.Drawing.Point(209, 47);
            this.textBoxRadius.Name = "textBoxRadius";
            this.textBoxRadius.Size = new System.Drawing.Size(303, 38);
            this.textBoxRadius.TabIndex = 6;
            // 
            // comboBoxUnits
            // 
            this.comboBoxUnits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxUnits.FormattingEnabled = true;
            this.comboBoxUnits.Items.AddRange(new object[] {
            "ft",
            "mi",
            "m",
            "km"});
            this.comboBoxUnits.Location = new System.Drawing.Point(518, 47);
            this.comboBoxUnits.Name = "comboBoxUnits";
            this.comboBoxUnits.Size = new System.Drawing.Size(200, 39);
            this.comboBoxUnits.TabIndex = 11;
            // 
            // labelFileFilter
            // 
            this.labelFileFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelFileFilter.AutoSize = true;
            this.labelFileFilter.BackColor = System.Drawing.SystemColors.Control;
            this.labelFileFilter.Location = new System.Drawing.Point(3, 89);
            this.labelFileFilter.Name = "labelFileFilter";
            this.labelFileFilter.Size = new System.Drawing.Size(134, 44);
            this.labelFileFilter.TabIndex = 8;
            this.labelFileFilter.Text = "FileFilter:";
            this.labelFileFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxFilter
            // 
            this.tableLayoutPanelParameters.SetColumnSpan(this.textBoxFilter, 2);
            this.textBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFilter.Location = new System.Drawing.Point(209, 92);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(509, 38);
            this.textBoxFilter.TabIndex = 9;
            // 
            // checkBoxWaypoints
            // 
            this.checkBoxWaypoints.AutoSize = true;
            this.tableLayoutPanelParameters.SetColumnSpan(this.checkBoxWaypoints, 2);
            this.checkBoxWaypoints.Location = new System.Drawing.Point(3, 136);
            this.checkBoxWaypoints.Name = "checkBoxWaypoints";
            this.checkBoxWaypoints.Size = new System.Drawing.Size(186, 36);
            this.checkBoxWaypoints.TabIndex = 2;
            this.checkBoxWaypoints.Text = "Waypoints";
            this.checkBoxWaypoints.UseVisualStyleBackColor = true;
            // 
            // checkBoxTracks
            // 
            this.checkBoxTracks.AutoSize = true;
            this.tableLayoutPanelParameters.SetColumnSpan(this.checkBoxTracks, 2);
            this.checkBoxTracks.Location = new System.Drawing.Point(518, 136);
            this.checkBoxTracks.Name = "checkBoxTracks";
            this.checkBoxTracks.Size = new System.Drawing.Size(137, 36);
            this.checkBoxTracks.TabIndex = 7;
            this.checkBoxTracks.Text = "Tracks";
            this.checkBoxTracks.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelPlacemark
            // 
            this.flowLayoutPanelPlacemark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanelPlacemark.AutoSize = true;
            this.flowLayoutPanelPlacemark.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelPlacemark.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanelParameters.SetColumnSpan(this.flowLayoutPanelPlacemark, 4);
            this.flowLayoutPanelPlacemark.Controls.Add(this.buttonPastePlacemark);
            this.flowLayoutPanelPlacemark.Controls.Add(this.buttonCopyPlacemark);
            this.flowLayoutPanelPlacemark.Controls.Add(this.buttonCopyCircle);
            this.flowLayoutPanelPlacemark.Location = new System.Drawing.Point(3, 178);
            this.flowLayoutPanelPlacemark.Name = "flowLayoutPanelPlacemark";
            this.flowLayoutPanelPlacemark.Size = new System.Drawing.Size(660, 68);
            this.flowLayoutPanelPlacemark.TabIndex = 10;
            this.flowLayoutPanelPlacemark.WrapContents = false;
            // 
            // buttonPastePlacemark
            // 
            this.buttonPastePlacemark.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonPastePlacemark.AutoSize = true;
            this.buttonPastePlacemark.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonPastePlacemark.Location = new System.Drawing.Point(3, 3);
            this.buttonPastePlacemark.Name = "buttonPastePlacemark";
            this.buttonPastePlacemark.Size = new System.Drawing.Size(239, 42);
            this.buttonPastePlacemark.TabIndex = 2;
            this.buttonPastePlacemark.Text = "Paste Placemark";
            this.buttonPastePlacemark.UseVisualStyleBackColor = true;
            this.buttonPastePlacemark.Click += new System.EventHandler(this.OnPastePlacemarkClick);
            // 
            // buttonCopyPlacemark
            // 
            this.buttonCopyPlacemark.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonCopyPlacemark.AutoSize = true;
            this.buttonCopyPlacemark.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCopyPlacemark.Location = new System.Drawing.Point(248, 3);
            this.buttonCopyPlacemark.Name = "buttonCopyPlacemark";
            this.buttonCopyPlacemark.Size = new System.Drawing.Size(232, 42);
            this.buttonCopyPlacemark.TabIndex = 1;
            this.buttonCopyPlacemark.Text = "Copy Placemark";
            this.buttonCopyPlacemark.UseVisualStyleBackColor = true;
            this.buttonCopyPlacemark.Click += new System.EventHandler(this.OnCopyPlacemarkClick);
            // 
            // buttonCopyCircle
            // 
            this.buttonCopyCircle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonCopyCircle.AutoSize = true;
            this.buttonCopyCircle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCopyCircle.Location = new System.Drawing.Point(486, 3);
            this.buttonCopyCircle.Name = "buttonCopyCircle";
            this.buttonCopyCircle.Size = new System.Drawing.Size(171, 42);
            this.buttonCopyCircle.TabIndex = 0;
            this.buttonCopyCircle.Text = "Copy Circle";
            this.buttonCopyCircle.UseVisualStyleBackColor = true;
            this.buttonCopyCircle.Click += new System.EventHandler(this.OnCopyCircleClick);
            // 
            // flowLayoutPanelButtons
            // 
            this.flowLayoutPanelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.flowLayoutPanelButtons.AutoSize = true;
            this.flowLayoutPanelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelButtons.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanelButtons.Controls.Add(this.buttonCancel);
            this.flowLayoutPanelButtons.Controls.Add(this.buttonOk);
            this.flowLayoutPanelButtons.Location = new System.Drawing.Point(423, 312);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            this.flowLayoutPanelButtons.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.flowLayoutPanelButtons.Size = new System.Drawing.Size(192, 61);
            this.flowLayoutPanelButtons.TabIndex = 0;
            this.flowLayoutPanelButtons.WrapContents = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCancel.Location = new System.Drawing.Point(3, 8);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(114, 42);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.OnCancelClick);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonOk.AutoSize = true;
            this.buttonOk.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonOk.Location = new System.Drawing.Point(123, 8);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(66, 42);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.OnOkClick);
            // 
            // FindFilesNearDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 376);
            this.Controls.Add(this.tableLayoutPanelTop);
            this.Name = "FindFilesNearDialog";
            this.Text = "Find Files Near";
            this.tableLayoutPanelTop.ResumeLayout(false);
            this.tableLayoutPanelTop.PerformLayout();
            this.tableLayoutPanelDirectory.ResumeLayout(false);
            this.tableLayoutPanelDirectory.PerformLayout();
            this.tableLayoutPanelParameters.ResumeLayout(false);
            this.tableLayoutPanelParameters.PerformLayout();
            this.flowLayoutPanelPlacemark.ResumeLayout(false);
            this.flowLayoutPanelPlacemark.PerformLayout();
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.flowLayoutPanelButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDirectory;
        private System.Windows.Forms.Label labelDirectory;
        private System.Windows.Forms.TextBox textBoxDirectory;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelParameters;
        private System.Windows.Forms.Label labelLatitiude;
        private System.Windows.Forms.TextBox textBoxLatitude;
        private System.Windows.Forms.Label labelLongitude;
        private System.Windows.Forms.TextBox textBoxLongitude;
        private System.Windows.Forms.Label labelRadius;
        private System.Windows.Forms.TextBox textBoxRadius;
        private System.Windows.Forms.ComboBox comboBoxUnits;
        private System.Windows.Forms.Label labelFileFilter;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.CheckBox checkBoxWaypoints;
        private System.Windows.Forms.CheckBox checkBoxTracks;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelPlacemark;
        private System.Windows.Forms.Button buttonPastePlacemark;
        private System.Windows.Forms.Button buttonCopyPlacemark;
        private System.Windows.Forms.Button buttonCopyCircle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
    }
}