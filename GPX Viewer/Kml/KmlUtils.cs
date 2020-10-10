using GPXViewer.model;
using KEUtils;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using www.topografix.com.GPX_1_1;
using static GPXViewer.KML.KmlOptions;

namespace GPXViewer.KML {
    class KmlUtils {
        public static readonly String NL = Environment.NewLine;

#if false
        /** The number of points for the find circle copied to the clipboard. */
        private static readonly int N_CIRCLE_POINTS = 101;
        /** The delta used to determine d(lat)/d(radius) and d(lon)/d(radius). */
        private static readonly double DELTA_LATLON = .0001;
#endif
        /** Set of hard-coded line colorSetColors. it will cycle through these. */
        private static readonly String[] colorSetColors = {"0000ff", "00ff00",
        "ff0000", "ffff00", "ff00ff", "00ffff", "0077ff", "ff0077"};
        /** Array to hold the track colors, values depend on the mode. */
        private static String[] trkColors;
        /** Array to hold the route colors, values depend on the mode. */
        private static String[] rteColors;
        /** Array to hold the waypoint, values depend on the mode. */
        private static String[] wptColors;

        /**
         * @param args
         */
        public static void createKml(GpxFileSetModel fileSetModel,
            KmlOptions kmlOptions) {
            // Generate the KML
            Kml kml = new Kml();

            // Create the Document for this file
            Document document = new Document() {
                Name = "GPXViewer",
                Open = true
            };
            kml.Feature = document;

            // Make the Styles for this Document
            // Trk Colors
            switch (kmlOptions.TrkColorMode) {
                case KmlColorMode.COLOR:
                    createTrkColorColors(kmlOptions);
                    break;
                case KmlColorMode.COLORSET:
                    createTrkColorSetColors(kmlOptions);
                    break;
                case KmlColorMode.RAINBOW:
                    createTrkRainbowColors(kmlOptions, fileSetModel);
                    break;
            }
            // Rte Colors
            switch (kmlOptions.RteColorMode) {
                case KmlColorMode.COLOR:
                    createRteColorColors(kmlOptions);
                    break;
                case KmlColorMode.COLORSET:
                    createRteColorSetColors(kmlOptions);
                    break;
                case KmlColorMode.RAINBOW:
                    createRteRainbowColors(kmlOptions, fileSetModel);
                    break;
            }
            // Wpt Colors
            switch (kmlOptions.WptColorMode) {
                case KmlColorMode.COLOR:
                    createWptColorColors(kmlOptions);
                    break;
                case KmlColorMode.COLORSET:
                    createWptColorSetColors(kmlOptions);
                    break;
                case KmlColorMode.RAINBOW:
                    createWptRainbowColors(kmlOptions, fileSetModel);
                    break;
            }
            // Create the color styles
            int nTrkColors = trkColors.Length;
            foreach (String color in trkColors) {
                document.AddStyle(new Style() {
                    Id = "trk" + color,
                    Line = new LineStyle() {
                        Color = Color32.Parse(color),
                        Width = kmlOptions.TrkLineWidth,
                    },
                    Icon = new SharpKml.Dom.IconStyle() {
                        Color = Color32.Parse(color),
                        ColorMode = ColorMode.Normal,
                        Scale = kmlOptions.IconScale,
                        Icon = new IconStyle.IconLink(new Uri(kmlOptions.TrkIconUrl))
                    }
                });
            }
            int nRteColors = rteColors.Length;
            foreach (String color in rteColors) {
                document.AddStyle(new Style() {
                    Id = "rte" + color,
                    Line = new LineStyle() {
                        Color = Color32.Parse(color),
                        Width = kmlOptions.TrkLineWidth,
                    },
                    Icon = new SharpKml.Dom.IconStyle() {
                        Color = Color32.Parse(color),
                        ColorMode = ColorMode.Normal,
                        Scale = kmlOptions.IconScale,
                        Icon = new IconStyle.IconLink(new Uri(kmlOptions.RteIconUrl))
                    }
                });
            }
            int nWptColors = wptColors.Length;
            foreach (String color in wptColors) {
                document.AddStyle(new Style() {
                    Id = "wpt" + color,
                    Line = new LineStyle() {
                        Color = Color32.Parse(color),
                        Width = kmlOptions.TrkLineWidth,
                    },
                    Icon = new SharpKml.Dom.IconStyle() {
                        Color = Color32.Parse(color),
                        ColorMode = ColorMode.Normal,
                        Scale = kmlOptions.IconScale,
                        Icon = new IconStyle.IconLink(new Uri(kmlOptions.WptIconUrl))
                    }
                });
            }

            // Loop over GPX files
            int nTrack = 0;
            int nRoute = 0;
            int nWaypoint = 0;
            List<GpxWaypointModel> waypointModels;
            wptType waypoint;
            Folder fileFolder;
            Folder waypointFolder;
            Folder trackFolder;
            Folder routeFolder;
            Folder folder;
            MultipleGeometry mg;
            Placemark placemark;
            Placemark trackPlacemark;
            MultipleTrack mt;
            Track track;
            LineString ls = null;
            double lat, lon, alt;
            String fileName;
            DateTime? when;
            List<GpxFileModel> fileModels = fileSetModel.Files;
            foreach (GpxFileModel fileModel in fileModels) {
                if (!fileModel.Checked) {
                    continue;
                }
                fileName = fileModel.FileName;
                if (!File.Exists(fileName)) {
                    Utils.errMsg("File does not exist: " + fileName);
                    continue;
                }
                fileFolder = new Folder() {
                    //Name = fileName,
                    Name = Path.GetFileName(fileName),
                    Open = false
                };
                document.AddFeature(fileFolder);
                // Loop over waypoints
                waypointFolder = new Folder() {
                    Name = "Waypoints",
                    Open = false
                };
                waypointModels = fileModel.Waypoints;
                if (waypointModels.Count > 0) {
                    fileFolder.AddFeature(waypointFolder);
                }
                foreach (GpxWaypointModel waypointModel in waypointModels) {
                    if (!waypointModel.Checked) {
                        continue;
                    }
                    waypoint = waypointModel.Waypoint;
                    lat = decimal.ToDouble(waypoint.lat);
                    lon = decimal.ToDouble(waypoint.lon);
                    if (waypoint.ele.HasValue) {
                        alt = decimal.ToDouble((decimal)waypoint.ele);
                    } else {
                        alt = 0;
                    }
                    // Make a Placemark
                    waypointFolder.AddFeature(new Placemark() {
                        Name = waypointModel.Label,
                        StyleUrl = new Uri("#wpt" + wptColors[nWaypoint % nWptColors],
                        UriKind.Relative),
                        Geometry = new SharpKml.Dom.Point() {
                            Coordinate = new Vector(lat, lon, alt)
                        }
                    });
                    nWaypoint++;
                }

                // Loop over tracks
                trackFolder = new Folder() {
                    Name = "Tracks",
                    Open = false
                };
                List<GpxTrackModel> trackModels;
                trackModels = fileModel.Tracks;
                bool useTrackIconFirst;
                bool useTrackTrackFirst;
                if (trackModels.Count > 0) {
                    fileFolder.AddFeature(trackFolder);
                }
                trackPlacemark = null;
                mt = null;
                track = null;
                foreach (GpxTrackModel trackModel in trackModels) {
                    if (!trackModel.Checked) {
                        continue;
                    }
                    // Make a Placemark for the track
                    if (kmlOptions.UseTrkTrack) {
                        trackPlacemark = new Placemark() {
                            Name = trackModel.Label,
                            Open = false,
                            StyleUrl = new Uri("#trk" + trkColors[nTrack % nTrkColors],
                            UriKind.Relative),
                            Snippet = new Snippet()
                        };
                        trackFolder.AddFeature(trackPlacemark);
                        mt = new MultipleTrack();
                        trackPlacemark.Geometry = mt;
                    }
                    // Make a Placemark with MultiGeometry
                    placemark = new Placemark() {
                        Name = trackModel.Label + " Lines",
                        StyleUrl = new Uri("#trk" + trkColors[nTrack % nTrkColors],
                            UriKind.Relative),
                    };
                    trackFolder.AddFeature(placemark);
                    // Need MultiGeometry to handle non-connected segments
                    mg = new MultipleGeometry();
                    placemark.Geometry = mg;
                    useTrackIconFirst = kmlOptions.UseTrkIcon ? true : false;
                    useTrackTrackFirst = kmlOptions.UseTrkTrack ? true : false;
                    foreach (trksegType trackSegment in trackModel.Track.trkseg) {
                        // Add a LineString to the MultiGeometry
                        ls = new LineString() {
                            Extrude = false,
                            Tessellate = true,
                            Coordinates = new CoordinateCollection()
                        };
                        mg.AddGeometry(ls);
                        if (mt != null) {
                            track = new Track();
                            mt.AddTrack(track);
                        }
                        foreach (wptType trackPoint in trackSegment.trkpt) {
                            lat = decimal.ToDouble(trackPoint.lat);
                            lon = decimal.ToDouble(trackPoint.lon);
                            if (trackPoint.ele.HasValue) {
                                alt = decimal.ToDouble((decimal)trackPoint.ele);
                            } else {
                                alt = 0;
                            }
                            if (trackPoint.time != null) {
                                when = (DateTime)trackPoint.time;
                            } else {
                                when = null;
                            }
                            // Add coordinates to the LineString
                            ls.Coordinates.Add(new Vector(lat, lon, alt));
                            if (track != null) {
                                // Add coordinates (and when to the track ?)
                                track.AddCoordinate(new Vector(lat, lon, alt));
                                if (when != null) {
                                    track.AddWhen((DateTime)when);
                                }
                            }
                            // Make a Placemark with an icon at the first point
                            // on the track
                            if (useTrackIconFirst) {
                                useTrackIconFirst = false;
                                trackFolder.AddFeature(new Placemark() {
                                    Name = trackModel.Label,
                                    Visibility = !kmlOptions.UseTrkTrack,
                                    StyleUrl = new Uri("#trk" + trkColors[nTrack % nTrkColors],
                                    UriKind.Relative),
                                    Geometry = new SharpKml.Dom.Point() {
                                        Coordinate = new Vector(lat, lon)
                                    }
                                });
                            }
                            if (trackPlacemark != null && useTrackTrackFirst) {
                                useTrackTrackFirst = false;
                                trackPlacemark.Description = new Description() {
                                    Text = trackPoint.time.ToString()
                                };
                            }
                        }
                    }
                    nTrack++;
                }

                // Loop over routes
                folder = new Folder() {
                    Name = "Routes",
                    Open = false
                };
                List<GpxRouteModel> routeModels;
                routeModels = fileModel.Routes;
                bool useRteIconFirst;
                if (routeModels.Count > 0) {
                    fileFolder.AddFeature(folder);
                }
                foreach (GpxRouteModel routeModel in routeModels) {
                    if (!routeModel.Checked) {
                        continue;
                    }
                    routeFolder = (new Folder() {
                        Name = routeModel.Label,
                        Open = false,
                    });
                    folder.AddFeature(routeFolder);
                    // Make a Placemark with MultiGeometry
                    placemark = new Placemark() {
                        Name = routeModel.Label + " Lines",
                        StyleUrl = new Uri("#rte" + rteColors[nRoute % nRteColors],
                        UriKind.Relative),
                    };
                    routeFolder.AddFeature(placemark);
                    // Need MultiGeometry to handle non-connected segments
                    mg = new MultipleGeometry();
                    placemark.Geometry = mg;
                    useRteIconFirst = kmlOptions.UseRteIcon ? true : false;
                    // Add a LineString to the MultiGeometry
                    ls = new LineString() {
                        Extrude = false,
                        Tessellate = true
                    };
                    mg.AddGeometry(ls);
                    foreach (wptType rtePoint in routeModel.Route.rtept) {
                        lat = decimal.ToDouble(rtePoint.lat);
                        lon = decimal.ToDouble(rtePoint.lon);
                        if (rtePoint.ele.HasValue) {
                            alt = decimal.ToDouble((decimal)rtePoint.ele);
                        } else {
                            alt = 0;
                        }
                        if (useRteIconFirst) {
                            // Make a Placemark with an Icon at the first point
                            // on the track
                            useRteIconFirst = false;
                            routeFolder.AddFeature(new Placemark() {
                                Name = routeModel.Label,
                                StyleUrl = new Uri("#rte" + rteColors[nRoute % nRteColors],
                                UriKind.Relative),
                                Geometry = new SharpKml.Dom.Point() {
                                    Coordinate = new Vector(lat, lon, alt)
                                }
                            });
                        }
                        // Make a Placemark
                        routeFolder.AddFeature(new Placemark() {
                            Name = rtePoint.name,
                            StyleUrl = new Uri("#rte" + rteColors[nRoute % nRteColors],
                            UriKind.Relative),
                            Geometry = new SharpKml.Dom.Point() {
                                Coordinate = new Vector(lat, lon, alt)
                            }
                        });
                    }
                }
                nRoute++;
            }

            // Create the file
            String kmlFileName = kmlOptions.KmlFileName;
            string outFile = kmlFileName;
            if (kmlOptions.PromptToOverwrite && File.Exists(outFile)) {
                DialogResult dr = MessageBox.Show("File exists: " + outFile
                    + "\nOK to overwrite?", "File Exists",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information);
                if (dr != DialogResult.Yes) {
                    return;
                }
            }

            try {
                Serializer serializer = new Serializer();
                serializer.Serialize(kml);
                File.WriteAllText(outFile, serializer.Xml);
            } catch (Exception ex) {
                Utils.excMsg("Error creating KML file: " + outFile, ex);
            }

            // Send it to Google Earth
            if (kmlOptions.SendToGoogle) {
                Process result;
                try {
                    result = Process.Start(outFile);
                    if (result == null) {
                        Utils.errMsg("Failed to send to Google Earth:" + NL
                            + "    " + outFile);
                    }
                } catch (Exception ex) {
                    Utils.excMsg("Failed to send to Google Earth:" + NL
                                + "    " + outFile, ex);
                }
            }
        }

