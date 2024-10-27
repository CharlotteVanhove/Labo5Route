namespace Labo5Route.Model
{
    public class SegmentLocation : Location
    {
        public bool IsStop { get; private set; }

        public SegmentLocation(string name, bool isStop) : base(name) 
           
        {
            IsStop = isStop;
        }

        public void UpdateIsStop(bool isStop)
        {
            IsStop = isStop;
        }
    }
}