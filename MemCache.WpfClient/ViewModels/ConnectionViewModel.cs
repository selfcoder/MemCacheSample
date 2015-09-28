using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MemCache.WpfClient
{
    public class ConnectionViewModel : ViewModelBase
    {
        private readonly ICacheClient _cacheClient;
        private readonly Action _connectedCollback;
        private ICommand _connectCommand;
        private bool _isConnecting;
        private Exception _connectionError;
        private ConnectionInfo _connectionInfo;

        public ConnectionViewModel(ICacheClient cacheClient, Action connectedCollback)
        {
            if (cacheClient == null)
                throw new ArgumentNullException(nameof(cacheClient));

            _cacheClient = cacheClient;
            _connectedCollback = connectedCollback;

            // todo: read from config or take from UI or take as dependency
            _connectionInfo = new ConnectionInfo
            {
                EndPoint = "tcp://localhost:4632"
            };
        }

        public ICommand ConnectCommand
        {
            get { return _connectCommand ?? (_connectCommand = new RelayAsyncCommand(Connect)); } //, () => !IsConnecting
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            private set
            {
                if (SetValue(ref _isConnecting, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsError
        {
            get { return _connectionError != null; }
            private set
            {
                if (value)
                    throw new InvalidOperationException();
                if (_connectionError != null)
                {
                    _connectionError = null;
                    OnPropertyChanged();
                }
            }
        }

        private Task Connect()
        {
            if (IsConnecting)
                return new Task(() => { });

            IsError = false;
            IsConnecting = true;
            return Task.Run(delegate
            {
                try
                {
                    // delay for animation only
                    Task.WaitAll(ConnectCore(), Task.Delay(1000));
                }
                catch (Exception ex)
                {
                    // todo
                    //Log(ex);
                    _connectionError = ex;
                    OnPropertyChanged(nameof(IsError));
                    return;
                }
                finally
                {
                    IsConnecting = false;
                }

                if (_connectedCollback != null)
                {
                    _connectedCollback();
                }
            });
        }

        private async Task ConnectCore()
        {
            _cacheClient.Connect(_connectionInfo);
        }
    }
}