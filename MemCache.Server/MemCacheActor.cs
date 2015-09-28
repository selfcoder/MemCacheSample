using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stacks.Actors;

namespace MemCache.Server
{
    public class MemCacheActor : Actor, ICacheActor
    {
        private readonly ConcurrentDictionary<string, byte[]> _dictionary;

        public MemCacheActor()
        {
            _dictionary = new ConcurrentDictionary<string, byte[]>();
        }

        public async Task<byte[]> Get(string key)
        {
            await Context;
            byte[] value;
            if (_dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        public async Task<IEnumerable<string>> GetKeys()
        {
            await Context;

            return _dictionary.Keys;
        }

        public async Task Set(string key, byte[] value)
        {
            await Context;

            _dictionary.AddOrUpdate(key, value, (k, old) => value);
        }

        public async Task<bool> Remove(string key)
        {
            await Context;

            byte[] value;
            return _dictionary.TryRemove(key, out value);
        }
    }
}