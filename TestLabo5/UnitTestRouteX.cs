using Labo5.Exceptions;
using Labo5Route.Model;

namespace TestLabo5
{
    public class UnitTestRouteX
    {
        [Fact]
        public void AddLocation_ShouldAddLocation()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 5, true);
            route.AddLocation("B", 10, false);

            Assert.True(route.HasLocation("A"));
            Assert.True(route.HasLocation("B"));
        }

        [Fact]
        public void GetDistance_ShouldReturnTotalDistance()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 5, true);
            route.AddLocation("B", 10, false);

            Assert.Equal(10, route.GetDistance());
        }

        [Fact]
        public void GetDistanceBetweenStops_ShouldGetDistance()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 0, true);
            route.AddLocation("B", 5, true);
            route.AddLocation("C", 10, true);
            route.AddLocation("D", 15, true);

            var distance = route.GetDistance("C", "D");

            Assert.Equal(15, distance);

            distance = route.GetDistance("A", "D");

            Assert.Equal(30, distance);
        }

        [Fact]
        public void ThrowException_WhenCalculateWrongDistance()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 0, true);
            route.AddLocation("B", 5, true);
            route.AddLocation("C", 10, true);
            route.AddLocation("D", 15, true);

            Assert.Throws<RouteException>(() => route.GetDistance("D", "A"));
        }
        
        [Fact]
        public void AddLocationWithoutCapitalLetter_ShouldThrowException()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());

            Assert.Throws<RouteException>(() => route.AddLocation("waregem", 0, true)); // Verifieren dat de exception gethrowd wordt
        }

        [Fact]
        public void AddNullLocation_ShouldThrowException()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());

            Assert.Throws<RouteException>(() => route.AddLocation(null, 0, true)); // Verifieren dat de exception gethrowd wordt
        }

        [Fact]
        public void AddEmptylLocation_ShouldThrowException()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());

            Assert.Throws<RouteException>(() => route.AddLocation(string.Empty, 0, true)); // Verifieren dat de exception gethrowd wordt
        }

        [Fact]
        public void ShowFullRoute_ShouldReturnFullRoute()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 0, true);
            route.AddLocation("B", 5, true);
            route.AddLocation("C", 10, true);
            route.AddLocation("D", 15, true);

            var result = route.ShowFullRoute();

            Assert.Equal("A", result.start);
            Assert.Equal(3, result.Item2.Count);
            Assert.Equal((5, "B"), result.Item2[0]);
            Assert.Equal((10, "C"), result.Item2[1]);
            Assert.Equal((15, "D"), result.Item2[2]);
        }

        [Fact]
        public void ShowFullRouteWithParams_ShouldReturnPartOfRoute()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 0, true);
            route.AddLocation("B", 5, true);
            route.AddLocation("C", 10, true);
            route.AddLocation("D", 15, true);
            route.AddLocation("E", 5, true);

            var result = route.ShowFullRoute("B", "D");

            Assert.Equal("B", result.start);
            Assert.Equal(2, result.Item2.Count);
            Assert.Equal((10, "C"), result.Item2[0]);
            Assert.Equal((15, "D"), result.Item2[1]);
        }

        [Fact]
        public void ShowRouteWithoutStops_ShouldReturnFullRoute()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 0, false);
            route.AddLocation("B", 5, false);
            route.AddLocation("C", 10, true);
            route.AddLocation("D", 15, false);
            route.AddLocation("E", 5, false);

            var result = route.ShowRoute();

            Assert.Equal("A", result.start);
            Assert.Equal(3, result.Item2.Count);
            Assert.Equal((5, "B"), result.Item2[0]);
            Assert.Equal((25, "D"), result.Item2[1]);
            Assert.Equal((5, "E"), result.Item2[2]);
        }

        [Fact]
        public void ShowRouteRouteWithoutStopsWithParams_ShouldReturnPartOfRoute()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 0, false);
            route.AddLocation("B", 5, false);
            route.AddLocation("C", 10, true);
            route.AddLocation("D", 15, false);
            route.AddLocation("E", 5, false);

            var result = route.ShowRoute("B", "E");

            Assert.Equal("B", result.start);
            Assert.Equal(2, result.Item2.Count);
            Assert.Equal((25, "D"), result.Item2[0]);
            Assert.Equal((5, "E"), result.Item2[1]);
        }

        [Fact]
        public void ShowLocations_ShouldReturnUniqueLocations()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 0, true);
            route.AddLocation("B", 5, true);
            route.AddLocation("C", 10, true);
            route.AddLocation("D", 15, true);

            var locations = route.ShowLocations();

            Assert.Equal(4, locations.Count);
            Assert.Contains("A", locations);
            Assert.Contains("B", locations);
            Assert.Contains("C", locations);
            Assert.Contains("D", locations);
        }

        [Fact]
        public void ShowStops_ShouldShowStops()
        {
            var route = RouteFactory.BuildRoute(new List<string>(), new List<bool>(), new List<double>());
            route.AddLocation("A", 0, false);
            route.AddLocation("B", 5, true);
            route.AddLocation("C", 10, false);
            route.AddLocation("D", 15, true);
            route.AddLocation("E", 15, true);

            var stops = route.ShowStops();

            Assert.Equal(3, stops.Count);

            Assert.Contains("B", stops);
            Assert.Contains("D", stops);
            Assert.Contains("E", stops);
        }
    }
}