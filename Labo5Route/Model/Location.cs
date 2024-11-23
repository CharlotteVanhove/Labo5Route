using Labo5.Exceptions;

namespace Labo5Route.Model
{
    public abstract class Location
    {
        public string Name { get; private set; }

        public Location(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !char.IsUpper(name[0]))
            {
                throw new RouteException("Locatienaam mag niet leeg zijn en moet met een hoofdletter starten.");
            }

            Name = name;
        }
    }
}
