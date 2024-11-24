using Labo5.Exceptions;
using Labo5Route.Model;

namespace TestLabo5
{
    public class UnitTestRouteX
    {
        [Fact]
        public void AddLocation_ShouldAddLocation()
        {
            var route = RouteFactory.BuildRoute(["Waregem", "Gent"], [true, true], [0, 25]);
            route.AddLocation("A", 5, true);
            route.AddLocation("B", 10, false);

            Assert.True(route.HasLocation("A"));
            Assert.True(route.HasLocation("B"));
        }

        [Fact]
        public void GetDistance_ShouldReturnTotalDistance()
        {
            var route = RouteFactory.BuildRoute(["Waregem", "Gent"], [true, true], [0, 25]);

            Assert.Equal(25, route.GetDistance());
        }

        [Fact]
        public void GetDistanceBetweenStops_ShouldGetDistance()
        {
            var route = RouteFactory.BuildRoute(["Kortrijk", "Waregem", "Gent", "Antwerpen"], [true, true, true,true], [0, 10, 25, 60]);

            var distance = route.GetDistance("Waregem", "Gent");

            Assert.Equal(25, distance);

            distance = route.GetDistance("Kortrijk", "Antwerpen");

            Assert.Equal(95, distance);
        }

        [Fact]
        public void ThrowException_WhenCalculateWrongDistance()
        {
            var route = RouteFactory.BuildRoute(["Kortrijk", "Waregem", "Gent", "Antwerpen"], [true, true, true, true], [0, 10, 25, 60]);

            Assert.Throws<RouteException>(() => route.GetDistance("Antwerpen", "Kortrijk"));
        }
        
        [Fact]
        public void AddLocationWithoutCapitalLetter_ShouldThrowException()
        {
            var route = RouteFactory.BuildRoute(["Waregem", "Gent"], [true, true], [0, 25]);

            Assert.Throws<RouteException>(() => route.AddLocation("antwerpen", 0, true)); // Verifieren dat de exception gethrowd wordt
        }

        [Fact]
        public void AddNullLocation_ShouldThrowException()
        {
            var route = RouteFactory.BuildRoute(["Waregem", "Gent"], [true, true], [0, 25]);

            Assert.Throws<RouteException>(() => route.AddLocation(null, 0, true)); // Verifieren dat de exception gethrowd wordt
        }

        [Fact]
        public void AddEmptylLocation_ShouldThrowException()
        {
            var route = RouteFactory.BuildRoute(["Waregem", "Gent"], [true, true], [0, 25]);

            Assert.Throws<RouteException>(() => route.AddLocation(string.Empty, 0, true)); // Verifieren dat de exception gethrowd wordt
        }

        [Fact]
        public void ShowFullRoute_ShouldReturnFullRoute()
        {
            var route = RouteFactory.BuildRoute(["A", "B", "C", "D"], [true, true, true, true], [0, 5, 10, 15]);

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
            var route = RouteFactory.BuildRoute(["A", "B", "C", "D", "E"], [true, true, true, true, true], [0, 5, 10, 15,5]);

            var result = route.ShowFullRoute("B", "D");

            Assert.Equal("B", result.start);
            Assert.Equal(2, result.Item2.Count);
            Assert.Equal((10, "C"), result.Item2[0]);
            Assert.Equal((15, "D"), result.Item2[1]);
        }

        [Fact]
        public void ShowRouteWithoutNonStops_ShouldReturnFullRoute()
        {
            var route = RouteFactory.BuildRoute(["A", "B", "C", "D", "E"], [true, true, true, true, true], [0, 5, 10, 15, 5]);

            var result = route.ShowRoute();

            Assert.Equal("A", result.start);
            Assert.Equal(4, result.Item2.Count);
            Assert.Equal((5, "B"), result.Item2[0]);
            Assert.Equal((10, "C"), result.Item2[1]);
            Assert.Equal((15, "D"), result.Item2[2]);
            Assert.Equal((5, "E"), result.Item2[3]);
        }

