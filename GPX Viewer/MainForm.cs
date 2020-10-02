using About;
using GPX_Viewer.model;
using KEUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPX_Viewer {
    public partial class MainForm : Form {
        public static readonly String NL = Environment.NewLine;
        public GpxFileSetModel FileSet { get; set; }
        public List<GpxFileModel> Files { get; set; }
        public ImageList ImageList { get; set; }
        public MainForm() {
            InitializeComponent();

            FileSet = new GpxFileSetModel(null);
            Files = new List<GpxFileModel>();
        }

        private void OnFormLoad(object sender, EventArgs e) {
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

#if true
            // Make ImageList
            ImageList = new ImageList();
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
                if (x is GpxTrackModel trk) {
                    return 0;
                }
                if (x is GpxTrackSegmentModel seg) {
                    return 1;
                }
                if (x is GpxRouteModel rte) {
                    return 2;
                }
                if (x is GpxWaypointModel wpt) {
                    return 3;
                }
                return null;
            };
            // Set the roots to be Files
            treeListView.Roots = Files;
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
                treeListView.Roots = Files;
                treeListView.Refresh();
            }
        }

        private void OnHelpStatusClick(object sender, EventArgs e) {
            IEnumerable roots = treeListView.Roots;
            if (roots == null) {
                Utils.errMsg("TreeListView has no Roots");
                return;
            }
            int nObjs = 0, nFileSet = 0, nFile = 0, nTrk = 0, nSeg = 0;
            int nTkpt = 0, nWpt = 0, nRte = 0;
            foreach (object x in roots) {
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
            string msg = "TreeListView" + NL
                + "    " + roots.GetType() + NL
                + "    Roots Hash=" + roots.GetHashCode() + NL
                + "    Files Hash=" + Files.GetHashCode() + NL
                + "    " + roots.GetType() + NL
                + "    nObjects=" + nObjs + " nFileSets=" + nFileSet
                + " nFiles=" + nFile + NL
                + "    nTracks=" + nTrk + " nSegments=" + nSeg
                + " nTrackPoints=" + nTkpt + NL
                + "    nWayPoints=" + nWpt + NL
                + "    nRoutes=" + nRte + NL
                + "Number of files=" + Files.Count + NL;
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
            Utils.infoMsg(msg);
        }
    }
}
