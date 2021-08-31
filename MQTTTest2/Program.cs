using MQTTTest2.AccessDb;
using MQTTTest2.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

namespace MQTTTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string a in args)
            {
                Console.WriteLine(a);
            }
            //ProcessMQTT(new RepositoryTerminal());
            ////ProcessMQTTV2(new RepositoryTerminal());

            Console.ReadLine();
        }

        //private static void ProcessMQTTV2(IRepositoryTerminal repositoryTerminal)
        //{
        //    var factory = new ConnectionFactory() { HostName = "localhost", Port = 1883 };
        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.ExchangeDeclare(exchange: "topic_logs",
        //                                type: "topic");

        //        var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
        //        var message = (args.Length > 1)
        //                      ? string.Join(" ", args.Skip(1).ToArray())
        //                      : "Hello World!";
        //        var body = Encoding.UTF8.GetBytes(message);
        //        channel.BasicPublish(exchange: "topic_logs",
        //                             routingKey: routingKey,
        //                             basicProperties: null,
        //                             body: body);
        //        Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
        //    }
        //}

        private static void ProcessMQTT(IRepositoryTerminal terminal, Terminal prueba = null)
        {
            var mqttClient = MqttClient.CreateAsync("localhost", 1883).Result;

            var sess = mqttClient.ConnectAsync().Result;

            string rcvTopic = "*.tcs";
            string sendTopic = "eebus/daenet/command1";

            mqttClient.SubscribeAsync(rcvTopic, MqttQualityOfService.AtLeastOnce);

            if (prueba != null)
            {
                Task.Run(() =>
                {
                    var mqttClient = MqttClient.CreateAsync("localhost", 1883).Result;

                    var sess = mqttClient.ConnectAsync().Result;

                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine("Enter the text to send.");

                    Console.ForegroundColor = ConsoleColor.Cyan;

                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(prueba));

                    mqttClient.PublishAsync(new MqttApplicationMessage(sendTopic, data), MqttQualityOfService.ExactlyOnce).Wait();
                });
            }

            //Task.Run(() =>
            //{
            //    Console.ForegroundColor = ConsoleColor.Yellow;

            //    Console.WriteLine("Enter the text to send.");

            //    Console.ForegroundColor = ConsoleColor.Cyan;

            //    string prueba2 = Console.ReadLine();

            //    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(prueba2));

            //    mqttClient.PublishAsync(new MqttApplicationMessage(sendTopic, data), MqttQualityOfService.ExactlyOnce).Wait();
            //});


            mqttClient.MessageStream.Subscribe(msg =>
            {
                Terminal prueba = terminal.GetTerminal(Encoding.UTF8.GetString(msg.Payload));


                //Console.ForegroundColor = ConsoleColor.Green;

                //Console.WriteLine(Encoding.UTF8.GetString(msg.Payload));

                //Console.ResetColor();
            });

            Console.ReadLine();
        }
    }
}
