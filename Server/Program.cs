using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Server
{
    public class Program
    {
        private static readonly ILogger Logger = Log.ForContext<Program>();

        public static IConfigurationRoot configuration;
        static void Main()
        {

            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                // ReSharper disable once AssignNullToNotNullAttribute
                .WriteTo.File(Path.Combine(currentPath,
                    @"log\SimpleMqttServer_.txt"), rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            var config = ReadConfiguration(currentPath);

            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint().WithDefaultEndpointPort(config.Port).WithConnectionValidator(
                    c =>
                    {
                        //var currentUser = config.Users.FirstOrDefault(u => u.UserName == c.Username);

                        //if (currentUser == null)
                        //{
                        //    c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        //    LogMessage(c, true);
                        //    return;
                        //}

                        //if (c.Username != currentUser.UserName)
                        //{
                        //    c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        //    LogMessage(c, true);
                        //    return;
                        //}

                        //if (c.Password != currentUser.Password)
                        //{
                        //    c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        //    LogMessage(c, true);
                        //    return;
                        //}

                        c.ReasonCode = MqttConnectReasonCode.Success;
                        LogMessage(c, false);
                    }).WithSubscriptionInterceptor(
                    c =>
                    {
                        c.AcceptSubscription = true;
                        LogMessage(c, true);
                    }).WithApplicationMessageInterceptor(
                    c =>
                    {
                        c.AcceptPublish = true;
                        LogMessage(c);
                    });

            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build());
            Console.ReadLine();
        }

        private static GeneralConfig ReadConfiguration(string currentPath)
        {
            var filePath = $"{currentPath}\\config.json";

            GeneralConfig config = null;

            // ReSharper disable once InvertIf
            if (File.Exists(filePath))
            {
                using var r = new StreamReader(filePath);
                var json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<GeneralConfig>(json);
            }

            return config;
        }
        private static void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
        {
            if (context == null)
            {
                return;
            }

            Logger.Information(
                successful
                    ? "New subscription: ClientId = {clientId}, TopicFilter = {topicFilter}"
                    : "Subscription failed for clientId = {clientId}, TopicFilter = {topicFilter}",
                context.ClientId,
                context.TopicFilter);
        }
        private static void LogMessage(MqttApplicationMessageInterceptorContext context)
        {
            if (context == null)
            {
                return;
            }

            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            Logger.Information(
                "Message: ClientId = {clientId}, Topic = {topic}, Payload = {payload}, QoS = {qos}, Retain-Flag = {retainFlag}",
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
        }
        private static void LogMessage(MqttConnectionValidatorContext context, bool showPassword)
        {
            if (context == null)
            {
                return;
            }

            if (showPassword)
            {
                Logger.Information(
                    "New connection: ClientId = {clientId}, Endpoint = {endpoint}, Username = {userName}, Password = {password}, CleanSession = {cleanSession}",
                    context.ClientId,
                    context.Endpoint,
                    context.Username,
                    context.Password,
                    context.CleanSession);
            }
            else
            {
                Logger.Information(
                    "New connection: ClientId = {clientId}, Endpoint = {endpoint}, Username = {userName}, CleanSession = {cleanSession}",
                    context.ClientId,
                    context.Endpoint,
                    context.Username,
                    context.CleanSession);
            }
        }

        private static IHostBuilder CreateDefaultBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile("appsettings.json");
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<GeneralConfig>();
                });
        }
    }
}
