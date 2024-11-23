using Labo5.Exceptions;
using Labo5.Interfaces;
using Labo5Route.Model;

namespace Labo5
{
    //eerste segement niet oplsaan, insert kan niet vooraan invoegen 
    public class XRoute : IRoute
    {
        private readonly List<Segment> _segments;

        // XRoute zelf is toegankelijk buiten de assembly,
        // maar gebruikers buiten de assembly kunnen geen instantie van deze klasse aanmaken,omdat de constructor internal is.
        // Dit betekent dat je de methoden en properties van XRoute nog steeds kunt gebruiken buiten de assembly,
        // als je bijvoorbeeld een XRoute-instantie ontvangt van een andere klasse zoals RouteFactory.
        internal XRoute(List<Segment> segmenten)
        {
            if (segmenten == null || !segmenten.Any())
            {
                throw new RouteException("Minstens 1 segment meegeven");
            }

            _segments = segmenten;
        }

        public void AddLocation(string location, double distance, bool isStop)
        {
            var segmentLocation = new SegmentLocation(location, isStop);

            // Toevoegen van nieuwe locatie
            var lastSegment = _segments.Last();
            var newDistance = new Distance(distance);
            _segments.Add(new Segment(lastSegment.End, segmentLocation, newDistance));
        }

        public double GetDistance()
        {
            //double totalDistance = 0;
            //foreach (var loc in _segments)
            //{
            //    totalDistance += loc.Distance.AfstandInKM;
            //}
            //return totalDistance;

            //zelfde met LINQ
            return _segments.Sum(segment => segment.Distance.AfstandInKM);
        }

        public double GetDistance(string startLocation, string endLocation)
        {
            //int startIndex = -1, endIndex = -1;

            //for (int i = 0; i < _segments.Count; i++)
            //{
            //    if (_segments[i].Start.Name == startLocation)
            //    {
            //        startIndex = i;
            //    }

            //    if (_segments[i].End.Name == endLocation)
            //    {
            //        endIndex = i;
            //        break;
            //    }
            //}

            //if (startIndex == -1 || endIndex == -1 || startIndex > endIndex)
            //{
            //    throw new RouteException("GetDistance");
            //}

            //double totalDistance = 0;
            //for (int i = startIndex; i <= endIndex; i++)
            //{
            //    totalDistance += _segments[i].Distance.AfstandInKM;
            //}
            //return totalDistance;

            //Met LINQ
            var range = _segments
            .SkipWhile(s => s.Start.Name != startLocation)
            .TakeWhile(s => s.End.Name != endLocation)
            .Concat(_segments.Where(s => s.End.Name == endLocation).Take(1)) // Include segment with end location
            .ToList();
            if (!range.Any() || range.First().Start.Name != startLocation || range.Last().End.Name != endLocation)
            {
                throw new RouteException("GetDistance");
            }
            return range.Sum(segment => segment.Distance.AfstandInKM);
        }

        public bool HasLocation(string location)
        {
            //foreach (var segment in _segments)
            //{
            //    if (segment.Start.Name == location ||
            //        segment.End.Name == location)
            //    {
            //        return true;
            //    }
            //}
            //return false;

            return _segments.Any(s => s.Start.Name == location || s.End.Name == location);

        }

        public bool HasStop(string location) // is stop of doorrijdplaats? true or false
        {
            //foreach (var segment in _segments)
            //{
            //    if (segment.Start.Name == location)
            //    {
            //        return segment.Start.IsStop;
            //    }

            //    if (segment.End.Name == location)
            //    {
            //        return segment.End.IsStop;
            //    }
            //}
            //return false;

            return _segments
           .Where(s => s.Start.Name == location || s.End.Name == location)
           .Select(s => s.Start.Name == location ? s.Start.IsStop : s.End.IsStop)
           .FirstOrDefault();
        }