        [Fact]
        public void ShowRouteRouteWithoutNonStopsWithParams_ShouldReturnPartOfRoute()
        {
            var route = RouteFactory.BuildRoute(["A", "B", "C", "D", "E"], [true, true, true, true, true], [0, 5, 10, 15, 5]);

            var result = route.ShowRoute("B", "E");

            Assert.Equal("B", result.start);
            Assert.Equal(3, result.Item2.Count);
            Assert.Equal((10, "C"), result.Item2[0]);
            Assert.Equal((15, "D"), result.Item2[1]);
            Assert.Equal((5, "E"), result.Item2[2]);

        }

        [Fact]
        public void ShowLocations_ShouldReturnUniqueLocations()
        {
            var route = RouteFactory.BuildRoute(["A", "B", "C", "D"], [true, true, true, true], [0, 5, 10, 15]);

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
            var route = RouteFactory.BuildRoute(["A", "B", "C", "D", "E"], [false, true, false, true, true], [0, 5, 10, 15, 5]);

            var stops = route.ShowStops();

            Assert.Equal(3, stops.Count);

            Assert.Contains("B", stops);
            Assert.Contains("D", stops);
            Assert.Contains("E", stops);
        }

        [Fact]
        public void HasStop_ShouldReturnTrueForStop()
        {
            var route = RouteFactory.BuildRoute(["A", "B"], [false, true], [0, 5]);

            var result = route.HasStop("B");

            Assert.True(result);
        }

        [Fact]
        public void HasStop_ShouldReturnFalseForNonStop()
        {
            var route = RouteFactory.BuildRoute(["A", "B"], [false, true], [0, 5]);

            var result = route.HasStop("A");

            Assert.False(result);
        }

        [Fact]
        public void InsertLocation_ShouldInsertLocation()
        {

            var route = RouteFactory.BuildRoute(["A", "B", "C"], [false, false, false], [0, 5, 10]);

            route.InsertLocation("Q", 7, "B", true);

            // controleer afstand
            var totalDistance = route.GetDistance();
            Assert.Equal(15, totalDistance);

            // controleer volledige route
            var fullRoute = route.ShowFullRoute();
            Assert.Equal("A", fullRoute.start);

            var expectedSegments = new List<(double distance, string location)>
            {
                (5.0, "B"),
                (7.0, "Q"),
                (3.0, "C")
            };

            Assert.Equal(expectedSegments.Count, fullRoute.Item2.Count);

            for (int i = 0; i < expectedSegments.Count; i++)
            {
                Assert.Equal(expectedSegments[i].distance, fullRoute.Item2[i].distance);
                Assert.Equal(expectedSegments[i].location, fullRoute.Item2[i].location);
            }

            // check dat Q een stop is
            bool isQStop = route.HasStop("Q");
            Assert.True(isQStop);
        }

        [Fact]
        public void RemoveLocation_ShouldRemoveLocation()
        {
            var route = RouteFactory.BuildRoute(["A", "B", "C"], [false, false, false], [0, 5, 10]);

            route.RemoveLocation("B");

            // check afstand nog klopt
            double totalDistance = route.GetDistance();
            Assert.Equal(15.0, totalDistance);

            // check dat b weg is
            Assert.False(route.HasLocation("B"));

            var fullRoute = route.ShowFullRoute();
            Assert.Equal("A", fullRoute.start);

            var expectedSegments = new List<(double distance, string location)>
            {
                (15.0, "C")
            };

            Assert.Equal(expectedSegments.Count, fullRoute.Item2.Count);

            for (int i = 0; i < expectedSegments.Count; i++)
            {
                Assert.Equal(expectedSegments[i].distance, fullRoute.Item2[i].distance);
                Assert.Equal(expectedSegments[i].location, fullRoute.Item2[i].location);
            }
        }

