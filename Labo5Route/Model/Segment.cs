using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labo5Route.Model
{
    public class Segment
    {
        public SegmentLocation Start { get; private set; }
        public SegmentLocation End { get; private set; }
        public Distance Distance { get; private set; }

        public Segment(SegmentLocation start, SegmentLocation end, Distance distance)
        {
            Start = start ?? throw new ArgumentNullException(nameof(start));
            End = end ?? throw new ArgumentNullException(nameof(end));
            Distance = distance ?? throw new ArgumentNullException(nameof(distance));
        }
    }
}
