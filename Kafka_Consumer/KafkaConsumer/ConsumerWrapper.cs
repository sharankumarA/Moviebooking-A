using Confluent.Kafka;
using Kafka_Consumer.IService;
using System.Text.Json;

namespace Kafka_Consumer.KafkaConsumer
{
    public class ConsumerWrapper : IConsumerWrapper
    {
        private readonly string topic = "ticketbooking";
        private readonly string groupId = "test";
        private readonly string bootstrapServer = "localhost:9092";
        private readonly IAdminService _adminService;

        private readonly ILogger<ConsumerWrapper> _logger;

        public ConsumerWrapper(IAdminService adminService, ILogger<ConsumerWrapper> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }
        public async void Consume(string topic)
        {
            var _config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = bootstrapServer
            };


            using (var consumerBuilder = new ConsumerBuilder
                    <Ignore, string>(_config).Build())
            {
                consumerBuilder.Subscribe(topic);
                try
                {
                    while (true)
                    {
                        _logger.LogInformation("Kafka consumer initiated");
                        var consumer = consumerBuilder.Consume();
                        var orderRequest = JsonSerializer.Deserialize<string>(consumer.Message.Value);

                        var booked = await _adminService.BookingCountAsync(orderRequest);
                        var available = await _adminService.BookingAvailableAsync(orderRequest);
                        await _adminService.UpdateTicketStatus(orderRequest);

                        consumerBuilder.Consume(booked.Count);
                        consumerBuilder.Consume(available.Count);
                        consumerBuilder.Commit(consumer);
                    }
                }
                catch (OperationCanceledException)
                {
                    consumerBuilder.Close();
                }
            }
        }
    }
}
