namespace Kafka_Consumer.KafkaConsumer
{
    public interface IConsumerWrapper
    {
        void Consume(string topic);
    }
}
