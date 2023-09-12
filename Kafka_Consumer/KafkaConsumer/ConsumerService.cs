namespace Kafka_Consumer.KafkaConsumer
{
    public class ConsumerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public ConsumerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            IServiceScope serviceScope = _serviceProvider.CreateScope();
            var consumer = serviceScope.ServiceProvider.GetRequiredService<IConsumerWrapper>();
            var topic = "ticketbooking";
            Task.Run(() => consumer.Consume(topic), cancellationToken);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
