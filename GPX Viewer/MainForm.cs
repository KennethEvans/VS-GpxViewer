using About;
using GPXViewer.KML;
using GPXViewer.model;
using KEUtils;
using ScrolledText;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Windows.Forms;

namespace GPXViewer {
    public partial class MainForm : Form {
#if true
        private static readonly string DEBUG_FILE_NAME = @"C:\Users\evans\Documents\GPSLink\Polar\Kenneth_Evans_2020-10-01_10-39-30_Walking_MT.gpx";
#else
        private static readonly string DEBUG_FILE_NAME = @"C:\Users\evans\Documents\GPSLink\Test\AAAtest9.gpx";
#endif
        private static ScrolledTextDialog textDlg;

        public static readonly String NL = Environment.NewLine;
        public GpxFileSetModel FileSet { get; set; }
        public List<GpxFileModel> Files { get; set; }
        public ImageList ImageList { get; set; }
        public Size ImageSize { get; set; }
        public PointF DPI { get; set; }
        
        public MainForm() {
            InitializeComponent();

            FileSet = new GpxFileSetModel(null);
            Files = FileSet.Files;
#if true
            // Load a file for testing
            try {
                Files.Add(new GpxFileModel(FileSet, DEBUG_FILE_NAME));
            } catch (Exception ex) {
                Utils.excMsg("Failed to load debug file", ex);
            }
#endif
        }

