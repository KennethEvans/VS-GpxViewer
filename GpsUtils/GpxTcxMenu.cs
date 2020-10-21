#undef FIX_POLAR_GPX_WITHOUT_PROMPT_TO_SAVE

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using www.garmin.com.xmlschemas.TrainingCenterDatabase.v2;
using www.topografix.com.GPX_1_1;

namespace KEGpsUtils {
    public class GpxTcxMenu {
        public static readonly String NL = Environment.NewLine;

        /// <summary>
        /// Used in events.
        /// </summary>
        public enum EventType { MSG, INFO, WARN, ERR, EXC }

        /// <summary>
        /// The event handler.
        /// </summary>
        public event EventHandler<GpxTcxEventArgs> GpxTcxEvent;

        /// <summary>
        /// Raises a GpxTcxEvent.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <param name="type">The type of the event.</param>
        /// <param name="msg">The message to include in the event.</param>
        protected virtual void raiseGpxTcxEvent(GpxTcxEventArgs e) {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<GpxTcxEventArgs> raiseEvent = GpxTcxEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null) {
                raiseEvent(this, e);
            }
        }

        /// <summary>
        /// Creates the menu and inserts into the the given MenuStrip at the
        /// given index.
        /// </summary>
        /// <param name="menuStrip">The menu strip where this menu is to be inserted.</param>
        /// <param name="index">The index where the menu is to be inserted.</param>
        public void createMenu(MenuStrip menuStrip, int index) {
            ToolStripMenuItem gpxTcxToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem formatTCXGPXToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem formatXMLToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripSeparator toolStripSeparator1 =
                new System.Windows.Forms.ToolStripSeparator();
            ToolStripMenuItem convertGPXToTCXToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem convertTCXToGPXToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem interpolateTCXFromGPXToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem deleteTCXTrackpointsToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem changeTCXTimesToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem recalculateTCXToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem fixGPXToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripSeparator toolStripSeparator2 =
                new System.Windows.Forms.ToolStripSeparator();
            ToolStripMenuItem singleFileInfoToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem useWholeIntervalToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItem matchLatLonToolStripMenuItem =
                new System.Windows.Forms.ToolStripMenuItem();

            // 
            // gpxTcxToolStripMenuItem (has subitems)
            // 
            gpxTcxToolStripMenuItem.DropDownItems.AddRange(
                new System.Windows.Forms.ToolStripItem[] {
            formatTCXGPXToolStripMenuItem,
            formatXMLToolStripMenuItem,
            toolStripSeparator1,
            convertGPXToTCXToolStripMenuItem,
            convertTCXToGPXToolStripMenuItem,
            interpolateTCXFromGPXToolStripMenuItem,
            deleteTCXTrackpointsToolStripMenuItem,
            changeTCXTimesToolStripMenuItem,
            recalculateTCXToolStripMenuItem,
            fixGPXToolStripMenuItem,
            toolStripSeparator2,
            singleFileInfoToolStripMenuItem});
            gpxTcxToolStripMenuItem.Name = "gpxTcxToolStripMenuItem";
            gpxTcxToolStripMenuItem.Size = new System.Drawing.Size(161, 48);
            gpxTcxToolStripMenuItem.Text = "GPX/TCX";
            // 
            // formatTCXGPXToolStripMenuItem
            // 
            formatTCXGPXToolStripMenuItem.Name = "formatTCXGPXToolStripMenuItem";
            formatTCXGPXToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            formatTCXGPXToolStripMenuItem.Text = "Format TCX/GPX...";
            formatTCXGPXToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxFormatTCX_GPXClick);
            // 
            // formatXMLToolStripMenuItem
            // 
            formatXMLToolStripMenuItem.Name = "formatXMLToolStripMenuItem";
            formatXMLToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            formatXMLToolStripMenuItem.Text = "Format XML...";
            formatTCXGPXToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxFormatXmlClick);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(541, 6);
            toolStripSeparator1.Click +=
                new System.EventHandler(OnGpxTcxFormatXmlClick);
            // 
            // convertGPXToTCXToolStripMenuItem
            // 
            convertGPXToTCXToolStripMenuItem.Name = "convertGPXToTCXToolStripMenuItem";
            convertGPXToTCXToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            convertGPXToTCXToolStripMenuItem.Text = "Convert GPX to TCX...";
            convertGPXToTCXToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxConvertTCXToGpxClick);
            // 
            // convertTCXToGPXToolStripMenuItem
            // 
            convertTCXToGPXToolStripMenuItem.Name = "convertTCXToGPXToolStripMenuItem";
            convertTCXToGPXToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            convertTCXToGPXToolStripMenuItem.Text = "Convert TCX to GPX...";
            convertTCXToGPXToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxConvertGpxToTcxClick);
            // 
            // interpolateTCXFromGPXToolStripMenuItem (has subitems)
            // 
            interpolateTCXFromGPXToolStripMenuItem.DropDownItems.AddRange(
                new System.Windows.Forms.ToolStripItem[] {
            matchLatLonToolStripMenuItem,
            useWholeIntervalToolStripMenuItem});
            interpolateTCXFromGPXToolStripMenuItem.Name = "interpolateTCXFromGPXToolStripMenuItem";
            interpolateTCXFromGPXToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            interpolateTCXFromGPXToolStripMenuItem.Text = "Interpolate TCX from GPX...";
            // 
            // matchLatLonToolStripMenuItem
            // 
            matchLatLonToolStripMenuItem.Name = "inaccurateGPSToolStripMenuItem";
            matchLatLonToolStripMenuItem.Size = new System.Drawing.Size(533, 54);
            matchLatLonToolStripMenuItem.Text = "Match Lat/Lon in Interval...";
            matchLatLonToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxInterpolateTcxClick);
            // 
            // useWholeIntervalToolStripMenuItem
            // 
            useWholeIntervalToolStripMenuItem.Name = "gpsDropoutToolStripMenuItem";
            useWholeIntervalToolStripMenuItem.Size = new System.Drawing.Size(533, 54);
            useWholeIntervalToolStripMenuItem.Text = "Use Whole Interval...";
            useWholeIntervalToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxInterpolateTcxClick);
            // 
            // deleteTCXTrackpointsToolStripMenuItem
            // 
            deleteTCXTrackpointsToolStripMenuItem.Name = "deleteTCXTrackpointsToolStripMenuItem";
            deleteTCXTrackpointsToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            deleteTCXTrackpointsToolStripMenuItem.Text = "Delete TCX Trackpoints...";
            deleteTCXTrackpointsToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxDeleteTcxTrackpointsClick);
            // 
            // changeTCXTimesToolStripMenuItem
            // 
            changeTCXTimesToolStripMenuItem.Name = "changeTCXTimesToolStripMenuItem";
            changeTCXTimesToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            changeTCXTimesToolStripMenuItem.Text = "Change TCX Times...";
            changeTCXTimesToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxChangeTimesTcxClick);
            // 
            // recalculateTCXToolStripMenuItem
            // 
            recalculateTCXToolStripMenuItem.Name = "recalculateTCXToolStripMenuItem";
            recalculateTCXToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            recalculateTCXToolStripMenuItem.Text = "Recalculate TCX...";
            recalculateTCXToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxRecalculateTcxClick);
            // 
            // fixGPXToolStripMenuItem
            // 
            fixGPXToolStripMenuItem.Name = "fixGPXToolStripMenuItem";
            fixGPXToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            fixGPXToolStripMenuItem.Text = "Fix Polar Access GPX...";
            fixGPXToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxFixPolarGpxClick);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(541, 6);
            // 
            // singleFileInfoToolStripMenuItem
            // 
            singleFileInfoToolStripMenuItem.Name = "singleFileInfoToolStripMenuItem";
            singleFileInfoToolStripMenuItem.Size = new System.Drawing.Size(544, 54);
            singleFileInfoToolStripMenuItem.Text = "Single File Info...";
            singleFileInfoToolStripMenuItem.Click +=
                new System.EventHandler(OnGpxTcxSingleFileInfoClick);

            ToolStripItemCollection items = menuStrip.Items;
            items.Insert(index, gpxTcxToolStripMenuItem);
        }

        public void formatTcxGpx() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX and TCX|*.gpx;*.tcx|GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select files to format";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    string msg = "Format TCX/GPX: Failed to open files to process";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    formatSingleTcxGpx(fileName);
                }
            }
        }

        public void formatSingleTcxGpx(string fileName) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                string msg = "formatSingleTcxGpx: Cannot handle files with no extension";
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                return;
            }
            if (ext.ToLower().Equals(".tcx")) {
                TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(fileName);
                string saveFilename = getSaveName(fileName, ".formatted");
                if (saveFilename != null) tcx.Save(saveFilename);
            } else if (ext.ToLower().Equals(".gpx")) {
                gpx gpx = gpx.Load(fileName);
                string saveFilename = getSaveName(fileName, ".formatted");
                if (saveFilename != null) gpx.Save(saveFilename);
            } else {
                string msg = "Not a supported extension: " + ext;
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                return;
            }
        }

        public void formatXml() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Typical|*.gpx;*.tcx;*.xml|GPX|*.gpx|TCX|*.tcx|XML|*.xml";
            dlg.Title = "Select files to format";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    string msg = "Format XML: Failed to open files to process";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    formatXml(fileName);
                }
            }
        }

        public void formatXml(string fileName) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                string msg = "formatXml: Cannot handle files with no extension";
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                return;
            }
            XDocument doc = XDocument.Load(fileName);
            string saveFilename = getSaveName(fileName, ".formatted-xml");
            if (saveFilename != null) doc.Save(saveFilename);
        }

        /// <summary>
        /// Prompts for a filename using a SaveFileDialog.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tag">String to be added before the extension,
        /// e.g. ".test".</param>
        /// <returns>The filename or null on cancel or failure.</returns>
        public static string getSaveName(string fileName, string tag) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Select saved file";
            string directory = Path.GetDirectoryName(fileName);
            string ext = Path.GetExtension(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);
            dlg.InitialDirectory = directory;
            dlg.FileName = name + tag + ext;
            if (ext.ToLower().Equals(".tcx")) {
                dlg.Filter = "TCX|*.tcx";
            } else if (ext.ToLower().Equals(".gpx")) {
                dlg.Filter = "GPX|*.gpx";
            } else if (ext.ToLower().Equals(".xml")) {
                dlg.Filter = "XML|*.xml";
            } else {
                dlg.Filter = "All files|*.*";
            }
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                return dlg.FileName;
            } else {
                return null;
            }
        }

        public void convertGpxToTcx() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX|*.gpx";
            dlg.Title = "Select files to convert";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    string msg = "Convert GPX to TCX: Failed to open files to convert";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    convertGpxToTcxSingle(fileName);
                }
            }
        }

        public static void convertGpxToTcxSingle(string fileName) {
            gpx gpx = gpx.Load(fileName);
            TrainingCenterDatabase tcx = GpsData.convertGpxToTcx(gpx);
            if (tcx != null) {
                fileName = Path.ChangeExtension(fileName, ".tcx");
                string saveFilename = getSaveName(fileName, ".converted");
                if (saveFilename != null) {
                    tcx.Save(saveFilename);
                }
            }
        }

        public void convertTcxToGpx() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select files to convert";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    string msg = "Convert TCX to GPX: Failed to open files to convert";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    convertTcxToGpxSingle(fileName);
                }
            }
        }

        public static void convertTcxToGpxSingle(string fileName) {
            TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(fileName);
            gpx gpx = GpsData.convertTcxToGpx(tcx);
            if (gpx != null) {
                fileName = Path.ChangeExtension(fileName, ".gpx");
                string saveFilename = getSaveName(fileName, ".converted");
                if (saveFilename != null) {
                    gpx.Save(saveFilename);
                }
            }
        }

        public void getSingleFileInfo() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX and TCX|*.gpx;*.tcx|GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select files for info";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    string msg = "Get Single File Info: Failed to open files to process";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    getSingleFileInfo(fileName);
                }
            }
        }

        public void getSingleFileInfo(string fileName) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                string msg = "getSingleFileInfo: Cannot handle files with no extension";
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                return;
            }
            if (ext.ToLower().Equals(".tcx")) {
                try {
                    GpsData data = GpsData.processTcx(fileName);
                    string msg = NL + data.info();
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                } catch (Exception ex) {
                    string msg = "Error getting TCX single file info";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.EXC, msg, ex));
                    return;
                }
            } else if (ext.ToLower().Equals(".gpx")) {
                try {
                    GpsData data = GpsData.processGpx(fileName);
                    string msg = NL + data.info();
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                } catch (Exception ex) {
                    string msg = "Error getting GPX single file info";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.EXC, msg, ex));
                    return;
                }
            } else {
                string msg = "Not supported extension: " + ext;
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                return;
            }
        }

        public void recalculateTcx() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select files to recalculate";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    string msg = "Recalculate TCX: Failed to open files to process";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    recalculateTcx(fileName);
                }
            }
        }

        public void recalculateTcx(string fileName) {
            try {
                TrainingCenterDatabase tcx = GpsData.recalculateTcx(fileName);
                string saveFileName = getSaveName(fileName, ".recalculated");
                if (saveFileName != null) {
                    tcx.Save(saveFileName);
                    string msg = NL + "Recalculated " + fileName + NL
                        + "  Output is " + saveFileName;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                } else {
                    return;
                }
            } catch (Exception ex) {
                string msg = "Error recalculating TCX";
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.EXC, msg, ex));
                return;
            }
        }

        public void interpolateTcxFromGpx(GpsData.InterpolateMode mode) {
            string tcxFile = null, gpxFile = null;
            OpenFileDialog dlg1 = new OpenFileDialog();
            dlg1.Filter = "TCX|*.tcx";
            dlg1.Title = "Select TCX file to interpolate to";
            dlg1.Multiselect = false;
            if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg1.FileName == null) {
                    string msg = "Interpolate TCX fro GPX: Failed to open" +
                        " file in which to interpolate";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                tcxFile = dlg1.FileName;
            } else {
                return;
            }
            OpenFileDialog dlg2 = new OpenFileDialog();
            dlg2.Filter = "GPX|*.gpx";
            dlg2.Title = "Select GPX file to interpolate from";
            dlg2.Multiselect = false;
            if (dlg2.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg2.FileName == null) {
                    string msg = "Interpolate TCX form GPX: Failed to" +
                        " open GPX file from which to interpolate";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                }
                gpxFile = dlg2.FileName;
                interpolateTcxFromGpx(tcxFile, gpxFile, mode);
            }
        }

        public void interpolateTcxFromGpx(string tcxFile, string gpxFile,
            GpsData.InterpolateMode mode) {
            try {
                TcxResult res =
                    GpsData.interpolateTcxFromGpx(tcxFile, gpxFile, mode);
                if (res.TCX == null) {
                    string msg = "Interpolate Tcx From Gpx failed:" + NL
                        + "for " + Path.GetFileName(tcxFile) + NL
                        + " and " + Path.GetFileName(gpxFile) + NL
                        + res.Message;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                    return;
                }
                TrainingCenterDatabase tcxInterp = res.TCX;

                string saveFileName = getSaveName(tcxFile, ".interpolated");
                if (saveFileName != null) {
                    tcxInterp.Save(saveFileName);
                    string msg = NL + "Recalculated " + tcxFile + NL
                        + "  from " + gpxFile + NL
                        + "  Output is " + saveFileName
                        + NL + "  " + res.Message;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                } else {
                    return;
                }
            } catch (Exception ex) {
                string msg = "Error interpolating TCX from GPX";
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.EXC, msg, ex));
                return;
            }
        }

        public void deleteTcxTrackpoints() {
            string tcxFile = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select TCX file to delete trackpoints from";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileName == null) {
                    string msg = "Delete TCX Trackpoints: Failed to open" +
                        " file from which to delete trackpoints";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                tcxFile = dlg.FileName;
                deleteTcxTrackpoints(tcxFile);
            }
        }

        public void deleteTcxTrackpoints(string tcxFile) {
            try {
                TcxResult res =
                    GpsData.deleteTcxTrackpoints(tcxFile);
                if (res.TCX == null) {
                    string msg = "Delete trackpoints from TCX failed:" + NL
                        + "for " + Path.GetFileName(tcxFile) + NL
                        + res.Message;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                    return;
                }
                string saveFileName = getSaveName(tcxFile, ".trimmed");
                if (saveFileName != null) {
                    res.TCX.Save(saveFileName);
                    string msg = NL + "Trimmed " + tcxFile + NL
                        + "  Output is " + saveFileName
                        + NL + "  " + res.Message;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                } else {
                    return;
                }
            } catch (Exception ex) {
                string msg = "Error deleting trackpoints from TCX";
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.EXC, msg, ex));
                return;
            }
        }

        public void changeTimesTcx() {
            string tcxFile = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select TCX file to change times in";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileName == null) {
                    string msg = "Change TCX Times: Failed to open file" +
                        " in which to change times";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg));
                    return;
                }
                tcxFile = dlg.FileName;
                changeTimesTcx(tcxFile);
            }
        }

        public void changeTimesTcx(string tcxFile) {
            try {
                TcxResult res =
                    GpsData.changeTimesTcx(tcxFile);
                if (res.TCX == null) {
                    string msg = "Change times in TCX failed:" + NL
                        + "for " + Path.GetFileName(tcxFile) + NL
                        + res.Message;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                    return;
                }
                string saveFileName = getSaveName(tcxFile, ".timechange");
                if (saveFileName != null) {
                    res.TCX.Save(saveFileName);
                    string msg = NL + "Changed times in " + tcxFile + NL
                        + "  Output is " + saveFileName
                        + NL + "  " + res.Message;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                } else {
                    return;
                }
            } catch (Exception ex) {
                string msg = "Error changing times in TCX";
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.EXC, msg, ex));
                return;
            }
        }

        public void fixPolarGpx() {
            bool silent = false;
            string msg = "You will be prompted to save the fixed files. Would "
                 + "you prefer to replace the existing files with the fixed "
                 + "version silently, keeping their modified times?";
            DialogResult res = MessageBox.Show(msg,
              "warning", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel) {
                string msg1 = NL + "Fix Polar Access GPX: Cancelled";
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg1));
                return;
            }
            if (res == DialogResult.Yes) {
                silent = true;
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX|*.gpx";
            dlg.Title = "Select GPX files to fix";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    string msg1 = "Fix Polar Access GPX: Failed to open files to fix";
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.WARN, msg1));
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    fixPolarGpx(fileName, silent);
                }
            }
        }

        public void fixPolarGpx(string gpxFile, bool silent) {
            try {
                GpxResult res =
                    GpsData.fixPolarGpx(gpxFile);
                if (res.GPX == null) {
                    string msg = "Fix Polar Access GPX: Fixing GPX failed:" + NL
                        + "for " + Path.GetFileName(gpxFile) + NL
                        + res.Message;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.ERR, msg));
                    return;
                }
                if (res.Message.StartsWith("Unmodified")) {
                    string msg = NL + "Fix Polar Access GPX: No changes to" +
                        " be made in " + gpxFile;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                    return;
                }
                if (silent) {
                    // Overwrite the existing file and restore the modified time
                    DateTime lastModifiedTime = File.GetLastWriteTime(gpxFile);
                    res.GPX.Save(gpxFile);
                    File.SetLastWriteTime(gpxFile, lastModifiedTime);
                    string msg = NL + "Fix Polar Access GPX: Overwrote "
                        + gpxFile + NL + res.Message;
                    raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                } else {
                    string saveFileName = getSaveName(gpxFile, ".fixed");
                    if (saveFileName != null) {
                        res.GPX.Save(saveFileName);
                        string msg = NL + "Fix Polar Access GPX: Changed times in "
                            + gpxFile + NL
                            + "  Output is " + saveFileName
                            + NL + "  " + res.Message;
                        raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                    } else {
                        string msg = NL + "Fix Polar Access GPX: Failed to change "
                          + "times in " + gpxFile + NL;
                        raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.MSG, msg));
                    }
                }
            } catch (Exception ex) {
                string msg = "Fix Polar Access GPX: Error fixing Gpx for "
                    + gpxFile;
                raiseGpxTcxEvent(new GpxTcxEventArgs(EventType.EXC, msg, ex));
                return;
            }
        }

        private void OnGpxTcxFormatTCX_GPXClick(object sender, EventArgs e) {
            formatTcxGpx();
        }

        private void OnGpxTcxFormatXmlClick(object sender, EventArgs e) {
            formatXml();
        }

        private void OnGpxTcxSingleFileInfoClick(object sender, EventArgs e) {
            getSingleFileInfo();
        }

        private void OnGpxTcxChangeTimesTcxClick(object sender, EventArgs e) {
            changeTimesTcx();
        }

        private void OnGpxTcxRecalculateTcxClick(object sender, EventArgs e) {
            recalculateTcx();
        }

        private void OnGpxTcxInterpolateTcxClick(object sender, EventArgs e) {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            GpsData.InterpolateMode mode;
            if (item.Text.StartsWith("Match")) {
                mode = GpsData.InterpolateMode.MatchLatLon;
            } else if (item.Text.StartsWith("Use")) {
                mode = GpsData.InterpolateMode.UseInterval;
            } else {
                return;
            }
            interpolateTcxFromGpx(mode);
        }

        private void OnGpxTcxDeleteTcxTrackpointsClick(object sender, EventArgs e) {
            deleteTcxTrackpoints();
        }

        private void OnGpxTcxFixPolarGpxClick(object sender, EventArgs e) {
            fixPolarGpx();
        }

        private void OnGpxTcxConvertGpxToTcxClick(object sender, EventArgs e) {
            convertGpxToTcx();
        }

        private void OnGpxTcxConvertTCXToGpxClick(object sender, EventArgs e) {
            convertTcxToGpx();
        }
    }

    // Define a class to hold custom event info
    public class GpxTcxEventArgs : EventArgs {
        public GpxTcxMenu.EventType Type { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; } = null;

        public GpxTcxEventArgs(GpxTcxMenu.EventType type, string message) {
            Type = type;
            Message = message;
        }

        public GpxTcxEventArgs(GpxTcxMenu.EventType type, string message, Exception ex) {
            Type = type;
            Message = message;
            Exception = ex;
        }
    }
}
