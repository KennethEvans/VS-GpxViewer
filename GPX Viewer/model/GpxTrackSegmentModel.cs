using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using www.topografix.com.GPX_1_1;

namespace GPX_Viewer.model {
    public class GpxTrackSegmentModel : GpxModel {
        public trksegType Segment { get; set; }
        public List<GpxTrackpointModel> Trackpoints { get; set; }
        public GpxTrackSegmentModel(GpxModel parent, trksegType seg) {
            Parent = parent;
            Segment = seg;
            Trackpoints = new List<GpxTrackpointModel>();
            foreach (wptType tkp in seg.trkpt) {
                Trackpoints.Add(new GpxTrackpointModel(Parent, tkp));
            }
        }
        public override string getLabel() {
            if (Parent is GpxTrackModel track) {
                return "Segment " + (track.Segments.IndexOf(this) + 1);
            }
            return "Segment";
        }
        public override void showInfo() {
            throw new NotImplementedException();
        }
    }
}
