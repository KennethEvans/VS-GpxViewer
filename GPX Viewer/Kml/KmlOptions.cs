using System;

namespace GPXViewer.KML {
    public class KmlOptions {
        public enum KmlColorMode { COLOR, COLORSET, RAINBOW };
        public string KmlFileName { get; set; }
        public double IconScale { get; set; }

        public string TrkColor { get; set; }
        public string TrkAlpha { get; set; }
        public double TrkLineWidth { get; set; }
        public KmlColorMode TrkColorMode { get; set; }
        public bool UseTrkIcon { get; set; }
        public bool UseTrkTrack { get; set; }
        public bool UseTrkLines { get; set; }
        public string TrkIconUrl { get; set; }

        public string RteColor { get; set; }
        public string RteAlpha { get; set; }
        public double RteLineWidth { get; set; }
        public KmlColorMode RteColorMode { get; set; }
        public bool UseRteIcon { get; set; }
        public string RteIconUrl { get; set; }

        public string WptColor { get; set; }
        public string WptAlpha { get; set; }
        public double WptLineWidth { get; set; }
        public KmlColorMode WptColorMode { get; set; }
        public bool UseWptIcon { get; set; }
        public string WptIconUrl { get; set; }

        public bool PromptToOverwrite { get; set; }
        public bool SendToGoogleEarth { get; set; }

        public KmlOptions() {
            setDefaults();
        }

        public void setDefaults() {
            KmlFileName = @"%temp%\gpxviewer.kml";
            IconScale = 1.0;

            TrkColor = "ff0000"; 
            TrkAlpha = "ff";
            TrkLineWidth = 2.0;
            TrkColorMode = KmlColorMode.RAINBOW;
            UseTrkIcon = true;
            UseTrkTrack = false;
            UseTrkLines = false;
            TrkIconUrl = "http://maps.google.com/mapfiles/kml/pushpin/wht-pushpin.png";
            RteColor = "0000ff";
            RteAlpha = "ff";
            RteLineWidth = 2.0;
            RteColorMode = KmlColorMode.COLOR;
            UseRteIcon = true;
            RteIconUrl = "http://maps.google.com/mapfiles/kml/paddle/wht-circle.png"; 

            WptColor = "ffcc66";
            WptAlpha = "ff";
            WptLineWidth = 2.0;
            WptColorMode = KmlColorMode.COLOR;
            UseWptIcon = true;
            WptIconUrl = "http://maps.google.com/mapfiles/kml/paddle/wht-circle.png";
            WptColorMode = KmlColorMode.COLOR;

            PromptToOverwrite = false;
            SendToGoogleEarth = true;

        }
    }
}