        private void OnFormLoad(object sender, EventArgs e) {
            // DPI
            float dpiX, dpiY;
            using (Graphics g = this.CreateGraphics()) {
                dpiX = g.DpiX;
                dpiY = g.DpiY;
            }
            DPI = new PointF(dpiX, dpiY);
            ImageSize = new Size((int)(16 * dpiX / 96), (int)(16 * dpiY / 96));

            // CanExpand getter
            treeListView.CanExpandGetter = delegate (object x) {
                return x is GpxFileSetModel || (x is GpxFileModel)
                || (x is GpxTrackModel) || (x is GpxTrackSegmentModel)
                || (x is GpxRouteModel rte);
            };
            // Children getter
            treeListView.ChildrenGetter = delegate (object x) {
                if (x is GpxFileSetModel fileSet) {
                    return fileSet.Files;
                }
                if (x is GpxFileModel file) {
                    bool multiple = false;
                    if (file.Waypoints != null && file.Waypoints.Count > 0) {
                        multiple = true;
                    }
                    if (file.Routes != null && file.Routes.Count > 0) {
                        multiple = true;
                    }
                    if (multiple) {
                        List<GpxModel> children = new List<GpxModel>();
                        foreach (GpxTrackModel track in file.Tracks) {
                            children.Add(track);
                        }
                        foreach (GpxWaypointModel waypoint in file.Waypoints) {
                            children.Add(waypoint);
                        }
                        foreach (GpxRouteModel route in file.Routes) {
                            children.Add(route);
                        }
                        return children;
                    }
                    return file.Tracks;
                }
                if (x is GpxTrackModel trk) {
                    return trk.Segments;
                }
                if (x is GpxTrackSegmentModel seg) {
                    return seg.Trackpoints;
                }
                if (x is GpxRouteModel rte) {
                    return rte.Waypoints;
                }
                // Empty list
                return new List<GpxModel>();
            };

            treeListView.CheckedAspectName = "Checked";
            // CheckedAspectName doesn't work with HierarchicalCheckboxes
            treeListView.HierarchicalCheckboxes = false;

#if true
            // Make ImageList
            ImageList = new ImageList();
            ImageList.ImageSize = ImageSize;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream(
                "GPX_Viewer.icons.route.png");
            if (imageStream != null) {
                ImageList.Images.Add("route", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPX_Viewer.icons.track.png");
            if (imageStream != null) {
                ImageList.Images.Add("track", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPX_Viewer.icons.trackSegment.png");
            if (imageStream != null) {
                ImageList.Images.Add("trackSegment", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPX_Viewer.icons.waypoint.png");
            if (imageStream != null) {
                ImageList.Images.Add("waypoint", Image.FromStream(imageStream));
            }
            treeListView.SmallImageList = ImageList;
#endif
            // Image getter
            this.col1.ImageGetter = delegate (object x) {
                if (x is GpxTrackModel) {
                    return "track";
                }
                if (x is GpxRouteModel) {
                    return "route";
                }
                if (x is GpxWaypointModel) {
                    return "waypoint";
                }
                if (x is GpxTrackSegmentModel) {
                    return "trackSegment";
                }
                return null;
            };
            // Set the roots to be Files
            treeListView.Roots = Files;
        }

        private void resetTree() {
            treeListView.Roots = Files;
            treeListView.Refresh();
        }

        private List<object> getAllTreeObjects() {
            List<object> objects = new List<object>();
            foreach (object x in treeListView.Objects) {
                objects.Add(x);
                objects.Add(getTreeChildren(x));
            }
            return objects;
        }

        private List<object> getTreeChildren(object x) {
            List<object> objects = new List<object>();
            foreach (object child in treeListView.GetChildren(x)) {
                objects.Add(child);
                objects.Add(getTreeChildren(child));
            }
            return objects;
        }

        private void expandToLevel(int level) {
            treeListView.CollapseAll();
            if (level == 0) return;
            foreach (object x in treeListView.Roots) {
                treeListView.Expand(x);
                if (level > 1) {
                    foreach (object child1 in treeListView.GetChildren(x)) {
                        treeListView.Expand(child1);
                        if (level > 2) {
                            foreach (object child2 in treeListView.GetChildren(child1)) {
                            }
                        }
                    }
                }
            }
            treeListView.TopItemIndex = 0;
        }

        /// <summary>
        /// Method to do an action for the given GpxModel with recursion over sub-models.
        /// </summary>
        /// <param name="model">The model to start with.</param>
        /// <param name="obj">A parameter for the action</param>
        /// <param name="action">The Action<GpxModel, object>.</param>
        public void recurseModels(GpxModel model, object obj, Action<GpxModel, object> action) {
            if (model is GpxFileSetModel fileSetModel) {
                // Don't do action here
                foreach (GpxFileModel fileModel in fileSetModel.Files) {
                    action(model, obj);
                    recurseModels(fileModel, obj, action);
                }
            } else if (model is GpxFileModel fileModel) {
                action(model, obj);
                foreach (GpxTrackModel trkModel in fileModel.Tracks) {
                    action(model, obj);
                    recurseModels(trkModel, obj, action);
                }
                foreach (GpxRouteModel rteModel in fileModel.Routes) {
                    action(model, obj);
                    recurseModels(rteModel, obj, action);
                }
                foreach (GpxWaypointModel wptModel in fileModel.Waypoints) {
                    action(model, obj);
                    recurseModels(wptModel, obj, action);
                }
            } else if (model is GpxTrackModel trkModel) {
                action(model, obj);
                foreach (GpxTrackSegmentModel segModel in trkModel.Segments) {
                    recurseModels(segModel, obj, action);
                }
            } else if (model is GpxTrackSegmentModel segModel) {
                action(model, obj);
                foreach (GpxTrackpointModel tkptModel in segModel.Trackpoints) {
                    recurseModels(tkptModel, obj, action);
                }
            } else if (model is GpxRouteModel rteModel) {
                action(model, obj);
                foreach (GpxWaypointModel wptModel in rteModel.Waypoints) {
                    recurseModels(wptModel, obj, action);
                }
            } else if (model is GpxTrackpointModel) {
                action(model, obj);
            } else if (model is GpxWaypointModel) {
                action(model, obj);
            }
        }

        public void showStatus() {
            IEnumerable roots = treeListView.Roots;
            if (roots == null) {
                Utils.errMsg("TreeListView has no Roots");
                return;
            }
            string msg = "TreeListView" + NL;
            msg += "    Number of files=" + Files.Count + NL;

#if false
                msg += "    " + roots.GetType() + NL
                + "    Roots Hash=" + roots.GetHashCode() + NL
                + "    Files Hash=" + Files.GetHashCode() + NL
                + "    " + roots.GetType() + NL
#endif
            // Tree statistics
            int nObjs = 0, nFileSet = 0, nFile = 0, nTrk = 0, nSeg = 0;
            int nTkpt = 0, nWpt = 0, nRte = 0;
            foreach (object x in getAllTreeObjects()) {
                nObjs++;
                if (x is GpxFileSetModel fileSet) {
                    nFileSet++;
                }
                if (x is GpxFileModel file) {
                    nFile++;
                }
                if (x is GpxTrackModel track) {
                    nTrk++;
                }
                if (x is GpxTrackSegmentModel segment) {
                    nSeg++;
                }
                if (x is GpxTrackpointModel) {
                    nTkpt++;
                }
                if (x is GpxWaypointModel) {
                    nWpt++;
                }
                if (x is GpxRouteModel) {
                    nRte++;
                }
            }
            msg += "TreeNode Statistics" + NL
                + "    nObjects=" + nObjs + " nFileSets=" + nFileSet
                + " nFiles=" + nFile + NL
                + "    nTracks=" + nTrk + " nSegments=" + nSeg
                + " nTrackPoints=" + nTkpt + NL
                + "    nWayPoints=" + nWpt + NL
                + "    nRoutes=" + nRte + NL;

            // Model statistics
            nObjs = nFileSet = nFile = nTrk = nSeg = 0;
            nTkpt = nWpt = nRte = 0;
            foreach (GpxModel x in getAllModels(FileSet)) {
                nObjs++;
                if (x is GpxFileSetModel fileSet) {
                    nFileSet++;
                }
                if (x is GpxFileModel file) {
                    nFile++;
                }
                if (x is GpxTrackModel track) {
                    nTrk++;
                }
                if (x is GpxTrackSegmentModel segment) {
                    nSeg++;
                }
                if (x is GpxTrackpointModel) {
                    nTkpt++;
                }
                if (x is GpxWaypointModel) {
                    nWpt++;
                }
                if (x is GpxRouteModel) {
                    nRte++;
                }
            }
            msg += "TreeNode Statistics" + NL
                + "    nObjects=" + nObjs + " nFileSets=" + nFileSet
                + " nFiles=" + nFile + NL
                + "    nTracks=" + nTrk + " nSegments=" + nSeg
                + " nTrackPoints=" + nTkpt + NL
                + "    nWayPoints=" + nWpt + NL
                + "    nRoutes=" + nRte + NL;


            // Expanded
            msg += "Expanded" + NL;
            int nExpanded = 0;
            foreach (object obj in treeListView.ExpandedObjects) {
                nExpanded++;
            }
            msg += " Model: nExpanded=" + nExpanded + NL;

            // Checkboxes
            msg += "Checkboxes" + NL;
            int nChecked = 0;
            foreach (object obj in treeListView.CheckedObjects) {
                nChecked++;
            }
            msg += " TreeListView: nChecked=" + nChecked + NL;
            msg += " Model: nChecked=" + calculateNumberCheckedInModel(FileSet) + NL;

#if false // All image stuff
#if true
            // Images
            msg += "Embedded Images" + NL;
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            string[] names = myAssembly.GetManifestResourceNames();
            foreach (string name in names) {
                msg += "    " + name + NL;
            }
            // ImageList
            msg += "ImageList Keys" + NL;
            if (ImageList != null && ImageList.Images != null) {
                ImageList.ImageCollection images = ImageList.Images;
                foreach (string key in ImageList.Images.Keys) {
                    msg += "    " + key + " (" + images.IndexOfKey(key) + ")" + NL;
                }
            } else {
                msg += "    " + "No Image List" + NL;
            }
#endif
            // TableListView Images
            msg += "TableListView Images" + NL;
            if (treeListView.SmallImageList != null && treeListView.SmallImageList.Images != null) {
                ImageList.ImageCollection images = treeListView.SmallImageList.Images;
                foreach (string key in treeListView.SmallImageList.Images.Keys) {
                    msg += "    " + key + " (" + images.IndexOfKey(key) + ")" + NL;
                }
            } else {
                msg += "    " + "No Image List" + NL;
            }
#endif // All image stuff
            Utils.infoMsg(msg);
        }

        public Action<GpxModel, object> checkAll = (model, obj) => {
            model.Checked = (bool)obj;
        };

        public int calculateNumberCheckedInModel(GpxModel model) {
            int nChecked = 0;
            if (model is GpxFileSetModel fileSetModel) {
                foreach (GpxFileModel fileModel in fileSetModel.Files) {
                    nChecked += calculateNumberCheckedInModel(fileModel);
                }
            } else if (model is GpxFileModel fileModel) {
                if (model.Checked) nChecked++;
                foreach (GpxTrackModel trkModel in fileModel.Tracks) {
                    nChecked += calculateNumberCheckedInModel(trkModel);
                }
                foreach (GpxRouteModel rteModel in fileModel.Routes) {
                    nChecked += calculateNumberCheckedInModel(rteModel);
                }
                foreach (GpxWaypointModel wptModel in fileModel.Waypoints) {
                    nChecked += calculateNumberCheckedInModel(wptModel);
                }
            } else if (model is GpxTrackModel trkModel) {
                if (model.Checked) nChecked++;
                foreach (GpxTrackSegmentModel segModel in trkModel.Segments) {
                    nChecked += calculateNumberCheckedInModel(segModel);
                }
            } else if (model is GpxTrackSegmentModel segModel) {
                if (model.Checked) nChecked++;
                foreach (GpxTrackpointModel tkptModel in segModel.Trackpoints) {
                    nChecked += calculateNumberCheckedInModel(tkptModel);
                }
            } else if (model is GpxRouteModel rteModel) {
                if (model.Checked) nChecked++;
                foreach (GpxWaypointModel wptModel in rteModel.Waypoints) {
                    nChecked += calculateNumberCheckedInModel(wptModel);
                }
            } else if (model is GpxTrackpointModel) {
                nChecked++;
            } else if (model is GpxWaypointModel) {
                nChecked++;
            }
            return nChecked;
        }

        public List<GpxModel> getAllModels(GpxModel model) {
            List<GpxModel> models = new List<GpxModel>();
            if (model is GpxFileSetModel fileSetModel) {
                models.Add(fileSetModel);
                foreach (GpxFileModel fileModel in fileSetModel.Files) {
                    models.AddRange(getAllModels(fileModel));
                }
            } else if (model is GpxFileModel fileModel) {
                models.Add(fileModel);
                foreach (GpxTrackModel trkModel in fileModel.Tracks) {
                    models.AddRange(getAllModels(trkModel));
                }
                foreach (GpxRouteModel rteModel in fileModel.Routes) {
                    models.AddRange(getAllModels(rteModel));
                }
                foreach (GpxWaypointModel wptModel in fileModel.Waypoints) {
                    models.AddRange(getAllModels(wptModel));
                }
            } else if (model is GpxTrackModel trkModel) {
                models.Add(trkModel);
                foreach (GpxTrackSegmentModel segModel in trkModel.Segments) {
                    models.AddRange(getAllModels(segModel));
                }
            } else if (model is GpxTrackSegmentModel segModel) {
                models.Add(segModel);
                foreach (GpxTrackpointModel tkptModel in segModel.Trackpoints) {
                    models.AddRange(getAllModels(tkptModel));
                }
            } else if (model is GpxRouteModel rteModel) {
                models.Add(rteModel);
                foreach (GpxWaypointModel wptModel in rteModel.Waypoints) {
                    models.AddRange(getAllModels(wptModel));
                }
            } else if (model is GpxTrackpointModel tkptModel) {
                models.Add(tkptModel);
            } else if (model is GpxWaypointModel wptModel) {
                models.Add(wptModel);
            }
            return models;
        }

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

        private void OnFileExitClick(object sender, EventArgs e) {
            Close();
        }

        private void OnAboutClick(object sender, EventArgs e) {
            AboutBox dlg = new AboutBox();
            dlg.ShowDialog();
        }

        private void OnFileOpenGpxClick(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX|*.gpx|TCX|*.tcx|GPX and TCX|*.gpx;*.tcx";
            dlg.Title = "Select files to process";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    Files.Add(new GpxFileModel(FileSet, fileName));
                }
                resetTree();
            }
        }

        private void OnHelpStatusClick(object sender, EventArgs e) {
            showStatus();
        }

        private void OnResetModelFromTree(object sender, EventArgs e) {
            Utils.infoMsg("Not implemented yet");
        }

        private void OnResetTreeFromModel(object sender, EventArgs e) {
            resetTree();
        }

        private void OnToolsCheckAllFilesClick(object sender, EventArgs e) {
            foreach (GpxFileModel model in FileSet.Files) {
                model.Checked = true;
            }
            resetTree();
        }

        private void OnToolsCheckNoFilesClick(object sender, EventArgs e) {
            foreach (GpxFileModel model in FileSet.Files) {
                model.Checked = false;
            }
            resetTree();
        }

        private void OnToolsCheckAllClick(object sender, EventArgs e) {
            recurseModels(FileSet, true, checkAll);
        }

        private void OnToolsCheckNoneClick(object sender, EventArgs e) {
            recurseModels(FileSet, false, checkAll);
        }

        private void OnViewExpandAllClick(object sender, EventArgs e) {
            treeListView.ExpandAll();

        }
        private void OnViewExpandNoneClick(object sender, EventArgs e) {
            treeListView.CollapseAll();
        }

        private void OnViewExpandToLevelClick(object sender, EventArgs e) {
            if (sender.ToString().Equals("0")) expandToLevel(0);
            else if (sender.ToString().Equals("1")) expandToLevel(1);
            else if (sender.ToString().Equals("2")) expandToLevel(2);
            else if (sender.ToString().Equals("3")) expandToLevel(3);
        }

        private void OnFileSendToGoogleEarth(object sender, EventArgs e) {
            try {
                KmlOptions options = new KmlOptions();
                KmlUtils.createKml(FileSet, options);
            } catch(Exception ex) {
                Utils.excMsg("Failed to send checked files to Google Earth", ex);
            }
        }

        private void OnToolsShowLogClick(object sender, EventArgs e) {
            if(textDlg != null) {
                textDlg.Visible = true;
            }
        }
    }
}
