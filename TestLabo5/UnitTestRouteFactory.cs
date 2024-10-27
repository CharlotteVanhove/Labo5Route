using Labo5Route.Model;

namespace TestLabo5;

public class UnitTestRouteFactory
{
    [Fact]
    public void Data1BuildRouteFromFile()
    {
        var route = RouteFactory.BuildRouteFromFile("data1.txt");

        var fullRoute = route.ShowFullRoute();
        Assert.Equal("The Shire", fullRoute.start);
        var expectedSegments = new List<(double distance, string location)>
        {
            (25.0, "Bree"),
            (60.0, "Rivendel"),
            (33.0, "Edoras"),
            (5.0, "Helm's Deep"),
            (30.0, "Isengard"),
            (28.0, "Minas Tirith"),
            (8.0, "Minas Morgul"),
            (38.0, "Mount Doom")
        };

        Assert.Equal(expectedSegments.Count, fullRoute.Item2.Count);

        for (int i = 0; i < expectedSegments.Count; i++)
        {
            Assert.Equal(expectedSegments[i].distance, fullRoute.Item2[i].distance, precision: 2);
            Assert.Equal(expectedSegments[i].location, fullRoute.Item2[i].location);
        }

        // 3. Verify the total distance is 227 km
        double totalDistance = route.GetDistance();
        Assert.Equal(227.0, totalDistance);
    }

    [Fact]
    public void Data2BuildRouteFromFile()
    {
        var route = RouteFactory.BuildRouteFromFile("data2.txt");

        // 1. Verify the start location is "The Shire"
        var fullRoute = route.ShowFullRoute();
        Assert.Equal("The Shire", fullRoute.start);
        var expectedSegments = new List<(double distance, string location)>
        {
            (25.0, "Bree"),
            (60.0, "Rivendel"),
            (33.0, "Edoras"),
            (5.0, "Helm's Deep"),
            (30.0, "Isengard"),
            (28.0, "Minas Tirith"),
            (8.0, "Minas Morgul"),
            (38.0, "Mount Doom")
        };

        Assert.Equal(expectedSegments.Count, fullRoute.Item2.Count);

        for (int i = 0; i < expectedSegments.Count; i++)
        {
            Assert.Equal(expectedSegments[i].distance, fullRoute.Item2[i].distance, precision: 2);
            Assert.Equal(expectedSegments[i].location, fullRoute.Item2[i].location);
        }

        // 3. Verify the total distance is 227 km
        double totalDistance = route.GetDistance();
        Assert.Equal(227.0, totalDistance);
    }

    [Fact]
    public void BuildRouteShouldBuildRoute()
    {
        var route = RouteFactory.BuildRoute(["A", "B", "C"], [false, true, false], [0, 5, 10]);

        var fullRoute = route.ShowFullRoute();
        Assert.Equal("A", fullRoute.start);
        var expectedSegments = new List<(double distance, string location)>
            {
                (5.0, "B"),
                (10.0, "C")
            };

        Assert.Equal(expectedSegments.Count, fullRoute.Item2.Count);

        for (int i = 0; i < expectedSegments.Count; i++)
        {
            Assert.Equal(expectedSegments[i].distance, fullRoute.Item2[i].distance, precision: 2);
            Assert.Equal(expectedSegments[i].location, fullRoute.Item2[i].location);
        }

        double totalDistance = route.GetDistance();
        Assert.Equal(15.0, totalDistance);
    }

    [Fact]
    public void ReverseRoute_ShouldReturnCorrectReversedRoute()
    {
        // Arrange
        var locations = new List<string>
            {
                "A",
                "B",
                "C"
            };

        var isStops = new List<bool>
            {
                false, // A
                true,  // B
                false  // C
            };

        var distances = new List<double>
            {
                0,  // A (starting point)
                5,  // A → B
                10  // B → C
            };

        var originalRoute = RouteFactory.BuildRoute(locations, isStops, distances);

        // Act
        var reversedRoute = RouteFactory.ReverseRoute(originalRoute);

        // Assert

        // 1. Verify the start location of the reversed route is "C"
        var reversedFullRoute = reversedRoute.ShowFullRoute();
        Assert.Equal("C", reversedFullRoute.start);

        // 2. Verify the segments of the reversed route
        // Expected segments:
        // C –(10 km) – B
        // B –(5 km) – A
        var expectedReversedSegments = new List<(double distance, string location)>
            {
                (10.0, "B"),
                (5.0, "A")
            };

        Assert.Equal(expectedReversedSegments.Count, reversedFullRoute.Item2.Count);

        for (int i = 0; i < expectedReversedSegments.Count; i++)
        {
            Assert.Equal(expectedReversedSegments[i].distance, reversedFullRoute.Item2[i].distance, precision: 2);
            Assert.Equal(expectedReversedSegments[i].location, reversedFullRoute.Item2[i].location);
        }

        // 3. Verify the total distance remains the same (15 km)
        double originalTotalDistance = originalRoute.GetDistance();
        double reversedTotalDistance = reversedRoute.GetDistance();
        Assert.Equal(originalTotalDistance, reversedTotalDistance, precision: 2);
    }
}
