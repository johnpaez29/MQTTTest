using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQTTFront.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

namespace MQTTFront.Controllers
{
    public class HomeController : Controller
    {
        public IMqttClient client;
        public string sendTopic;
        public HomeController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessMessage(Client user)
        {
            client = MqttClient.CreateAsync("localhost").Result;
            var sess = client.ConnectAsync().Result;

            string rcvTopic = "eebus/daenet/command";
            sendTopic = "eebus/daenet/command";

            client.SubscribeAsync(rcvTopic, MqttQualityOfService.AtLeastOnce);


            Task.Run(() =>
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));

                client.PublishAsync(new MqttApplicationMessage(sendTopic, data), MqttQualityOfService.ExactlyOnce).Wait();

                client.DisconnectAsync();
            });

            //client.MessageStream.Subscribe(msg =>
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;

            //    Console.WriteLine(Encoding.UTF8.GetString(msg.Payload));

            //    Console.ResetColor();
            //});
            return View("Index");
        }
    }
}
