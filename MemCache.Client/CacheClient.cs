using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stacks.Actors;

namespace MemCache.Client
{
    public class CacheClient : ICacheClient
    {
        private ICacheActor _actor;
        private ConnectionInfo _connectionInfo;

        public ConnectionInfo ConnectionInfo
        {
            get { return _connectionInfo; }
        }

        /// <param name="connectionInfo">tcp://localhost:4632</param>
        public void Connect(ConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException(nameof(connectionInfo));

            if (_actor != null)
                throw new InvalidOperationException("Already connected");

            connectionInfo.Freeze();
            
            _actor = ActorClientProxy.CreateActor<ICacheActor>(connectionInfo.EndPoint).Result;
            _connectionInfo = connectionInfo;
        }

        public void Disconnect()
        {
            CheckConnected();
            ((IActorClientProxy)_actor).Close();
            _actor = null;
            _connectionInfo = null;
        }

        public async Task<object> Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CheckConnected();

            var buffer = await _actor.Get(key);
            return Serializer.Deserialize(buffer);
        }

        public Task<IEnumerable<string>> GetKeys()
        {
            CheckConnected();
            return _actor.GetKeys();
        }

        public Task Set(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CheckConnected();

            byte[] buffer = Serializer.Serialize(value);
            return _actor.Set(key, buffer);
        }

        public Task<bool> Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CheckConnected();

            return _actor.Remove(key);
        }

        private void CheckConnected()
        {
            if (_actor == null)
                throw new InvalidOperationException("Not connected");
        }
    }
}
