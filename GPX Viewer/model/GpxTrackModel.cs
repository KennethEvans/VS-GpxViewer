using System;
using System.Collections.Generic;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxTrackModel : GpxModel {
        public trkType Track { get; set; }
        public List<GpxTrackSegmentModel> Segments { get; set; }

        public GpxTrackModel(GpxModel parent, trkType trk) {
            Parent = parent;
            if (trk == null) {
                Track = new trkType();
                Track.name = "New Track";
            } else {
                Track = trk;
            }
            Segments = new List<GpxTrackSegmentModel>();
            if (Track.trkseg != null) {
                foreach (trksegType seg in Track.trkseg) {
                    Segments.Add(new GpxTrackSegmentModel(this, seg));
                }
            }
        }
        public override string getLabel() {
            if (Track.name != null) return Track.name;
            if (Parent is GpxFileModel file) {
                return "Track " + (file.Tracks.IndexOf(this) + 1);
            }
            return "Track";
        }
        public override string info() {
            string msg = this.GetType().Name + NL + this + NL;
            msg += "parent=" + Parent + NL;
            msg += "nSegments=" + Segments.Count + NL;
            msg += "cmt=" + Track.cmt + NL;
            msg += "desc=" + Track.desc + NL;
            msg += "nLinks=" + Track.link.Count + NL;
            msg += "name=" + Track.name + NL;
            msg += "number=" + Track.number + NL;
            msg += "src=" + Track.src + NL;
            int nTkpts = 0;
            foreach (trksegType seg in Track.trkseg) {
                nTkpts += seg.trkpt.Count;
            }
            msg += "nTrkSeg=" + Track.trkseg.Count + " nTrkpt=" + nTkpts + NL;
            msg += "type=" + Track.type + NL;
            return msg;
        }

        public override void delete() {
            if (Parent != null && Parent is GpxFileModel fileModel &&
              fileModel.Tracks != null) {
                fileModel.Tracks.Remove(this);
            }
            Track = null;
            Segments = null;
        }

        public override bool add(GpxModel oldModel, GpxModel newModel, PasteMode mode) {
            bool retVal = true;
            int index;
            switch (mode) {
                case PasteMode.BEGINNING: {
                        if (newModel is GpxTrackSegmentModel segModel) {
                            segModel.Parent = this;
                            Segments.Insert(0, segModel);
                        }
                        break;
                    }
                case PasteMode.BEFORE: {
                        if (newModel is GpxTrackSegmentModel segModel) {
                            index = Segments.IndexOf((GpxTrackSegmentModel)oldModel);
                            if (index == -1) {
                                retVal = false;
                            } else {
                                segModel.Parent = this;
                                Segments.Insert(index, segModel);
                            }
                        }
                        break;
                    }
                case PasteMode.AFTER: {
                        if (newModel is GpxTrackSegmentModel segModel) {
                            index = Segments.IndexOf((GpxTrackSegmentModel)oldModel);
                            segModel.Parent = this;
                            Segments.Insert(index + 1, segModel);
                        }
                        break;
                    }
                case PasteMode.END: {
                        if (newModel is GpxTrackSegmentModel segModel) {
                            segModel.Parent = this;
                            Segments.Add(segModel);
                        }
                        break;
                    }
            }
            return retVal;
        }

        public override void synchronize() {
            Track.trkseg.Clear();
            foreach (GpxTrackSegmentModel model in Segments) {
                Track.trkseg.Add(model.Segment);
                model.synchronize();
            }
        }

        public override GpxModel clone() {
            GpxTrackModel newModel = null;
            trkType trk = (trkType)Track.Clone();
            if (trk != null) {
                newModel = new GpxTrackModel(null, trk);
            }
            return newModel;
        }
    }
}
