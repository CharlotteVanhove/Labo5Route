using Labo5;
using Labo5.Exceptions;

namespace Labo5Route.Model
{
    public static class RouteFactory
    {
        //vraag hier om nieuwe routes aan te maken en xroute internal zetten 
        public static XRoute BuildRouteFromFile(string fileName)
        {
            try
            {
                var segmenten = new List<Segment>();
                SegmentLocation previousSegmentLocation = null;
                List<(string location, double distance, bool isStop)> routeData = new List<(string, double, bool)>(); // Gebruik van Tuples

                string[] lines = File.ReadAllLines(fileName); // Lees alle regels van het bestand
                int counter = 0;
                foreach (var line in lines)
                {
                    string[] parts;
                    if (fileName.EndsWith("data1.txt"))
                    {
                        // Voor het eerste bestandsformaat (data1.txt)
                        parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            string locationName = parts[0].Trim();
                            double distance = double.Parse(parts[1]);
                            bool isStop = bool.Parse(parts[2]);

                            routeData.Add((locationName, distance, isStop));
                        }
                    }
                    else if (fileName.EndsWith("data2.txt"))
                    {
                        // Voor het tweede bestandsformaat (data2.txt)
                        parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            string location1 = parts[0].Split('(')[0].Trim();
                            string location2 = parts[1].Split('(')[0].Trim();
                            double distance = double.Parse(parts[2]);

                            bool isStop1 = parts[0].Contains("stop");
                            bool isStop2 = parts[1].Contains("stop");

                            if (counter == 0)
                            {
                                // we voegen hier enkel de eerste locatie toe van op de eerste lijn
                                // voor de andere lijnen voegen we gewoon altijd de tweede locatie toe op de lijn
                                routeData.Add((location1, 0, isStop1));  // Voeg startlocatie toe
                            }

                            routeData.Add((location2, distance, isStop2)); // Voeg eindlocatie met afstand toe
                        }
                    }

                    counter++;
                }

                foreach (var (location, distance, isStop) in routeData)
                {
                    var segmentLocation = new SegmentLocation(location, isStop);

                    if (previousSegmentLocation != null)
                    {
                        segmenten.Add(new Segment(previousSegmentLocation, segmentLocation, new Distance(distance)));
                    }

                    previousSegmentLocation = segmentLocation;
                }

                return new XRoute(segmenten); ;
            }
            catch (FileNotFoundException ex)
            {
                throw new RouteException($"File {fileName} niet gevonden.", ex);
            }
            catch (Exception ex)
            {
                throw new RouteException("BuildRouteFromFile.", ex);
            }
        }

        public static XRoute BuildRoute(List<string> locations, List<bool> stops, List<double> distances)
        {
            var segmenten = new List<Segment>();

            SegmentLocation? previousSegmentLocation = null;

            for (int i = 0; i < locations.Count; i++)
            {
                var segmentLocation = new SegmentLocation(locations[i], stops[i]);
                if (previousSegmentLocation != null)
                {
                    segmenten.Add(new Segment(previousSegmentLocation, segmentLocation, new Distance(distances[i])));
                }
                previousSegmentLocation = segmentLocation;
            }

            return new XRoute(segmenten);
        }

        public static XRoute ReverseRoute(XRoute route)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route), "Input route cannot be null.");
            }
            //todo ask thibault. mag je private list aanspreken

            // Retrieve the full route information
            var (start, segments) = route.ShowFullRoute();
            var allLocations = new List<string> { start };
            foreach (var segment in segments)
            {
                allLocations.Add(segment.location);
            }

            // Retrieve stop statuses
            var stops = route.ShowStops();
            var locationIsStop = allLocations.ToDictionary(
                loc => loc,
                loc => stops.Contains(loc)
            );

            // Reverse the locations and distances
            var reversedLocations = allLocations.AsEnumerable().Reverse().ToList();
            var reversedDistances = segments.AsEnumerable().Reverse().Select(seg => seg.distance).ToList();

            // Initialize a new reversed route
            var segmenten = new List<Segment>();

            // Add the first location with distance 0
            string newStart = reversedLocations[0];
            bool isStop = locationIsStop[newStart];
            var firstSementLocation = new SegmentLocation(newStart, isStop);

            // Add the rest of the locations with corresponding distances and stop statuses
            for (int i = 0; i < reversedDistances.Count; i++)
            {
                string location = reversedLocations[i + 1];
                double distance = reversedDistances[i];
                bool isStopLocation = locationIsStop[location];
                segmenten.Add(new Segment(firstSementLocation, new SegmentLocation(location, isStopLocation), new Distance(distance)));
            }

            return new XRoute(segmenten);
        }

    }
}
