using GPXViewer.Dialogs;
using GPXViewer.KML;
using GPXViewer.model;
using KEGpsUtils;
using KEUtils.About;
using KEUtils.ScrolledHTML;
using KEUtils.ScrolledText;
using KEUtils.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using www.topografix.com.GPX_1_1;
using static GPXViewer.model.GpxModel;

namespace GPXViewer {
    public partial class MainForm : Form {
#if false
        private static readonly string DEBUG_FILE_NAME = @"C:\Users\evans\Documents\GPSLink\Test\AAAtest9.gpx";
#endif
        public enum Task {
            SORT, REVERSE, NEWFILE, NEWTRK, NEWSEG, NEWTKPT, NEWRTE, NEWWPT
        };
        public static readonly int INITIAL_TREE_LEVEL = 0;
        private int treeLevel = INITIAL_TREE_LEVEL;

        private static ScrolledHTMLDialog overviewDlg;
        private static ScrolledTextDialog logDialog;
        private GpxTcxMenu gpxTcxMenu;

        public static readonly String NL = Environment.NewLine;
        private static readonly string SAVE_FILE_TAG = ".gpxv";
        public GpxFileSetModel FileSet { get; set; }
        public List<GpxFileModel> Files { get; set; }
        public List<GpxModel> ClipboardList { get; set; }
        public ImageList ToolsImageList { get; set; }
        public Size ToolsImageSize { get; set; }
        public ImageList TreeImageList { get; set; }
        public Size TreeImageSize { get; set; }
        public PointF DPI { get; set; }
        public KmlOptions kmlOptions { get; set; }

