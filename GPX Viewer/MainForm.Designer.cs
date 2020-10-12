﻿namespace GPXViewer {
    partial class MainForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.treeListView = new BrightIdeasSoftware.TreeListView();
            this.col1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGPXTCXFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToGoogleEarthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resetModelFromTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetTreeFromModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.expandToLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allFilesToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.checkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.everythingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nothingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gpxTcxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatTCXGPXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.interpolateTCXFromGPXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inaccurateGPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gPSDropoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTCXTrackpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeTCXTimesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recalculateTCXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixGPXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.singleFileInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelTop
            // 
            this.tableLayoutPanelTop.AutoSize = true;
            this.tableLayoutPanelTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelTop.ColumnCount = 1;
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelTop.Controls.Add(this.treeListView, 0, 0);
            this.tableLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelTop.Location = new System.Drawing.Point(0, 52);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            this.tableLayoutPanelTop.RowCount = 1;
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(1627, 913);
            this.tableLayoutPanelTop.TabIndex = 0;
            // 
            // treeListView
            // 
            this.treeListView.AllColumns.Add(this.col1);
            this.treeListView.CellEditUseWholeCell = false;
            this.treeListView.CheckBoxes = true;
            this.treeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col1});
            this.treeListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListView.HideSelection = false;
            this.treeListView.Location = new System.Drawing.Point(3, 3);
            this.treeListView.Name = "treeListView";
            this.treeListView.ShowGroups = false;
            this.treeListView.ShowImagesOnSubItems = true;
            this.treeListView.Size = new System.Drawing.Size(1621, 907);
            this.treeListView.TabIndex = 0;
            this.treeListView.UseCompatibleStateImageBehavior = false;
            this.treeListView.View = System.Windows.Forms.View.Details;
            this.treeListView.VirtualMode = true;
            // 
            // col1
            // 
            this.col1.AspectName = "Label";
            this.col1.FillsFreeSpace = true;
            this.col1.Text = "Files";
            this.col1.Width = 118;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem1,
            this.toolsToolStripMenuItem,
            this.gpxTcxToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1627, 52);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGPXTCXFileToolStripMenuItem,
            this.sendToGoogleEarthToolStripMenuItem,
            this.toolStripSeparator1,
            this.resetModelFromTreeToolStripMenuItem,
            this.resetTreeFromModelToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(87, 48);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openGPXTCXFileToolStripMenuItem
            // 
            this.openGPXTCXFileToolStripMenuItem.Name = "openGPXTCXFileToolStripMenuItem";
            this.openGPXTCXFileToolStripMenuItem.Size = new System.Drawing.Size(484, 54);
            this.openGPXTCXFileToolStripMenuItem.Text = "Open GPX/TCX File...";
            this.openGPXTCXFileToolStripMenuItem.Click += new System.EventHandler(this.OnFileOpenGpxClick);
            // 
            // sendToGoogleEarthToolStripMenuItem
            // 
            this.sendToGoogleEarthToolStripMenuItem.Name = "sendToGoogleEarthToolStripMenuItem";
            this.sendToGoogleEarthToolStripMenuItem.Size = new System.Drawing.Size(484, 54);
            this.sendToGoogleEarthToolStripMenuItem.Text = "Send to Google Earth";
            this.sendToGoogleEarthToolStripMenuItem.Click += new System.EventHandler(this.OnFileSendToGoogleEarth);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(481, 6);
            // 
            // resetModelFromTreeToolStripMenuItem
            // 
            this.resetModelFromTreeToolStripMenuItem.Name = "resetModelFromTreeToolStripMenuItem";
            this.resetModelFromTreeToolStripMenuItem.Size = new System.Drawing.Size(484, 54);
            this.resetModelFromTreeToolStripMenuItem.Text = "Reset Model from Tree";
            this.resetModelFromTreeToolStripMenuItem.Click += new System.EventHandler(this.OnResetModelFromTree);
            // 
            // resetTreeFromModelToolStripMenuItem
            // 
            this.resetTreeFromModelToolStripMenuItem.Name = "resetTreeFromModelToolStripMenuItem";
            this.resetTreeFromModelToolStripMenuItem.Size = new System.Drawing.Size(484, 54);
            this.resetTreeFromModelToolStripMenuItem.Text = "Reset Tree from Model";
            this.resetTreeFromModelToolStripMenuItem.Click += new System.EventHandler(this.OnResetTreeFromModel);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(481, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(484, 54);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnFileExitClick);
            // 
            // viewToolStripMenuItem1
            // 
            this.viewToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandToLevelToolStripMenuItem,
            this.collapseToolStripMenuItem});
            this.viewToolStripMenuItem1.Name = "viewToolStripMenuItem1";
            this.viewToolStripMenuItem1.Size = new System.Drawing.Size(106, 48);
            this.viewToolStripMenuItem1.Text = "View";
            // 
            // expandToLevelToolStripMenuItem
            // 
            this.expandToLevelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.expandToLevelToolStripMenuItem.Name = "expandToLevelToolStripMenuItem";
            this.expandToLevelToolStripMenuItem.Size = new System.Drawing.Size(392, 54);
            this.expandToLevelToolStripMenuItem.Text = "Expand to Level";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(200, 54);
            this.toolStripMenuItem5.Text = "0";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.OnViewExpandToLevelClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(200, 54);
            this.toolStripMenuItem2.Text = "1";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.OnViewExpandToLevelClick);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(200, 54);
            this.toolStripMenuItem3.Text = "2";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.OnViewExpandToLevelClick);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(200, 54);
            this.toolStripMenuItem4.Text = "3";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.OnViewExpandToLevelClick);
            // 
            // collapseToolStripMenuItem
            // 
            this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
            this.collapseToolStripMenuItem.Size = new System.Drawing.Size(392, 54);
            this.collapseToolStripMenuItem.Text = "Collapse All";
            this.collapseToolStripMenuItem.Click += new System.EventHandler(this.OnViewExpandNoneClick);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem,
            this.checkToolStripMenuItem,
            this.toolStripSeparator3,
            this.showLogToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(111, 48);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allFilesToolStripMenuItem2,
            this.allSelectedToolStripMenuItem});
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(448, 54);
            this.removeToolStripMenuItem.Text = "Remove";
            // 
            // allSelectedToolStripMenuItem
            // 
            this.allSelectedToolStripMenuItem.Name = "allSelectedToolStripMenuItem";
            this.allSelectedToolStripMenuItem.Size = new System.Drawing.Size(448, 54);
            this.allSelectedToolStripMenuItem.Text = "All Selected";
            this.allSelectedToolStripMenuItem.Click += new System.EventHandler(this.OnToolsRemoveAllSelectedClick);
            // 
            // allFilesToolStripMenuItem2
            // 
            this.allFilesToolStripMenuItem2.Name = "allFilesToolStripMenuItem2";
            this.allFilesToolStripMenuItem2.Size = new System.Drawing.Size(448, 54);
            this.allFilesToolStripMenuItem2.Text = "All Files";
            this.allFilesToolStripMenuItem2.Click += new System.EventHandler(this.OnToolsRemoveAllFilesClick);
            // 
            // checkToolStripMenuItem
            // 
            this.checkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allFilesToolStripMenuItem,
            this.noFilesToolStripMenuItem,
            this.everythingToolStripMenuItem,
            this.nothingToolStripMenuItem});
            this.checkToolStripMenuItem.Name = "checkToolStripMenuItem";
            this.checkToolStripMenuItem.Size = new System.Drawing.Size(448, 54);
            this.checkToolStripMenuItem.Text = "Check";
            // 
            // allFilesToolStripMenuItem
            // 
            this.allFilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.expandToolStripMenuItem});
            this.allFilesToolStripMenuItem.Name = "allFilesToolStripMenuItem";
            this.allFilesToolStripMenuItem.Size = new System.Drawing.Size(323, 54);
            this.allFilesToolStripMenuItem.Text = "All Files";
            this.allFilesToolStripMenuItem.Click += new System.EventHandler(this.OnToolsCheckAllFilesClick);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(281, 54);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(281, 54);
            this.expandToolStripMenuItem.Text = "Expand";
            // 
            // noFilesToolStripMenuItem
            // 
            this.noFilesToolStripMenuItem.Name = "noFilesToolStripMenuItem";
            this.noFilesToolStripMenuItem.Size = new System.Drawing.Size(323, 54);
            this.noFilesToolStripMenuItem.Text = "No Files";
            this.noFilesToolStripMenuItem.Click += new System.EventHandler(this.OnToolsCheckNoFilesClick);
            // 
            // everythingToolStripMenuItem
            // 
            this.everythingToolStripMenuItem.Name = "everythingToolStripMenuItem";
            this.everythingToolStripMenuItem.Size = new System.Drawing.Size(323, 54);
            this.everythingToolStripMenuItem.Text = "Everything";
            this.everythingToolStripMenuItem.Click += new System.EventHandler(this.OnToolsCheckAllClick);
            // 
            // nothingToolStripMenuItem
            // 
            this.nothingToolStripMenuItem.Name = "nothingToolStripMenuItem";
            this.nothingToolStripMenuItem.Size = new System.Drawing.Size(323, 54);
            this.nothingToolStripMenuItem.Text = "Nothing";
            this.nothingToolStripMenuItem.Click += new System.EventHandler(this.OnToolsCheckNoneClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(445, 6);
            // 
            // showLogToolStripMenuItem
            // 
            this.showLogToolStripMenuItem.Name = "showLogToolStripMenuItem";
            this.showLogToolStripMenuItem.Size = new System.Drawing.Size(448, 54);
            this.showLogToolStripMenuItem.Text = "Show Log...";
            this.showLogToolStripMenuItem.Click += new System.EventHandler(this.OnToolsShowLogClick);
            // 
            // gpxTcxToolStripMenuItem
            // 
            this.gpxTcxToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.formatTCXGPXToolStripMenuItem,
            this.formatXMLToolStripMenuItem,
            this.toolStripSeparator5,
            this.interpolateTCXFromGPXToolStripMenuItem,
            this.deleteTCXTrackpointsToolStripMenuItem,
            this.changeTCXTimesToolStripMenuItem,
            this.recalculateTCXToolStripMenuItem,
            this.fixGPXToolStripMenuItem,
            this.toolStripSeparator6,
            this.singleFileInfoToolStripMenuItem});
            this.gpxTcxToolStripMenuItem.Name = "gpxTcxToolStripMenuItem";
            this.gpxTcxToolStripMenuItem.Size = new System.Drawing.Size(161, 48);
            this.gpxTcxToolStripMenuItem.Text = "GPX/TCX";
            // 
            // formatTCXGPXToolStripMenuItem
            // 
            this.formatTCXGPXToolStripMenuItem.Name = "formatTCXGPXToolStripMenuItem";
            this.formatTCXGPXToolStripMenuItem.Size = new System.Drawing.Size(523, 54);
            this.formatTCXGPXToolStripMenuItem.Text = "Format TCX/GPX...";
            this.formatTCXGPXToolStripMenuItem.Click += new System.EventHandler(this.OnGpxTcxFormatTCX_GPXClick);
            // 
            // formatXMLToolStripMenuItem
            // 
            this.formatXMLToolStripMenuItem.Name = "formatXMLToolStripMenuItem";
            this.formatXMLToolStripMenuItem.Size = new System.Drawing.Size(523, 54);
            this.formatXMLToolStripMenuItem.Text = "Format XML...";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(520, 6);
            this.toolStripSeparator5.Click += new System.EventHandler(this.OnGpxTcxFormatXmlClick);
            // 
            // interpolateTCXFromGPXToolStripMenuItem
            // 
            this.interpolateTCXFromGPXToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inaccurateGPSToolStripMenuItem,
            this.gPSDropoutToolStripMenuItem});
            this.interpolateTCXFromGPXToolStripMenuItem.Name = "interpolateTCXFromGPXToolStripMenuItem";
            this.interpolateTCXFromGPXToolStripMenuItem.Size = new System.Drawing.Size(523, 54);
            this.interpolateTCXFromGPXToolStripMenuItem.Text = "Interpolate TCX from GPX";
            this.interpolateTCXFromGPXToolStripMenuItem.Click += new System.EventHandler(this.OnGpxTcxInterpolateTcxClick);
            // 
            // inaccurateGPSToolStripMenuItem
            // 
            this.inaccurateGPSToolStripMenuItem.Name = "inaccurateGPSToolStripMenuItem";
            this.inaccurateGPSToolStripMenuItem.Size = new System.Drawing.Size(533, 54);
            this.inaccurateGPSToolStripMenuItem.Text = "Match Lat/Lon in Interval...";
            // 
            // gPSDropoutToolStripMenuItem
            // 
            this.gPSDropoutToolStripMenuItem.Name = "gPSDropoutToolStripMenuItem";
            this.gPSDropoutToolStripMenuItem.Size = new System.Drawing.Size(533, 54);
            this.gPSDropoutToolStripMenuItem.Text = "Use Whole Interval...";
            // 
            // deleteTCXTrackpointsToolStripMenuItem
            // 
            this.deleteTCXTrackpointsToolStripMenuItem.Name = "deleteTCXTrackpointsToolStripMenuItem";
            this.deleteTCXTrackpointsToolStripMenuItem.Size = new System.Drawing.Size(523, 54);
            this.deleteTCXTrackpointsToolStripMenuItem.Text = "Delete TCX Trackpoints...";
            this.deleteTCXTrackpointsToolStripMenuItem.Click += new System.EventHandler(this.OnGpxTcxDeleteTcxTrackpointsClick);
            // 
            // changeTCXTimesToolStripMenuItem
            // 
            this.changeTCXTimesToolStripMenuItem.Name = "changeTCXTimesToolStripMenuItem";
            this.changeTCXTimesToolStripMenuItem.Size = new System.Drawing.Size(523, 54);
            this.changeTCXTimesToolStripMenuItem.Text = "Change TCX Times...";
            this.changeTCXTimesToolStripMenuItem.Click += new System.EventHandler(this.OnGpxTcxChangeTimesTcxClick);
            // 
            // recalculateTCXToolStripMenuItem
            // 
            this.recalculateTCXToolStripMenuItem.Name = "recalculateTCXToolStripMenuItem";
            this.recalculateTCXToolStripMenuItem.Size = new System.Drawing.Size(523, 54);
            this.recalculateTCXToolStripMenuItem.Text = "Recalculate TCX...";
            this.recalculateTCXToolStripMenuItem.Click += new System.EventHandler(this.OnGpxTcxRecalculateTcxClick);
            // 
            // fixGPXToolStripMenuItem
            // 
            this.fixGPXToolStripMenuItem.Name = "fixGPXToolStripMenuItem";
            this.fixGPXToolStripMenuItem.Size = new System.Drawing.Size(523, 54);
            this.fixGPXToolStripMenuItem.Text = "Fix Polar Access GPX...";
            this.fixGPXToolStripMenuItem.Click += new System.EventHandler(this.OnGpxTcxFixPolarGpxClick);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(520, 6);
            // 
            // singleFileInfoToolStripMenuItem
            // 
            this.singleFileInfoToolStripMenuItem.Name = "singleFileInfoToolStripMenuItem";
            this.singleFileInfoToolStripMenuItem.Size = new System.Drawing.Size(523, 54);
            this.singleFileInfoToolStripMenuItem.Text = "Single File Info...";
            this.singleFileInfoToolStripMenuItem.Click += new System.EventHandler(this.OnGpxTcxSingleFileInfoClick);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(104, 48);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // statusToolStripMenuItem
            // 
            this.statusToolStripMenuItem.Name = "statusToolStripMenuItem";
            this.statusToolStripMenuItem.Size = new System.Drawing.Size(285, 54);
            this.statusToolStripMenuItem.Text = "Status...";
            this.statusToolStripMenuItem.Click += new System.EventHandler(this.OnHelpStatusClick);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(285, 54);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.OnAboutClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1627, 965);
            this.Controls.Add(this.tableLayoutPanelTop);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "GPX Viewer";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.tableLayoutPanelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private BrightIdeasSoftware.TreeListView treeListView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGPXTCXFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn col1;
        private System.Windows.Forms.ToolStripMenuItem statusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetModelFromTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetTreeFromModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem everythingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nothingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem collapseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandToLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem sendToGoogleEarthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gpxTcxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatTCXGPXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem interpolateTCXFromGPXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inaccurateGPSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gPSDropoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTCXTrackpointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeTCXTimesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recalculateTCXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixGPXToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem singleFileInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem showLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allFilesToolStripMenuItem2;
    }
}

