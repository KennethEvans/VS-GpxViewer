using System;
using System.Collections.Generic;

namespace GPXViewer.model {
    public class GpxFileSetModel : GpxModel {
        public List<GpxFileModel> Files { get; set; }

        public GpxFileSetModel(GpxModel parent) {
            Parent = parent;
            Files = new List<GpxFileModel>();
        }

        public override string getLabel() {
            return "Files";
        }

        public override string info() {
            string msg = this.GetType() + NL + this + NL;
            msg += "nFiles=" + Files.Count + NL;
            return msg;
        }

        public override void delete() {
            Files = null;
        }

        public override bool add(GpxModel oldModel, GpxModel newModel, PasteMode mode) {
            throw new NotImplementedException();
        }

        public override void synchronize() {
            throw new NotImplementedException();
        }
    }
}
