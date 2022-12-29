using System;
using System.Text;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace Manager
{
    public class MessageQueueConsumer
    {
        private readonly string rabbitMQHostName = "localhost";
        private string consumeQueueName;
        public IConnection connection;
        public IModel channel;
        public EventingBasicConsumer consumer;
        public bool IsRunning = false;

        public MessageQueueConsumer(string consumeQueueName)
        {
            this.consumeQueueName = consumeQueueName;
            IsRunning = false;

        }

        public void Start()
        {
            this.ConnectAndOpenChannelAsync(this.consumeQueueName);
        }

        public void Restart()
        {
            Logger.LogInfo("Queuee name: " + this.consumeQueueName + " trying to connect to RabbitMQ Broker...");
            if (this.connection != null)
            {
                if(this.channel != null)
                    this.channel.Dispose();
                this.connection.Dispose();

                this.consumer = null;
                this.channel = null;
                this.connection = null;

            }

            this.Start();

        }

        private void ConnectAndOpenChannelAsync(string consumeQueueName)
        {
            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory()
                {
                    HostName = rabbitMQHostName
                };
                this.connection = connectionFactory.CreateConnection();
                if (this.connection.IsOpen)
                {
                    this.channel = connection.CreateModel();
                    this.consumer = new EventingBasicConsumer(this.channel);
                    this.channel.BasicConsume(consumeQueueName, true, consumer);
                    this.IsRunning = true;
                    Logger.LogInfo("Queuee name: " + consumeQueueName + "Connected to RabbitMQ Broker.");
                }
                else
                {
                    this.IsRunning = false;
                    Logger.LogError("Queuee name: " + consumeQueueName + "CAN NOT Connected to RabbitMQ Broker");
                }


            }
            catch(Exception ex)
            {
                this.IsRunning = false;
                Logger.LogError("Queuee name: " + consumeQueueName + "CAN NOT Connected to RabbitMQ Broker");
            }

        }

    }
}
