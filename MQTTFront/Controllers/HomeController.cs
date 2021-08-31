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
        public IActionResult ProcessMessage()
        {
            client = MqttClient.CreateAsync("localhost", 1883).Result;
            var sess = client.ConnectAsync().Result;

            string rcvTopic = "eebus/daenet/command1";
            sendTopic = "eebus/daenet/command2";

            client.SubscribeAsync(rcvTopic, MqttQualityOfService.AtLeastOnce);


            Task.Run(() =>
            {
                var data = Encoding.UTF8.GetBytes("T1");

                client.PublishAsync(new MqttApplicationMessage(sendTopic, data), MqttQualityOfService.ExactlyOnce).Wait();

            });
            ProcessMessage();

            client.MessageStream.Subscribe(msg =>
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(Encoding.UTF8.GetString(msg.Payload));

                Console.ResetColor();
            });
            return View("Index");
        }
    }
}
