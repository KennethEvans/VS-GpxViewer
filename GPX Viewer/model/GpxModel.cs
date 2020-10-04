using KEUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GPX_Viewer.model {
    public abstract class GpxModel {
        public static readonly String NL = Environment.NewLine;
        private string label;
        public GpxModel Parent { get; set; }
        public string Label { get { return getLabel(); } set { label = value; } }
        public bool Checked { get; set; } = true;

        public abstract string getLabel();
        public abstract void showInfo();

        public override string ToString() {
            return getLabel();
        }
    }
}
