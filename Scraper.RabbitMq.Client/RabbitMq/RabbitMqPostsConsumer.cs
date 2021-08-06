using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    internal class RabbitMqPostsConsumer : INewPostsConsumer
    {
        private readonly IModel _channel;
        private readonly RabbitMqConsumerConfig _config;
        private readonly Subject<RabbitMqMessage<NewPost>> _newPosts = new();

        public IObservable<RabbitMqMessage<NewPost>> NewPosts => _newPosts.Retry();

        public RabbitMqPostsConsumer(
            IModel channel,
            RabbitMqConsumerConfig config)
        {
            _channel = channel;
            _config = config;

            PostsExchangeFactory.DeclareExchange(channel);
            QueueDeclareOk queue = DeclareQueue();
            
            BindQueue(queue);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessage;
            consumer.Shutdown += OnShutdown;
            
            Consume(queue, consumer);
        }

        private QueueDeclareOk DeclareQueue()
        {
            return _channel.QueueDeclare(
                queue: RabbitMqConstants.PipeName,
                durable: true,
                exclusive: false,
                autoDelete: false);
        }

        private void BindQueue(QueueDeclareOk queue)
        {
            _channel.QueueBind(
                queue: queue.QueueName,
                exchange: RabbitMqConstants.PipeName,
                routingKey: string.Empty);
        }

        private string Consume(QueueDeclareOk queue, IBasicConsumer consumer)
        {
            return _channel.BasicConsume(
                queue: queue.QueueName,
                autoAck: true,
                consumer: consumer);
        }

        private void OnMessage(object sender, BasicDeliverEventArgs delivery)
        {
            try
            {
                Post post = ParsePost(delivery);
                string platform = delivery.RoutingKey;
                var newPost = new NewPost(post, platform);

                var message = new RabbitMqMessage<NewPost>(
                    newPost,
                    delivery,
                    _channel,
                    _config);
                
                _newPosts.OnNext(message);
            }
            catch (Exception e)
            {
                _newPosts.OnError(e);
            }
        }

        private void OnShutdown(object sender, ShutdownEventArgs e)
        {
            _newPosts.OnCompleted();
        }

        private static Post ParsePost(BasicDeliverEventArgs message)
        {
            var json = "No Json";

            try
            {
                ReadOnlyMemory<byte> readOnlyMemory = message.Body;

                byte[] bytes = readOnlyMemory.ToArray();

                json = Encoding.UTF8.GetString(bytes);

                return JsonSerializer.Deserialize<Post>(json) 
                       ?? throw new NullReferenceException($"Failed to deserialize {json}");
            }
            catch (Exception e)
            {
                throw new JsonException($"Failed to parse json {json}", e);
            }
        }
    }
}