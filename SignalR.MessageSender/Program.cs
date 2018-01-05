using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
 

namespace SignalR.MessageSender
{
    class Program
    {
        static HubConnection _hubConnection;
        static IHubProxy _hubProxy;
        private static List<NotificationObject> _notificationObjects;
 
        static void Main(string[] args)
        {
            // Initialise
            _notificationObjects = new List<NotificationObject>();

            string url = "http://localhost:8080";

            // First, start SignalR
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);

                // Now that SignalR is connected, we want to create a Client Proxy
                // This enables this same machine to acts as a client enabling it to send data to everyone 
                // If everything in your application lives in the Web, there's no need for this

                _hubConnection = new HubConnection(url);

                // We reference to the hub
                _hubProxy = _hubConnection.CreateHubProxy("notificationHub");
               
                _hubConnection.Start().Wait();

                var command = Console.ReadLine();
                while (command.ToLower() != "exit")
                {
                    AddNotification(command);
                    command = Console.ReadLine();
                } 


            }
        }
        
        static void AddNotification(string message)
        {
            var newNotification = new NotificationObject {Id = _notificationObjects.Count, Message = message};
            _notificationObjects.Add(newNotification); 

            _hubProxy.Invoke("AddNotification", newNotification).Wait();
        }

    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    [HubName("notificationHub")]
    public class NotificationHub : Hub
    { 
        public void AddNotification(NotificationObject notification)
        {
            Clients.All.addNotification(JsonConvert.SerializeObject(notification,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()}));
        }

        public void RemoveNotification(int id)
        {
            // Do you want the Server to do something about this message that you wanted to remove?
            // You have the ID now. You can now do whatever you want.

            Clients.All.removeNotification(id);
        }
    }

    public class NotificationObject
    {
        public int Id { get; set; }
        public string Message { get; set; }

    }
      
}
