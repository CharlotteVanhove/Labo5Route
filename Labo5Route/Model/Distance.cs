using Labo5.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labo5Route.Model
{
    public class Distance
    {
        public double Value { get; private set; }
        const int minOpgegevenWaarde = 2;
        public Distance(double value)
        {
            if (value <= minOpgegevenWaarde)
            {
                throw new RouteException($"Afstand moet groter zijn dan {minOpgegevenWaarde}");
            }
            Value = value;
        }

    }
}
