using System;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxWaypointModel : GpxModel {
        public wptType Waypoint { get; set; }
        public GpxWaypointModel(GpxModel parent, wptType wpt) {
            Parent = parent;
            Waypoint = wpt;
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
        public override void showInfo() {
            throw new NotImplementedException();
        }

        public override void delete() {
            if (Parent != null && Parent is GpxFileModel fileModel &&
             fileModel.Waypoints != null) {
                fileModel.Waypoints.Remove(this);
            }
            Waypoint = null;
        }
    }
}
