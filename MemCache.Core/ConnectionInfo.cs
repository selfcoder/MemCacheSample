namespace MemCache
{
    public class ConnectionInfo : FreezableBase
    {
        private string _endPoint;

        public string EndPoint
        {
            get { return _endPoint; }
            set
            {
                CheckIfFrozen();
                _endPoint = value;
            }
        }
    }
}