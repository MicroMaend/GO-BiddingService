using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;
using GOCore;
using NLog;

namespace GO_Bidding.Services
{
    public class BiddingNotification : IBiddingNotification
    {
        private readonly ConnectionFactory _factory;
        
        private readonly ILogger<BiddingNotification> _logger;
        private readonly IConfiguration _configuration;

        public BiddingNotification(ILogger<BiddingNotification> logger, IConfiguration configuration)
        {

            _logger = logger;
            _configuration = configuration;

            var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
            var port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672");
            var userName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ??
                          Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "admin";

            _logger.LogInformation($"RabbitMQ konfiguration: Host={hostName}, Port={port}, User={userName}");

            _factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
        }

        public async Task SendBidding(Bidding bidding)
        {
            try
            {
                // Create a connection
                using var connection = await _factory.CreateConnectionAsync();

                // Create a channel
                using var channel = await connection.CreateChannelAsync();

                // Declare the queue
                await channel.QueueDeclareAsync(
                    queue: "bidding",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Serialize the bidding object
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(bidding));

                // Publish the message
                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "bidding",
                    body: body
                );
            }
            catch (BrokerUnreachableException ex)
            {
                // Log or handle connection issues
                Console.WriteLine($"RabbitMQ connection error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.WriteLine($"Error sending bidding: {ex.Message}");
                throw;
            }
        }
    }
}