        /**
         * Create the trkColors array when the mode is KmlColorMode.COLOR.
         * 
         * @param kmlOptions
         */
        private static void createTrkColorColors(KmlOptions kmlOptions) {
            // Make a single element with the options track color
            trkColors = new String[1];
            trkColors[0] = kmlOptions.TrkAlpha + kmlOptions.TrkColor;
        }

        /**
         * Create the trkColors array when the mode is KmlColorMode.COLORSET.
         * 
         * @param kmlOptions
         */
        private static void createTrkColorSetColors(KmlOptions kmlOptions) {
            // Use the hard-coded colorset colors with alpha prepended
            int nColors = colorSetColors.Length;
            trkColors = new String[nColors];
            String alpha = kmlOptions.TrkAlpha;
            // Insure alpha has two characters
            if (alpha.Length == 0) {
                alpha = "00";
            } else if (alpha.Length == 1) {
                alpha = "0" + alpha;
            } else if (alpha.Length > 2) {
                alpha = alpha.Substring(0, 2);
            }
            for (int i = 0; i < nColors; i++) {
                trkColors[i] = alpha + colorSetColors[i];
            }
        }

        /**
         * Create the trkColors array when the mode is KmlColorMode.RAINBOW.
         * 
         * @param kmlOptions
         */
        private static void createTrkRainbowColors(KmlOptions kmlOptions,
            GpxFileSetModel fileSetModel) {
            // Make a color for each track
            int nColors = 0;
            List<GpxFileModel> fileModels = fileSetModel.Files;
            foreach (GpxFileModel fileModel in fileModels) {
                if (!fileModel.Checked) {
                    continue;
                }
                List<GpxTrackModel> trackModels = fileModel.Tracks;
                foreach (GpxTrackModel trackModel in trackModels) {
                    if (!trackModel.Checked) {
                        continue;
                    }
                    nColors++;
                }
            }
            trkColors = new String[nColors];

            // Calculate the trkColors
            Color color;
            int red, green, blue;
            String alpha = kmlOptions.TrkAlpha;
            // Insure alpha has two characters
            if (alpha.Length == 0) {
                alpha = "00";
            } else if (alpha.Length == 1) {
                alpha = "0" + alpha;
            } else if (alpha.Length > 2) {
                alpha = alpha.Substring(0, 2);
            }
            for (int i = 0; i < nColors; i++) {
                color = RainbowColorScheme.defineColor(i, nColors);
                red = color.R;
                green = color.G;
                blue = color.B;
                trkColors[i] = String.Format("{0}{1:X2}{2:X2}{3:X2}", alpha, blue, green,
                    red);
            }
        }

