using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using www.topografix.com.GPX_1_1;

namespace GPX_Viewer.model {
    public class GpxWaypointModel : GpxModel {
        wptType Waypoint { get; set; }
        public GpxWaypointModel(GpxModel parent, wptType wpt) {
            Parent = parent;
            Waypoint = wpt;
        }
        public override string getLabel() {
            return Waypoint.name != null? Waypoint.name : "";
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
    }
}