        public void InsertLocation(string location, double distance, string fromLocation, bool isStop) //todo enkel mogelijk als er 2 segmenten zijn 
        {
            // Check if the fromLocation exists in the route
            bool fromLocationExists = HasLocation(fromLocation);
            if (!fromLocationExists)
            {
                throw new RouteException($"FromLocation '{fromLocation}' not found in the route.");
            }

            // Create the new SegmentLocation
            var newSegmentLocation = new SegmentLocation(location, isStop);

            // Find the segment where fromLocation is the start
            var fromSegment = _segments.FirstOrDefault(s => s.Start.Name == fromLocation);

            // Check if the distance to insert is valid
            if (distance >= fromSegment.Distance.AfstandInKM)
            {
                throw new RouteException("The specified distance exceeds or equals the existing segment's distance.");
            }

            // Calculate the remaining distance after inserting the new location
            double remainingDistance = fromSegment.Distance.AfstandInKM - distance;

            // Create the new segments
            var newSegment = new Segment(fromSegment.Start, newSegmentLocation, new Distance(distance));
            var adjustedSegment = new Segment(newSegmentLocation, fromSegment.End, new Distance(remainingDistance));

            // Find the index of the original segment
            int fromSegmentIndex = _segments.IndexOf(fromSegment);

            // Replace the original segment with the new segments
            _segments[fromSegmentIndex] = newSegment; // Replace with the segment to the new location
            _segments.Insert(fromSegmentIndex + 1, adjustedSegment); // Insert the adjusted segment

        }

        public void RemoveLocation(string location) 
        {
            // Check if the location exists in the route
            if (!HasLocation(location))
            {
                throw new RouteException($"Location '{location}' not found in the route.");
            }

            // Find the segment where the location is the end
            //todo : waarom niet gewoon (s => s.End.Name == location)
            var segmentBefore = _segments.FirstOrDefault(s => s.End.Name.Equals(location, StringComparison.OrdinalIgnoreCase));
            // Find the segment where the location is the start
            var segmentAfter = _segments.FirstOrDefault(s => s.Start.Name.Equals(location, StringComparison.OrdinalIgnoreCase));

            if (segmentBefore != null && segmentAfter != null)
            {
                // The location is a middle point; merge the two surrounding segments

                // Calculate the new distance by summing the distances of the two segments
                double newDistance = segmentBefore.Distance.AfstandInKM + segmentAfter.Distance.AfstandInKM;

                // Create a new segment from the start of segmentBefore to the end of segmentAfter
                var newSegment = new Segment(segmentBefore.Start, segmentAfter.End, new Distance(newDistance));

                // Find the indices of the existing segments
                int indexBefore = _segments.IndexOf(segmentBefore);
                int indexAfter = _segments.IndexOf(segmentAfter);

                // Remove the old segments
                // Remove the segment after first to avoid shifting the index
                _segments.RemoveAt(indexAfter);
                _segments.RemoveAt(indexBefore);

                // Insert the new merged segment at the position of the first removed segment
                _segments.Insert(indexBefore, newSegment);
            }
            else if (segmentBefore != null)
            {
                // The location is at the end of the route; remove the last segment
                _segments.Remove(segmentBefore);
            }
            else if (segmentAfter != null)
            {
                // The location is at the start of the route; remove the first segment
                _segments.Remove(segmentAfter);
            }
            else
            {
                // The location is isolated and cannot be removed without breaking the route
                throw new RouteException($"Cannot remove location '{location}' as it does not connect to other segments.");
            }
            if (_segments.Count == 0)
            {
                throw new RouteException("Cannot remove the last segment.");
            }
        }

