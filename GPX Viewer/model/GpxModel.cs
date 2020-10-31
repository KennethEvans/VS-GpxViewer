using KEUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GPXViewer.model {
    public abstract class GpxModel {
        public static readonly string NL = Environment.NewLine;
        public static readonly string UTC_FORMAT =
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
        /// Specifies how a paste special is to be done.
        public enum PasteMode {
            BEGINNING, BEFORE, AFTER, END,
        };

        private string label;
        public GpxModel Parent { get; set; }
        public string Label { get { return getLabel(); } set { label = value; } }
        public bool Checked { get; set; } = true;

        /// <summary>
        /// Returns the label.  (The label is used in ToString()).
        /// </summary>
        /// <returns></returns>
        public abstract string getLabel();

        /// <summary>
        /// Deletes this model and from any lists in its parent.
        /// </summary>
        public abstract void delete();

        /// <summary>
        /// Clones the model.
        /// </summary>
        /// <returns>The clone.</returns>
        public abstract GpxModel clone();

        /// <summary>
        /// Returns information about this model.
        /// </summary>
        /// <returns></returns>
        public abstract string info();

        /// <summary>
        /// Adds a new model 
        /// </summary>
        /// <param name="oldModel">Used for locating the place in the list to
        /// put the new model. Can be null for PasteMode.BEGINNING
        /// or PasteMode.End.</param>
        /// <param name="newModelThe model to be added."></param>
        /// <param name="mode">PasteMode specifying where in the list to
        /// place the new model relative to the old model.</param>
        /// <returns></returns>
        public abstract bool add(GpxModel oldModel, GpxModel newModel, PasteMode mode);

        /// <summary>
        /// Resets the lists from the Xml file from the GpxModel lists. This is
        /// not done when doing add, delete, etc. so the two lists are
        /// inconsistent until this method is called. The lists are in the gpx,
        /// trkSegType, trkType, rteType, and wptType.
        /// </summary>
        public abstract void synchronize();

        public override string ToString() {
            return getLabel();
        }

        public static string utcTime(DateTime? time) {
            string timeVal = "NA";
            if (time != null) {
                timeVal = ((DateTime)time).
                    ToString(UTC_FORMAT);
            }
            return timeVal;
        }
    }
}
