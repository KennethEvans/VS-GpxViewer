using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

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
    }
}
