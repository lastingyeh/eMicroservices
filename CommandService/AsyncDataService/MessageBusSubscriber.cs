using System.Text;
using System.Threading;
using System.Threading.Tasks;
using commandservice.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace commandservice.AsyncDataService
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;
        private readonly ILogger<MessageBusSubscriber> _logger;
        
        public MessageBusSubscriber(IConfiguration config, IEventProcessor eventProcessor, ILogger<MessageBusSubscriber> logger)
        {
            _logger = logger;
            _config = config;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (hanlder, e) =>
            {
                _logger.LogInformation("---> Event Received...");

                var body = e.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer);

            return Task.CompletedTask;
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"]),
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _config["RabbitMQExchange"], type: ExchangeType.Fanout);

            _queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: _queueName, exchange: _config["RabbitMQExchange"], routingKey: "");

            _logger.LogInformation("---> Listening on the Message Bus...");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning("---> Connection Shutdown");
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();

            base.Dispose();
        }
    }
}