        /**
         * Create the rteColors array when the mode is KmlColorMode.COLOR.
         * 
         * @param kmlOptions
         */
        private static void createRteColorColors(KmlOptions kmlOptions) {
            // Make a single element with the options route color
            rteColors = new String[1];
            rteColors[0] = kmlOptions.RteAlpha + kmlOptions.RteColor;
        }

        /**
         * Create the rteColors array when the mode is KmlColorMode.COLORSET.
         * 
         * @param kmlOptions
         */
        private static void createRteColorSetColors(KmlOptions kmlOptions) {
            // Use the hard-coded colorset colors with alpha prepended
            int nColors = colorSetColors.Length;
            rteColors = new String[nColors];
            String alpha = kmlOptions.RteAlpha;
            // Insure alpha has two characters
            if (alpha.Length == 0) {
                alpha = "00";
            } else if (alpha.Length == 1) {
                alpha = "0" + alpha;
            } else if (alpha.Length > 2) {
                alpha = alpha.Substring(0, 2);
            }
            for (int i = 0; i < nColors; i++) {
                rteColors[i] = alpha + colorSetColors[i];
            }
        }

        /**
         * Create the rteColors array when the mode is KmlColorMode.RAINBOW.
         * 
         * @param kmlOptions
         */
        private static void createRteRainbowColors(KmlOptions kmlOptions,
            GpxFileSetModel fileSetModel) {
            // Make a color for each route
            int nColors = 0;
            List<GpxFileModel> fileModels = fileSetModel.Files;
            foreach (GpxFileModel fileModel in fileModels) {
                if (!fileModel.Checked) {
                    continue;
                }
                List<GpxRouteModel> routeModels = fileModel.Routes;
                foreach (GpxRouteModel routeModel in routeModels) {
                    if (!routeModel.Checked) {
                        continue;
                    }
                    nColors++;
                }
            }
            rteColors = new String[nColors];

            // Calculate the rteColors
            Color color;
            int red, green, blue;
            String alpha = kmlOptions.RteAlpha;
            // Insure alpha has two characters
            if (alpha.Length == 0) {
                alpha = "00";
            } else if (alpha.Length == 1) {
                alpha = "0" + alpha;
            } else if (alpha.Length > 2) {
                alpha = alpha.Substring(0, 2);
            }
            for (int i = 0; i < nColors; i++) {
                color = RainbowColorScheme.defineColor(i, nColors);
                red = color.R;
                green = color.G;
                blue = color.B;
                rteColors[i] = String.Format("{0}{1:X2}{2:X2}{3:X2}", alpha, blue, green,
                    red);
                // DEBUG
                // System.out.println(rteColors[i]);
            }
            // DEBUG
            // System.out.println("nColors=" + nColors);
        }

