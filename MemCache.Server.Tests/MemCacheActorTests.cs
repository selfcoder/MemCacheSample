using System.Linq;
using Xunit;

namespace MemCache.Server.Tests
{
    public class MemCacheActorTests
    {
        [Fact]
        public async void Get_should_return_array_which_was_setted()
        {
            var actor = new MemCacheActor();
            var buffer = new byte[] {12, 34, 56};

            await actor.Set("key", buffer);

            Assert.Equal(buffer, await actor.Get("key"));
            Assert.Equal(new[] {"key"}, await actor.GetKeys());
        }

        [Fact]
        public async void Remove_should_remove_value()
        {
            var actor = new MemCacheActor();
            var buffer = new byte[] { 12, 34, 56 };

            await actor.Set("key", buffer);
            await actor.Remove("key");

            Assert.Equal(null, await actor.Get("key"));
            Assert.Equal(Enumerable.Empty<string>(), await actor.GetKeys());
        }

        [Fact]
        public async void Remove_should_not_throws_if_key_not_exists()
        {
            var actor = new MemCacheActor();

            await actor.Remove("key");
        }

        [Fact]
        public async void GetKeys_should_return_all_keys()
        {
            var actor = new MemCacheActor();

            await actor.Set("key1", new byte[] { 12, 34, 56 });
            await actor.Set("key2", new byte[] { 98, 76 });
            var keys = await actor.GetKeys();

            Assert.Equal(new [] { "key1", "key2" }, keys.OrderBy(k => k));
        }
    }
}