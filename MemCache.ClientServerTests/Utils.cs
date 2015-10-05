using System.Net;
using System.Net.Sockets;

namespace MemCache.ClientServerTests
{
    public class Utils
    {
        public static int FindFreePort()
        {
            TcpListener l = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public static string GetEndpoint()
        {
            return "tcp://localhost:" + FindFreePort();
        }
    }
}