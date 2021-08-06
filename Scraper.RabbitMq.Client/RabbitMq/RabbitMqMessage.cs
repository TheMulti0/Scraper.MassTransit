using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Scraper.RabbitMq.Client
{
    public class RabbitMqMessage<T>
    {
        public T Content { get; }

        private readonly BasicDeliverEventArgs _message;
        private readonly IModel _channel;
        private readonly RabbitMqConsumerConfig _config;

        public RabbitMqMessage(
            T content,
            BasicDeliverEventArgs message,
            IModel channel,
            RabbitMqConsumerConfig config)
        {
            Content = content;
            _message = message;
            _channel = channel;
            _config = config;
        }

        public RabbitMqMessage<V> Select<V>(Func<T, V> map)
        {
            return new(
                map(Content),
                _message,
                _channel,
                _config);
        }

        public void Acknowledge()
        {
            _channel.BasicAck(_message.DeliveryTag, false);
        }

        public void Reject()
        {
            _channel.BasicReject(_message.DeliveryTag, _config.RejectRequeue);
        }
    }
}