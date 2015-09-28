using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
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
            return Deserialize(buffer);
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

            byte[] buffer = Serialize(value);
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

        private byte[] Serialize(object value)
        {
            if (value == null)
                return null;

            var valueType = value.GetType();
            var serializer = MessagePackSerializer.Get(valueType);
            var valueTypeName = Encoding.UTF8.GetBytes(valueType.FullName);
            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes((ushort)valueTypeName.Length), 0, 2);
                stream.Write(valueTypeName, 0, valueTypeName.Length);
                serializer.Pack(stream, value);
                return stream.ToArray();
            }
        }

        private object Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            var typeNameLength = BitConverter.ToUInt16(buffer, 0);
            var typeName = Encoding.UTF8.GetString(buffer, 2, typeNameLength);
            var type = Type.GetType(typeName, true);
            var serializer = MessagePackSerializer.Get(type);
            using (var stream = new MemoryStream(buffer))
            {
                stream.Seek(typeNameLength + 2, SeekOrigin.Begin);
                return serializer.Unpack(stream);
            }
        }
    }
}
