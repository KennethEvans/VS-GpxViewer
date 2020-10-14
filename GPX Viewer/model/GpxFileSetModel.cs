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

        public override void showInfo() {
            throw new NotImplementedException();
        }

        public override void delete() {
            Files = null;
        }
    }
}
