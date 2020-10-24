using System;
using System.Collections.Generic;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxRouteModel : GpxModel {
        public rteType Route { get; set; }
        public List<GpxWaypointModel> Waypoints { get; set; }

        public GpxRouteModel(GpxModel parent, rteType rte) {
            Parent = parent;
            if (rte == null) {
                Route = new rteType();
                Route.name = "New Route";
            } else {
                Route = rte;
            }
            Waypoints = new List<GpxWaypointModel>();
            if (Route.rtept != null) {
                foreach (wptType wpt in Route.rtept) {
                    Waypoints.Add(new GpxWaypointModel(this, wpt));
                }
            }
        }
        public override string getLabel() {
            return Route.name != null ? Route.name : "Route";
        }

        public override string info() {
            string msg = this.GetType().Name + NL + this + NL;
            msg += "parent=" + Parent + NL;
            msg += "nWaypoints=" + Waypoints.Count + NL;
            msg += "cmt=" + Route.cmt + NL;
            msg += "desc=" + Route.desc + NL;
            msg += "nLinks=" + Route.link.Count + NL;
            msg += "name=" + Route.name + NL;
            msg += "number=" + Route.number + NL;
            msg += "nRtept=" + Route.rtept.Count + NL;
            msg += "src=" + Route.src + NL;
            msg += "type=" + Route.type + NL;
            return msg;
        }

        public override void delete() {
            if (Parent != null && Parent is GpxFileModel fileModel &&
            fileModel.Waypoints != null) {
                fileModel.Routes.Remove(this);
            }
            Route = null;
        }

        public override bool add(GpxModel oldModel, GpxModel newModel, PasteMode mode) {
            bool retVal = true;
            int index;
            switch (mode) {
                case PasteMode.BEGINNING: {
                        if (newModel is GpxWaypointModel wptModel) {
                            wptModel.Parent = this;
                            Waypoints.Insert(0, wptModel);
                        }
                        break;
                    }
                case PasteMode.BEFORE: {
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
                        if (newModel is GpxWaypointModel wptModel) {
                            index = Waypoints.IndexOf((GpxWaypointModel)oldModel);
                            wptModel.Parent = this;
                            Waypoints.Insert(index + 1, wptModel);
                        }
                        break;
                    }
                case PasteMode.END: {
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
            Route.rtept.Clear();
            foreach (GpxWaypointModel model in Waypoints) {
                Route.rtept.Add(model.Waypoint);
            }
        }

        public override GpxModel clone() {
            GpxRouteModel newModel = null;
            rteType rte = (rteType)Route.Clone();
            if (rte != null) {
                newModel = new GpxRouteModel(null, rte);
            }
            return newModel;
        }
    }
}
