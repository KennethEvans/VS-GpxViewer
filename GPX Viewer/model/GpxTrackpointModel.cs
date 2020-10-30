using System;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxTrackpointModel : GpxModel {
        public wptType Trackpoint { get; set; }

        public GpxTrackpointModel(GpxModel parent, wptType wpt) {
            Parent = parent;
            if (wpt == null) {
                Trackpoint = new wptType();
                Trackpoint.name = "New Trackpoint";
            } else {
                Trackpoint = wpt;
            }
        }
        public override string getLabel() {
            string info = "";
            if (Trackpoint.name != null) {
                info += Trackpoint.name + " ";
            }
            string timeVal = "UnknownTime";
            if (Trackpoint.time != null) {
                timeVal = ((DateTime)Trackpoint.time).
                    ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            }
            info += Trackpoint.lon + ", " + Trackpoint.lat + ", " + timeVal;
            if (Parent is GpxTrackSegmentModel segment) {
                info += " (" + (segment.Trackpoints.IndexOf(this) + 1) + ")";
            }
            return info;
        }

        public override string info() {
            string msg = this.GetType().Name + NL + this + NL;
            string timeVal = "unknown";
            if(Trackpoint.time != null) {
                timeVal = ((DateTime)Trackpoint.time).
                    ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            }
            msg += "parent=" + Parent + NL;
            msg += "latitude=" + Trackpoint.lat + " longitude=" + Trackpoint.lon + NL;
            msg += "elevation=" + Trackpoint.ele + NL;
            msg += "time=" + timeVal + NL;
            msg += "description=" + Trackpoint.desc + NL;
            msg += "symbol=" + Trackpoint.sym + NL;
            msg += "cmt=" + Trackpoint.cmt + NL;
            msg += "fix=" + Trackpoint.fix + NL;
            msg += "geoidheight=" + Trackpoint.geoidheight + NL;
            msg += "hdop=" + Trackpoint.hdop + NL;
            msg += "nLinks=" + Trackpoint.link.Count + NL;
            msg += "magvar=" + Trackpoint.magvar + NL;
            msg += "pdop=" + Trackpoint.pdop + NL;
            msg += "sat=" + Trackpoint.sat + NL;
            msg += "src=" + Trackpoint.src + NL;
            msg += "type=" + Trackpoint.type + NL;
            msg += "vdop=" + Trackpoint.vdop + NL;
            return msg;
        }

        public override void delete() {
            if (Parent != null && Parent is GpxTrackSegmentModel trackSegmentModel &&
              trackSegmentModel.Trackpoints != null) {
                trackSegmentModel.Trackpoints.Remove(this);
            }
            Trackpoint = null;
        }

        public override bool add(GpxModel oldModel, GpxModel newModel, PasteMode mode) {
            throw new NotImplementedException();
        }

        public override void synchronize() {
            throw new NotImplementedException();
        }

        public override GpxModel clone() {
            GpxTrackpointModel newModel = null;
            wptType trkpt = (wptType)Trackpoint.Clone();
            if (trkpt != null) {
                newModel = new GpxTrackpointModel(null, trkpt);
            }
            return newModel;
        }
    }
}
