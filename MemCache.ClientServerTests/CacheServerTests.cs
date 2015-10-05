using System;
using MemCache.Server;
using Xunit;

namespace MemCache.ClientServerTests
{
    public class CacheServerTests
    {
        [Fact]
        public void CacheServer_should_start_and_stop()
        {
            var server = new CacheServer();

            server.Start(Utils.GetEndpoint());
            server.Stop();
        }

        [Fact]
        public void Start_throws_if_already_started()
        {
            var server = new CacheServer();
            var endpoint = Utils.GetEndpoint();

            server.Start(endpoint);

            Assert.Throws<InvalidOperationException>(() =>
            {
                server.Start(endpoint);
            });
        }

        [Fact]
        public void Stop_throws_if_not_started()
        {
            var server = new CacheServer();

            Assert.Throws<InvalidOperationException>(() =>
            {
                server.Stop();
            });
        }
    }
}