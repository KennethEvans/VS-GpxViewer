using System;
using System.Collections.Generic;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxTrackSegmentModel : GpxModel {
        public trksegType Segment { get; set; }
        public List<GpxTrackpointModel> Trackpoints { get; set; }
        public GpxTrackSegmentModel(GpxModel parent, trksegType seg) {
            Parent = parent;
            if (seg == null) {
                Segment = new trksegType();
            } else {
                Segment = seg;
            }
            Trackpoints = new List<GpxTrackpointModel>();
            foreach (wptType tkp in Segment.trkpt) {
                Trackpoints.Add(new GpxTrackpointModel(this, tkp));
            }
        }
        public override string getLabel() {
            if (Parent is GpxTrackModel track) {
                return "Segment " + (track.Segments.IndexOf(this) + 1);
            }
            return "Segment";
        }
        public override string info() {
            //synchronize();
            string msg = this.GetType() + NL + this + NL;
            msg += "nTrackpoints=" + Trackpoints.Count + NL;
            msg += "nTrkpt=" + Segment.trkpt.Count + NL;
            return msg;
        }

        public override void delete() {
            if (Parent != null && Parent is GpxTrackModel trackModel &&
              trackModel.Segments != null) {
                trackModel.Segments.Remove(this);
            }
            Segment = null;
            Trackpoints = null;
        }

        public override bool add(GpxModel oldModel, GpxModel newModel, PasteMode mode) {
            bool retVal = true;
            int index;
            switch (mode) {
                case PasteMode.BEGINNING: {
                        if (newModel is GpxTrackpointModel segModel) {
                            Trackpoints.Insert(0, segModel);
                        }
                        break;
                    }
                case PasteMode.BEFORE: {
                        if (newModel is GpxTrackpointModel trkModel) {
                            index = Trackpoints.IndexOf((GpxTrackpointModel)oldModel);
                            if (index == -1) {
                                retVal = false;
                            } else {
                                Trackpoints.Insert(index, trkModel);
                            }
                        }
                        break;
                    }
                case PasteMode.AFTER: {
                        if (newModel is GpxTrackpointModel trkModel) {
                            index = Trackpoints.IndexOf((GpxTrackpointModel)oldModel);
                            Trackpoints.Insert(index, trkModel);
                        }
                        break;
                    }
                case PasteMode.END: {
                        if (newModel is GpxTrackpointModel trkModel) {
                            Trackpoints.Add(trkModel);
                        }
                        break;
                    }
            }
            return retVal;
        }

        public override void synchronize() {
            Segment.trkpt.Clear();
            foreach (GpxTrackpointModel model in Trackpoints) {
                Segment.trkpt.Add(model.Trackpoint);
            }
        }
    }
}
