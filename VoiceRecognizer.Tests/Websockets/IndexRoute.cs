using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace VoiceRecognizer.Tests.Websockets
{
    public class IndexRoute : WebSocketBehavior
    {
        private NetworkClass _instanceOfNetworkClass;
        public IndexRoute(NetworkClass instanceOfNetworkClass)
        {
            _instanceOfNetworkClass = instanceOfNetworkClass;
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Received message from client: " + e.Data);

            //Broadcast someData string to clients
            Sessions.Broadcast(_instanceOfNetworkClass.buffer);
        }
    }
}