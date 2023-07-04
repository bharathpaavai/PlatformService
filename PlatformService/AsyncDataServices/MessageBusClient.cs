﻿using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _chennel;
        private readonly string _routingKey;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"] ,  Port = int.Parse( _configuration["RabbitMQPort"] )};

            try
            {
                _connection = factory.CreateConnection();
                _chennel = _connection.CreateModel();


                _chennel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to MessageBus");

            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> could not connect to the message bus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void publishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMq Connection Open, Sending Message");
                SendMessage(message);

            }
            else
            {
                Console.WriteLine("--> RabbitMq Connection closed, not Sending Message");
            }
        }



        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _chennel.BasicPublish(exchange: "trigger",
                routingKey: "", 
                basicProperties: null,
                body: body);

            Console.WriteLine($"--> We have sent the {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message bus Disposed");
            if (_chennel.IsOpen)
            {
                _chennel.Close();
                _connection.Close();
            }

        }
    }
}
