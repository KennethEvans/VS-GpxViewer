using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using www.topografix.com.GPX_1_1;

namespace GPX_Viewer.model {
    public class GpxTrackpointModel : GpxModel {
        wptType Trackpoint { get; set; }
        public GpxTrackpointModel(GpxModel parent, wptType wpt) {
            Parent = parent;
            Trackpoint = wpt;
        }
        public override string getLabel() {
            string info = "";
            if (Trackpoint.name != null) {
                info += Trackpoint.name + " ";
            }
            info += Trackpoint.time;
            if (Parent is GpxTrackSegmentModel segment) {
                info += " (" + (segment.Trackpoints.IndexOf(this) + 1) + ")";
            }
            return info;
        }
        public override void showInfo() {
            throw new NotImplementedException();
        }
    }
}
