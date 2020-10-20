#define convertTimeToUTC
#undef debugging
#undef interpVerbose

using GeoTimeZone;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using TimeZoneConverter;
using TimeZoneNames;
using www.garmin.com.xmlschemas.TrainingCenterDatabase.v2;
using www.topografix.com.GPX_1_1;
#if debugging
using System.Diagnostics;
#endif

namespace KEGpsUtils {
    public class GpsData {
        public static readonly String NL = Environment.NewLine;
        /// <summary>
        /// Used for StartTimeRounded.
        /// </summary>
        public const int START_TIME_THRESHOLD_SECONDS = 300;
        /// <summary>
        /// Emiprically determined factor to make Polar distances match.
        /// </summary>
        public static readonly double POLAR_DISTANCE_FACTOR = 1.002;
        public static string TimeFormatUTC { get; } = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
        public static readonly string AUTHOR = "GpxUtils " + Assembly.GetExecutingAssembly().
                            GetName().Version.ToString();
        public enum InterpolateMode { MatchLatLon, UseInterval }
        public static readonly XNamespace TrackPointExtensionsV2_NAMESPACE =
            "http://www.garmin.com/xmlschemas/TrackPointExtension/v2";

        public string FileName { get; set; }
        public int NTracks { get; set; }
        public int NSegments { get; set; }
        public int NTrackPoints { get; set; }
        public int NHrValues { get; set; }
        public string TZId { get; set; } = null;
        public bool TZInfoFromLatLon { get; set; } = false;
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public DateTime HrStartTime { get; set; } = DateTime.MinValue;
        public DateTime HrEndTime { get; set; } = DateTime.MinValue;
        public double Distance { get; set; }  // m
        public TimeSpan Duration { get; set; }
        public TimeSpan HrDuration { get; set; }
        public string Author { get; set; }
        public string Creator { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public double LatStart { get; set; } = Double.NaN;
        public double LatMax { get; set; } = -Double.MaxValue;
        public double LatMin { get; set; } = Double.MaxValue;
        public double LonStart { get; set; } = Double.NaN;
        public double LonMax { get; set; } = -Double.MaxValue;
        public double LonMin { get; set; } = Double.MaxValue;
        public double EleStart { get; set; } = Double.NaN; // m
        public double EleMax { get; set; } = -Double.MaxValue;  // m
        public double EleMin { get; set; } = Double.MaxValue;  // m
        public double EleLoss { get; set; }  // m
        public double EleGain { get; set; }  // m
        public double EleAvg { get; set; }  // m
        public double SpeedAvg { get; set; } // m/s
        public double SpeedAvgSimple { get; set; } // m/s
        public double SpeedAvgMoving { get; set; } // m/s
        public double SpeedMax { get; set; } // m/s
        public double SpeedMin { get; set; } // m/s
        public double HrAvg { get; set; } = 0;
        public int HrMax { get; set; } = 0;
        public int HrMin { get; set; } = 0;

        // GPS Constants
        /**
          * Nominal radius of the earth in miles. The radius actually varies from
          * 3937 to 3976 mi.
          */
        public const double REARTH = 3956;
        /** Multiplier to convert miles to nautical miles. */
        public const double MI2NMI = 1.852; // Exact
        /** Multiplier to convert degrees to radians. */
        public const double DEG2RAD = Math.PI / 180.0;
        /** Multiplier to convert feet to miles. */
        public const double FT2MI = 1.0 / 5280.0;
        /** Multiplier to convert meters to miles. */
        public const double M2MI = .00062137119224;
        /** Multiplier to convert kilometers to miles. */
        public const double KM2MI = .001 * M2MI;
        /** Multiplier to convert meters to feet. */
        public const double M2FT = 3.280839895;
        /** Multiplier to convert sec to hours. */
        public const double SEC2HR = 1.0 / 3600.0;
        /** Multiplier to convert millisec to hours. */
        public const double MS2HR = .001 * SEC2HR;

        /** String used for non-available data */
        private const String NOT_AVAILABLE = "NA";

        /**
         * The speed in m/sec below which there is considered to be no movement for
         * the purposes of calculating Moving Time. This is, of course, arbitrary.
         * Note that 1 mi/hr is 0.44704 m/sec. This is expected to be set from
         * preferences.
         */
        public static double NO_MOVE_SPEED = .5;

        public static GpsData processTcx(string fileName) {
            TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(fileName);
            return processTcx(fileName, tcx);
        }

        public static GpsData processTcx(string fileName, TrainingCenterDatabase tcx) {
            GpsData data = new GpsData();
            data.FileName = fileName;

            IList<ActivityLap_t> lapList;
            IList<Track_t> trackList;
            IList<Trackpoint_t> trackpointList;
            Position_t position;

            // Get Author info
            if (tcx.Author != null) {
                AbstractSource_t author = tcx.Author;
                data.Author = tcx.Author.Name;
            }

            List<long> timeValsList = new List<long>();
            List<long> speedTimeValsList = new List<long>();
            List<Double> speedValsList = new List<double>();
            List<double> eleValsList = new List<double>();
            List<long> hrTimeValsList = new List<long>();
            List<double> hrValsList = new List<double>();
            long startTime = long.MaxValue;
            long endTime = 0;
            double deltaLength, speed;
            double deltaTime;
            long lastTimeValue = -1;

            int nAct = 0, nLaps = 0, nSegs = 0, nTpts = 0, nHr = 0;
            double lat, lon, ele, distance = 0, hrSum = 0;
            DateTime time;
            int hr, cad;

            // Loop over activities
            nAct = 0;
            foreach (Activity_t activity in tcx.Activities.Activity) {
#if debugging
                Debug.WriteLine("Activity " + nAct);
#endif
                nAct++;
                if (activity.Creator != null && activity.Creator.Name != null) {
                    data.Creator = activity.Creator.Name;
                }
                // Loop over laps (are like tracks in GPX)
                nLaps = 0;
                lapList = activity.Lap;
                foreach (ActivityLap_t lap in lapList) {
                    nLaps++;
                    // Loop over tracks
                    trackList = lap.Track;
                    nSegs = 0;
                    foreach (Track_t trk in trackList) {
                        double prevTime = -1;
                        double prevLat = 0, prevLon = 0;
                        nSegs++;
                        if (nSegs > 1) {
                            // Use NaN to make a break between segments
                            hrValsList.Add(Double.NaN);
                            hrTimeValsList.Add(lastTimeValue);
                            speedValsList.Add(Double.NaN);
                            speedTimeValsList.Add(lastTimeValue);
                            eleValsList.Add(Double.NaN);
                            timeValsList.Add(lastTimeValue);
                        }
                        // Loop over trackpoints
                        nTpts = 0;
                        trackpointList = trk.Trackpoint;
                        foreach (Trackpoint_t tpt in trackpointList) {
                            nTpts++;
                            lat = lon = ele = Double.NaN;
                            hr = 0;
                            time = DateTime.MinValue;
                            if (tpt.Position != null) {
                                position = tpt.Position;
                                lat = position.LatitudeDegrees;
                                lon = position.LongitudeDegrees;
                            } else {
                                lat = lon = Double.NaN;
                            }
                            if (tpt.AltitudeMeters != null) {
                                ele = tpt.AltitudeMeters.Value;
                            } else {
                                ele = Double.NaN;
                            }
                            if (tpt.Time != null) {
                                // Fix for bad times in Polar GPX
                                time = tpt.Time.ToUniversalTime();
                                if (time.Ticks < startTime) {
                                    startTime = time.Ticks;
                                }
                                if (time.Ticks > endTime) {
                                    endTime = time.Ticks;
                                }
                                timeValsList.Add(time.Ticks);
                            }
                            if (tpt.HeartRateBpm != null) {
                                hr = tpt.HeartRateBpm.Value;
                            } else {
                                hr = 0;
                            }
                            if (tpt.Cadence != null) {
                                cad = tpt.Cadence.Value;
                            } else {
                                cad = 0;
                            }
                            // Process
                            if (time != DateTime.MinValue) {
                                if (data.StartTime == DateTime.MinValue) {
                                    data.StartTime = time;
                                }
                                data.EndTime = time;
                            }
                            if (!Double.IsNaN(lat) && !Double.IsNaN(lon)) {
                                if (Double.IsNaN(data.LatStart)) {
                                    data.LatStart = lat;
                                    data.LonStart = lon;
                                }
                                if (lat > data.LatMax) data.LatMax = lat;
                                if (lat < data.LatMin) data.LatMin = lat;
                                if (lon > data.LonMax) data.LonMax = lon;
                                if (lon < data.LonMin) data.LonMin = lon;
                                if (prevTime != -1) {
                                    // Should be the second track point
                                    deltaLength = greatCircleDistance(
                                        prevLat, prevLon, lat, lon);
                                    distance += deltaLength;
                                    deltaTime = time.Ticks - prevTime;
                                    speed = deltaTime > 0
                                        ? TimeSpan.TicksPerSecond * deltaLength / deltaTime
                                        : 0;
                                    // Convert from m/sec to mi/hr
                                    speedValsList.Add(speed);
                                    speedTimeValsList
                                        .Add(time.Ticks - (long)Math.Round(.5 * deltaTime));
                                    if (Double.IsNaN(ele)) eleValsList.Add(0.0);
                                }
                                prevTime = time.Ticks;
                                prevLat = lat;
                                prevLon = lon;
                            }
                            if (!Double.IsNaN(ele)) {
                                if (Double.IsNaN(data.EleStart)) {
                                    data.EleStart = ele;
                                }
                                if (ele > data.EleMax) data.EleMax = ele;
                                if (ele < data.EleMin) data.EleMin = ele;
                                eleValsList.Add(ele);
                            }
                            if (hr > 0) {
                                if (time != DateTime.MinValue) {
                                    if (data.HrStartTime == DateTime.MinValue) {
                                        data.HrStartTime = time;
                                    }
                                    data.HrEndTime = time;
                                    hrSum += hr;
                                    nHr++;
                                    hrValsList.Add((double)hr);
                                    hrTimeValsList.Add(time.Ticks);
                                }
                            }
                        }
                    }
                }
            }

            // End of loops, process what was obtained
            data.Distance = distance;
            data.processValues(timeValsList, speedValsList, speedTimeValsList,
            eleValsList, hrValsList, hrTimeValsList, nLaps, nSegs, nTpts, nHr);
            return data;
        }

        public static GpsData processGpx(string fileName) {
            gpx gpx = gpx.Load(fileName);
            return processGpx(fileName, gpx);
        }

        public static GpsData processGpx(string fileName, gpx gpx) {
            GpsData data = new GpsData();
            data.FileName = fileName;

            if (gpx.creator != null) {
                data.Creator = gpx.creator;
            }

            // STL files have this information
            // They have category and location, but it is not standard
            if (gpx.metadata != null) {
                metadataType metaData = gpx.metadata;
                if (metaData.author != null) {
                    data.Author = metaData.author.name;
                }
                // STL has location and category in the metadata
                if (metaData.Untyped != null) {
                    XElement element = (XElement)metaData.Untyped;
                    foreach (XElement elem in from item in element.Descendants()
                                              select item) {
                        if (elem.Name.LocalName == "category") {
                            data.Category = (string)elem;
                        }
                        if (elem.Name.LocalName == "location") {
                            data.Location = (string)elem;
                        }
                    }
                }
            }

            extensionsType extensions;

            List<long> timeValsList = new List<long>();
            List<long> speedTimeValsList = new List<long>();
            List<Double> speedValsList = new List<double>();
            List<double> eleValsList = new List<double>();
            List<long> hrTimeValsList = new List<long>();
            List<double> hrValsList = new List<double>();

            long startTime = long.MaxValue;
            long endTime = 0;
            double deltaLength, speed;
            double deltaTime;
            long lastTimeValue = -1;

            int nSegs = 0, nTrks = 0, nTpts = 0, nHr = 0;
            double lat, lon, ele, distance = 0, hrSum = 0;
            DateTime time;
            int hr, cad;
            foreach (trkType trk in gpx.trk) {
                nTrks++;
                foreach (trksegType seg in trk.trkseg) {
                    double prevTime = -1;
                    double prevLat = 0, prevLon = 0;
                    nSegs++;
                    if (nSegs > 1) {
                        // Use NaN to make a break between segments
                        hrValsList.Add(Double.NaN);
                        hrTimeValsList.Add(lastTimeValue);
                        speedValsList.Add(Double.NaN);
                        speedTimeValsList.Add(lastTimeValue);
                        eleValsList.Add(Double.NaN);
                        timeValsList.Add(lastTimeValue);
                    }
                    foreach (wptType wpt in seg.trkpt) {
                        nTpts++;
                        lat = lon = ele = Double.NaN;
                        hr = cad = 0;
                        time = DateTime.MinValue;
                        lat = (double)wpt.lat;
                        lon = (double)wpt.lon;
                        if (wpt.ele != null) {
                            ele = (double)wpt.ele.Value;
                        }
                        if (wpt.time != null) {
                            // Fix for bad times in Polar GPX
                            time = wpt.time.Value.ToUniversalTime();
                            if (time.Ticks < startTime) {
                                startTime = time.Ticks;
                            }
                            if (time.Ticks > endTime) {
                                endTime = time.Ticks;
                            }
                            timeValsList.Add(time.Ticks);
                        }
                        // Get hr and cad from the extension
                        extensions = wpt.extensions;
                        if (extensions != null && extensions.Untyped != null) {
                            XElement extensionsElement = extensions.Untyped;
                            foreach (XElement element in extensionsElement.Elements()) {
                                if (element == null || !element.HasElements) continue;
                                foreach (XElement elem in from item in element.Descendants()
                                                          select item) {
                                    if (elem.Name.LocalName == "hr") {
                                        hr = (int)elem;
                                    }
                                    if (elem.Name.LocalName == "cad") {
                                        cad = (int)elem;
                                    }
                                }
                            }
                        }
                        if (time != DateTime.MinValue) {
                            if (data.StartTime == DateTime.MinValue) {
                                data.StartTime = time;
                            }
                            data.EndTime = time;
                        }
                        if (!Double.IsNaN(lat) && !Double.IsNaN(lon)) {
                            if (Double.IsNaN(data.LatStart)) {
                                data.LatStart = lat;
                                data.LonStart = lon;
                            }
                            if (lat > data.LatMax) data.LatMax = lat;
                            if (lat < data.LatMin) data.LatMin = lat;
                            if (lon > data.LonMax) data.LonMax = lon;
                            if (lon < data.LonMin) data.LonMin = lon;
                            if (prevTime != -1) {
                                // Should be the second track point
                                deltaLength = greatCircleDistance(
                                    prevLat, prevLon, lat, lon);
                                distance += deltaLength;
                                deltaTime = time.Ticks - prevTime;
                                speed = deltaTime > 0
                                    ? TimeSpan.TicksPerSecond * deltaLength / deltaTime
                                    : 0;
                                // Convert from m/sec to mi/hr
                                speedValsList.Add(speed);
                                speedTimeValsList
                                    .Add(time.Ticks - (long)Math.Round(.5 * deltaTime));
                                if (Double.IsNaN(ele)) eleValsList.Add(0.0);
                            }
                            prevTime = time.Ticks;
                            prevLat = lat;
                            prevLon = lon;
                        }
                        if (!Double.IsNaN(ele)) {
                            if (Double.IsNaN(data.EleStart)) {
                                data.EleStart = ele;
                            }
                            if (ele > data.EleMax) data.EleMax = ele;
                            if (ele < data.EleMin) data.EleMin = ele;
                            eleValsList.Add(ele);
                        }
                        if (hr > 0) {
                            if (time != DateTime.MinValue) {
                                if (data.HrStartTime == DateTime.MinValue) {
                                    data.HrStartTime = time;
                                }
                                data.HrEndTime = time;
                                hrSum += hr;
                                nHr++;
                                hrValsList.Add((double)hr);
                                hrTimeValsList.Add(time.Ticks);
                            }
                        }
                    }
                }
            }

            // End of loops, process what was obtained
            data.Distance = distance;
            data.processValues(timeValsList, speedValsList, speedTimeValsList,
            eleValsList, hrValsList, hrTimeValsList, nTrks, nSegs, nTpts, nHr);
            return data;
        }

        public static TrainingCenterDatabase convertGpxToTcx(gpx gpx) {
            TrainingCenterDatabase tcx = new TrainingCenterDatabase();

            // TCX types
            Position_t position;
            HeartRateInBeatsPerMinute_t hrBpm;
            AbstractSource_t author;
            Training_t training;
            Plan_t plan;
            ActivityLap_t lap;
            Track_t track;
            Trackpoint_t trackpoint;

            // Will only have one Activity
            ActivityList_t activities = new ActivityList_t();
            tcx.Activities = activities;
            Activity_t activity = new Activity_t();
            activities.Activity.Add(activity);

            // Metadata
            metadataType metadata = gpx.metadata;
            personType person;
            string desc = null, name = null, location = null, category = null;
            if (metadata != null) {
                desc = metadata.desc;
                // Polar TCX from Exercise Analyzer uses desc as the name
                // This may be a mistake
                person = metadata.author;
                if (person != null) {
                    name = person.name;
                }
                if (metadata.Untyped != null) {
                    XElement element = (XElement)metadata.Untyped;
                    foreach (XElement elem in from item in element.Descendants()
                                              select item) {
                        if (elem.Name.LocalName == "category") {
                            category = (string)elem;
                        }
                        if (elem.Name.LocalName == "location") {
                            location = (string)elem;
                        }
                    }
                }
            }

            // Set the TCX name
            if (name != null) {
                author = new Application_t();
                author.Name = name;
                tcx.Author = author;
            }

            // Set the TCX Sport name and Training Plan name
            if (!String.IsNullOrEmpty(category)) {
                activity.Sport = category;
                training = new Training_t();
                activity.Training = training;
                plan = new Plan_t();
                training.Plan = plan;
                plan.Name = category;
            }

            // Set the TCX notes
            string notes = "";
            string blank = "";
            if (!String.IsNullOrEmpty(location)) {
                notes = location + ". ";
                blank = " ";
            }
            if (!String.IsNullOrEmpty(category)) {
                notes += "category=";
                blank = " ";
            }
            if (!String.IsNullOrEmpty(desc)) {
                notes += blank + "desc=" + desc;
                blank = " ";
            }
            if (!String.IsNullOrEmpty(name)) {
                notes += blank + "author=" + name;
                blank = " ";
            }
            if (!String.IsNullOrEmpty(notes)) {
                activity.Notes = notes;
            }

            double lat, lon, ele;
            extensionsType extensions;
            DateTime time;
            short hr, cad;
            foreach (trkType trk in gpx.trk) {
                lap = new ActivityLap_t();
                activity.Lap.Add(lap);
                foreach (trksegType seg in trk.trkseg) {
                    track = new Track_t();
                    lap.Track.Add(track);
                    foreach (wptType wpt in seg.trkpt) {
                        trackpoint = new Trackpoint_t();
                        track.Trackpoint.Add(trackpoint);
                        lat = lon = ele = Double.NaN;
                        hr = cad = -1;
                        time = DateTime.MinValue;
                        lat = (double)wpt.lat;
                        lon = (double)wpt.lon;
                        position = new Position_t();
                        trackpoint.Position = position;
                        position.LatitudeDegrees = lat;
                        position.LongitudeDegrees = lon;
                        if (wpt.ele != null) {
                            ele = (double)wpt.ele.Value;
                            trackpoint.AltitudeMeters = ele;
                        }
                        if (wpt.time != null) {
                            // Fix for bad times in Polar GPX
                            time = wpt.time.Value.ToUniversalTime();
                            trackpoint.Time = time;
                        }
                        // Get hr and cad from the extension
                        extensions = wpt.extensions;
                        if (extensions != null && extensions.Untyped != null) {
                            XElement extensionsElement = extensions.Untyped;
                            foreach (XElement element in extensionsElement.Elements()) {
                                if (element == null || !element.HasElements) continue;
                                foreach (XElement elem in from item in element.Descendants()
                                                          select item) {
                                    if (elem.Name.LocalName == "hr") {
                                        hr = (short)elem;
                                    }
                                    if (elem.Name.LocalName == "cad") {
                                        cad = (short)elem;
                                    }
                                }
                            }
                            if (hr != -1) {
                                hrBpm = new HeartRateInBeatsPerMinute_t();
                                trackpoint.HeartRateBpm = hrBpm;
                                hrBpm.Value = (byte)hr;
                            }
                            if (cad != -1) {
                                trackpoint.Cadence = (byte)cad;
                            }
                        }
                    }
                }
            }
            recalculateTcx(null, tcx);
            return tcx;
        }

        public static gpx convertTcxToGpx(TrainingCenterDatabase tcx) {
            gpx gpx = new gpx();
            XElement root = gpx.Untyped.AncestorsAndSelf().First();
            root.Add(new XAttribute(XNamespace.Xmlns + "ns2",
                TrackPointExtensionsV2_NAMESPACE));
            XNamespace defaultNamespace = root.Name.NamespaceName;
            gpx.creator = "GpxUtils " + Assembly.GetExecutingAssembly().
                            GetName().Version.ToString();

            // GPX types
            trkType track;
            trksegType segment;
            wptType waypoint;
            metadataType metadata;
            personType person;

            // TCX types
            Position_t position;
            Training_t training;
            string trainingType;
            Plan_t plan;
            AbstractSource_t author;


            double lat, lon, ele;
            extensionsType extensions;
            DateTime time;
            short hr, cad;
            string notes, desc, planName;

            // Metadata
            metadata = new metadataType();
            gpx.metadata = metadata;
            author = tcx.Author;
            if (author != null && author.Name != null) {
                person = new personType();
                person.name = author.Name;
                metadata.author = person;
            }

            // Loop over activities
            foreach (Activity_t activity in tcx.Activities.Activity) {
                // Loop over laps (are like tracks in GPX)
                foreach (ActivityLap_t lap in activity.Lap) {
                    track = new trkType();
                    gpx.trk.Add(track);
                    notes = activity.Notes;
                    // Get the description from the notes and training plan
                    desc = "";
                    planName = "";
                    notes = "";
                    training = activity.Training;
                    if (training != null) {
                        plan = training.Plan;
                        if (plan != null) {
                            trainingType = plan.Type;
                            desc += "Training Type: " + trainingType;
                            planName = plan.Name;
                            if (planName != null && !String.IsNullOrEmpty(planName)) {
                                if (!String.IsNullOrEmpty(desc)) {
                                    desc += " ";
                                }
                                desc += "PlanName: " + planName;
                            }
                        }
                    }
                    notes = activity.Notes;
                    if (notes != null && !String.IsNullOrEmpty(notes)) {
                        if (!string.IsNullOrEmpty(desc)) {
                            desc += " ";
                        }
                        desc += "Notes: " + notes;
                    }
                    if (!String.IsNullOrEmpty(desc)) {
                        track.desc = desc;
                    }
                    // Loop over tracks
                    foreach (Track_t trk in lap.Track) {
                        segment = new trksegType();
                        track.trkseg.Add(segment);
                        // Loop over trackpoints
                        foreach (Trackpoint_t tpt in trk.Trackpoint) {
                            waypoint = new wptType();
                            lat = lon = ele = Double.NaN;
                            hr = -1;
                            cad = -1;
                            time = DateTime.MinValue;
                            if (tpt.Position != null) {
                                position = tpt.Position;
                                lat = position.LatitudeDegrees;
                                lon = position.LongitudeDegrees;
                                waypoint.lat = (Decimal)lat;
                                waypoint.lon = (Decimal)lon;
                            }
                            // lat and lon are required in GPX spec
                            if (Double.IsNaN(lat) || Double.IsNaN(lon)) continue;
                            if (tpt.Time != null) {
                                // Fix for bad times in Polar GPX
                                time = tpt.Time.ToUniversalTime();
                                waypoint.time = time;
                            }
                            if (tpt.AltitudeMeters != null) {
                                ele = tpt.AltitudeMeters.Value;
                                waypoint.ele = (Decimal)ele;
                            }
                            if (tpt.HeartRateBpm != null) {
                                hr = tpt.HeartRateBpm.Value;
                            }
                            if (tpt.Cadence != null) {
                                cad = tpt.Cadence.Value;
                            }
                            if (hr < 0 && cad < 0) continue;
                            // Add a TrackPointExtensionsV2 extension
                            extensions = new extensionsType();
                            extensions.Untyped.Name = defaultNamespace
                                + "extensions";
                            waypoint.extensions = extensions;
                            if (hr >= 0) {
                                XElement elemHr = new XElement(defaultNamespace
                                    + "trackPointExtensionT",
                                  new XElement(TrackPointExtensionsV2_NAMESPACE
                                  + "hr", hr));
                                extensions.Untyped.Add(elemHr);
                            }
                            if (cad >= 0) {
                                XElement elemCad = new XElement(defaultNamespace
                                    + "trackPointExtensionT",
                                  new XElement(TrackPointExtensionsV2_NAMESPACE
                                  + "cad", cad));
                                extensions.Untyped.Add(elemCad);
                            }
                            segment.trkpt.Add(waypoint);
                        }  // End of trackpoints
                    }  // End of tracks (segments)
                }  // End of laps
            } // End of activities

            return gpx;
        }

        /// <summary>
        /// Used to recalculate items in the Polar TCX files if the tracks have
        /// been modified.  This is very definitely Polar-specific.
        /// Polar StartTime is (usuallly) 1 sec before the first trckpoint
        /// time and the duration is 1 sec longer.
        /// Polar does not write DistanceMeters if there was no change in
        /// distance.  This mehod writes it always.
        /// There is a correction factor applied to distances.  This is 
        /// empirical.  The actual correction factor needed varies slightly.
        /// See POLAR_DISTANCE_FACTOR for the value used.
        /// Since Polar Beat does not make more than one Activity, Lap, and 
        /// Track per file, the logic for more laps o tracks (segments) is
        /// not tested.
        /// The MaxSpeed is typically much greater than in the Polar files.
        /// A speed calculation that does some sort of low pass filter is 
        /// probably needed as the data are noisy.  The same is true for
        /// elevation.  This method does not treat elevation as Polar Beat does
        /// not collect elevation data.
        /// In the end this calculationis probably a waste of time since the TCX
        /// parameters that are corrected can be calculated from the track data.
        /// </summary>
        /// <param name="tcx: The TCX to process."></param>
        /// <returns></returns>
        public static TrainingCenterDatabase recalculateTcx(string tcxFile,
            TrainingCenterDatabase tcx) {
            if (tcx == null) return null;
            GpsData data = new GpsData();
            data.FileName = tcxFile;

            IList<ActivityLap_t> lapList;
            IList<Track_t> trackList;
            IList<Trackpoint_t> trackpointList;

            Position_t position;

            List<long> timeValsList = new List<long>();
            List<long> speedTimeValsList = new List<long>();
            List<Double> speedValsList = new List<double>();
            List<double> eleValsList = new List<double>();
            List<long> hrTimeValsList = new List<long>();
            List<double> hrValsList = new List<double>();
            long startTime = long.MaxValue;
            long endTime = 0;
            double deltaLength, speed;
            double deltaTime;

            int nAct = 0, nLaps = 0, nSegs = 0, nTpts = 0, nHr = 0;
            double lat, lon, ele, distance = 0, hrSum = 0;
            DateTime time;
            int hr, cad;

            // Loop over activities
            nAct = 0;
            foreach (Activity_t activity in tcx.Activities.Activity) {
#if debugging
                Debug.WriteLine("Activity " + nAct);
#endif
                nAct++;
                if (activity.Creator != null && activity.Creator.Name != null) {
                    data.Creator = activity.Creator.Name;
                }
                // Loop over laps (are like tracks in GPX)
                nLaps = 0;
                lapList = activity.Lap;
                foreach (ActivityLap_t lap in lapList) {
                    nLaps++;
                    // Reset the lists (Is different from processTcx)
                    timeValsList = new List<long>();
                    speedTimeValsList = new List<long>();
                    speedValsList = new List<double>();
                    eleValsList = new List<double>();
                    hrTimeValsList = new List<long>();
                    hrValsList = new List<double>();
                    // Loop over tracks
                    trackList = lap.Track;
                    nSegs = 0;
                    foreach (Track_t trk in trackList) {
                        double prevTime = -1;
                        double prevLat = 0, prevLon = 0;
                        nSegs++;
                        // Loop over trackpoints
                        nTpts = 0;
                        trackpointList = trk.Trackpoint;
                        foreach (Trackpoint_t tpt in trackpointList) {
                            nTpts++;
                            lat = lon = ele = Double.NaN;
                            hr = 0;
                            time = DateTime.MinValue;
                            if (tpt.Position != null) {
                                position = tpt.Position;
                                lat = position.LatitudeDegrees;
                                lon = position.LongitudeDegrees;
                            } else {
                                lat = lon = Double.NaN;
                            }
                            if (tpt.AltitudeMeters != null) {
                                ele = tpt.AltitudeMeters.Value;
                            } else {
                                ele = Double.NaN;
                            }
                            if (tpt.Time != null) {
                                // Fix for bad times in Polar GPX
                                time = tpt.Time.ToUniversalTime();
                                if (time.Ticks < startTime) {
                                    startTime = time.Ticks;
                                }
                                if (time.Ticks > endTime) {
                                    endTime = time.Ticks;
                                }
                                timeValsList.Add(time.Ticks);
                            }
                            if (tpt.HeartRateBpm != null) {
                                hr = tpt.HeartRateBpm.Value;
                            } else {
                                hr = 0;
                            }
                            if (tpt.Cadence != null) {
                                cad = tpt.Cadence.Value;
                            } else {
                                cad = 0;
                            }
                            // Process
                            if (time != DateTime.MinValue) {
                                if (data.StartTime == DateTime.MinValue) {
                                    data.StartTime = time;
                                }
                                data.EndTime = time;
                            }
                            if (!Double.IsNaN(lat) && !Double.IsNaN(lon)) {
                                if (Double.IsNaN(data.LatStart)) {
                                    data.LatStart = lat;
                                    data.LonStart = lon;
                                }
                                if (lat > data.LatMax) data.LatMax = lat;
                                if (lat < data.LatMin) data.LatMin = lat;
                                if (lon > data.LonMax) data.LonMax = lon;
                                if (lon < data.LonMin) data.LonMin = lon;
                                if (prevTime != -1) {
                                    // Should be the second track point
                                    deltaLength = greatCircleDistance(
                                        prevLat, prevLon, lat, lon);
                                    distance += deltaLength;
                                    if (true && tpt.DistanceMeters != null) {
                                        // This is the accumulated distance
                                        tpt.DistanceMeters = POLAR_DISTANCE_FACTOR * distance;
                                    }
                                    deltaTime = time.Ticks - prevTime;
                                    speed = deltaTime > 0
                                        ? TimeSpan.TicksPerSecond * deltaLength / deltaTime
                                        : 0;
                                    // Convert from m/sec to mi/hr
                                    speedValsList.Add(speed);
                                    speedTimeValsList
                                        .Add(time.Ticks - (long)Math.Round(.5 * deltaTime));
                                    if (Double.IsNaN(ele)) eleValsList.Add(0.0);
                                } else {
                                    if (true && tpt.DistanceMeters != null) {
                                        tpt.DistanceMeters = 0;
                                    }
                                }
                                prevTime = time.Ticks;
                                prevLat = lat;
                                prevLon = lon;
                            }
                            if (!Double.IsNaN(ele)) {
                                if (Double.IsNaN(data.EleStart)) {
                                    data.EleStart = ele;
                                }
                                if (ele > data.EleMax) data.EleMax = ele;
                                if (ele < data.EleMin) data.EleMin = ele;
                                eleValsList.Add(ele);
                            }
                            if (hr > 0) {
                                if (time != DateTime.MinValue) {
                                    if (data.HrStartTime == DateTime.MinValue) {
                                        data.HrStartTime = time;
                                    }
                                    data.HrEndTime = time;
                                    hrSum += hr;
                                    nHr++;
                                    hrValsList.Add((double)hr);
                                    hrTimeValsList.Add(time.Ticks);
                                }
                            }
                        }  // End of trackpoints
                    }  // End of tracks (segments)
                       // Get lap data (Note this accumulates over tracks (segments))
                       // Do this here instead of at end as for processTcx
                    data.Distance = distance;
                    data.processValues(timeValsList, speedValsList, speedTimeValsList,
                    eleValsList, hrValsList, hrTimeValsList, nLaps, nSegs, nTpts, nHr);
                    // Required
                    lap.DistanceMeters = POLAR_DISTANCE_FACTOR * data.Distance;
                    // Corrected to match Polar convention.
                    lap.TotalTimeSeconds = data.Duration.TotalSeconds + 1;
                    // Corrected to match Polar convention.
                    lap.StartTime = data.StartTime.AddSeconds(-1).ToUniversalTime();
                    // Optional
                    if (true && lap.AverageHeartRateBpm != null) {
                        lap.AverageHeartRateBpm.Value = (byte)Math.Round(data.HrAvg);
                    }
                    if (true && lap.MaximumHeartRateBpm != null) {
                        lap.MaximumHeartRateBpm.Value = (byte)data.HrMax;
                    }
                    if (true && lap.MaximumSpeed != null) {
                        // Assume m/sec
                        lap.MaximumSpeed = data.SpeedMax / POLAR_DISTANCE_FACTOR;
                    }
                    // AverageSpeed is an extension
                    if (lap.Extensions != null) {
                        XElement element = (XElement)lap.Extensions;
                        if (element != null) {
                            foreach (XElement elem in from item in element.Descendants()
                                                      select item) {
                                if (elem.Name.LocalName == "AvgSpeed") {
                                    // Assume m/sec
                                    elem.Value = $"{data.SpeedAvg / POLAR_DISTANCE_FACTOR}";
                                }
                            }
                        }
                    }
                }  // End of laps
            } // End of activities
            return tcx;
        }

        public static TrainingCenterDatabase recalculateTcx(string fileName) {
            TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(fileName);
            return recalculateTcx(fileName, tcx);
        }

        /// <summary>
        /// Interpolates the lat lon values from the GPX interpFile to the
        /// TCX inputFile and saves the result to the outputFile.
        /// Only processes the fist activity, lap, and track.
        /// </summary>
        /// <param name="tcxFile">Original file</param>
        /// <param name="gpxFile">File with lat lon to interpolate</param>
        public static TcxResult interpolateTcxFromGpx(string tcxFile,
        string gpxFile, InterpolateMode mode) {
            TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(tcxFile);
            gpx gpx = gpx.Load(gpxFile);
            if (tcx.Activities == null) {
                return new TcxResult(null, "No activities");
            }

            // Prompt for time interval
            GpsData data = processTcx(tcxFile);
            DateTime start = data.StartTime.ToUniversalTime();
            DateTime end = data.EndTime.ToUniversalTime();
            TimeIntervalDialog dlg = new TimeIntervalDialog();
            dlg.Title = "Time Interval";
            dlg.Label = "Time Interval for Finding Lat/Lon Matches";
            dlg.StartDate = start;
            dlg.EndDate = end;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                start = dlg.StartDate.ToUniversalTime();
                end = dlg.EndDate.ToUniversalTime();
            } else {
                return new TcxResult(null, "Canceled");
            }

            IList<trkType> tracks = gpx.trk;

            IList<ActivityLap_t> lapList;
            IList<Track_t> trackList;
            IList<Trackpoint_t> trackpointList;

            Position_t position;
            int nActs, nLaps, nTrks, nTkpts, nSegs;
            double lat, lon, dist, totalDist;

            // Process interpolation file (GPX)
            List<LatLon> latLonList = new List<LatLon>();
            IList<trkType> tracks1 = gpx.trk;

            nTrks = nSegs = nTkpts = 0;
            totalDist = 0;
            foreach (trkType trk in tracks1) {
#if debugging
                Debug.WriteLine("Track: " + nTrks++);
#endif
                foreach (trksegType seg in trk.trkseg) {
                    nSegs++;
#if debugging
                    Debug.WriteLine("Segment: " + nSegs);
#endif
                    nTrks = 0;
                    bool first = true;
                    double latPrev = 0, lonPrev = 0;
                    dist = 0;
                    foreach (wptType wpt in seg.trkpt) {
                        nTkpts++;
                        lat = (double)wpt.lat;
                        lon = (double)wpt.lon;
                        if (!first) {
                            dist = greatCircleDistance(lat, lon, latPrev, lonPrev);
                            totalDist += dist;
                        }
                        latLonList.Add(new LatLon(lat, lon, totalDist));
                        first = false;
                        latPrev = lat;
                        lonPrev = lon;
                    }
                }
            }
            int nInterp = latLonList.Count;
            if (nInterp < 2) {
                return new TcxResult(null, "Interpolation file has only "
                    + nInterp + " items");
            }
            LatLon latLonFirst = latLonList[0];
            LatLon latLonLast = latLonList[nInterp - 1];

            // Process tcx file
            nActs = nLaps = nTrks = nTkpts = 0;
            double distFirst = Double.MaxValue, distLast = Double.MaxValue;
            LatLon latLonFirstMatch = null;
            LatLon latLonLastMatch = null;
            int indexFirstLatLon = -1;
            int indexFirst = -1;
            int indexLast = -1;
            DateTime timeFirst = DateTime.MinValue;
            DateTime timeLast = DateTime.MinValue;
            // Loop over activities
            foreach (Activity_t activity in tcx.Activities.Activity) {
#if debugging
                Debug.WriteLine("Activity " + nActs);
#endif
                nActs++;
                if (nActs > 1) {
                    // Only the first Activity is processed
                    continue;
                }
                // Loop over laps (are like tracks in GPX)
                nLaps = 0;
                lapList = activity.Lap;
                foreach (ActivityLap_t lap in lapList) {
#if debugging
                    Debug.WriteLine("Lap (Track) " + nLaps);
#endif
                    nLaps++;
                    if (nLaps > 1) {
                        // Only the first Lap is processed
                        continue;
                    }
                    // Loop over tracks
                    trackList = lap.Track;
                    nTrks = 0;
                    foreach (Track_t trk in trackList) {
                        nTrks++;
                        if (nTrks > 1) {
                            // Only the first track is processed
                            continue;
                        }
                        // Loop over trackpoints
                        nTkpts = 0;
                        trackpointList = trk.Trackpoint;
                        foreach (Trackpoint_t tpt in trackpointList) {
                            if (tpt.Time == null) continue;
#if debugging
                            Debug.WriteLine("start=" + start);
                            Debug.WriteLine("end=" + end);
                            Debug.WriteLine("time=" + tpt.Time);
                            Debug.WriteLine("time(UTC)=" + tpt.Time.ToUniversalTime());
                            Debug.WriteLine("compare=" + DateTime.Compare(tpt.Time.ToUniversalTime(), start));
#endif
                            if (DateTime.Compare(tpt.Time.ToUniversalTime(), start) < 0) {
                                continue;
                            }
                            if (DateTime.Compare(tpt.Time.ToUniversalTime(), end) > 0) {
                                continue;
                            }
                            nTkpts++;
                            if (mode == InterpolateMode.MatchLatLon) {
                                if (tpt.Position != null) {
                                    position = tpt.Position;
                                    lat = position.LatitudeDegrees;
                                    lon = position.LongitudeDegrees;
#if interpVerbose
                                    Debug.WriteLine(trackpointList.IndexOf(tpt)
                                       + " lat=" + lat + " lon=" + lon + " time=" + tpt.Time);
#endif
                                    if (indexFirstLatLon < 0) indexFirstLatLon = trackpointList.IndexOf(tpt);
                                } else {
#if interpVerbose
                                    Debug.WriteLine(trackpointList.IndexOf(tpt) +
                                        " Warning: No Position for input activity="
                                        + nActs + " lap=" + nLaps + " trk=" + nTrks
                                        + " tkpt=" + nTkpts);
#endif
                                    continue;
                                }
                                // Find first match to first lat lon in interp
                                dist = greatCircleDistance(lat, lon, latLonFirst.Lat, latLonFirst.Lon);
                                if (dist < distFirst) {
                                    distFirst = dist;
                                    latLonFirstMatch = new LatLon(lat, lon, 0, tpt.Time);
                                    indexFirst = trackpointList.IndexOf(tpt);
                                    timeFirst = tpt.Time;
                                }
                                // Find last match to last lat lon in interp
                                dist = greatCircleDistance(lat, lon, latLonLast.Lat, latLonLast.Lon);
                                if (dist <= distLast) {
                                    distLast = dist;
                                    latLonLastMatch = new LatLon(lat, lon, 0.0, tpt.Time);
                                    indexLast = trackpointList.IndexOf(tpt);
                                    timeLast = tpt.Time;
                                }
                            } else if (mode == InterpolateMode.UseInterval) {
                                if (indexFirst == -1) {
                                    indexFirst = trackpointList.IndexOf(tpt);
                                }
                                indexLast = trackpointList.IndexOf(tpt);
                            }
                        }  // End loop over trackpoints
                    } // End loop over tracks
                } // End loop over laps
            } // End loop over activities
            double firstDist = latLonList[0].Distance;
            double lastDist = latLonList[latLonList.Count - 1].Distance;
#if debugging
            Debug.WriteLine("latLonFirst: lat="
                + latLonFirst.Lat + " lon=" + latLonFirst.Lon);
            Debug.WriteLine("latLonLast: lat="
                + latLonLast.Lat + " lon=" + latLonLast.Lon);
            Debug.WriteLine("firstDist=" + firstDist + " m lastDist=" + lastDist + " m");
#endif
            if (indexFirst < 0) {
                return new TcxResult(null,
                    "Did not find a match to the first item in interpolation file");
            }
            if (indexLast < 0) {
                return new TcxResult(null,
                    "Did not find a match to the last item in interpolation file");
            }
            if (indexFirst == indexLast) {
#if interpVerbose
                Debug.WriteLine("Warning: Match to first and last item is the same point");
#endif
            }
            if (indexFirst > indexLast) {
#if interpVerbose
                Debug.WriteLine("Did not find a match for the" +
                    " first item before the one for the last item");
                Debug.WriteLine("  Therefore using index where first lat lon values were specified: "
                    + indexFirstLatLon);
#endif
                indexFirst = indexFirstLatLon;
            }
#if interpVerbose
            Debug.WriteLine("Matching from index " + indexFirst + " to " + indexLast);
#endif

            // Loop over these indices
            Activity_t activity0 = tcx.Activities.Activity[0];
            ActivityLap_t lap0 = activity0.Lap[0];
            Track_t trk0 = lap0.Track[0];
            Trackpoint_t tpt0;
            long firstTime = trk0.Trackpoint[indexFirst].Time.Ticks;
            long lastTime = trk0.Trackpoint[indexLast].Time.Ticks;
            long deltaTime = lastTime - firstTime;
            double speed = TimeSpan.TicksPerSecond * totalDist / deltaTime;
#if debugging
            Debug.WriteLine("TotalDist=" + $"{M2MI * totalDist:f2}"
                + " mi Duration=" + new TimeSpan(deltaTime)
                + " Speed=" + $"{M2MI / SEC2HR * speed:f2}"
                + " mph");
#endif
            for (int i = indexFirst; i <= indexLast; i++) {
                tpt0 = trk0.Trackpoint[i];
                if (tpt0.Position == null) {
#if interpVerbose
                    Debug.WriteLine(i +
                        " Warning: No Position for tkpt=" + nTkpts + "skipping");
#endif
                    continue;
                }
                long thisTime = tpt0.Time.Ticks - firstTime;
                double latInterp = 0, lonInterp = 0;
                long time0, time1;
                double val0, val1, deltaVal, deltaDist, distInterp;
                // Assume dist is proportional to time
                dist = totalDist * thisTime / deltaTime;
                if (dist == 0) {
                    latInterp = latLonList[0].Lat;
                    lonInterp = latLonList[0].Lon;
#if debugging
                    Debug.WriteLine(i + " latInterp=" + $"{latInterp:f8}"
                        + " lonInterp=" + $"{lonInterp:f8}"
                        + " dist=" + $"{dist:f4}"
                    );
#endif
                } else {
                    // Linearly interpolate
                    for (int j = 1; j < latLonList.Count; j++) {
                        if (dist > latLonList[j].Distance) continue;
                        time0 = latLonList[j - 1].Time.Ticks;
                        time1 = latLonList[j].Time.Ticks;
                        distInterp = dist - latLonList[j - 1].Distance;
                        deltaDist = latLonList[j].Distance - latLonList[j - 1].Distance;
                        val0 = latLonList[j - 1].Lat;
                        val1 = latLonList[j].Lat;
                        deltaVal = val1 - val0;
                        latInterp = val0;
                        if (deltaVal != 0) {
                            latInterp = val0 + distInterp * deltaVal / deltaDist;
                        }
                        val0 = latLonList[j - 1].Lon;
                        val1 = latLonList[j].Lon;
                        deltaVal = val1 - val0;
                        lonInterp = val0;
                        if (deltaVal != 0) {
                            lonInterp = val0 + distInterp * deltaVal / deltaDist;
                        }
#if debugging
                        Debug.WriteLine(i + " latInterp =" + $"{latInterp:f8}"
                            + " j=" + j
                            + " lonInterp=" + $"{lonInterp:f8}"
                            + " deltaVal=" + $"{deltaVal:f8}"
                            + " distInterp=" + $"{distInterp:f4}"
                            + " deltaDist=" + $"{deltaDist:f4}");
#endif
                        // Don't do any more
                        break;
                    }
#if interpVerbose
                    if (latInterp == 0 || lonInterp == 0) {
                        Debug.WriteLine(i + " Not found"
                            + " dist=" + $"{dist:f4}"
                            + " m firstDist=" + $"{firstDist:f4}"
                            + " m lastDist=" + $"{lastDist:f4}"
                            + " m");
                    }
#endif
                }
                position = tpt0.Position;
#if debugging
                Debug.WriteLine(i + " Before: "
                    + " LatitudeDegrees=" + position.LatitudeDegrees
                    + " LongitudeDegrees=" + position.LongitudeDegrees
                    + " latInterp=" + latInterp
                    + " lonInterp=" + lonInterp);
#endif
                position.LatitudeDegrees = latInterp;
                position.LongitudeDegrees = lonInterp;
#if debugging
                Debug.WriteLine(i + " After: "
                    + " LatitudeDegrees=" + position.LatitudeDegrees
                    + " LongitudeDegrees=" + position.LongitudeDegrees);
#endif
            }

            // Recalculate parameters
            TrainingCenterDatabase txcRecalculate = recalculateTcx(tcxFile, tcx);
            string msg = "Matched from trackpoint " + indexFirst + " to " + indexLast
                + " [" + timeFirst.ToUniversalTime().ToString("u")
                + " to " + timeLast.ToUniversalTime().ToString("u") + "]"
                + NL + "  Distance=" + $"{M2MI * totalDist:f2}" + " mi"
                + " Duration=" + new TimeSpan(deltaTime)
                + " Speed=" + $"{M2MI / SEC2HR * speed:f2}"
                + " mph" + " Mode=" + mode;
            return new TcxResult(tcx, msg);
        }

