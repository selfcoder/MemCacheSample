using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace MemCache.WpfClient
{
    public class CacheViewModel : ViewModelBase, ICacheValueProvider
    {
        private readonly ICacheClient _cacheClient;
        private readonly Action _disconnectedCallback;
        private ObservableCollection<ICacheItem> _items;
        private ICollectionView _itemsSource;
        private ICommand _disconnectCommand;
        private ICommand _refreshCommand;
        private ICommand _removeCommand;
        private ICommand _setCommand;

        public CacheViewModel(ICacheClient cacheClient, Action disconnectedCallback = null)
        {
            _cacheClient = cacheClient;
            _disconnectedCallback = disconnectedCallback;
        }

        public ICollectionView Items
        {
            get
            {
                if (_itemsSource == null)
                {
                    _items = new ObservableCollection<ICacheItem>();
                    _items.Add(new NewCacheItemViewModel());
                    _itemsSource = CollectionViewSource.GetDefaultView(_items);

                    RefreshAvailableItems();
                }

                return _itemsSource;
            }
        }

        public ICommand DisconnectCommand
        {
            get { return _disconnectCommand ?? (_disconnectCommand = new RelayCommand(Disconnect)); }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(RefreshItems)); }
        }

        public ICommand RemoveCommand
        {
            get { return _removeCommand ?? (_removeCommand = new RelayCommand(RemoveCurrentItem, () => CurrentItem != null)); }
        }

        public ICommand SetCommand
        {
            get { return _setCommand ?? (_setCommand = new RelayCommand(
                SaveCurrentItem, () => CurrentItem != null && CurrentItem.IsDirty)); }
        }

        public ICacheItem CurrentItem
        {
            get { return _itemsSource?.CurrentItem as ICacheItem; }
        }

        public string ConnectionInfo
        {
            get { return _cacheClient.ConnectionInfo.EndPoint; }
        }

        private async void RefreshAvailableItems()
        {
            IEnumerable<string> keys = null;
            try
            {
                keys = await _cacheClient.GetKeys();
            }
            catch (Exception ex)
            {
                Handle(ex);
            }
            
            _items.Clear();
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    _items.Add(new CacheItemViewModel(key, this));
                }
            }
            
            _items.Add(new NewCacheItemViewModel());
            _itemsSource.MoveCurrentToLast();
        }

        private void RefreshItems()
        {
            RefreshAvailableItems();
            OnPropertyChanged(nameof(Items));
        }

        private async void SaveCurrentItem()
        {
            var item = CurrentItem;
            if (item == null)
                return;

            try
            {
                await _cacheClient.Set(item.Key, item.Value);
            }
            catch (Exception ex)
            {
                Handle(ex);
                return;
            }
            
            if (item is NewCacheItemViewModel)
            {
                _items.Insert(_items.Count - 1, new CacheItemViewModel(item.Key, item.Value));
            }

            item.IsDirty = false;
        }

        private async void RemoveCurrentItem()
        {
            var item = CurrentItem;
            if (item == null)
                return;

            var newItem = item as NewCacheItemViewModel;
            if (newItem != null)
            {
                newItem.IsDirty = false;
                return;
            }

            try
            {
                await _cacheClient.Remove(item.Key);
            }
            catch (Exception ex)
            {
                Handle(ex);
                return;
            }
            
            _items.Remove(item);
        }

        private void Disconnect()
        {
            _cacheClient.Disconnect();
            if (_disconnectedCallback != null)
            {
                _disconnectedCallback();
            }
        }

        public object GetValue(string key)
        {
            return _cacheClient.Get(key).Result;
        }

        private void Handle(Exception exception)
        {
            if (exception is SocketException)
            {
                // todo
            }

            // todo
        }
    }

    public interface ICacheItem
    {
        string Key { get; }
        object Value { get; }
        bool IsDirty { get; set; }
    }

    public interface ICacheValueProvider
    {
        object GetValue(string key);
    }

    public class CacheItemViewModel : ViewModelBase, ICacheItem
    {
        private readonly string _key;
        private readonly ICacheValueProvider _valueProvider;
        private object _value;
        private bool _loaded;

        public CacheItemViewModel(string key, ICacheValueProvider valueProvider)
        {
            _key = key;
            _valueProvider = valueProvider;
        }

        public CacheItemViewModel(string key, object value)
        {
            _key = key;
            _value = value;
            _loaded = true;
        }

        public string Key
        {
            get { return _key; }
            // TwoWay binding bug workaround
            set { }
        }

        public object Value
        {
            get
            {
                if (!_loaded)
                {
                    LoadValueAsync();
                }

                return _value;
            }
            set
            {
                if (SetValue(ref _value, value))
                {
                    IsDirty = true;
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }

        public bool IsDirty { get; set; }

        private void LoadValueAsync()
        {
            Task.Run(() =>
            {
                _value = _valueProvider.GetValue(_key);
                _loaded = true;
                OnPropertyChanged(nameof(Value));
            });
        }
    }

    public class NewCacheItemViewModel : ViewModelBase, ICacheItem
    {
        private string _key;
        private object _value;

        public string Key
        {
            get { return _key; }
            set
            {
                if (SetValue(ref _key, value))
                {
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (SetValue(ref _value, value))
                {
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }

        public bool IsDirty
        {
            get { return !string.IsNullOrEmpty(_key) && _value != null; }
            set
            {
                Key = null;
                Value = null;
            }
        }
    }
}