        /**
         * Create the wptColors array when the mode is KmlColorMode.COLOR.
         * 
         * @param kmlOptions
         */
        private static void createWptColorColors(KmlOptions kmlOptions) {
            // Make a single element with the options waypoint color
            wptColors = new String[1];
            wptColors[0] = kmlOptions.WptAlpha + kmlOptions.WptColor;
        }

        /**
         * Create the wptColors array when the mode is KmlColorMode.COLORSET.
         * 
         * @param kmlOptions
         */
        private static void createWptColorSetColors(KmlOptions kmlOptions) {
            // Use the hard-coded colorset colors with alpha prepended
            int nColors = colorSetColors.Length;
            wptColors = new String[nColors];
            String alpha = kmlOptions.WptAlpha;
            // Insure alpha has two characters
            if (alpha.Length == 0) {
                alpha = "00";
            } else if (alpha.Length == 1) {
                alpha = "0" + alpha;
            } else if (alpha.Length > 2) {
                alpha = alpha.Substring(0, 2);
            }
            for (int i = 0; i < nColors; i++) {
                wptColors[i] = alpha + colorSetColors[i];
            }
        }

        /**
         * Create the wptColors array when the mode is KmlColorMode.RAINBOW.
         * 
         * @param kmlOptions
         */
        private static void createWptRainbowColors(KmlOptions kmlOptions,
            GpxFileSetModel fileSetModel) {
            // Make a color for each waypoint
            int nColors = 0;
            List<GpxFileModel> fileModels = fileSetModel.Files;
            foreach (GpxFileModel fileModel in fileModels) {
                if (!fileModel.Checked) {
                    continue;
                }
                List<GpxWaypointModel> waypointModels = fileModel.Waypoints;
                foreach (GpxWaypointModel waypointModel in waypointModels) {
                    if (!waypointModel.Checked) {
                        continue;
                    }
                    nColors++;
                }
            }
            wptColors = new String[nColors];

            // Calculate the wptColors
            Color color;
            int red, green, blue;
            String alpha = kmlOptions.WptAlpha;
            // Insure alpha has two characters
            if (alpha.Length == 0) {
                alpha = "00";
            } else if (alpha.Length == 1) {
                alpha = "0" + alpha;
            } else if (alpha.Length > 2) {
                alpha = alpha.Substring(0, 2);
            }
            for (int i = 0; i < nColors; i++) {
                color = RainbowColorScheme.defineColor(i, nColors);
                red = color.R;
                green = color.G;
                blue = color.B;
                wptColors[i] = String.Format("{0}{1:X2}{2:X2}{3:X2}", alpha, blue, green,
                    red);
                // DEBUG
                // System.out.println(wptColors[i]);
            }
            // DEBUG
            // System.out.println("nColors=" + nColors);
        }

#if false
       /**
         * Get the coordinates of a placemark placed in the system clipboard.
         * 
         * @return { latitude, longitude, elevation [meters] } or null on failure.
         */
        public static double[] coordinatesFromClipboardPlacemark() {
            // Get the placemark from the clipboard
            Clipboard clipboard = Toolkit.getDefaultToolkit().getSystemClipboard();
            DataFlavor flavor = DataFlavor.stringFlavor;
            Transferable contents = clipboard.getContents(null);
            if (contents == null || !contents.isDataFlavorSupported(flavor)) {
                return null;
            }
            String text = null;
            try {
                text = (String)contents.getTransferData(flavor);
            } catch (UnsupportedFlavorException ex) {
                Utils.excMsgAsync("Clipboard does not contain a placemark", ex);
                return null;
            } catch (IOException ex) {
                Utils.excMsgAsync("Clipboard does not contain a placemark", ex);
                return null;
            }
            if (text == null || text.Length == 0) {
                return null;
            }
            // Parse the coordinates
            double[] coords = { Double.NaN, Double.NaN, Double.NaN };
            String regex = "(<coordinates>.*</coordinates>)";
            Pattern pattern = Pattern.compile(regex);
            Matcher matcher = pattern.matcher(text);
            if (matcher.find()) {
                String match = matcher.group();
                int len = match.Length;
                if (match.startsWith("<coordinates>")
                    && match.endsWith("</coordinates>")) {
                    match = match.Substring(13, len - 14);
                    String[] tokens = match.split(",");
                    for (int i = 0; i < 3; i++) {
                        try {
                            coords[i] = Double.parseDouble(tokens[i]);
                        } catch (NumberFormatException ex) {
                            // Do nothing
                        }
                    }
                } else {
                    Utils.errMsg(
                        "Could not find coordinates from " + "clipboard placemark");
                    return null;
                }
            } else {
                Utils.errMsg("Could not parse placemark from Clipboard. "
                    + "May not be a placemark.");
                return null;
            }
            return coords;
        }

