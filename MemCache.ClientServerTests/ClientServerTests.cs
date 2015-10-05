using System;
using MemCache.Client;
using MemCache.Server;
using Xunit;

namespace MemCache.ClientServerTests
{
    public class ClientServerTests
    {
        [Fact]
        public void Client_should_connect_to_server()
        {
            var endPoint = Utils.GetEndpoint();

            var server = new CacheServer();
            server.Start(endPoint);

            var client = new CacheClient();
            client.Connect(new ConnectionInfo { EndPoint = endPoint });
        }

        [Fact]
        public async void First_client_set_and_second_client_get_the_same_value()
        {
            var endPoint = Utils.GetEndpoint();

            var server = new CacheServer();
            server.Start(endPoint);

            var client1 = new CacheClient();
            client1.Connect(new ConnectionInfo { EndPoint = endPoint });
            await client1.Set("key", "value");

            var client2 = new CacheClient();
            client2.Connect(new ConnectionInfo { EndPoint = endPoint });

            Assert.Equal("value", await client2.Get("key"));
        }

        [Fact]
        public async void Set_and_Get_primitive_types()
        {
            var endPoint = Utils.GetEndpoint();

            var server = new CacheServer();
            server.Start(endPoint);

            var client = new CacheClient();
            client.Connect(new ConnectionInfo { EndPoint = endPoint });

            var values = new object[] {34, "abc", true, new DateTime(2020, 10, 20, 1, 2, 3), Int64.MinValue};
            for (int i = 0; i < values.Length; i++)
            {
                await client.Set(values[i].ToString(), values[i]);
            }

            for (int i = 0; i < values.Length; i++)
            {
                Assert.Equal(values[i], await client.Get(values[i].ToString()));
            }
        }

        [Fact]
        public async void Set_and_Get_complex_type()
        {
            var endPoint = Utils.GetEndpoint();

            var server = new CacheServer();
            server.Start(endPoint);

            var client = new CacheClient();
            client.Connect(new ConnectionInfo { EndPoint = endPoint });

            var date = DateTime.Now;
            var value1 = new Foo { Int = 123, Str = "string", Bar = new Bar { Date = date } };
            var value2 = new Foo { Int = 123, Str = "string", Bar = new Bar { Date = date } };

            await client.Set("key", value1);
            
            Assert.Equal(value2, await client.Get("key"));
        }
    }

    public class Foo
    {
        public int Int { get; set; }
        public string Str { get; set; }
        public Bar Bar { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Foo))
                return false;

            var foo = obj as Foo;
            return Int == foo.Int
                   && Str == foo.Str
                   && Equals(Bar, foo.Bar);
        }
    }

    public class Bar
    {
        public DateTime Date { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Bar))
                return false;

            var bar = obj as Bar;
            return Date == bar.Date;
        }
    }
}