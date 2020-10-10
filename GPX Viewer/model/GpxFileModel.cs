using KEUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override void showInfo() {
            string msg = this + NL + FileName;
            Utils.infoMsg(msg);
        }
    }
}