        /**
         * Copies a Google Earth Placemark to the system clipboard as well as an
         * optional circle about the Placemark denoting the given radius. The
         * clipboard contents can be pasted into Google Earth.
         * 
         * @param documentName The name of the KML Document or null to not make a
         *            Document. There does not have to be a document if only a
         *            Placemark is used.
         * @param lat The latitude.
         * @param lon The longitude.
         * @param ele The elevation.
         */
        public static void copyPlacemarkToClipboard(String documentName,
            String name, String lat, String lon, String ele) {
            Clipboard clipboard = Toolkit.getDefaultToolkit().getSystemClipboard();
            String text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            text += "<kml xmlns=\"http://www.opengis.net/kml/2.2\" "
                + "xmlns:gx=\"http://www.google.com/kml/ext/2.2\" "
                + "xmlns:kml=\"http://www.opengis.net/kml/2.2\" " + ""
                + "xmlns:atom=\"http://www.w3.org/2005/Atom\" "
                + "xmlns:xal=\"urn:oasis:names:tc:ciq:xsdschema:xAL:2.0\">\n";
            if (documentName != null) {
                text += "<Document>\n";
                text += "  <name>" + documentName + "</name>\n";
            }

            // Point
            text += "  <Placemark>\n";
            text += "    <name>" + name + "</name>\n";
            text += "    <Point>\n";
            text += "      <coordinates>" + lon + "," + lat + "," + ele
                + "</coordinates>\n";
            text += "    </Point>\n";
            text += "  </Placemark>\n";

            if (documentName != null) {
                text += "</Document>\n";
            }
            text += "</kml>\n";
            StringSelection selection = new StringSelection(text);
            clipboard.setContents(selection, null);
        }

/**
 * Copies a Google Earth Placemark to the system clipboard with a circle
 * about the coordinates with the given radius. The clipboard contents can
 * be pasted into Google Earth.
 * 
 * @param documentName The name of the KML Document or null to not make a
 *            Document. There does not have to be a document if only a
 *            Placemark is used.
 * @param lat The latitude.
 * @param lon The longitude.
 * @param ele The elevation.
 * @param radius The radius in meters of a circle to be drawn about the
 *            center or Double.NaN to not draw one.
 */
public static void copyPlacemarkCircleToClipboard(String documentName,
    String name, String lat, String lon, String ele, double radius) {
    Clipboard clipboard = Toolkit.getDefaultToolkit().getSystemClipboard();
    String text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
    text += "<kml xmlns=\"http://www.opengis.net/kml/2.2\" "
        + "xmlns:gx=\"http://www.google.com/kml/ext/2.2\" "
        + "xmlns:kml=\"http://www.opengis.net/kml/2.2\" " + ""
        + "xmlns:atom=\"http://www.w3.org/2005/Atom\" "
        + "xmlns:xal=\"urn:oasis:names:tc:ciq:xsdschema:xAL:2.0\">\n";
    if (documentName != null) {
        text += "<Document>\n";
        text += "  <name>" + documentName + "</name>\n";
    }

    // Circle
    if (radius != Double.NaN && radius > 0) {
        double lat0 = 0, lon0 = 0;
        try {
            lat0 = Double.parseDouble(lat);
        } catch (NumberFormatException ex) {
            // Do nothing
        }
        try {
            lon0 = Double.parseDouble(lon);
        } catch (NumberFormatException ex) {
            // Do nothing
        }
        double rlat = GpxUtils.greatCircleDistance(lat0, lon0,
            lat0 + DELTA_LATLON, lon0);
        double rlon = GpxUtils.greatCircleDistance(lat0, lon0, lat0,
            lon0 + DELTA_LATLON);
        double a = DELTA_LATLON / rlon * radius;
        double b = DELTA_LATLON / rlat * radius;
        if (!Double.isNaN(a) && !Double.isInfinite(a) && !Double.isNaN(b)
            && !Double.isInfinite(b)) {
            text += "  <Placemark>\n";
            text += "    <name>" + name + "</name>\n";
            text += "    <MultiGeometry>\n";
            text += "      <LineString>\n";
            text += "        <coordinates>";
            double delta = 2 * Math.PI / N_CIRCLE_POINTS;
            double lat1, lon1;
            for (int i = 0; i <= N_CIRCLE_POINTS; i++) {
                lat1 = lat0 + b * Math.sin(i * delta);
                lon1 = lon0 + a * Math.cos(i * delta);
                if (i != 0) {
                    text += ",";
                }
                text += String.format("%.6f", lon1) + ","
                    + String.format("%.6f", lat1) + ",0";
            }
            text += "</coordinates>\n";
            text += "      </LineString>\n";
            text += "    </MultiGeometry>\n";
        }
        text += "  </Placemark>\n";
    }
    if (documentName != null) {
        text += "</Document>\n";
    }
    text += "</kml>\n";
    StringSelection selection = new StringSelection(text);
    clipboard.setContents(selection, null);
}
#endif
    }
}
