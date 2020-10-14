﻿using System;
using System.Collections.Generic;
using www.topografix.com.GPX_1_1;

namespace GPXViewer.model {
    public class GpxRouteModel : GpxModel {
        public rteType Route { get; set; }
        public List<GpxWaypointModel> Waypoints { get; set; }
        public GpxRouteModel(GpxModel parent, rteType rte) {
            Parent = parent;
            Route = rte;
            Waypoints = new List<GpxWaypointModel>();
            foreach(wptType wpt in rte.rtept) {
                Waypoints.Add(new GpxWaypointModel(this, wpt));
            }
        }
        public override string getLabel() {
            return Route.name != null ? Route.name : "Route";
        }
        public override void showInfo() {
            throw new NotImplementedException();
        }

        public override void delete() {
            if (Parent != null && Parent is GpxFileModel fileModel &&
            fileModel.Waypoints != null) {
                fileModel.Routes.Remove(this);
            }
            Route = null;
        }
    }
}
