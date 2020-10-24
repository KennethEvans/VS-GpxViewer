
using KEGpsUtils;
using KEUtils;
using System;
using System.Collections.Generic;
using System.IO;
using www.garmin.com.xmlschemas.TrainingCenterDatabase.v2;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxFileModel : GpxModel {
        public string FileName { get; set; }
        public gpx Gpx { get; set; }
        public List<GpxTrackModel> Tracks { get; set; }
        public List<GpxWaypointModel> Waypoints { get; set; }
        public List<GpxRouteModel> Routes { get; set; }
        public bool IsTcx { get; set; } = false;

        public GpxFileModel(GpxModel parent, string fileName) {
            Parent = parent;
            FileName = fileName;
            if (Path.GetExtension(fileName).ToLower().Equals(".tcx")) {
                IsTcx = true;
                TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(fileName);
                if (tcx != null) {
                    Gpx = GpsData.convertTcxToGpx(tcx);
                } else {
                    Utils.errMsg("Failed to convert to GPX: " + NL + fileName);
                }
            } else {
                Gpx = gpx.Load(fileName);
            }
            reset();
        }

        public GpxFileModel(GpxModel parent, string fileName, gpx gpx) {
            Parent = parent;
            Gpx = gpx;
            FileName = fileName;
            reset();
        }

        public void reset() {
            Tracks = new List<GpxTrackModel>();
            Waypoints = new List<GpxWaypointModel>();
            Routes = new List<GpxRouteModel>();

            if (Gpx.trk != null) {
                foreach (trkType trk in Gpx.trk) {
                    Tracks.Add(new GpxTrackModel(this, trk));
                }
            }
            if (Gpx.wpt != null) {
                foreach (wptType wpt in Gpx.wpt) {
                    Waypoints.Add(new GpxWaypointModel(this, wpt));
                }
            }
            if (Gpx.rte != null) {
                foreach (rteType rte in Gpx.rte) {
                    Routes.Add(new GpxRouteModel(this, rte));
                }
            }
        }

        public override string getLabel() {
            return FileName;
        }

        public override string info() {
            string msg = this.GetType().Name + NL + this + NL;
            if (IsTcx) msg += "(Converted to GPX)" + NL;
            msg += "parent=" + Parent + NL;
            msg += "nTracks=" + Tracks.Count + NL;
            msg += "nWaypoints=" + Waypoints.Count + NL;
            msg += "nRoutes=" + Routes.Count + NL;
            // Get information from GpsUtils
            try {
                msg += NL + GpsData.processGpx(FileName, Gpx).info();
            } catch (Exception) {
                // Do nothing;
            }
            return msg;
        }

        public override void delete() {
            if (Parent != null && Parent is GpxFileSetModel fileSetModel &&
              fileSetModel.Files != null) {
                fileSetModel.Files.Remove(this);
            }
            FileName = null;
            Gpx = null;
            Tracks = null;
            Waypoints = null;
            Routes = null;
        }

        public override bool add(GpxModel oldModel, GpxModel newModel, PasteMode mode) {
            bool retVal = true;
            int index = -1;
            switch (mode) {
                case PasteMode.BEGINNING: {
                        if (newModel is GpxTrackModel trkModel) {
                            trkModel.Parent = this;
                            Tracks.Insert(0, trkModel);
                        }
                        if (newModel is GpxRouteModel rteModel) {
                            rteModel.Parent = this;
                            Routes.Insert(0, rteModel);
                        }
                        if (newModel is GpxWaypointModel wptModel) {
                            wptModel.Parent = this;
                            Waypoints.Insert(0, wptModel);
                        }
                        break;
                    }
                case PasteMode.BEFORE: {
                        if (newModel is GpxTrackModel trkModel) {
                            index = Tracks.IndexOf((GpxTrackModel)oldModel);
                            if (index == -1) {
                                retVal = false;
                            } else {
                                trkModel.Parent = this;
                                Tracks.Insert(index, trkModel);
                            }
                        }
                        if (newModel is GpxRouteModel rteModel) {
                            index = Routes.IndexOf((GpxRouteModel)oldModel);
                            if (index == -1) {
                                retVal = false;
                            } else {
                                rteModel.Parent = this;
                                Routes.Insert(index, rteModel);
                            }
                        }
                        if (newModel is GpxWaypointModel wptModel) {
                            index = Waypoints.IndexOf((GpxWaypointModel)oldModel);
                            if (index == -1) {
                                retVal = false;
                            } else {
                                wptModel.Parent = this;
                                Waypoints.Insert(index, wptModel);
                            }
                        }
                        break;
                    }
                case PasteMode.AFTER: {
                        if (newModel is GpxTrackModel trkModel) {
                            index = Tracks.IndexOf((GpxTrackModel)oldModel);
                            trkModel.Parent = this;
                            Tracks.Insert(index + 1, trkModel);
                        }
                        if (newModel is GpxRouteModel rteModel) {
                            index = Routes.IndexOf((GpxRouteModel)oldModel);
                            rteModel.Parent = this;
                            Routes.Insert(index + 1, rteModel);
                        }
                        if (newModel is GpxWaypointModel wptModel) {
                            index = Waypoints.IndexOf((GpxWaypointModel)oldModel);
                            wptModel.Parent = this;
                            Waypoints.Insert(index + 1, wptModel);
                        }
                        break;
                    }
                case PasteMode.END: {
                        if (newModel is GpxTrackModel trkModel) {
                            trkModel.Parent = this;
                            Tracks.Add(trkModel);
                        }
                        if (newModel is GpxRouteModel rteModel) {
                            rteModel.Parent = this;
                            Routes.Add(rteModel);
                        }
                        if (newModel is GpxWaypointModel wptModel) {
                            wptModel.Parent = this;
                            Waypoints.Add(wptModel);
                        }
                        break;
                    }
            }
            return retVal;
        }

        public override void synchronize() {
            Gpx.trk.Clear();
            foreach (GpxTrackModel model in Tracks) {
                Gpx.trk.Add(model.Track);
                model.synchronize();
            }
            Gpx.rte.Clear();
            foreach (GpxRouteModel model in Routes) {
                Gpx.rte.Add(model.Route);
                model.synchronize();
            }
            Gpx.wpt.Clear();
            foreach (GpxWaypointModel model in Waypoints) {
                Gpx.wpt.Add(model.Waypoint);
            }
        }

        public override GpxModel clone() {
            GpxFileModel newModel = null;
            gpx gpx = (gpx)Gpx.Clone();
            if (gpx != null) {
                newModel = new GpxFileModel(null, FileName, gpx);
            }
            return newModel;
        }
    }
}