        public void SetDistance(double distance, string location1, string location2) //distance aanpassen tussen 2 locaties/ update segment
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                var segment = _segments[i];
                if (segment.Start.Name == location1 && segment.End.Name == location2)
                {
                    _segments[i] = new Segment(segment.Start, segment.End, new Distance(distance));
                }
            }
        }

        public (string start, List<(double distance, string location)>) ShowFullRoute()
        {
            //// Controleer of er segmenten zijn
            //if (_segments.Count == 0)
            //{
            //    throw new RouteException("ShowFullRoute");
            //}

            //// De startlocatie is de start van het eerste segment
            //string startLocation = _segments[0].Start.Name;
            //var routeSegments = new List<(double distance, string location)>();

            //// Loop door alle segmenten en verzamel de afstanden en eindlocaties
            //foreach (var segment in _segments)
            //{
            //    routeSegments.Add((segment.Distance.AfstandInKM, segment.End.Name));
            //}

            //// Retourneer de startlocatie en de volledige route
            //return (startLocation, routeSegments);

            //linq
            if (!_segments.Any())
            {
                throw new RouteException("ShowFullRoute");
            }

            var route = _segments.Select(s => (s.Distance.AfstandInKM, s.End.Name)).ToList();
            return (_segments.First().Start.Name, route);
        }

        public (string start, List<(double distance, string location)>) ShowFullRoute(string startLocation, string endLocation)
        {
            // Zoek het segment waar de startlocatie overeenkomt
            var routeSegments = new List<(double distance, string location)>();
            bool startFound = false;
            double totalDistance = 0;

            foreach (var segment in _segments)
            {
                // Controleer of de startlocatie van het segment overeenkomt
                if (segment.Start.Name == startLocation || startFound)
                {
                    // Markeer dat we de startlocatie hebben gevonden
                    startFound = true;

                    // Voeg de startlocatie en afstand toe aan de lijst
                    routeSegments.Add((segment.Distance.AfstandInKM, segment.End.Name));

                    // Voeg de afstand van het huidige segment toe
                    totalDistance += segment.Distance.AfstandInKM;

                    // Stop met zoeken wanneer de eindlocatie is gevonden
                    if (segment.End.Name == endLocation)
                    {
                        break;
                    }
                }
            }

            // Als de start- of eindlocatie niet is gevonden, gooien we een exception
            if (!startFound || routeSegments.Count == 0)
            {
                throw new RouteException("ShowFullRoute");
            }

            // Retourneer de startlocatie samen met de lijst van afstanden en locaties
            return (startLocation, routeSegments);

            ////todo linq klopt nog niet 
            //var route = _segments
            //.SkipWhile(s => s.Start.Name != startLocation)
            //.TakeWhile(s => s.End.Name != endLocation)
            //.Concat(_segments.Where(s => s.End.Name == endLocation).Take(1)) // Include segment with end location
            //.Select(s => (s.Distance.AfstandInKM, s.End.Name))
            //.ToList();

            //if (!route.Any() || route.First().Item2 != startLocation || route.Last().Item2 != endLocation)
            //{
            //    throw new RouteException("ShowFullRoute");
            //}

            //return (startLocation, route);
        }

        public List<string> ShowLocations()
        {
            ////hashset zodat de locaties niet dubbel in de lijst komen
            //var locations = new HashSet<string>();

            //// Loop door alle segmenten en voeg start- en eindlocaties toe aan de HashSet
            //foreach (var segment in _segments)
            //{
            //    locations.Add(segment.Start.Name); // Voeg de startlocatie toe
            //    locations.Add(segment.End.Name);   // Voeg de eindlocatie toe
            //}

            //// Converteer de HashSet naar een List en retourneer deze
            //return locations.ToList();

            return _segments
            .SelectMany(s => new[] { s.Start.Name, s.End.Name })
            .Distinct()
            .ToList();
        }

        public (string start, List<(double distance, string location)>) ShowRoute() //toont route met enkel de stops 
        {

            // Controleer of er segmenten zijn
            // of (!_segments.Any())
            if (_segments.Count == 0)
            {
                throw new RouteException("ShowRoute");
            }

            // Startlocatie van het eerste segment
            string startLocation = _segments[0].Start.Name;
            var routeSegments = new List<(double distance, string location)>();

            // Afstand bijhouden in volgende variabele voor wanneer we een stop passeren en doorrijden
            double stopDistance = 0;

            // Loop door alle segmenten
            foreach (var segment in _segments)
            {
                // Voeg alleen segmenten toe als de eindlocatie een stopplaats is
                if (!segment.End.IsStop)
                {
                    // als het toch geen stopplaats is, dan willen we de afgelegde afstand wel bijhouden
                    stopDistance += segment.Distance.AfstandInKM;
                }
                else
                {
                    routeSegments.Add((segment.Distance.AfstandInKM + stopDistance, segment.End.Name));
                    stopDistance = 0;
                }
            }

            // Retourneer de startlocatie en de gefilterde route met stops
            return (startLocation, routeSegments);
        }

        public (string start, List<(double distance, string location)>) ShowRoute(string startLocation, string endLocation) // toon stops tussen bepaalde locaties
        {
            // Controleer of er segmenten zijn
            if (_segments.Count == 0)
            {
                throw new RouteException("ShowRoute");
            }

            var routeSegments = new List<(double distance, string location)>();
            bool startFound = false;

            // Afstand bijhouden in volgende variabele voor wanneer we een stop passeren en doorrijden
            double stopDistance = 0;

            // Loop door alle segmenten om de route tussen startLocation en endLocation te verzamelen
            foreach (var segment in _segments)
            {
                // Check of we bij de startlocatie zijn aangekomen
                if (segment.Start.Name == startLocation || startFound)
                {
                    // Markeer dat we de startlocatie hebben gevonden
                    startFound = true;

                    // Voeg alleen segmenten toe als de eindlocatie een stopplaats is
                    if (!segment.End.IsStop)
                    {
                        // als het toch geen stopplaats is, dan willen we de afgelegde afstand wel bijhouden
                        stopDistance += segment.Distance.AfstandInKM;
                    }
                    else
                    {
                        routeSegments.Add((segment.Distance.AfstandInKM + stopDistance, segment.End.Name));
                        stopDistance = 0;
                    }

                    // Stop met zoeken zodra we de eindlocatie hebben bereikt
                    if (segment.End.Name == endLocation)
                    {
                        break;
                    }
                }
            }
            // Als de start- of eindlocatie niet is gevonden, gooi een RouteException [^1] = laatste element van achteren
            //routeSegments[routeSegments.Count - 1] is hetzelfde
            if (!startFound || routeSegments.Count == 0 || routeSegments[^1].location != endLocation)
            {
                throw new RouteException("ShowRoute");
            }

            // Retourneer de startlocatie en de tussenliggende route
            return (startLocation, routeSegments);
        }

        public List<string> ShowStops()
        {
            //var stops = new HashSet<string>();

            //// Loop door alle segmenten
            //foreach (var segment in _segments)
            //{
            //    // Voeg de startlocatie toe als het een stopplaats is
            //    if (segment.Start.IsStop)
            //    {
            //        stops.Add(segment.Start.Name);
            //    }

            //    // Voeg de eindlocatie toe als het een stopplaats is
            //    if (segment.End.IsStop)
            //    {
            //        stops.Add(segment.End.Name);
            //    }
            //}
            //// Converteer de HashSet naar een List en retourneer deze
            //return stops.ToList();

            //LINQ
            return _segments
            .SelectMany(s => new[] { s.Start, s.End })
            .Where(location => location.IsStop)
            .Select(location => location.Name)
            .Distinct()
            .ToList();
        }

        public void UpdateLocation(string location, string newName, bool isStop)
        {
            for (var i = 0; i < _segments.Count; i++)
            {
                var segment = _segments[i];

                if (segment.Start.Name == location)
                {
                    _segments[i] = new Segment(new SegmentLocation(newName, isStop), segment.End, segment.Distance);
                }

                if (segment.End.Name == location)
                {
                    _segments[i] = new Segment(segment.Start, new SegmentLocation(newName, isStop), segment.Distance);
                }
            }
        }
    }
}
