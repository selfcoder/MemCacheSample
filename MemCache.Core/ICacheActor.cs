using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemCache
{
    public interface ICacheActor
    {
        Task<byte[]> Get(string key);
        Task<IEnumerable<string>> GetKeys();
        Task Set(string key, byte[] value);
        Task<bool> Remove(string key);
    }
}