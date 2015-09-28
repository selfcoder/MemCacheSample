namespace MemCache.WpfClient
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICacheClient _cacheClient;
        private object _content;

        public MainViewModel(ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
            _content = new ConnectionViewModel(_cacheClient, OnConnected);
        }

        public object Content
        {
            get { return _content; }
            private set { SetValue(ref _content, value); }
        }

        private void OnConnected()
        {
            Content = new CacheViewModel(_cacheClient, OnDisconnected);
        }

        private void OnDisconnected()
        {
            Content = new ConnectionViewModel(_cacheClient, OnConnected);
        }
    }
}