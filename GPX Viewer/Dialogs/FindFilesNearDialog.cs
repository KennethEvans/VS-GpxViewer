using GPXViewer.KML;
using KEGpsUtils;
using KEUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.Dialogs {
    public partial class FindFilesNearDialog : Form {
        public static readonly String NL = Environment.NewLine;
        public List<string> FoundFiles { get; set; }


        public FindFilesNearDialog() {
            InitializeComponent();
            getSettingsFromSettings();
        }

        private bool findFilesNear() {
            string directory = textBoxDirectory.Text;
            if (!Directory.Exists(directory)) {
                Utils.errMsg("Directory does not exist:" + NL + directory);
                return false;
            }
            double lat0, lon0, radius;
            try {
                lat0 = Convert.ToDouble(textBoxLatitude.Text);
            } catch (Exception) {
                Utils.errMsg("Invalid value for latitude");
                return false;
            }
            try {
                lon0 = Convert.ToDouble(textBoxLongitude.Text);
            } catch (Exception) {
                Utils.errMsg("Invalid value for longitude");
                return false;
            }
            try {
                radius = Convert.ToDouble(textBoxRadius.Text);
            } catch (Exception) {
                Utils.errMsg("Invalid value for radius");
                return false;
            }
            radius = getRadiusInMeters();
            bool doTracks = checkBoxTracks.Checked;
            bool doWaypoints = checkBoxWaypoints.Checked;
            IEnumerable<string> files = Directory.EnumerateFiles(directory,
                textBoxFilter.Text);
            foreach (string file in files) {
                if (find(file, lat0, lon0, radius, doTracks, doWaypoints)) {
                    FoundFiles.Add(file);
                }
            }
            return true;
        }

        /// <summary>
        /// Ckecks if the given file has points in side the given radius (in m).
        /// </summary>
        /// <param name="fileName">The file to search.</param>
        /// <param name="lat0">The latitude of the center point.</param>
        /// <param name="lon0">The latitude of the center point.</param>
        /// <param name="radius">Radius in meters.</param>
        /// <param name="doTracks">Whether to look at tracks or not.</param>
        /// <param name="doWaypoints">Whether to look at waypoints or not.</param>
        /// <returns></returns>
        private bool find(string fileName, double lat0, double lon0,
            double radius, bool doTracks, bool doWaypoints) {
            try {
                gpx gpxType = gpx.Load(fileName);
                double lat, lon;
                if (doTracks) {
                    // Do the tracks
                    IList<trkType> tracks = gpxType.trk;
                    foreach (trkType trk in tracks) {
                        foreach (trksegType seg in trk.trkseg) {
                            foreach (wptType wpt in seg.trkpt) {
                                lat = (double)wpt.lat;
                                lon = (double)wpt.lon;
                                if (GpsData.greatCircleDistance(lat0, lon0, lat, lon) <= radius) {
                                    return true;
                                }
                            }
                        }
                    }
                }
                if (doWaypoints) {
                    // Do the waypoints
                    IList<wptType> waypoints = gpxType.wpt;
                    foreach (wptType wpt in waypoints) {
                        lat = (double)wpt.lat;
                        lon = (double)wpt.lon;
                        if (GpsData.greatCircleDistance(lat0, lon0, lat, lon) <= radius) {
                            return true;
                        }
                    }
                }
            } catch (Exception) {
                return false;
            }
            return false;
        }

        private void OnDirectoryBrowseClick(object sender, EventArgs e) {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select a Directory to search";
            string fileName = "";
            // Set initial directory
            fileName = textBoxDirectory.Text;
            if (File.Exists(fileName)) {
                dlg.SelectedPath = fileName;
            }
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                textBoxDirectory.Text = dlg.SelectedPath;
            }
        }

        private void getSettingsFromSettings() {
            textBoxDirectory.Text = Properties.FindNear.Default.Directory;
            textBoxLatitude.Text = Properties.FindNear.Default.Latitude.ToString();
            textBoxLongitude.Text = Properties.FindNear.Default.Longitude.ToString();
            textBoxRadius.Text = Properties.FindNear.Default.Radius.ToString();
            textBoxFilter.Text = Properties.FindNear.Default.Filter.ToString();
            comboBoxUnits.SelectedIndex = comboBoxUnits.FindString(
                Properties.FindNear.Default.Units);
            checkBoxTracks.Checked = Properties.FindNear.Default.DoTracks;
            checkBoxWaypoints.Checked = Properties.FindNear.Default.DoWaypoints;
        }

        private void setSettingsFromDialog() {
            Properties.FindNear.Default.Directory = textBoxDirectory.Text;
            try {
                Properties.FindNear.Default.Latitude = Convert.ToDouble(textBoxLatitude.Text);
            } catch (Exception) {
                // Do nothing
            }
            try {
                Properties.FindNear.Default.Longitude = Convert.ToDouble(textBoxLongitude.Text);
            } catch (Exception) {
                // Do nothing
            }
            try {
                Properties.FindNear.Default.Radius = Convert.ToDouble(textBoxRadius.Text);
            } catch (Exception) {
                // Do nothing
            }
            Properties.FindNear.Default.Units = comboBoxUnits.Text;
            Properties.FindNear.Default.DoTracks = checkBoxTracks.Checked;
            Properties.FindNear.Default.DoWaypoints = checkBoxWaypoints.Checked;
        }

        private double getRadiusInMeters() {
            string units = comboBoxUnits.Text;
            double radius;
            try {
                radius = Convert.ToDouble(textBoxRadius.Text);
            } catch (Exception) {
                Utils.errMsg("Invalid value for radius");
                return 0;
            }
            // Convert the radius to meters
            if (units.Equals("ft")) {
                return radius /= GpsData.M2FT;
            } else if (units.Equals("mi")) {
                return radius /= GpsData.M2MI;
            } else if (units.Equals("meters")) {
                return radius;
            } else if (units.Equals("km")) {
                return radius /= 1000.0;
            } else {
                Utils.errMsg("Invalid value for units");
                return 0;
            }
        }

        private void OnPastePlacemarkClick(object sender, EventArgs e) {
            double[] coords = KmlUtils.coordinatesFromClipboardPlacemark();
            if (coords == null) {
                return;
            }
            if (Double.IsNaN(coords[0])) {
                textBoxLongitude.Text = "";
            } else {
                textBoxLongitude.Text = String.Format("{0:0.000000}", coords[0]);
            }
            if (Double.IsNaN(coords[1])) {
                textBoxLatitude.Text = "";
            } else {
                textBoxLatitude.Text = String.Format("{0:0.000000}", coords[1]);
            }
        }

        private void OnCopyPlacemarkClick(object sender, EventArgs e) {
            string name = "Find Center";
            string lat = textBoxLatitude.Text;
            string lon = textBoxLongitude.Text;
            string ele = "0";
            KmlUtils.copyPlacemarkToClipboard(null, name, lat,
                lon, ele);
        }

        private void OnCopyCircleClick(object sender, EventArgs e) {
            string name = "Find Radius";
            string lat = textBoxLatitude.Text;
            string lon = textBoxLongitude.Text;
            string ele = "0";
            double radius = getRadiusInMeters();
             KmlUtils.copyPlacemarkCircleToClipboard(null,
                name, lat, lon, ele, radius);
        }

        private void OnCancelClick(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Visible = false;
        }

        private void OnOkClick(object sender, EventArgs e) {
            Cursor.Current = Cursors.WaitCursor;
            FoundFiles = new List<string>();
            if (findFilesNear()) {
                this.DialogResult = DialogResult.OK;
                setSettingsFromDialog();
            } else {
                this.DialogResult = DialogResult.Abort;
            }
            Cursor.Current = Cursors.Default;
            this.DialogResult = DialogResult.OK;
            this.Visible = false;
        }
    }
}
