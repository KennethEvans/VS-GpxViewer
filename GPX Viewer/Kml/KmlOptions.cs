using System;

namespace GPXViewer.KML {
    public class KmlOptions {
        public enum KmlColorMode { COLOR, COLORSET, RAINBOW };
        public String KmlFileName { get; set; }
        public double IconScale { get; set; }

        public String TrkColor { get; set; }
        public String TrkAlpha { get; set; }
        public double TrkLineWidth { get; set; }
        public KmlColorMode TrkColorMode { get; set; }
        public bool UseTrkIcon { get; set; }
        public bool UseTrkTrack { get; set; }
        public String TrkIconUrl { get; set; }

        public String RteColor { get; set; }
        public String RteAlpha { get; set; }
        public double RteLineWidth { get; set; }
        public KmlColorMode RteColorMode { get; set; }
        public Boolean UseRteIcon { get; set; }
        public String RteIconUrl { get; set; }

        public String WptColor { get; set; }
        public String WptAlpha { get; set; }
        public String WptIconUrl { get; set; }
        public KmlColorMode WptColorMode { get; set; }

        public bool PromptToOverwrite { get; set; }
        public bool SendToGoogle { get; set; }

        public KmlOptions() {
            setDefaults();
        }

        public void setDefaults() {
            KmlFileName = @"C:\Users\evans\Documents\GPSLink\AAA.GPXV.kml";
            IconScale = 1.0;

            TrkColor = "ff0000"; ;
            TrkAlpha = "ff"; ;
            TrkLineWidth = 2.0;
            TrkColorMode = KmlColorMode.RAINBOW;
            UseTrkIcon = true;
            UseTrkTrack = false;
            TrkIconUrl = "http://maps.google.com/mapfiles/kml/pushpin/wht-pushpin.png";
            RteColor = "0000ff";
            RteAlpha = "ff";
            RteLineWidth = 2.0;
            RteColorMode = KmlColorMode.COLOR;
            UseRteIcon = true;
            RteIconUrl = "http://maps.google.com/mapfiles/kml/paddle/wht-circle.png"; ;

            WptColor = "ffcc66";
            WptAlpha = "ff";
            WptIconUrl = "http://maps.google.com/mapfiles/kml/paddle/wht-circle.png";
            WptColorMode = KmlColorMode.COLOR;

            PromptToOverwrite = false;
            SendToGoogle = true;

        }
    }
}