        public static TcxResult deleteTcxTrackpoints(string tcxFile) {
            TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(tcxFile);
            if (tcx.Activities == null) {
                return new TcxResult(null, "No avtivities");
            }

            // Prompt for time interval
            GpsData data = processTcx(tcxFile);
            DateTime start = data.StartTime.ToUniversalTime();
            DateTime end = data.EndTime.ToUniversalTime();
            TimeIntervalDialog dlg = new TimeIntervalDialog();
            dlg.Title = "Time Interval";
            dlg.Label = "Time Interval for Deleting Trackpoints";
            dlg.StartDate = start;
            dlg.EndDate = end;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                start = dlg.StartDate.ToUniversalTime();
                end = dlg.EndDate.ToUniversalTime();
            } else {
                return new TcxResult(null, "Canceled");
            }

            IList<ActivityLap_t> lapList;
            IList<Track_t> trackList;
            IList<Trackpoint_t> trackpointList;
            IList<Trackpoint_t> deleteList = new List<Trackpoint_t>();

            int nActs, nLaps, nTrks, nTkpts;

            nActs = nLaps = nTrks = nTkpts = 0;
            int indexFirst = -1;
            int indexLast = -1;
            DateTime timeFirst = DateTime.MinValue;
            DateTime timeLast = DateTime.MinValue;
            // Loop over activities
            foreach (Activity_t activity in tcx.Activities.Activity) {
#if debugging
                Debug.WriteLine("Activity " + nActs);
#endif
                nActs++;
                if (nActs > 1) {
                    // Only the first Activity is processed
                    continue;
                }
                // Loop over laps (are like tracks in GPX)
                nLaps = 0;
                lapList = activity.Lap;
                foreach (ActivityLap_t lap in lapList) {
#if debugging
                    Debug.WriteLine("Lap (Track) " + nLaps);
#endif
                    nLaps++;
                    if (nLaps > 1) {
                        // Only the first Lap is processed
                        continue;
                    }
                    // Loop over tracks
                    trackList = lap.Track;
                    nTrks = 0;
                    foreach (Track_t trk in trackList) {
                        nTrks++;
                        if (nTrks > 1) {
                            // Only the first track is processed
                            continue;
                        }
                        // Loop over trackpoints
                        nTkpts = 0;
                        trackpointList = trk.Trackpoint;
                        foreach (Trackpoint_t tpt in trackpointList) {
                            if (tpt.Time == null) continue;
#if debugging
                            Debug.WriteLine("start=" + start);
                            Debug.WriteLine("end=" + end);
                            Debug.WriteLine("time=" + tpt.Time);
                            Debug.WriteLine("time (UTC)=" + tpt.Time.ToUniversalTime());
                            Debug.WriteLine("compare=" + DateTime.Compare(tpt.Time.ToUniversalTime(), start));
#endif
                            if (DateTime.Compare(tpt.Time.ToUniversalTime(), start) < 0) {
                                continue;
                            }
                            if (DateTime.Compare(tpt.Time.ToUniversalTime(), end) > 0) {
                                continue;
                            }
                            nTkpts++;
                            if (indexFirst == -1) {
                                indexFirst = trackpointList.IndexOf(tpt);
                                timeFirst = tpt.Time.ToUniversalTime();
                            }
                            indexLast = trackpointList.IndexOf(tpt);
                            timeLast = tpt.Time;
                            deleteList.Add(tpt);
                        } // End of Trackpoints
                          // Remove the ones in the deleteList
                        foreach (Trackpoint_t tpt in deleteList) {
                            trackpointList.Remove(tpt);
                        }
                    }  // End of tracks
                }  // End of laps
            }  // End of activities

