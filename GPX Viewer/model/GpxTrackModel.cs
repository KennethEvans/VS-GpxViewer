using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using www.topografix.com.GPX_1_1;

namespace GPX_Viewer.model {
    public class GpxTrackModel : GpxModel {
        public trkType Track { get; set; }
        public List<GpxTrackSegmentModel> Segments { get; set; }
        public GpxTrackModel(GpxModel parent, trkType trk) {
            Parent = parent;
            Track = trk;
            Segments = new List<GpxTrackSegmentModel>();
            foreach (trksegType seg in trk.trkseg) {
                Segments.Add(new GpxTrackSegmentModel(this, seg));
            }
        }
        public override string getLabel() {
            if (Track.name != null) return Track.name;
            if (Parent is GpxFileModel file) {
                return "Track " + (file.Tracks.IndexOf(this) + 1);
            }
            return "Track";
        }
        public override void showInfo() {
            throw new NotImplementedException();
        }
    }
}
