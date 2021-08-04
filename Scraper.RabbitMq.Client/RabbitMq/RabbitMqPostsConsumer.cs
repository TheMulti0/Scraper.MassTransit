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
        private readonly Subject<NewPost> _newPosts = new();

        public IObservable<NewPost> NewPosts => _newPosts.Retry();

        public RabbitMqPostsConsumer(IModel channel)
        {
            QueueDeclareOk queue = DeclareQueue(channel);
            
            BindQueue(channel, queue);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessage;
            consumer.Shutdown += OnShutdown;

            Consume(channel, queue, consumer);
        }

        private static QueueDeclareOk DeclareQueue(IModel channel)
        {
            return channel.QueueDeclare(
                queue: RabbitMqConstants.PipeName,
                durable: true,
                exclusive: false,
                autoDelete: false);
        }

        private static void BindQueue(IModel channel, QueueDeclareOk queue)
        {
            channel.QueueBind(
                queue: queue.QueueName,
                exchange: RabbitMqConstants.PipeName,
                routingKey: string.Empty);
        }

        private static string Consume(IModel channel, QueueDeclareOk queue, EventingBasicConsumer consumer)
        {
            return channel.BasicConsume(
                queue: queue.QueueName,
                autoAck: true,
                consumer: consumer);
        }

        private void OnMessage(object sender, BasicDeliverEventArgs message)
        {
            try
            {
                Post post = ParsePost(message);
                string platform = message.RoutingKey;
                var newPost = new NewPost(post, platform);
                
                _newPosts.OnNext(newPost);
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