            // Recalculate parameters
            TrainingCenterDatabase txcRecalculate = recalculateTcx(tcxFile, tcx);
            string msg = "Deleted from trackpoint " + indexFirst + " to " + indexLast
                + " [" + timeFirst.ToUniversalTime().ToString("u")
                + " to " + timeLast.ToUniversalTime().ToString("u") + "]";
            return new TcxResult(tcx, msg);
        }

        public static TcxResult changeTimesTcx(string tcxFile) {
            TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(tcxFile);
            if (tcx.Activities == null) {
                return new TcxResult(null, "No avtivities");
            }

            // Prompt for time interval
            GpsData data = processTcx(tcxFile);
            DateTime start = data.StartTime;
            DateTime oldStart = data.StartTime.ToUniversalTime();
            DateTime newStart = oldStart;
            TimeIntervalDialog dlg = new TimeIntervalDialog();
            dlg.Title = "Start Time";
            dlg.Label = "Select a new start time";
            dlg.StartDate = oldStart;
            dlg.EndDate = oldStart;
            dlg.setStartDateVisible(false);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                newStart = dlg.EndDate.ToUniversalTime();
            } else {
                return new TcxResult(null, "Canceled");
            }
            TimeSpan deltaTime = newStart.Subtract(oldStart);
#if true
            IList<ActivityLap_t> lapList;
            IList<Track_t> trackList;
            IList<Trackpoint_t> trackpointList;
            int nActs, nLaps, nTrks, nTkpts;

