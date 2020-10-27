# GPX Viewer

GPX Viewer is a C# Windows application that allows you to examine and edit GPS Exchange Format (GPX) files through a graphical user interface. You can open multiple files, which will be shown in a tree view. You can expand the tree to see the tracks, waypoints, and routes, and further to see their sub-elements. You can send selected files to Google Earth to view the tracks, waypoints, and routes there. You can edit by using Cut, Copy, and Paste on the elements of the tree, and you can also edit the XML directly for each element.

See https://kenevans.net/opensource/GPXViewer/Help/Overview.html

**GpsUtils**

GpsUtils is a class library that provides a number of utilities for dealing with GPX and Training Center Database (TCX) files. It is used to provide the GPX / TCX menu in GPX Viewer and is also used in Exercise Analyzer. See https://github.com/KennethEvans/VS-Exercise-Analyzer. 

**Installation**

If you are installing GPX Viewer from a download, just unzip the files into a directory somewhere convenient. Then run it from there. If you are installing from a build, copy all the files and directories from the bin/Release directory to a convenient directory.

To uninstall, just delete these files.

**Development**

GPX Viewer uses the NuGet packages GeoTimeZone, LinqToXsd, Newtonsoft.Json, ObjectListView.Official, SharpKml.Core, TimeZoneConverter, and TimeZoneNames as well as the class library Utils.dll from https://github.com/KennethEvans/VS-Utils.

**More Information**

More information and FAQ are at https://kennethevans.github.io as well as more projects from the same author.

Licensed under the MIT license. (See: https://en.wikipedia.org/wiki/MIT_License)