        [Fact]
        public void SetDistance_ShouldUpdateDistance()
        {
            var route = RouteFactory.BuildRoute(["A", "B", "C"], [false, false, false], [0, 5, 10]);

            route.SetDistance(15, "B", "C");

            var fullRoute = route.ShowFullRoute();
            Assert.Equal("A", fullRoute.start);
            var expectedSegments = new List<(double distance, string location)>
            {
                (5.0, "B"),
                (15.0, "C")
            };

            Assert.Equal(expectedSegments.Count, fullRoute.Item2.Count);

            for (int i = 0; i < expectedSegments.Count; i++)
            {
                Assert.Equal(expectedSegments[i].distance, fullRoute.Item2[i].distance, precision: 2);
                Assert.Equal(expectedSegments[i].location, fullRoute.Item2[i].location);
            }

            double totalDistance = route.GetDistance();
            Assert.Equal(20.0, totalDistance);
        }

        [Fact]
        public void UpdateLocation_ShouldSetNewName()
        {
            var route = RouteFactory.BuildRoute(["A", "B", "C"], [false, false, false], [0, 5, 10]);

            route.UpdateLocation("B", "Q", true);

            var fullRoute = route.ShowFullRoute();
            Assert.Equal("A", fullRoute.start);

            var expectedSegments = new List<(double distance, string location)>
            {
                (5.0, "Q"),
                (10.0, "C")
            };

            Assert.Equal(expectedSegments.Count, fullRoute.Item2.Count);

            for (int i = 0; i < expectedSegments.Count; i++)
            {
                Assert.Equal(expectedSegments[i].distance, fullRoute.Item2[i].distance, precision: 2);
                Assert.Equal(expectedSegments[i].location, fullRoute.Item2[i].location);
            }
        }

        [Theory]
        [InlineData("A", true)]      // Correct: begint met een hoofdletter
        [InlineData("Location", true)] // Correct: begint met een hoofdletter
        [InlineData("location", false)] // Fout: begint niet met een hoofdletter
        [InlineData("", false)]       // Fout: naam is leeg
        [InlineData("  ", false)]     // Fout: naam is whitespace
        [InlineData("123", false)]    // Fout: begint met een cijfer
        [InlineData("A1", true)]      // Correct: begint met een hoofdletter, cijfer is toegestaan na de hoofdletter
        public void LocationName_ShouldBeValid(string name, bool isValid)
        {
            if (isValid)
            {
                var location = new SegmentLocation(name, true);

                Assert.Equal(name, location.Name); // Controleer of de naam correct is opgeslagen
            }
            else
            {
                Assert.Throws<RouteException>(() => new SegmentLocation(name, true)); // Controleer dat er een uitzondering wordt gegooid
            }
        }

        [Theory]
        [InlineData(3, true)]  // Geldige afstand
        [InlineData(10, true)] // Geldige afstand
        [InlineData(2, false)] // Ongeldig: gelijk aan minOpgegevenWaarde
        [InlineData(1, false)] // Ongeldig: kleiner dan minOpgegevenWaarde
        [InlineData(0, false)] // Ongeldig: kleiner dan minOpgegevenWaarde
        [InlineData(-5, false)] // Ongeldig: negatief
        public void Distance_ShouldBeValid(double value, bool isValid)
        {
            if (isValid)
            {
                var distance = new Distance(value);
                Assert.Equal(value, distance.AfstandInKM); // Controleer of de afstand correct is opgeslagen
            }
            else
            {
                Assert.Throws<RouteException>(() => new Distance(value)); // Controleer dat er een uitzondering wordt gegooid
            }
        }

        [Fact]
        public void Route_ShouldContainSpecificStop()
        {
            var route = RouteFactory.BuildRoute(["A", "B", "C", "D"], [true, false, true, false], [5.0, 10.0, 15.0, 20.0]);

            // Haal alle stoplocaties op
            var stops = route.ShowRoute().Item2.Select(segment => segment.location).ToList();

            // Controleer dat "B" niet aanwezig is en "C" wel aanwezig is
            Assert.Contains("C", stops);  // "C" moet een stop zijn
            Assert.DoesNotContain("B", stops); // "B" is geen stop
        }

        [Fact]
        public void LocationName_ShouldContainSubstring()
        {
            var location = new SegmentLocation("Amsterdam", true);

            // Controleer dat "Amster" in de locatienaam zit
            Assert.Contains("Amster", location.Name);

            // Controleer dat "Rotter" niet in de locatienaam zit
            Assert.DoesNotContain("Rotter", location.Name);
        }

    }
}