            nActs = nLaps = nTrks = nTkpts = 0;
            DateTime timeFirst = DateTime.MinValue;
            DateTime timeLast = DateTime.MinValue;
            // Loop over activities
            foreach (Activity_t activity in tcx.Activities.Activity) {
#if debugging
                Debug.WriteLine("Activity " + nActs);
#endif
                nActs++;
                if (nActs > 1) {
                    // Only the first Activity is processed
                    continue;
                }
                // Loop over laps (are like tracks in GPX)
                nLaps = 0;
                lapList = activity.Lap;
                foreach (ActivityLap_t lap in lapList) {
#if debugging
                    Debug.WriteLine("Lap (Track) " + nLaps);
#endif
                    nLaps++;
                    if (nLaps > 1) {
                        // Only the first Lap is processed
                        continue;
                    }
                    // Loop over tracks
                    trackList = lap.Track;
                    nTrks = 0;
                    foreach (Track_t trk in trackList) {
                        nTrks++;
                        if (nTrks > 1) {
                            // Only the first track is processed
                            continue;
                        }
                        // Loop over trackpoints
                        nTkpts = 0;
                        trackpointList = trk.Trackpoint;
                        foreach (Trackpoint_t tpt in trackpointList) {
                            if (tpt.Time == null) continue;
#if debugging
                            Debug.WriteLine("start=" + start);
                            Debug.WriteLine("end=" + end);
                            Debug.WriteLine("time=" + tpt.Time);
                            Debug.WriteLine("time (UTC)=" + tpt.Time.ToUniversalTime());
                            Debug.WriteLine("compare=" + DateTime.Compare(tpt.Time.ToUniversalTime(), start));
#endif
                            tpt.Time = tpt.Time + deltaTime;
                            nTkpts++;
                        } // End of Trackpoints
                    }  // End of tracks
                }  // End of laps
            }  // End of activities
#endif
            // Recalculate parameters
            TrainingCenterDatabase txcRecalculate = recalculateTcx(tcxFile, tcx);
            string msg = "Changed start time from "
                + start + " to " + start.Add(deltaTime)
                + " [" + oldStart.ToUniversalTime().ToString("u")
                + " to " + newStart.ToUniversalTime().ToString("u") + "]";
            return new TcxResult(tcx, msg);
        }

        public static GpxResult fixPolarGpx(string gpxFile) {
            gpx gpx = gpx.Load(gpxFile);
            GpsData data = processGpx(gpxFile);
            bool modified = false;
            bool modifiedTime = false;
            string fixedItems = "";

            // Only handle the case where there is metadata because currently
            // there is no way to determine Category and Location from the name
            // except for Polar and STL files, which do have metadata
            if (gpx.metadata != null) {
                metadataType metaData = gpx.metadata;
                // STL has location and category in the metadata
                if (metaData.Untyped != null) {
                    XElement element = (XElement)metaData.Untyped;
                    // This probably isn't the right thing to do, but it mimics
                    // what STL does.  Better would be to have a namespace for
                    // this element
                    XNamespace ns = element.GetDefaultNamespace();
                    bool categoryFound = false;
                    bool locationFound = false;
                    foreach (XElement elem in from item in element.Descendants()
                                              select item) {
                        if (elem.Name.LocalName == "category") {
                            categoryFound = true;
                        }
                        if (elem.Name.LocalName == "location") {
                            locationFound = true;
                        }
                    }
                    if (!locationFound && !String.IsNullOrEmpty(data.Location)) {
                        XElement newElem = new XElement(ns + "location", data.Location);
                        element.Add(newElem);
                        fixedItems += " Location";
                        modified = true;
                    }
                    if (!categoryFound && !String.IsNullOrEmpty(data.Category)) {
                        XElement newElem = new XElement(ns + "category", data.Category);
                        element.Add(newElem);
                        fixedItems += " Category";
                        modified = true;
                    }
                }
            }

            // Process bad timestamps
            TimeZoneInfo tzi = null;
            if (data.TZId != null) {
                tzi = TimeZoneInfo.FindSystemTimeZoneById(data.TZId);
            }
            IList<trkType> tracks = gpx.trk;
            long timeTicks, utcTimeTicks;
            foreach (trkType trk in tracks) {
                foreach (trksegType seg in trk.trkseg) {
                    foreach (wptType wpt in seg.trkpt) {
                        if (wpt.time != null) {
                            // Fix for bad times in Polar GPX
                            timeTicks = wpt.time.Value.Ticks;
                            utcTimeTicks = wpt.time.Value.ToUniversalTime().Ticks;
                            if (timeTicks != utcTimeTicks) {
                                if (tzi != null) {
                                    wpt.time = TimeZoneInfo.ConvertTimeToUtc(wpt.time.Value, tzi);
                                    if (!modifiedTime) {
                                        modified = true;
                                        modifiedTime = true;
                                        fixedItems += " Time";
                                    }
                                } else {
                                    if (!modifiedTime) {
                                        modified = true;
                                        modifiedTime = true;
                                        fixedItems += " Time(Failed)";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!modified) return new GpxResult(gpx, "Unmodified");
            if (fixedItems.StartsWith(", ")) {
                fixedItems = fixedItems.Substring(2);
            }
            string msg = "  Fixed:" + fixedItems;
            if (String.IsNullOrEmpty(fixedItems)) {
                msg = "  Fixed: None";
            }
            return new GpxResult(gpx, msg);
        }
        /// <summary>
        /// Calculates paramters from array of data collected during parsing. 
        /// The techniques uses match those in Exercise Viewer.  Speed and 
        /// elevation could be handled ber by doing some sort of low pass
        /// filter, such as moving average.  Statistics are for a whole session,
        /// not per lap (track), track (segment), nor per activity, as for 
        /// Exercise Viewer.
        /// </summary>
        /// <param name="timeValsList"></param>
        /// <param name="speedValsList"></param>
        /// <param name="speedTimeValsList"></param>
        /// <param name="eleValsList"></param>
        /// <param name="hrValsList"></param>
        /// <param name="hrTimeValsList"></param>
        /// <param name="nTracks"></param>
        /// <param name="nSegments"></param>
        /// <param name="nTrackPoints"></param>
        /// <param name="nHrValues"></param>
        private void processValues(List<long> timeValsList,
            List<double> speedValsList, List<long> speedTimeValsList,
            List<double> eleValsList,
            List<double> hrValsList, List<long> hrTimeValsList,
            int nTracks, int nSegments, int nTrackPoints, int nHrValues) {
#if debugging
            // DEBUG
            Debug.WriteLine("speedTimeValsList");
            Debug.WriteLine("nSpeedVals=" + speedValsList.Count + " nSpeedTimeVals=" + speedTimeValsList.Count);
            for (int i = 0; i < speedValsList.Count; i++) {
                Debug.WriteLine(speedValsList[i] + " " + speedTimeValsList[i]);
            }
#endif
            // Convert to arrays
            long[] timeVals = timeValsList.ToArray();
            double[] speedVals = speedValsList.ToArray();
            long[] speedTimeVals = speedTimeValsList.ToArray();
            double[] eleVals = eleValsList.ToArray();
            double[] hrVals = hrValsList.ToArray();
            long[] hrTimeVals = hrTimeValsList.ToArray();

            NTracks = nTracks;
            NSegments = nSegments;
            NTrackPoints = nTrackPoints;
            NHrValues = nHrValues;

            setLocationAndCategoryFromFileName(FileName);

            // Convert times to the time zone of the location
            if (!Double.IsNaN(LatStart) && !Double.IsNaN(LonStart) &&
                StartTime != DateTime.MinValue && EndTime != DateTime.MinValue) {
                try {
                    TZId = getTimeZoneIdForLocation(LatStart, LonStart);
                    TZInfoFromLatLon = true;
                } catch (Exception) {
                    TZId = TimeZoneInfo.Local.Id;
                }
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(TZId);
                StartTime = TimeZoneInfo.ConvertTimeFromUtc(StartTime, tzi);
                EndTime = TimeZoneInfo.ConvertTimeFromUtc(EndTime, tzi);
            } else {
                TZId = TimeZoneInfo.Local.Id;
            }
            int nTimeVals = timeVals.Length;

            if (StartTime != DateTime.MinValue && EndTime != DateTime.MinValue) {
                TimeSpan duration = EndTime - StartTime;
                Duration = duration;
                if (duration.TotalMilliseconds > 0) {
                    SpeedAvgSimple = 1000 * Distance / duration.TotalMilliseconds;
                }
            }

            // Process arrays
            // HR
            double[] stats = null, stats1 = null;
            if (hrVals.Length > 0 && HrStartTime != DateTime.MinValue &&
                HrEndTime != DateTime.MinValue) {
                HrDuration = HrEndTime - HrStartTime;
                stats = getTimeAverageStats(hrVals, hrTimeVals, 0);
                if (stats != null) {
                    HrMin = (int)Math.Round(stats[0]);
                    HrMax = (int)Math.Round(stats[1]);
                    HrAvg = stats[2];
                } else {
                    // Get simple average
                    stats = getSimpleStats(hrVals, hrTimeVals, 0);
                    if (stats != null) {
                        HrMin = (int)Math.Round(stats[0]);
                        HrMax = (int)Math.Round(stats[1]);
                        HrAvg = stats[2];
                    }
                }
            }

            // Speed
            if (speedVals.Length > 0) {
                stats = getTimeAverageStats(speedVals, speedTimeVals, 0);
                if (stats != null) {
                    SpeedMin = stats[0];
                    SpeedMax = stats[1];
                    SpeedAvg = stats[2];
                } else {
                    // Get simple average
                    stats = getSimpleStats(speedVals, speedTimeVals, 0);
                    if (stats != null) {
                        SpeedMin = stats[0];
                        SpeedMax = stats[1];
                        SpeedAvg = stats[2];
                    }
                }
                // Moving average
                stats = getTimeAverageStats(speedVals, speedTimeVals, NO_MOVE_SPEED);
                if (stats != null) {
                    SpeedAvgMoving = stats[2];
                } else {
                    // Get simple average
                    stats = getSimpleStats(speedVals, speedTimeVals, NO_MOVE_SPEED);
                    if (stats != null) {
                        SpeedAvgMoving = stats[2];
                    }
                }

            }

            // Elevation
            if (eleVals.Length > 0) {
                stats = getTimeAverageStats(eleVals, timeVals, 0);
                stats1 = getEleStats(eleVals, timeVals);
                if (stats != null) {
                    EleMin = stats[0];
                    EleMax = stats[1];
                    EleAvg = stats[2];
                }
                if (stats1 != null) {
                    EleGain = stats1[0];
                    EleLoss = stats1[1];
                }
            } else {
                // Get simple average
                stats = getSimpleStats(eleVals, timeVals, 0);
                if (stats != null) {
                    EleMin = stats[0];
                    EleMax = stats[1];
                    EleAvg = stats[2];
                }
            }
        }

        /// <summary>
        /// Gets the Location and Category from the file name, using my formats
        /// for Polar Beat and the standard format from SportsTrackLive.
        /// </summary>
        /// <param name="fileName"></param>
        public void setLocationAndCategoryFromFileName(string fileName) {
            if (String.IsNullOrEmpty(fileName) ||
                String.IsNullOrEmpty(fileName)) return;
            // Polar
            if (Creator.ToLower().Contains("polar")) {
                string name = Path.GetFileNameWithoutExtension(fileName);
                // Only process up to the first dot
                int index = name.IndexOf(".");
                if (index != -1) {
                    name = name.Substring(0, index);
                }
                string[] tokens = name.Split('_');

                if (tokens.Length < 4) {
                    // Not the expected form for the filename
                    if (String.IsNullOrEmpty(Category)) Category = "Polar";
                    if (String.IsNullOrEmpty(Location)) Location = "Polar";
                    return;
                }
                string[] tokens1;
                int date = 0;
                int time = 0;
                // Look for the date and time tokens
                for (int i = 1; i < tokens.Length; i++) {
                    tokens1 = tokens[i].Split('-');
                    if (tokens1.Length == 3) {
                        if (date == 0) date = i;
                        else if (time == 0) {
                            time = i;
                            break;
                        }
                    }
                }
                if (time == 0 || time > tokens.Length - 3) {
                    // Date and time not found
                    if (String.IsNullOrEmpty(Category)) Category = "Polar";
                    if (String.IsNullOrEmpty(Location)) Location = "Polar";
                    return;
                }
                if (String.IsNullOrEmpty(Category)) Category = tokens[time + 1];
                // Assume Category has no _'s.  Then the rest is Location
                if (String.IsNullOrEmpty(Location)) {
                    Location = tokens[time + 2];
                }
                for (int i = time + 3; i < tokens.Length; i++) {
                    Location += " " + tokens[i];
                }
            }
            // STL
            if (Creator.ToLower().Contains("sportstracklive")
                && String.IsNullOrEmpty(Category) && String.IsNullOrEmpty(Location)) {
                string name = Path.GetFileNameWithoutExtension(fileName);
                // Only process up to the first dot
                int index = name.IndexOf(".");
                if (index != -1) {
                    name = name.Substring(0, index);
                }
                string[] tokens = name.Split('-');
                int nTokens = tokens.Length;
                if (nTokens < 6) {
                    // Not the expected form for the filename
                    Category = "STL";
                    Location = "STL";
                    return;
                }
                Category = tokens[3];
                Location = tokens[4];
                // The rest minus the last are other terms in the Location.
                for (int i = 5; i < nTokens - 1; i++) {
                    Location += " " + tokens[i];
                }
            }
        }

        /// <summary>
        /// Gets the time for the location of the exercise.  Requires GeoTimeZone,
        /// TimeZoneConvertor, and TimeZoneNames.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static string getTimeZoneIdForLocation(double lat, double lon) {
            string tzIana = TimeZoneLookup.GetTimeZone(lat, lon).Result;
            string tzId = TZConvert.IanaToWindows(tzIana);
            return tzId;
        }

        /// <summary>
        /// Gets an info string for this ExerciseData.
        /// Uses Geo Time Zone and Time Zone Converter NuGet packages.
        /// </summary>
        /// <returns></returns>
        public string info() {
            if (String.IsNullOrEmpty(FileName)) {
                return "No fileName defined";
            }
            string info = FileName + NL;
            info += "Author: " + Author + NL;
            info += "Creator: " + Creator + NL;
            info += "Category: " + Category + NL;
            info += "Location: " + Location + NL;
            info += "NTracks=" + NTracks + " NSegments=" + NSegments
                + " NTrackPoints=" + NTrackPoints + NL;
            if (TZId == null) {
                info += "TimeZone: " + "Not defined" + NL;
            } else {
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(TZId);
                string standardName = tzi.StandardName;
                string daylightName = tzi.DaylightName;
                TimeSpan baseUtcOffset = tzi.BaseUtcOffset;
                TimeSpan utcOffset = tzi.GetUtcOffset(StartTime);
                TimeZoneValues abbrs = TZNames.GetAbbreviationsForTimeZone(tzi.Id, "en-US");
                info += "TimeZone: " + (tzi.IsDaylightSavingTime(StartTime) ?
                    daylightName : standardName) + " (UTC" + utcOffset + ")"
                    + NL;
                info += "TimeZoneInfoFromLatLon: " + TZInfoFromLatLon + NL;
            }
            info += "Start Time: " + StartTime + " End Time: " + EndTime + NL;
            if (TZId != null) {
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(TZId);
                DateTime startTimeUtc = TimeZoneInfo.ConvertTimeToUtc(StartTime, tzi);
                DateTime endTimeUtc = TimeZoneInfo.ConvertTimeToUtc(EndTime, tzi);
                info += "StartTime[UTC]: " + startTimeUtc.ToString(TimeFormatUTC)
                    + " End Time[UTC]: " + endTimeUtc.ToString(TimeFormatUTC) + NL;
            }
            info += "Duration: " + Duration + NL;
            info += "Distance: " + String.Format("{0:f2} mi", M2MI * Distance) + NL;
            info += "Average Speed: " + String.Format("{0:f2} mi/hr",
                M2MI / SEC2HR * SpeedAvg) + NL;
            info += "Moving Speed: " + String.Format("{0:f2} mi/hr",
                M2MI / SEC2HR * SpeedAvgMoving) + NL;
            string hrFormat = "Heart Rate: Avg={0:f1} Min={1:f0} Max={2:f0}";
            info += ((HrAvg > 0) ?
                String.Format(hrFormat, HrAvg, HrMin, HrMax) :
                "Heart Rate: No heart rate data") + NL;

            string eleFormat = "Elevation: Start={0:f0} Min={1:f0} Max={2:f0} Gain={3:f0} Loss={4:f0} ft";
            info += (!Double.IsNaN(EleStart) ?
                    String.Format(eleFormat, M2MI * EleStart,
                    M2FT * EleMin, M2MI * EleMax,
                    M2FT * (EleMax - EleStart),
                    M2FT * (EleStart - EleMin)) :
                    "Elevation: No elevation data") + NL;
            string boundsFormat = "Bounds: LatMin={0:f6} LatMax=={1:f6} LonMin={2:f6} LonMax={3:f6}";
            info += (!Double.IsNaN(LatStart) ?
                    String.Format(boundsFormat, LatMin, LatMax, LonMin, LonMax) :
                    "Bounds: No location data") + NL;

            // Strip the final NL
            info = info.Substring(0, info.Length - NL.Length);

            return info;
        }

        ///
        /// Gets the statistics from the given values and time values by averaging
        /// over the values, not over the actual time.
        /// 
        /// @param vals
        /// @param timeVals
        /// @return {min, max, avg} or null on error.
        public static double[] getSimpleStats(double[] vals, long[] timeVals,
            double omitBelow) {
            // System.out.println("vals: " + vals.Length + ", timeVals: "
            // + timeVals.Length);
            if (vals.Length != timeVals.Length) {
                //Utils.errMsg("getSimpleStats: Array sizes (vals: " + vals.Length
                //    + ", timeVals: " + timeVals.Length + ") do not match");
                return null;
            }
            int len = vals.Length;
            if (len == 0) {
                return new double[] { 0, 0, 0 };
            }
            double max = -Double.MaxValue;
            double min = Double.MaxValue;
            double sum = 0;
            double val;
            int nVals = 0;
            for (int i = 0; i < len; i++) {
                val = vals[i];
                if (Double.IsNaN(val)) continue;
                if (val < omitBelow) continue;
                nVals++;
                sum += val;
                if (val > max) {
                    max = val;
                }
                if (val < min) {
                    min = val;
                }
            }
            if (nVals == 0) {
                return null;
            }
            sum /= nVals;
            return new double[] { min, max, sum };
        }

        ///
        /// Gets the statistics from the given values and time values by averaging
        /// over the values weighted by the time.
        /// 
        /// @param vals
        /// @param timeVals
        /// @return {min, max, avg} or null on error.
        public static double[] getTimeAverageStats(double[] vals, long[] timeVals,
            double omitBelow) {
            // System.out.println("vals: " + vals.length + ", timeVals: "
            // + timeVals.Length);
            if (vals.Length != timeVals.Length) {
                //Utils
                //    .errMsg("getTimeAverageStats: Array sizes (vals: " + vals.Length
                //        + ", timeVals: " + timeVals.Length + ") do not match");
                return null;
            }
            int len = vals.Length;
            if (len == 0) {
                return new double[] { 0, 0, 0 };
            }
            if (len < 2) {
                return new double[] { vals[0], vals[0], vals[0] };
            }
            double max = -Double.MaxValue;
            double min = Double.MaxValue;
            double sum = 0;
            double val;
            // Check for NaN
            for (int i = 0; i < len; i++) {
                val = vals[i];
                if (Double.IsNaN(val)) {
                    return null;
                }
            }

            // Loop over values.
            double totalWeight = 0;
            double weight = 0; ;
            for (int i = 0; i < len; i++) {
                val = vals[i];
                if (Double.IsNaN(val)) continue;
                if (val < omitBelow) continue;
                if (i == 0) {
                    weight = .5 * (timeVals[i + 1] - timeVals[i]);
                } else if (i == len - 1) {
                    weight = .5 * (timeVals[i] - timeVals[i - 1]);
                } else {
                    weight = .5 * (timeVals[i] - timeVals[i - 1]);
                }
                totalWeight += weight;
                // Shoudn't happen
                sum += val * weight;
                if (val > max) {
                    max = val;
                }
                if (val < min) {
                    min = val;
                }
            }
            if (totalWeight == 0) {
                return null;
            }
            sum /= (totalWeight);
            return new double[] { min, max, sum };
        }

        ///
        /// Gets the elevation statistics from the given values and time values.
        /// These include elevation gain and loss.
        /// 
        /// @param vals
        /// @param timeVals
        /// @return {gain, loss} or null on error.
        public static double[] getEleStats(double[] vals, long[] timeVals) {
            // System.out.println("vals: " + vals.Length + ", timeVals: "
            // + timeVals.Length);
            if (vals.Length != timeVals.Length) {
                //Utils.errMsg("getSimpleStats: Array sizes (vals: " + vals.Length
                //    + ", timeVals: " + timeVals.Length + ") do not match");
                return null;
            }
            int len = vals.Length;
            if (len == 0) {
                return new double[] { 0, 0 };
            }
            double gain = 0;
            double loss = 0;
            double val;
            int nVals = 0;
            for (int i = 1; i < len; i++) {
                val = vals[i] - vals[i - 1];
                if (Double.IsNaN(val)) continue;
                nVals++;
                if (val > 0) {
                    gain += val;
                } else if (val < 0) {
                    loss += -val;
                }
            }
            if (nVals == 0) {
                return null;
            }
            return new double[] { gain, loss };
        }

        public static DateTime getStartOfWeek(DateTime dt) {
            // Hard code as Sunday, could be changed
            DayOfWeek startOfWeek = DayOfWeek.Sunday;
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static string formatDuration(TimeSpan duration) {
            int days = duration.Days;
            int hours = duration.Hours;
            int minutes = duration.Minutes;
            int seconds = duration.Seconds;
            string val = "";
            if (days > 0) val += days + "d ";
            if (hours > 0) val += hours + "h ";
            if (minutes > 0) val += minutes + "m ";
            if (seconds > 0) val += seconds + "s ";
            return val;
        }

        public static string formatDurationMinutes(TimeSpan duration) {
            return $"{duration.TotalMinutes:f1}";
        }

        public static string formatSpeed(double speed) {
            if (speed == 0) return "";
            return $"{M2MI / SEC2HR * speed:f2}";
        }

        public static string formatPaceSec(double speed) {
            if (speed == 0) return "";
            double pace = SEC2HR / M2MI * 3600 / speed;
            return $"{pace:f2}";
        }

        public static string formatPace(double speed) {
            if (speed == 0) return "";
            double pace = SEC2HR / M2MI * 3600 / speed;
            TimeSpan span =
                new TimeSpan((long)(Math.Round(pace * TimeSpan.TicksPerSecond)));
            return formatDuration(span);
        }

        public static string formatHeartRate(double hr) {
            if (hr == 0) return "";
            return $"{hr:f0}";
        }

        public static string formatHeartRateAvg(double hr) {
            if (hr == 0) return "";
            return $"{hr:f1}";
        }

        public static string formatHeartRateStlAvg(double hr) {
            if (hr == 0) return "";
            return $"{hr:f0}";
        }

        public static string formatElevation(double elevation) {
            if (Double.IsNaN(elevation) ||
                elevation == -Double.MaxValue || elevation == Double.MaxValue) {
                return "";
            }
            return $"{M2FT * elevation:f2}";
        }

        public static string formatTimeZone(DateTime time, string tzid) {
            if (tzid == null) {
                return "Not defined";
            } else {
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(tzid);
                string standardName = tzi.StandardName;
                string daylightName = tzi.DaylightName;
                TimeSpan baseUtcOffset = tzi.BaseUtcOffset;
                TimeSpan utcOffset = tzi.GetUtcOffset(time);
                TimeZoneValues abbrs = TZNames.GetAbbreviationsForTimeZone(tzi.Id, "en-US");
                return (tzi.IsDaylightSavingTime(time) ?
                    daylightName : standardName);
            }
        }

        public static string formatTime(DateTime time) {
            if (time == DateTime.MinValue) return "";
            return time.ToString("yyyy-MM-dd HH:mm:ss ");
        }

        public static string formatTimeStl(DateTime time) {
            if (time == DateTime.MinValue) return "";
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string formatTimeWeekday(DateTime time) {
            if (time == DateTime.MinValue) return "";
            return time.ToString("ddd MMM dd yyyy");
        }

        public static string formatMonthStl(DateTime time) {
            if (time == DateTime.MinValue) return "";
            return (time.Month - 1).ToString();
        }

        [JsonIgnore]
        public string Source {
            get {
                if (String.IsNullOrEmpty(Creator)) {
                    return "NA";
                } else if (Creator.ToLower().Contains("polar")) {
                    return "Polar";
                } else if (Creator.ToLower().Contains("sportstracklive")) {
                    return "STL";
                } else if (Creator.ToLower().Contains("gpx inspector")) {
                    return "GPX Inspector";
                } else if (Creator.ToLower().Contains("gpslink")) {
                    return "GpsLink";
                } else if (Creator.ToLower().Contains("mapsource")) {
                    return "MapSource";
                } else {
                    return "Other";
                }
            }
        }

        [JsonIgnore]
        public DateTime StartTimeRounded {
            get {
                TimeSpan tolerance =
                    new TimeSpan(START_TIME_THRESHOLD_SECONDS *
                    TimeSpan.TicksPerSecond);
                var halfIntervalTicks = (tolerance.Ticks + 1) >> 1;

                return StartTime.AddTicks(halfIntervalTicks -
                    ((StartTime.Ticks + halfIntervalTicks) % tolerance.Ticks));
            }
        }

        [JsonIgnore]
        public string Extension {
            get {
                if (String.IsNullOrEmpty(FileName)) return null;
                return Path.GetExtension(FileName);
            }
        }

        [JsonIgnore]
        public string SimpleFileName {
            get {
                if (String.IsNullOrEmpty(FileName)) return null;
                return Path.GetFileName(FileName);
            }
        }

        [JsonIgnore]
        public string FileNameWithoutExtension {
            get {
                if (String.IsNullOrEmpty(FileName)) return null;
                return Path.GetFileNameWithoutExtension(FileName);
            }
        }

        [JsonIgnore]
        public bool IsTcx {
            get {
                return Path.GetExtension(FileName).ToLower() == ".tcx";
            }
        }

        [JsonIgnore]
        public bool IsGpx {
            get {
                return Path.GetExtension(FileName).ToLower() == ".gpx";
            }
        }

        /// <summary>
        /// Returns great circle distance in meters. assuming a spherical earth.
        /// Uses Haversine formula.
        /// </summary>
        /// <param name="lat1">Start latitude in deg.</param>
        /// <param name="lon1">Start longitude in deg.</param>
        /// <param name="lat2">End latitude in deg.</param>
        /// <param name="lon2">End longitude in deg.</param>
        /// <returns></returns>
        public static double greatCircleDistance(double lat1, double lon1,
            double lat2, double lon2) {
            double slon, slat, a, c, d;

            // Convert to radians
            lat1 *= DEG2RAD;
            lon1 *= DEG2RAD;
            lat2 *= DEG2RAD;
            lon2 *= DEG2RAD;

            // Haversine formula
            slon = Math.Sin((lon2 - lon1) / 2.0);
            slat = Math.Sin((lat2 - lat1) / 2.0);
            a = slat * slat + Math.Cos(lat1) * Math.Cos(lat2) * slon * slon;
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            d = REARTH / M2MI * c;

            return (d);
        }
    }

    public class LatLon {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Distance { get; set; }
        public DateTime Time { get; set; }

        public LatLon(double lat, double lon, double distance) {
            Lat = lat;
            Lon = lon;
            Distance = distance;
        }

        public LatLon(double lat, double lon, double distance, DateTime time) {
            Lat = lat;
            Lon = lon;
            Distance = distance;
            Time = time;
        }
    }

    public class TcxResult {
        public TrainingCenterDatabase TCX { get; set; }
        public string Message { get; set; }

        public TcxResult(TrainingCenterDatabase tcx, string message) {
            this.TCX = tcx;
            this.Message = message;
        }
    }

    public class GpxResult {
        public gpx GPX { get; set; }
        public string Message { get; set; }

        public GpxResult(gpx gpx, string message) {
            this.GPX = gpx;
            this.Message = message;
        }
    }
}