        public MainForm() {
            InitializeComponent();

            // Set KmlOptions from Settings
            try {
                kmlOptions = KmlOptionsDialog.getOptionsFromSettings();
            } catch (Exception ex) {
                Utils.excMsg("Error getting KmlOptions from Settings", ex);
            }

            // Add GpxTcxMenu at position 3
            gpxTcxMenu = new GpxTcxMenu();
            gpxTcxMenu.GpxTcxEvent += onGpxTcxEvent;
            gpxTcxMenu.createMenu(menuStrip1, 3);

            FileSet = new GpxFileSetModel(null);
            Files = FileSet.Files;
            ClipboardList = new List<GpxModel>();
#if false
            // Load a file for testing
            try {
                Files.Add(new GpxFileModel(FileSet, DEBUG_FILE_NAME));
            } catch (Exception ex) {
                Utils.excMsg("Failed to load debug file", ex);
            }
#endif
            // Load startup files
            try {
                string json = Properties.Settings.Default.StartupFiles;
                if (!String.IsNullOrEmpty(json)) {
                    List<string> fileNames = JsonConvert.DeserializeObject<List<string>>(json);
                    foreach (string fileName in fileNames) {
                        if (File.Exists(fileName)) {
                            Files.Add(new GpxFileModel(FileSet, fileName));
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.excMsg("Error setting startup files", ex);
            }
        }

        private void onGpxTcxEvent(object sender, GpxTcxEventArgs e) {
            string msg = e.Message;
            switch (e.Type) {
                case GpxTcxMenu.EventType.MSG:
                    if (logDialog == null) {
                        logDialog = new ScrolledTextDialog(
                        Utils.getDpiAdjustedSize(this, new Size(600, 400)),
                        "GPX Viewer Log");
                        logDialog.appendTextAndNL("GpxViewer Log");
                    }
                    logDialog.ButtonCancel.Visible = false;
                    logDialog.appendTextAndNL(msg);
                    logDialog.Visible = true;
                    break;
                case GpxTcxMenu.EventType.INFO:
                    Utils.infoMsg(msg);
                    break;
                case GpxTcxMenu.EventType.WARN:
                    Utils.warnMsg(msg);
                    break;
                case GpxTcxMenu.EventType.ERR:
                    Utils.errMsg(msg);
                    break;
                case GpxTcxMenu.EventType.EXC:
                    Utils.excMsg(msg, e.Exception);
                    break;
                default:
                    Utils.errMsg("Received unknown event type (" + e.Type
                        + ") from GpsTcxMenu");
                    break;
            }
        }

        private void OnFormLoad(object sender, EventArgs e) {
            // DPI
            float dpiX, dpiY;
            using (Graphics g = this.CreateGraphics()) {
                dpiX = g.DpiX;
                dpiY = g.DpiY;
            }
            DPI = new PointF(dpiX, dpiY);
            TreeImageSize = new Size((int)(16 * dpiX / 96), (int)(16 * dpiY / 96));
            ToolsImageSize = new Size((int)(16 * dpiX / 96), (int)(16 * dpiY / 96));

            // ContextMenu
            treeListView.ContextMenuStrip = contextMenuStrip1;

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
            // Make ToolsImageList
            ToolsImageList = new ImageList();
            ToolsImageList.ImageSize = ToolsImageSize;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.remove.png");
            if (imageStream != null) {
                ToolsImageList.Images.Add("remove", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.removeAll.png");
            if (imageStream != null) {
                ToolsImageList.Images.Add("removeAll", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.expand.png");
            if (imageStream != null) {
                ToolsImageList.Images.Add("expand", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.collapse.png");
            if (imageStream != null) {
                ToolsImageList.Images.Add("collapse", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.kml.png");
            if (imageStream != null) {
                ToolsImageList.Images.Add("kml", Image.FromStream(imageStream));
            }
            toolStrip1.ImageList = ToolsImageList;
            toolStripButtonRemoveSelected.ImageKey = "remove";
            toolStripButtonRemoveAll.ImageKey = "removeAll";
            toolStripButtonExpand.ImageKey = "expand";
            toolStripButtonCollapse.ImageKey = "collapse";
            toolStripButtonSendToGoogleEarth.ImageKey = "kml";
#endif
#if true
            // Make TreeImageList
            TreeImageList = new ImageList();
            TreeImageList.ImageSize = TreeImageSize;
            assembly = Assembly.GetExecutingAssembly();
            imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.route.png");
            if (imageStream != null) {
                TreeImageList.Images.Add("route", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.track.png");
            if (imageStream != null) {
                TreeImageList.Images.Add("track", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.trackSegment.png");
            if (imageStream != null) {
                TreeImageList.Images.Add("trackSegment", Image.FromStream(imageStream));
            }
            imageStream = assembly.GetManifestResourceStream(
                "GPXViewer.icons.waypoint.png");
            if (imageStream != null) {
                TreeImageList.Images.Add("waypoint", Image.FromStream(imageStream));
            }
            treeListView.SmallImageList = TreeImageList;
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
                                treeListView.Expand(child2);
                                if (level > 3) {
                                    foreach (object child3 in treeListView.GetChildren(child1)) {
                                        treeListView.Expand(child3);
                                    }
                                }
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
            msg += "Model Statistics" + NL
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

#if true // All image stuff
#if true
            // Images
            msg += "Embedded Images" + NL;
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            string[] names = myAssembly.GetManifestResourceNames();
            foreach (string name in names) {
                msg += "    " + name + NL;
            }
            // Tools ImageList
            msg += "Tools ImageList Keys" + NL;
            if (ToolsImageList != null && ToolsImageList.Images != null) {
                ImageList.ImageCollection images = ToolsImageList.Images;
                foreach (string key in ToolsImageList.Images.Keys) {
                    msg += "    " + key + " (" + images.IndexOfKey(key) + ")" + NL;
                }
            } else {
                msg += "    " + "No Image List" + NL;
            }
            msg += "    ImageScalingSize=" + toolStrip1.ImageScalingSize + NL;
            // Tree ImageList
            msg += "Tree ImageList Keys" + NL;
            if (TreeImageList != null && TreeImageList.Images != null) {
                ImageList.ImageCollection images = TreeImageList.Images;
                foreach (string key in TreeImageList.Images.Keys) {
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

        private void doTask(Task task, GpxModel model) {
            if (model == null) return;
            GpxModel parent = model.Parent;
            switch (task) {
                case Task.NEWFILE:
                    OnFileOpenGpxClick(null, null);
                    break;
                case Task.NEWTRK:
                    if (model is GpxFileModel fileModel) {
                        GpxTrackModel newModel = new GpxTrackModel(model, null);
                        fileModel.add(null, newModel, PasteMode.END);
                    } else if (model is GpxTrackModel) {
                        GpxTrackModel newModel = new GpxTrackModel(parent, null);
                        parent.add(model, newModel, PasteMode.END);
                    }
                    break;
                case Task.NEWSEG:
                    if (model is GpxTrackModel trackModel) {
                        GpxTrackSegmentModel newModel = new GpxTrackSegmentModel(model, null);
                        trackModel.add(null, newModel, PasteMode.END);
                    } else if (model is GpxTrackSegmentModel) {
                        GpxTrackSegmentModel newModel = new GpxTrackSegmentModel(parent, null);
                        parent.add(model, newModel, PasteMode.END);
                    }
                    break;
                case Task.NEWTKPT:
                    if (model is GpxTrackSegmentModel trackSegmentModel) {
                        GpxTrackpointModel newModel = new GpxTrackpointModel(model, null);
                        trackSegmentModel.add(null, newModel, PasteMode.END);
                    } else if (model is GpxTrackpointModel) {
                        GpxTrackpointModel newModel = new GpxTrackpointModel(parent, null);
                        parent.add(model, newModel, PasteMode.END);
                    }
                    break;
                case Task.NEWRTE:
                    if (model is GpxFileModel fileModel1) {
                        GpxRouteModel newModel = new GpxRouteModel(model, null);
                        fileModel1.add(null, newModel, PasteMode.END);
                    } else if (model is GpxRouteModel) {
                        GpxRouteModel newModel = new GpxRouteModel(parent, null);
                        parent.add(model, newModel, PasteMode.END);
                    }
                    break;
                case Task.NEWWPT:
                    if (model is GpxRouteModel routeModel) {
                        GpxWaypointModel newModel = new GpxWaypointModel(model, null);
                        routeModel.add(null, newModel, PasteMode.END);
                    } else if (model is GpxWaypointModel) {
                        GpxWaypointModel newModel = new GpxWaypointModel(parent, null);
                        parent.add(model, newModel, PasteMode.END);
                    }
                    break;
            }
            synchronize();
            resetTree();
        }

        private void synchronize() {
            foreach (GpxFileModel fileModel in Files) {
                // Should be recursive
                fileModel.synchronize();
            }
        }

        private void cut() {
            if (treeListView.SelectedObjects.Count == 0) {
                Utils.errMsg("No selected items");
                return;
            }
            ClipboardList.Clear();
            synchronize();
            List<GpxModel> removeList = new List<GpxModel>();
            foreach (object x in treeListView.SelectedObjects) {
                if (x is GpxModel model) {
                    ClipboardList.Add(model.clone());
                    removeList.Add(model);
                }
                foreach (GpxModel model1 in removeList) {
                    model1.delete();
                }
            }
            synchronize();
            resetTree();
        }

        private void copy() {
            if (treeListView.SelectedObjects.Count == 0) {
                Utils.errMsg("No selected items");
                return;
            }
            ClipboardList.Clear();
            synchronize();
            foreach (object x in treeListView.SelectedObjects) {
                if (x is GpxModel model) {
                    ClipboardList.Add(model.clone());
                }
            }
        }

        private void paste(PasteMode mode) {
            if (ClipboardList.Count == 0) {
                Utils.errMsg("No items to paste");
                return;
            }
            GpxModel targetModel = (GpxModel)treeListView.SelectedObject;
            if (targetModel == null) {
                Utils.errMsg("Selected paste location must be one single item");
                return;
            }
            GpxModel targetParent = targetModel.Parent;
            bool added;
            int iClipboardItem = 0;
            foreach (GpxModel clipboardModel in ClipboardList) {
                iClipboardItem++;
                added = false;
                if (targetModel == null) {
                    if (clipboardModel is GpxFileModel) {
                        added = FileSet.add((GpxFileModel)targetModel, clipboardModel.clone(),
                                mode);
                    }
                } else if (targetModel is GpxFileModel) {
                    if (clipboardModel is GpxFileModel) {
                        added = ((GpxFileSetModel)targetParent).add((GpxFileModel)targetModel,
                                clipboardModel.clone(), mode);
                    } else if (mode == PasteMode.BEGINNING || mode == PasteMode.END) {
                        if (clipboardModel is GpxTrackModel) {
                            GpxFileModel fileModel = (GpxFileModel)targetModel;
                            added = fileModel.add(null, (GpxTrackModel)clipboardModel.clone(), mode);
                        } else if (clipboardModel is GpxRouteModel) {
                            GpxFileModel fileModel = (GpxFileModel)targetModel;
                            added = fileModel.add(null, (GpxRouteModel)clipboardModel.clone(), mode);
                        } else if (clipboardModel is GpxWaypointModel) {
                            GpxFileModel fileModel = (GpxFileModel)targetModel;
                            added = fileModel.add(null, (GpxWaypointModel)clipboardModel.clone(), mode);
                        }
                    }
                } else if (targetModel is GpxTrackModel) {
                    if (clipboardModel is GpxTrackModel) {
                        added = ((GpxFileModel)targetParent).add((GpxTrackModel)targetModel,
                                (GpxTrackModel)clipboardModel.clone(), mode);
                    } else if (mode == PasteMode.BEGINNING || mode == PasteMode.END) {
                        if (clipboardModel is GpxTrackSegmentModel) {
                            GpxTrackModel trackModel = (GpxTrackModel)targetModel;
                            added = trackModel.add(null, (GpxTrackSegmentModel)clipboardModel.clone(), mode);
                        }
                    }
                } else if (targetModel is GpxTrackSegmentModel) {
                    if (clipboardModel is GpxTrackSegmentModel) {
                        added = ((GpxTrackModel)targetParent).add((GpxTrackSegmentModel)targetModel,
                                (GpxTrackSegmentModel)clipboardModel.clone(), mode);
                    } else if (mode == PasteMode.BEGINNING || mode == PasteMode.END) {
                        if (clipboardModel is GpxTrackpointModel) {
                            GpxTrackSegmentModel trackSegmentModel = (GpxTrackSegmentModel)targetModel;
                            added = trackSegmentModel.add(null, (GpxTrackpointModel)clipboardModel.clone(), mode);
                        }
                    }
                } else if (targetModel is GpxRouteModel) {
                    if (clipboardModel is GpxRouteModel) {
                        added = ((GpxFileModel)targetParent).add((GpxRouteModel)targetModel,
                                (GpxRouteModel)clipboardModel.clone(), mode);
                    } else if (mode == PasteMode.BEGINNING || mode == PasteMode.END) {
                        if (clipboardModel is GpxWaypointModel) {
                            GpxRouteModel routeModel = (GpxRouteModel)targetModel;
                            added = routeModel.add(null, (GpxWaypointModel)clipboardModel.clone(), mode);
                        }
                    }
                } else if (targetModel is GpxTrackpointModel) {
                    if (clipboardModel is GpxTrackpointModel) {
                        if (targetParent is GpxFileModel) {
                            added = ((GpxFileModel)targetParent).add((GpxTrackpointModel)targetModel,
                                    (GpxTrackpointModel)clipboardModel.clone(), mode);
                        } else if (targetParent is GpxTrackSegmentModel) {
                            added = ((GpxTrackSegmentModel)targetParent).add((GpxTrackpointModel)targetModel,
                                    (GpxTrackpointModel)clipboardModel.clone(), mode);
                        }
                    }
                } else if (targetModel is GpxWaypointModel) {
                    if (clipboardModel is GpxWaypointModel) {
                        if (targetParent is GpxFileModel) {
                            added = ((GpxFileModel)targetParent).add((GpxWaypointModel)targetModel,
                                    (GpxWaypointModel)clipboardModel.clone(), mode);
                        } else if (targetParent is GpxRouteModel) {
                            added = ((GpxRouteModel)targetParent).add((GpxWaypointModel)targetModel,
                                    (GpxWaypointModel)clipboardModel.clone(), mode);
                        }
                    }
                }
                if (!added) {
                    String from = clipboardModel.GetType().ToString();
                    int index = from.LastIndexOf("Gpx");
                    if (index != -1) {
                        from = from.Substring(index);
                    }
                    String to = targetModel.GetType().ToString();
                    index = to.LastIndexOf("Gpx");
                    if (index != -1) {
                        to = to.Substring(index);
                    }
                    string msg = "Failed to add " + from + " to " + to
                        + " for add at " + mode + "." + NL;
                    if (mode == PasteMode.BEFORE || (mode == PasteMode.AFTER)) {
                        msg += "(Note that pasting an obect into the parent of" + NL
                            + "the selected object is not supported for" +
                            " AFTER and BEFORE.)" + NL;
                    }
                    msg += "Press OK to continue with remaining "
                        + "Clipboard items or Cancel to abort.";
                    if (iClipboardItem < ClipboardList.Count) {
                        DialogResult res = MessageBox.Show(msg + "\n" + "OK to continue?",
                            "Warning", MessageBoxButtons.YesNo);
                        if (res == DialogResult.No) {
                            break;
                        }
                    } else {
                        Utils.errMsg(msg);
                    }
                }
            }
            synchronize();
            resetTree();
        }

        private void edit() {
            object x = treeListView.SelectedObject;
            if (x == null) {
                Utils.errMsg("Must be only one item selected to use this option");
                return;
            }
            //if (x is GpxModel model) {
            //    //Utils.infoMsg("Label=" + model.Label);
            //    Utils.infoMsg(model.info());
            //}
            bool error = false;
            try {
                if (x is GpxFileModel fileModel) {
                    // GpxFileModel
                    fileModel.Parent.synchronize();
                    XElement elem = fileModel.Gpx.Untyped;
                    XElement newElem = getEditedXElement(elem);
                    if (newElem == null) {
                        error = true;
                    } else {
                        gpx newGpx = (gpx)newElem;
                        if (fileModel.Parent is GpxFileSetModel fileSetModel) {
                            int index = fileSetModel.Files.IndexOf(fileModel);
                            fileSetModel.Files[index] = new GpxFileModel
                                (fileModel.Parent, fileModel.FileName, newGpx);
                            fileSetModel.synchronize();
                        }
                    }
                } else if (x is GpxTrackModel trkModel) {
                    // GpxTrackModel
                    trkModel.Parent.synchronize();
                    XElement elem = trkModel.Track.Untyped;
                    XElement newElem = getEditedXElement(elem);
                    if (newElem == null) {
                        error = true;
                    } else {
                        trkType newTrack = (trkType)newElem;
                        if (trkModel.Parent is GpxFileModel fileModel1) {
                            int index = fileModel1.Tracks.IndexOf(trkModel);
                            fileModel1.Tracks[index] = new GpxTrackModel
                                (trkModel.Parent, newTrack);
                            fileModel1.synchronize();
                        }
                    }
                } else if (x is GpxTrackSegmentModel segModel) {
                    // GpxTrackSegmentModel
                    segModel.Parent.synchronize();
                    XElement elem = segModel.Segment.Untyped;
                    XElement newElem = getEditedXElement(elem);
                    if (newElem == null) {
                        error = true;
                    } else {
                        trksegType newSegment = (trksegType)newElem;
                        if (segModel.Parent is GpxTrackModel trkModel1) {
                            int index = trkModel1.Segments.IndexOf(segModel);
                            trkModel1.Segments[index] = new GpxTrackSegmentModel
                                (segModel.Parent, newSegment);
                            trkModel1.synchronize();
                        }
                    }
                } else if (x is GpxTrackpointModel trkptModel) {
                    // GpxTrackpointModel
                    trkptModel.Parent.synchronize();
                    XElement elem = trkptModel.Trackpoint.Untyped;
                    XElement newElem = getEditedXElement(elem);
                    if (newElem == null) {
                        error = true;
                    } else {
                        wptType newTrackpoint = (wptType)newElem;
                        if (trkptModel.Parent is GpxTrackSegmentModel segModel1) {
                            int index = segModel1.Trackpoints.IndexOf(trkptModel);
                            segModel1.Trackpoints[index] = new GpxTrackpointModel
                                (trkptModel.Parent, newTrackpoint);
                            segModel1.synchronize();
                        }
                    }
                } else if (x is GpxWaypointModel wptModel) {
                    // GpxWaypointModel
                    wptModel.Parent.synchronize();
                    XElement elem = wptModel.Waypoint.Untyped;
                    XElement newElem = getEditedXElement(elem);
                    if (newElem == null) {
                        error = true;
                    } else {
                        wptType newWaypoint = (wptType)newElem;
                        if (wptModel.Parent is GpxFileModel fileModel1) {
                            int index = fileModel1.Waypoints.IndexOf(wptModel);
                            fileModel1.Waypoints[index] = new GpxWaypointModel
                                (wptModel.Parent, newWaypoint);
                            fileModel1.synchronize();
                        } else if (wptModel.Parent is GpxRouteModel rteModel1) {
                            int index = rteModel1.Waypoints.IndexOf(wptModel);
                            rteModel1.Waypoints[index] = new GpxWaypointModel
                                (wptModel.Parent, newWaypoint);
                            rteModel1.synchronize();
                        }
                    }
                } else if (x is GpxRouteModel rteModel) {
                    // GpxRouteModel
                    rteModel.Parent.synchronize();
                    XElement elem = rteModel.Route.Untyped;
                    XElement newElem = getEditedXElement(elem);
                    if (newElem == null) {
                        error = true;
                    } else {
                        rteType newRoute = (rteType)newElem;
                        if (rteModel.Parent is GpxFileModel fileModel1) {
                            int index = fileModel1.Routes.IndexOf(rteModel);
                            fileModel1.Routes[index] = new GpxRouteModel
                                (rteModel.Parent, newRoute);
                            fileModel1.synchronize();
                        }
                    }
                } else {
                    // Unknown type, should not happen
                    Utils.errMsg("Unexpected type found for edit: "
                        + x.GetType().Name);
                    return;
                }
            } catch (Exception ex) {
                Utils.excMsg(x.GetType().Name + ": Error resetting tree", ex);
            }
            if (error) {
                Utils.errMsg(x.GetType().Name + ": XML editing canceled or invalid edited XML");
            }
            resetTree();
        }

        /// <summary>
        /// Brings up a ScrolledTextDialog to edit the XML from the given 
        /// element and returns the element obtained from parsing the new XML
        /// from the dialog.
        /// </summary>
        /// <param name="elem"></param>
        /// <returns>The new XElement or null on failure.</returns>
        private XElement getEditedXElement(XElement elem) {
            try {
                ScrolledTextDialog dlg = new ScrolledTextDialog(
                    Utils.getDpiAdjustedSize(this, new Size(600, 400)),
                    "Edit");
                dlg.setText(elem.ToString());
                DialogResult res = dlg.ShowDialog();
                if (res != DialogResult.OK) return null;
                return XElement.Parse(dlg.getText());
            } catch (Exception) {
                return null;
            }
        }

        private void addFilesFromFileSet(string fileName) {
            List<string> failedList = new List<string>();
            try {
                foreach (string newFileName in File.ReadAllLines(fileName)) {
                    if (File.Exists(newFileName)) {
                        try {
                            Files.Add(new GpxFileModel(FileSet, newFileName));
                        } catch (Exception) {
                            failedList.Add(newFileName);
                        }
                    } else {
                        failedList.Add("Does not exist: " + newFileName);
                    }
                }
            } catch (Exception ex) {
                Utils.excMsg("Failed to read " + fileName, ex);
            }
            if (failedList.Count > 0) {
                string msg = "Failed to read:" + NL;
                foreach (string newFileName in failedList) {
                    msg += newFileName + NL;
                }
                Utils.errMsg(msg);
            }
        }

        private void saveFilesToFileSet(string setFileName) {
            List<string> filesToSave = new List<string>();
            foreach (GpxFileModel model in FileSet.Files) {
                if (model.Checked) filesToSave.Add(model.FileName);
            }
            if (filesToSave.Count == 0) {
                Utils.warnMsg("There are no checked files to save in a file set");
                return;
            }
            try {
                using (StreamWriter outputFile = File.CreateText(setFileName)) {
                    foreach (string fileName1 in filesToSave) {
                        outputFile.WriteLine(fileName1);
                    }
                    outputFile.Close();
                }
            } catch (Exception ex) {
                Utils.excMsg("Error writing" + setFileName, ex);
                return;
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            Properties.FindNear.Default.Save();
        }

        private void OnFileExitClick(object sender, EventArgs e) {
            Close();
        }

        private void OnAboutClick(object sender, EventArgs e) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image image = null;
            try {
                image = Image.FromFile(@".\Help\GPXViewer256.png");
            } catch (Exception ex) {
                Utils.excMsg("Failed to get AboutBox image", ex);
            }
            AboutBox dlg = new AboutBox(image, assembly);
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
                Cursor.Current = Cursors.WaitCursor;
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    Files.Add(new GpxFileModel(FileSet, fileName));
                }
                Cursor.Current = Cursors.Default;
                resetTree();
            }
        }

        private void OnHelpStatusClick(object sender, EventArgs e) {
            showStatus();
        }

        private void OnHelpOverviewClick(object sender, EventArgs e) {
            // Create, show, or set visible the overview dialog as appropriate
            if (overviewDlg == null) {
                MainForm app = (MainForm)FindForm().FindForm();
                overviewDlg = new ScrolledHTMLDialog(
                    Utils.getDpiAdjustedSize(app, new Size(800, 600)),
                    "Overview", @"Help\Overview.html");
                overviewDlg.Show();
            } else {
                overviewDlg.Visible = true;
            }
        }

        private void OnHelpOverviewOnlineClick(object sender, EventArgs e) {
            try {
                Process.Start("https://kenevans.net/opensource/GPXViewer/Help/Overview.html");
            } catch (Exception ex) {
                Utils.excMsg("Failed to start browser", ex);
            }
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
            treeLevel = 3;
        }
        private void OnViewExpandNoneClick(object sender, EventArgs e) {
            treeListView.CollapseAll();
            treeLevel = 0;
        }

        private void OnViewExpandToLevelClick(object sender, EventArgs e) {
            try {
                expandToLevel(Int32.Parse(sender.ToString()));
            } catch (Exception ex) {
                Utils.excMsg("Error determining expand level", ex);
            }
        }

        private void OnExpandCollapseButtonClick(object sender, EventArgs e) {
            if (sender.ToString().Equals("Expand")) {
                treeLevel++;
                if (treeLevel > 3) treeLevel = 3;
                expandToLevel(treeLevel);
            } else if (sender.ToString().Equals("Collapse")) {
                treeLevel--;
                if (treeLevel < 0) treeLevel = 0;
                expandToLevel(treeLevel);
            }
        }
        private void OnFileSendToGoogleEarth(object sender, EventArgs e) {
            try {
                KmlOptionsDialog dlg = new KmlOptionsDialog(kmlOptions);
                DialogResult res = dlg.ShowDialog();
                if (res != DialogResult.OK) return;
                kmlOptions = dlg.KmlOptions;
                KmlUtils.createKml(FileSet, kmlOptions);
            } catch (Exception ex) {
                Utils.excMsg("Failed to send checked files to Google Earth", ex);
            }
        }

        private void OnToolsShowLogClick(object sender, EventArgs e) {
            if (logDialog != null) {
                logDialog.Visible = true;
            } else {
                Utils.infoMsg("No log has been created yet");
            }
        }

        private void OnToolsRemoveAllSelectedClick(object sender, EventArgs e) {
            foreach (object x in treeListView.SelectedObjects) {
                if (x is GpxModel model) model.delete();
            }
            synchronize();
            resetTree();
        }

        private void OnToolsRemoveAllFilesClick(object sender, EventArgs e) {
            FileSet.Files.Clear();
            resetTree();
        }

        private void OnFileSaveSelectedFilesClick(object sender, EventArgs e) {
            string fileName = null;
            if (treeListView.SelectedObjects.Count == 0) {
                Utils.errMsg("No files selected");
                return;
            }
            try {
                foreach (object x in treeListView.SelectedObjects) {
                    if (x is GpxFileModel fileModel) {
                        fileName = fileModel.FileName;
                        gpx gpx = (gpx)fileModel.Gpx.Clone();
                        string version = Assembly.GetExecutingAssembly().
                            GetName().Version.ToString();
                        gpx.creator = "GPXViewer " + version;
                        if (gpx == null) {
                            Utils.errMsg("Gpx is not defined for " + fileName);
                            continue;
                        }
                        if (fileModel.IsTcx) {
                            fileName = Path.ChangeExtension(fileName, ".gpx");
                        }
                        string saveFilename = GpxTcxMenu.getSaveName(fileName, SAVE_FILE_TAG);
                        if (saveFilename != null) gpx.Save(saveFilename);
                    }
                }
            } catch (Exception ex) {
                string msg = "Error saving files";
                if (fileName != null) msg += NL + "Current file is " + fileName;
                Utils.excMsg(msg, ex);
            }
        }

        private void OnToolsFindFilesNearClick(object sender, EventArgs e) {
            FindFilesNearDialog dlg = new FindFilesNearDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK) {
                Cursor.Current = Cursors.WaitCursor;
                List<string> files = dlg.FoundFiles;
                foreach (string fileName in files) {
                    Files.Add(new GpxFileModel(FileSet, fileName));
                }
                resetTree();
                Cursor.Current = Cursors.Default;
            }
        }

        private void OnContextMenuClick(object sender,
            BrightIdeasSoftware.CellRightClickEventArgs e) {
            bool doItem = treeListView.SelectedItem != null;
            // Can't access SelectedItems here.
            bool doSelected = true;
            contextMenuItemAdd.Enabled = doItem;
            contextMenuItemInfo.Enabled = doItem;
            contextMenuItemRemove.Enabled = doItem;
            contextMenuItemRemoveSelected.Enabled = doSelected;
            object x = e.Model;
            if (x != null) {
                contextMenuItemEdit.Visible = x is GpxFileModel ||
                    x is GpxTrackModel || x is GpxWaypointModel ||
                    x is GpxRouteModel || x is GpxTrackSegmentModel || x is GpxTrackpointModel;
                contextMenuItemAddFile.Visible = x is GpxFileModel;
                contextMenuItemAddTrack.Visible = x is GpxFileModel ||
                    x is GpxTrackModel;
                contextMenuItemAddSegment.Visible = x is GpxTrackModel ||
                    x is GpxTrackSegmentModel;
                contextMenuItemAddTrackpoint.Visible = x is GpxTrackSegmentModel ||
                    x is GpxTrackpointModel;
                contextMenuItemAddRoute.Visible = x is GpxFileModel ||
                    x is GpxRouteModel;
                contextMenuItemAddWaypoint.Visible = x is GpxFileModel ||
                    x is GpxRouteModel || x is GpxWaypointModel;
            }
            e.MenuStrip = contextMenuStrip1;
        }

        private void OnContextMenuInfoClicked(object sender, EventArgs e) {
            //Utils.infoMsg("OnContextMenuInfoClicked e=" + e);
            object x = treeListView.SelectedObject;
            if (x == null) {
                Utils.errMsg("Must be only one item selected to use this option");
                return;
            }
            if (x is GpxModel model) {
                //Utils.infoMsg("Label=" + model.Label);
                Utils.infoMsg(model.info());
            }
        }

        private void OnContextMenuRemoveClick(object sender, EventArgs e) {
            object x = treeListView.SelectedObject;
            if (x == null) {
                Utils.errMsg("Must be only one item selected to use this option");
                return;
            }
            if (x is GpxModel model) {
                model.delete();
                resetTree();
            }
        }

        private void OnContextMenuAddFileClick(object sender, EventArgs e) {
            object x = treeListView.SelectedObject;
            if (x == null || !(x is GpxModel model)) {
                Utils.errMsg("No model selected");
                return;
            }
            doTask(Task.NEWFILE, model);
        }

        private void OnContextMenuAddTrackClick(object sender, EventArgs e) {
            object x = treeListView.SelectedObject;
            if (x == null || !(x is GpxModel model)) {
                Utils.errMsg("No model selected");
                return;
            }
            doTask(Task.NEWTRK, model);
        }

        private void OnContextMenuAddSegmentClick(object sender, EventArgs e) {
            object x = treeListView.SelectedObject;
            if (x == null || !(x is GpxModel model)) {
                Utils.errMsg("No model selected");
                return;
            }
            doTask(Task.NEWSEG, model);
        }

        private void OnContextMenuAddTrackpointClick(object sender, EventArgs e) {
            object x = treeListView.SelectedObject;
            if (x == null || !(x is GpxModel model)) {
                Utils.errMsg("No model selected");
                return;
            }
            doTask(Task.NEWTKPT, model);
        }

        private void OnContextMenuAddRouteClick(object sender, EventArgs e) {
            object x = treeListView.SelectedObject;
            if (x == null || !(x is GpxModel model)) {
                Utils.errMsg("No model selected");
                return;
            }
            doTask(Task.NEWRTE, model);
        }

        private void OnContextMenuAddWaypointClick(object sender, EventArgs e) {
            object x = treeListView.SelectedObject;
            if (x == null || !(x is GpxModel model)) {
                Utils.errMsg("No model selected");
                return;
            }
            doTask(Task.NEWWPT, model);
        }

        private void OnToolsSynchronizeClick(object sender, EventArgs e) {
            synchronize();
        }

        private void OnCopyClick(object sender, EventArgs e) {
            copy();
        }

        private void OnCutClick(object sender, EventArgs e) {
            cut();
        }

        private void OnPasteClick(object sender, EventArgs e) {
            if (sender.ToString().Equals("Beginning")) {
                paste(PasteMode.BEGINNING);
            } else if (sender.ToString().Equals("Before")) {
                paste(PasteMode.BEFORE);
            } else if (sender.ToString().Equals("After")) {
                paste(PasteMode.AFTER);
            } else if (sender.ToString().Equals("End")) {
                paste(PasteMode.END);
            } else {
                paste(PasteMode.END);
            }
        }

        private void OnSaveSelectedFilesAsStartupPreferences(object sender, EventArgs e) {
            string json;
            if (treeListView.SelectedObjects.Count == 0) {
                json = "";
                return;
            }
            List<string> fileNames = new List<string>();
            foreach (object x in treeListView.SelectedObjects) {
                if (x is GpxFileModel fileModel) {
                    fileNames.Add(fileModel.FileName);
                }
            }
            json = JsonConvert.SerializeObject(fileNames);
            Properties.Settings.Default.StartupFiles = json;
            Properties.Settings.Default.Save();
        }

        private void OnSaveCheckedFilesAsStartupPreferences(object sender, EventArgs e) {
            string json;
            List<string> fileNames = new List<string>();
            foreach (GpxFileModel fileModel in Files) {
                if (fileModel.Checked) {
                    fileNames.Add(fileModel.FileName);
                }
            }
            json = JsonConvert.SerializeObject(fileNames);
            Properties.Settings.Default.StartupFiles = json;
            Properties.Settings.Default.Save();
        }

        private void OnContextMenuEditClicked(object sender, EventArgs e) {
            edit();
        }

        private void OnFileOpenFileSet(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "File Set|*.gpxfs;*.gpxfg";
            dlg.Title = "Select file sets to open";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files sets");
                    return;
                }
                Cursor.Current = Cursors.WaitCursor;
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    addFilesFromFileSet(fileName);
                }
                Cursor.Current = Cursors.Default;
                resetTree();
            }
        }

        private void OnFileSaveFileSet(object sender, EventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "File Set|*.gpxfs";
            dlg.Title = "Select file set to write";
            dlg.CheckFileExists = false;
            if (dlg.ShowDialog() == DialogResult.OK) {
                Cursor.Current = Cursors.WaitCursor;
                saveFilesToFileSet(dlg.FileName);
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
