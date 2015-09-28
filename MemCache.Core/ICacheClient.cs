using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemCache
{
    public interface ICacheClient
    {
        Task<object> Get(string key);
        Task<IEnumerable<string>> GetKeys();
        Task Set(string key, object value);
        Task<bool> Remove(string key);

        ConnectionInfo ConnectionInfo { get; }
        void Connect(ConnectionInfo info);
        void Disconnect();
    }
}