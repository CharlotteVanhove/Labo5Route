using Labo5.Exceptions;
using Labo5.Interfaces;
using Labo5Route.Model;

namespace Labo5
{
    //eerste segement noet oplsaan, insert kan niet vooraan invoegen 
    public class XRoute : IRoute
    {
        private List<Segment> _segments = new List<Segment>();
        private SegmentLocation _firstTempSegmentLocation = null;

        // XRoute zelf is toegankelijk buiten de assembly,
        // maar gebruikers buiten de assembly kunnen geen instantie van deze klasse aanmaken,
        // omdat de constructor internal is.
        // Dit betekent dat je de methoden en properties van XRoute nog steeds kunt gebruiken buiten de assembly,
        // als je bijvoorbeeld een XRoute-instantie ontvangt van een andere klasse zoals RouteFactory.
        internal XRoute() { }

        public void AddLocation(string location, double distance, bool isStop)
        {
            var segmentLocation = new SegmentLocation(location, isStop);

            if (_segments.Count == 0 && _firstTempSegmentLocation == null)
            {
                // Eerste locatie
                _firstTempSegmentLocation = segmentLocation;
            }
            else if (_segments.Count == 0 && _firstTempSegmentLocation != null)
            {
                _segments.Add(new Segment(_firstTempSegmentLocation, segmentLocation, new Distance(distance)));
            }
            else
            {
                // Toevoegen van nieuwe locatie
                var lastSegment = _segments.Last();
                var newDistance = new Distance(distance);
                _segments.Add(new Segment(lastSegment.End, segmentLocation, newDistance));
            }
        }

        public double GetDistance()
        {
            double totalDistance = 0;
            foreach (var loc in _segments)
            {
                totalDistance += loc.Distance.Value;
            }
            return totalDistance;
        }

        public double GetDistance(string startLocation, string endLocation)
        {
            int startIndex = -1, endIndex = -1;

            for (int i = 0; i < _segments.Count; i++)
            {
                if (_segments[i].Start.Name == startLocation)
                {
                    startIndex = i;
                }

                if (_segments[i].End.Name == endLocation)
                {
                    endIndex = i;
                    break;
                }
            }

            if (startIndex == -1 || endIndex == -1 || startIndex > endIndex)
            {
                throw new RouteException("GetDistance");
            }

            double totalDistance = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                totalDistance += _segments[i].Distance.Value;
            }

            return totalDistance;
        }

        public bool HasLocation(string location) 
        {
            foreach (var segment in _segments)
            {
                if (segment.Start.Name == location ||
                    segment.End.Name == location)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasStop(string location) // is stop of doorrijdplaats? true or false
        {
            foreach (var segment in _segments)
            {
                if (segment.Start.Name == location)
                {
                    return segment.Start.IsStop;
                }

                if (segment.End.Name == location)
                {
                    return segment.End.IsStop;
                }
            }
            return false;
        }

        public void InsertLocation(string location, double distance, string fromLocation, bool isStop)  
        {
            var segmentLocation = new SegmentLocation(location, isStop);

            var fromSegment = _segments.FirstOrDefault(s => s.End.Name == fromLocation);
            if (fromSegment == null)
            {
                throw new RouteException("Start location niet gevonden");
            }

            var remainingDistance = fromSegment.Distance.Value - distance;
            if (remainingDistance < 0)
            {
                throw new RouteException("ongeldige afstand");
            }

            // Maak het nieuwe segment en pas de bestaande segmenten aan
            _segments.Insert(_segments.IndexOf(fromSegment) + 1, new Segment(fromSegment.End, segmentLocation, new Distance(distance)));
            fromSegment = new Segment(fromSegment.Start, fromSegment.End, new Distance(remainingDistance));
        }

        public (string start, List<(double distance, string location)>) ShowFullRoute()
        {
            // Controleer of er segmenten zijn
            if (_segments.Count == 0)
            {
                throw new RouteException("ShowFullRoute");
            }

            // De startlocatie is de start van het eerste segment
            string startLocation = _segments[0].Start.Name;
            var routeSegments = new List<(double distance, string location)>();

            // Loop door alle segmenten en verzamel de afstanden en eindlocaties
            foreach (var segment in _segments)
            {
                routeSegments.Add((segment.Distance.Value, segment.End.Name));
            }

            // Retourneer de startlocatie en de volledige route
            return (startLocation, routeSegments);
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
                    routeSegments.Add((segment.Distance.Value, segment.End.Name));

                    // Voeg de afstand van het huidige segment toe
                    totalDistance += segment.Distance.Value;

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
        }

        public List<string> ShowLocations()
        {
            var locations = new HashSet<string>();

            // Loop door alle segmenten en voeg start- en eindlocaties toe aan de HashSet
            foreach (var segment in _segments)
            {
                locations.Add(segment.Start.Name); // Voeg de startlocatie toe
                locations.Add(segment.End.Name);   // Voeg de eindlocatie toe
            }

            // Converteer de HashSet naar een List en retourneer deze
            return locations.ToList();


        }

        public (string start, List<(double distance, string location)>) ShowRoute() //allee afstanden stops mogen er niet bij  
        {
            // Controleer of er segmenten zijn
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
                // Voeg alleen segmenten toe als de eindlocatie geen stopplaats is
                if (segment.End.IsStop)
                {
                    // als het toch een stopplaats is, dan willen we de afgelegde afstand wel bijhouden
                    stopDistance += segment.Distance.Value;
                }
                else
                {
                    routeSegments.Add((segment.Distance.Value + stopDistance, segment.End.Name));
                    stopDistance = 0;
                }
            }

            // Retourneer de startlocatie en de gefilterde route zonder stops
            return (startLocation, routeSegments);
        }

        public (string start, List<(double distance, string location)>) ShowRoute(string startLocation, string endLocation) // toon afstanden tussen bepaalde afstanden
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

                    // Voeg alleen segmenten toe als de eindlocatie geen stopplaats is
                    if (segment.End.IsStop)
                    {
                        // als het toch een stopplaats is, dan willen we de afgelegde afstand wel bijhouden
                        stopDistance += segment.Distance.Value;
                    }
                    else
                    {
                        routeSegments.Add((segment.Distance.Value + stopDistance, segment.End.Name));
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
            var stops = new HashSet<string>();

            // Loop door alle segmenten
            foreach (var segment in _segments)
            {
                // Voeg de startlocatie toe als het een stopplaats is
                if (segment.Start.IsStop)
                {
                    stops.Add(segment.Start.Name);
                }

                // Voeg de eindlocatie toe als het een stopplaats is
                if (segment.End.IsStop)
                {
                    stops.Add(segment.End.Name);
                }
            }

            // Converteer de HashSet naar een List en retourneer deze
            return stops.ToList();
        }


    }
}
