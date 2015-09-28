using System;
using Stacks.Actors;

namespace MemCache.Server
{
    public class CacheServer
    {
        private IActorServerProxy _actorServer;

        /// <param name="bindEndPoint">tcp://*:4632</param>
        public void Start(string bindEndPoint)
        {
            if (_actorServer != null)
                throw new InvalidOperationException("Already started");

            _actorServer = ActorServerProxy.Create<MemCacheActor>(bindEndPoint);
        }

        public void Stop()
        {
            if (_actorServer == null)
                throw new InvalidOperationException("Not started");

            _actorServer.Stop();
        }
    }
}
