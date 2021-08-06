using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Tests
{
    [TestClass]
    public class RabbitMqPostsPublisherTests
    {
        private readonly RabbitMqPostsPublisher _postsPublisher;
        private readonly IModel _channel;

        public RabbitMqPostsPublisherTests()
        {
            _channel = RabbitMqChannelFactory.Create(new RabbitMqConfig());
            
            _postsPublisher = new RabbitMqPostsPublisher(_channel, NullLogger<RabbitMqPostsPublisher>.Instance);

        }
        
        [TestMethod]
        public void TestConsumeProduce()
        {
            TestConsumeProduce(string.Empty);
            TestConsumeProduce("facebook");
        }
        
        private void TestConsumeProduce(string platform)
        {
            string queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, "posts", string.Empty);

            var semaphore = new Semaphore(0, 1);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, args) =>
            {
                Console.WriteLine(
                    Encoding.UTF8.GetString(
                        args.Body.ToArray()));
                
                semaphore.Release();
            };
            _channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
            
            _postsPublisher.Send(new Post(), platform);
            
            Assert.IsTrue(semaphore.WaitOne(TimeSpan.FromSeconds(1)));
        }

        [TestMethod]
        public void TestProduceConsume()
        {
            TestProduceConsume(string.Empty);
            TestProduceConsume("facebook");
        }
        
        private void TestProduceConsume(string platform)
        {
            string queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, "posts", "");
            
            _postsPublisher.Send(new Post(), platform);

            var semaphore = new Semaphore(0, 1);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, args) =>
            {
                Console.WriteLine(
                    Encoding.UTF8.GetString(
                        args.Body.ToArray()));
                
                semaphore.Release();
            };
            
            _channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
            
            Assert.IsTrue(semaphore.WaitOne(TimeSpan.FromSeconds(1)));
        }
    }
}