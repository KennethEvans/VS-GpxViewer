using KEUtils;
using System.Collections.Generic;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxFileModel : GpxModel {
        public string FileName { get; set; }
        public gpx Gpx { get; set; }
        public List<GpxTrackModel> Tracks { get; set; }
        public List<GpxWaypointModel> Waypoints { get; set; }
        public List<GpxRouteModel> Routes { get; set; }

        public GpxFileModel(GpxModel parent, string fileName) {
            Parent = parent;
            reset(fileName);
        }

        public void reset(string fileName) {
            FileName = fileName;
            Gpx = gpx.Load(fileName);
            Tracks = new List<GpxTrackModel>();
            Waypoints = new List<GpxWaypointModel>();
            Routes = new List<GpxRouteModel>();

            foreach (trkType trk in Gpx.trk) {
                Tracks.Add(new GpxTrackModel(this, trk));
            }
            foreach (wptType wpt in Gpx.wpt) {
                Waypoints.Add(new GpxWaypointModel(this, wpt));
            }
            foreach (rteType rte in Gpx.rte) {
                Routes.Add(new GpxRouteModel(this, rte));
            }
        }

        public override string getLabel() {
            return FileName;
        }

        public override string info() {
            string msg = this.GetType() + NL + this + NL;
            msg += "nTracks=" + Tracks.Count + NL;
            msg += "nWaypoints=" + Waypoints.Count + NL;
            msg += "nRoutes=" + Routes.Count + NL;
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
                            Tracks.Insert(0, trkModel);
                        }
                        if (newModel is GpxRouteModel rteModel) {
                            Routes.Insert(0, rteModel);
                        }
                        if (newModel is GpxWaypointModel wptModel) {
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
                                Tracks.Insert(index, trkModel);
                            }
                        }
                        if (newModel is GpxRouteModel rteModel) {
                            index = Routes.IndexOf((GpxRouteModel)oldModel);
                            if (index == -1) {
                                retVal = false;
                            } else {
                                Routes.Insert(index, rteModel);
                            }
                        }
                        if (newModel is GpxWaypointModel wptModel) {
                            index = Waypoints.IndexOf((GpxWaypointModel)oldModel);
                            if (index == -1) {
                                retVal = false;
                            } else {
                                Waypoints.Insert(index, wptModel);
                            }
                        }
                        break;
                    }
                case PasteMode.AFTER: {
                        if (newModel is GpxTrackModel trkModel) {
                            index = Tracks.IndexOf((GpxTrackModel)oldModel);
                            Tracks.Insert(index, trkModel);
                        }
                        if (newModel is GpxRouteModel rteModel) {
                            index = Routes.IndexOf((GpxRouteModel)oldModel);
                            Routes.Insert(index, rteModel);
                        }
                        if (newModel is GpxWaypointModel wptModel) {
                            index = Waypoints.IndexOf((GpxWaypointModel)oldModel);
                            Waypoints.Insert(index, wptModel);
                        }
                        break;
                    }
                case PasteMode.END: {
                        if (newModel is GpxTrackModel trkModel) {
                            Tracks.Add(trkModel);
                        }
                        if (newModel is GpxRouteModel rteModel) {
                            Routes.Add(rteModel);
                        }
                        if (newModel is GpxWaypointModel wptModel) {
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
    }
}
