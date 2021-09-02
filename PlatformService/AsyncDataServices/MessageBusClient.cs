using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MessageBusClient> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public MessageBusClient(IConfiguration config, ILogger<MessageBusClient> logger)
        {
            _logger = logger;
            _config = config;

            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQHost"],
                Port = int.Parse(config["RabbitMQPort"]),
            };

            try
            {
                _connection = factory.CreateConnection();

                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: config["RabbitMQExchange"], type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                _logger.LogInformation("Connect to Message Bus");
            }
            catch (Exception ex)
            {
                logger.LogError("---> Connect to Message Bus Error:", ex.Message);
            }
        }

        public void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning("RabbitMQ Connection shutdown");
        }
        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                _logger.LogInformation("---> RabbitMQ Connection Open, sending message...");

                SendMessage(message);
            }
            else
            {
                _logger.LogInformation("---> RabbitMQ Connection closed...");
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("MessageBus Disposed");

            _channel?.Close();
            _connection?.Close();
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: _config["RabbitMQExchange"], routingKey: "", basicProperties: null, body);

            _logger.LogInformation($"---> send {message}");
        }
    }
}