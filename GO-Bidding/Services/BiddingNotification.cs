using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;
using GOCore;

namespace GO_Bidding.Services
{
    public class BiddingNotification : IBiddingNotification
    {
        private readonly ConnectionFactory _factory;

        public BiddingNotification()
        {
            _factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
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