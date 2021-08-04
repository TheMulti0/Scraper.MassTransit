using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Scraper.RabbitMq.Client
{
    internal class RabbitMqPostsConsumer : INewPostsConsumer
    {
        private const string QueueName = "posts";
        private readonly Subject<NewPost> _newPosts = new();

        public IObservable<NewPost> NewPosts => _newPosts.Retry();

        public RabbitMqPostsConsumer(IModel channel)
        {
            QueueDeclareOk queue = channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessage;
            consumer.Shutdown += OnShutdown;

            channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
        }

        private void OnMessage(object sender, BasicDeliverEventArgs message)
        {
            try
            {
                _newPosts.OnNext(ParsePost(message));
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

        private static NewPost ParsePost(BasicDeliverEventArgs message)
        {
            var json = "No Json";

            try
            {
                ReadOnlyMemory<byte> readOnlyMemory = message.Body;

                byte[] bytes = readOnlyMemory.ToArray();

                json = Encoding.UTF8.GetString(bytes);

                return JsonSerializer.Deserialize<NewPost>(json) 
                       ?? throw new NullReferenceException($"Failed to deserialize {json}");
            }
            catch (Exception e)
            {
                throw new JsonException($"Failed to parse json {json}", e);
            }
        }
    }
}