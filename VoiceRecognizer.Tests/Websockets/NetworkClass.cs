using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace VoiceRecognizer.Tests.Websockets
{
    public class NetworkClass
    {
        public string buffer = string.Empty;
        private WebSocketServer server = new WebSocketServer("ws://127.0.0.1:8080");
        public NetworkClass()
        {
            server.AddWebSocketService<IndexRoute>("/", 
                () => new IndexRoute(this)
                { IgnoreExtensions = true });
            server.Start();

            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + "Server started");
        }

        public void broadcastData()
        {
            server.WebSocketServices["/"].Sessions.Broadcast(buffer);
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + "Message:");
        }

        public void Stop()
        {
            server.Stop();
        }
    }
}
