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
                var route = new XRoute();
                List<(string location, double distance, bool isStop)> routeData = new List<(string, double, bool)>(); // Gebruik van Tuples

                string[] lines = File.ReadAllLines(fileName); // Lees alle regels van het bestand

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

                            bool isStop = parts[1].Contains("stop");

                            routeData.Add((location1, 0, true));  // Voeg startlocatie toe
                            routeData.Add((location2, distance, isStop)); // Voeg eindlocatie met afstand toe
                        }
                    }
                }
                foreach (var (location, distance, isStop) in routeData)
                {
                    route.AddLocation(location, distance, isStop);
                }
                return route;
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
            var route = new XRoute();
            for (int i = 0; i < locations.Count; i++)
            {
                route.AddLocation(locations[i], distances[i], stops[i]);
            }
            return route;
        }

        public static XRoute ReverseRoute(XRoute route)
        {
            var reversedRoute = new XRoute();
            var locations = route.ShowLocations();
            var stops = route.ShowStops();
            var distances = new List<double>();

            for (int i = locations.Count - 1; i > 0; i--)
            {
                distances.Add(route.GetDistance(locations[i], locations[i - 1]));
            }

            reversedRoute = BuildRoute(locations, stops.ConvertAll(x => route.HasStop(x)), distances);
            return reversedRoute;
        }

    }
}
