using System;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxWaypointModel : GpxModel {
        public wptType Waypoint { get; set; }

        public GpxWaypointModel(GpxModel parent, wptType wpt) {
            Parent = parent;
            if (wpt == null) {
                Waypoint = new wptType();
                Waypoint.name = "New Waypoint";
            } else {
                Waypoint = wpt;
            }
        }
        public override string getLabel() {
            return Waypoint.name != null ? Waypoint.name : "";
#if false
            string info = Waypoint.name != null ? Waypoint.name : "";
            if (Parent is GpxRouteModel route) {
                info += " (" + (route.Waypoints.IndexOf(this) + 1) + ")";
            }
            return info;
#endif
        }
        public override string info() {
            string msg = this.GetType().Name + NL + this + NL;
            msg += "parent=" + Parent + NL;
            msg += "latitude=" + Waypoint.lat + " longitude=" + Waypoint.lon + NL;
            msg += "elevation=" + Waypoint.ele + NL;
            msg += "time=" + Waypoint.time + NL;
            msg += "description=" + Waypoint.desc + NL;
            msg += "symbol=" + Waypoint.sym + NL;
            msg += "cmt=" + Waypoint.cmt + NL;
            msg += "fix=" + Waypoint.fix + NL;
            msg += "geoidheight=" + Waypoint.geoidheight + NL;
            msg += "hdop=" + Waypoint.hdop + NL;
            msg += "nLinks=" + Waypoint.link.Count + NL;
            msg += "magvar=" + Waypoint.magvar + NL;
            msg += "pdop=" + Waypoint.pdop + NL;
            msg += "sat=" + Waypoint.sat + NL;
            msg += "src=" + Waypoint.src + NL;
            msg += "type=" + Waypoint.type + NL;
            msg += "vdop=" + Waypoint.vdop + NL;
            return msg;
        }

        public override void delete() {
            if (Parent != null && Parent is GpxFileModel fileModel &&
             fileModel.Waypoints != null) {
                fileModel.Waypoints.Remove(this);
            } else {
                if (Parent != null && Parent is GpxRouteModel rteModel &&
            rteModel.Waypoints != null) {
                    rteModel.Waypoints.Remove(this);
                }
            }
            Waypoint = null;
        }

        public override bool add(GpxModel oldModel, GpxModel newModel, PasteMode mode) {
            throw new NotImplementedException();
        }

        public override void synchronize() {
            throw new NotImplementedException();
        }

        public override GpxModel clone() {
            GpxWaypointModel newModel = null;
            wptType wpt = (wptType)Waypoint.Clone();
            if (wpt != null) {
                newModel = new GpxWaypointModel(null, wpt);
            }
            return newModel;
        }
    }
}
