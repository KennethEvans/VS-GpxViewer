using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using KEGpsUtils;
using KEUtils;
using ScrolledText;
using www.garmin.com.xmlschemas.TrainingCenterDatabase.v2;
using www.topografix.com.GPX_1_1;

namespace GPXViewer {
    public partial class MainForm : Form {
        public void gpxTxcCallback(string info) {
            if (textDlg == null) {
                MainForm app = (MainForm)FindForm().FindForm();
                textDlg = new ScrolledTextDialog(
                    Utils.getDpiAdjustedSize(app, new Size(600, 400)),
                    info);
                textDlg.Text = "GpxViewer Log";
                textDlg.Show();
            } else {
                textDlg.Visible = true;
            }
            textDlg.appendTextAndNL(info);
        }

        public static void formatTcxGpx() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX and TCX|*.gpx;*.tcx|GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select files to format";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    formatSingleTcxGpx(fileName);
                }
            }
        }

        public static void formatSingleTcxGpx(string fileName) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                Utils.errMsg("formatSingleTcxGpx: Cannot handle files with no extension");
                return;
            }
            if (ext.ToLower().Equals(".tcx")) {
                TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(fileName);
                string saveFilename = getSaveName(fileName, ".formatted");
                if (saveFilename != null) tcx.Save(saveFilename);
            } else if (ext.ToLower().Equals(".gpx")) {
                gpx gpxType = gpx.Load(fileName);
                string saveFilename = getSaveName(fileName, ".formatted");
                if (saveFilename != null) gpxType.Save(saveFilename);
            } else {
                Utils.errMsg("Not a supported extension: " + ext);
                return;
            }
        }

        public static void formatXml() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Typical|*.gpx;*.tcx;*.xml|GPX|*.gpx|TCX|*.tcx|XML|*.xml";
            dlg.Title = "Select files to format";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    formatXml(fileName);
                }
            }
        }

        public static void formatXml(string fileName) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                Utils.errMsg("formatXml: Cannot handle files with no extension");
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

        public static void getSingleFileInfo(MainForm mainForm) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX and TCX|*.gpx;*.tcx|GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select files for info";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    getSingleFileInfo(fileName, mainForm);
                }
            }
        }

        public static void getSingleFileInfo(string fileName, MainForm mainForm) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                Utils.errMsg("getSingleFileInfo: Cannot handle files with no extension");
                return;
            }
            if (ext.ToLower().Equals(".tcx")) {
                try {
                    GpsUtils data = GpsUtils.processTcx(fileName);
                    mainForm.gpxTxcCallback(NL + data.info());
                } catch (Exception ex) {
                    Utils.excMsg("Error getting TCX single file info", ex);
                    return;
                }
            } else if (ext.ToLower().Equals(".gpx")) {
                try {
                    GpsUtils data = GpsUtils.processGpx(fileName);
                    mainForm.gpxTxcCallback(NL + data.info());
                } catch (Exception ex) {
                    Utils.excMsg("Error getting GPX single file info", ex);
                    return;
                }
            } else {
                Utils.errMsg("Not supported extension: " + ext);
                return;
            }
        }

        public static void recalculateTcx(MainForm mainForm) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select files to recalculate";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    recalculateTcx(fileName, mainForm);
                }
            }
        }

        public static void recalculateTcx(string fileName, MainForm mainForm) {
            try {
                TrainingCenterDatabase tcx = GpsUtils.recalculateTcx(fileName);
                string saveFileName = getSaveName(fileName, ".recalculated");
                if (saveFileName != null) {
                    tcx.Save(saveFileName);
                    mainForm.gpxTxcCallback(NL + "Recalculated " + fileName + NL
                        + "  Output is " + saveFileName);
                } else {
                    return;
                }
            } catch (Exception ex) {
                Utils.excMsg("Error recalculating TCX", ex);
                return;
            }
        }

        public static void interpolateTcxFromGpx(MainForm mainForm, GpsUtils.InterpolateMode mode) {
            string tcxFile = null, gpxFile = null;
            OpenFileDialog dlg1 = new OpenFileDialog();
            dlg1.Filter = "TCX|*.tcx";
            dlg1.Title = "Select TCX file to interpolate to";
            dlg1.Multiselect = false;
            if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg1.FileName == null) {
                    Utils.warnMsg("Failed to open file to interpolate to");
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
                    Utils.warnMsg("Failed to open GPX file to interpolate from");
                    return;
                }
                gpxFile = dlg2.FileName;
                interpolateTcxFromGpx(tcxFile, gpxFile, mainForm, mode);
            }
        }

        public static void interpolateTcxFromGpx(string tcxFile, string gpxFile,
            MainForm mainForm, GpsUtils.InterpolateMode mode) {
            try {
                TcxResult res =
                    GpsUtils.interpolateTcxFromGpx(tcxFile, gpxFile, mode);
                if (res.TCX == null) {
                    Utils.errMsg("Interpolate Tcx From Gpx failed:" + NL
                        + "for " + Path.GetFileName(tcxFile) + NL
                        + " and " + Path.GetFileName(gpxFile) + NL
                        + res.Message);
                    return;
                }
                TrainingCenterDatabase tcxInterp = res.TCX;

                string saveFileName = getSaveName(tcxFile, ".interpolated");
                if (saveFileName != null) {
                    tcxInterp.Save(saveFileName);
                    mainForm.gpxTxcCallback(NL + "Recalculated " + tcxFile + NL
                        + "  from " + gpxFile + NL
                        + "  Output is " + saveFileName
                        + NL + "  " + res.Message);
                } else {
                    return;
                }
            } catch (Exception ex) {
                Utils.excMsg("Error interpolating TCX from GPX", ex);
                return;
            }
        }

        public static void deleteTcxTrackpoints(MainForm mainForm) {
            string tcxFile = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select TCX file to delete trackpoints from";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileName == null) {
                    Utils.warnMsg("Failed to open file to delete trackpoints from");
                    return;
                }
                tcxFile = dlg.FileName;
                deleteTcxTrackpoints(tcxFile, mainForm);
            }
        }

        public static void deleteTcxTrackpoints(string tcxFile, MainForm mainForm) {
            try {
                TcxResult res =
                    GpsUtils.deleteTcxTrackpoints(tcxFile);
                if (res.TCX == null) {
                    Utils.errMsg("Delete trackpoints from TCX failed:" + NL
                        + "for " + Path.GetFileName(tcxFile) + NL
                        + res.Message);
                    return;
                }
                string saveFileName = getSaveName(tcxFile, ".trimmed");
                if (saveFileName != null) {
                    res.TCX.Save(saveFileName);
                    mainForm.gpxTxcCallback(NL + "Trimmed " + tcxFile + NL
                        + "  Output is " + saveFileName
                        + NL + "  " + res.Message);
                } else {
                    return;
                }
            } catch (Exception ex) {
                Utils.excMsg("Error trackpoints from TCX", ex);
                return;
            }
        }

        public static void changeTimesTcx(MainForm mainForm) {
            string tcxFile = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select TCX file to change times in";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileName == null) {
                    Utils.warnMsg("Failed to open file to change times in");
                    return;
                }
                tcxFile = dlg.FileName;
                changeTimesTcx(tcxFile, mainForm);
            }
        }

        public static void changeTimesTcx(string tcxFile, MainForm mainForm) {
            try {
                TcxResult res =
                    GpsUtils.changeTimesTcx(tcxFile);
                if (res.TCX == null) {
                    Utils.errMsg("Change times in TCX failed:" + NL
                        + "for " + Path.GetFileName(tcxFile) + NL
                        + res.Message);
                    return;
                }
                string saveFileName = getSaveName(tcxFile, ".timechange");
                if (saveFileName != null) {
                    res.TCX.Save(saveFileName);
                    mainForm.gpxTxcCallback(NL + "Changed times in " + tcxFile + NL
                        + "  Output is " + saveFileName
                        + NL + "  " + res.Message);
                } else {
                    return;
                }
            } catch (Exception ex) {
                Utils.excMsg("Error change times in TCX", ex);
                return;
            }
        }

        public static void fixPolarGpx(MainForm mainForm) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX|*.gpx";
            dlg.Title = "Select GPX files to fix";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to fix");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    fixPolarGpx(fileName, mainForm);
                }
            }
        }

        public static void fixPolarGpx(string gpxFile, MainForm mainForm) {
            try {
                GpxResult res =
                    GpsUtils.fixPolarGpx(gpxFile);
                if (res.GPX == null) {
                    Utils.errMsg("Fixing GPX failed:" + NL
                        + "for " + Path.GetFileName(gpxFile) + NL
                        + res.Message);
                    return;
                }
                if (res.Message.StartsWith("Unmodified")) {
                    mainForm.gpxTxcCallback(NL + "Did not change " + gpxFile + NL);
                    return;
                }
                // Overwrite the existing file and restore the modified time
                DateTime lastModifiedTime = File.GetLastWriteTime(gpxFile);
                res.GPX.Save(gpxFile);
                File.SetLastWriteTime(gpxFile, lastModifiedTime);
                mainForm.gpxTxcCallback(NL + "Overwrote " + gpxFile + NL
                    + res.Message);
            } catch (Exception ex) {
                Utils.excMsg("Error fixing Gpx for " + gpxFile, ex);
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
            getSingleFileInfo(this);
        }

        private void OnGpxTcxChangeTimesTcxClick(object sender, EventArgs e) {
            changeTimesTcx(this);
        }

        private void OnGpxTcxRecalculateTcxClick(object sender, EventArgs e) {
            recalculateTcx(this);
        }

        private void OnGpxTcxInterpolateTcxClick(object sender, EventArgs e) {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            GpsUtils.InterpolateMode mode;
            if (item.Text.StartsWith("Match")) {
                mode = GpsUtils.InterpolateMode.MatchLatLon;
            } else if (item.Text.StartsWith("Use")) {
                mode = GpsUtils.InterpolateMode.UseInterval;
            } else {
                return;
            }
            interpolateTcxFromGpx(this, mode);
        }

        private void OnGpxTcxDeleteTcxTrackpointsClick(object sender, EventArgs e) {
            deleteTcxTrackpoints(this);
        }

        private void OnGpxTcxFixPolarGpxClick(object sender, EventArgs e) {
            string msg = "Existing files will be overwritten with the fixed version."
                + NL + "The modified time will be retained.";
            DialogResult res = MessageBox.Show(msg + "\n" + "OK to continue?",
              "Warning", MessageBoxButtons.YesNo);
            if (res == DialogResult.No) {
                return;
            }
            fixPolarGpx(this);
        }


    }
}
