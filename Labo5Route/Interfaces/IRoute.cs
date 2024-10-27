namespace Labo5.Interfaces
{
    public interface IRoute
    {
        void AddLocation(string location, double distance, bool isStop);
        double GetDistance();
        double GetDistance(string startLocation, string endLocation);
        bool HasLocation (string location);
        bool HasStop ( string location);
        void InsertLocation (string location, double distance, string fromLocation, bool isStop);
        void RemoveLocation (string location);
        //void SetDistance (double distance, string location1, string loocation2);
        (string start, List<(double distance, string location)>) ShowFullRoute(); // dit is een tuple
        (string start, List<(double distance, string location)>) ShowFullRoute(string startLocation, string endLocation);
        List<string> ShowLocations();
        (string start, List<(double distance, string location)>) ShowRoute();
        (string start, List<(double distance, string location)>) ShowRoute(string startLocation, string endLocation);
        List<string> ShowStops();
        //void UpdateLocation(string location, string newName, bool isStop);
    }
}
