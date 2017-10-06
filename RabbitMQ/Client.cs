using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Shared;

namespace RabbitMQClient
{
    public class Client : IDisposable
    {
        private IConnection _c;
        private List<CustomEventingBasicConsumer> _subscribes;
        private MessageStorage _messageStorage;

        private ConnectionFactory _cf;
        private int _maxLengthKB = 100;
        private List<IMessageProcessor> _processors;
        private volatile int _taskCount = 0;

        private Client()
        {
            _subscribes = new List<CustomEventingBasicConsumer>();
            _processors = new List<IMessageProcessor>();
            _messageStorage = new MessageStorage();
            _messageStorage.OnMessageRecived += OnCompleteMessageRecived;
        }

        public Client(string hostname, int port, int maxLengthKB = 0) : this()
        {
            if (maxLengthKB == 0)
                _maxLengthKB = int.MaxValue;
            else
                _maxLengthKB = maxLengthKB;
            _cf = new ConnectionFactory()
            {
                HostName = hostname,
                Port = port,

            };

            foreach (var type in ClientHelper.GetTypes<MessageProcessorAttribute>(Assembly.GetEntryAssembly()))
            {
                if (type.GetInterface(nameof(IMessageProcessor)) == null)
                    throw new Exception($"The class {type.FullName} not realize interface {nameof(IMessageProcessor)}");
                var msgProcessorInstance = Activator.CreateInstance(type) as IMessageProcessor;
                _processors.Add(msgProcessorInstance);
            }
        }

        public Client(string hostname, int port, int maxLengthKB = 1024, string username = "", string password = "") : this(hostname, port, maxLengthKB)
        {
            _cf = new ConnectionFactory()
            {
                HostName = hostname,
                Port = port,
                UserName = username,
                Password = password,
            };
        }

        public Client(string hostname, int port, string username = "", string password = "", string sslServerName = "",
            string sslCertificatePath = "", bool sslEnable = false) : this()
        {
            _cf = new ConnectionFactory()
            {
                HostName = hostname,
                Port = port,
                UserName = username,
                Password = password,
                Ssl = new SslOption(sslServerName, sslCertificatePath, sslEnable)
            };
        }

        public bool IsConnected => _c.IsOpen;

        public void Connect()
        {
            _c = _cf.CreateConnection();
        }

        public List<CustomEventingBasicConsumer> Subscribes => _subscribes;

        public void Subscribe(string channelName)
        {
            var channel = _c.CreateModel();
            var declaration = channel.QueueDeclare(channelName, true, false, false, null);

            var consumer = new CustomEventingBasicConsumer(channel);
            consumer.ChannelNameListener = channelName;

            _subscribes.Add(consumer);
            consumer.Received += OnReceived;
            channel.BasicConsume(channelName, false, consumer);
        }

        public void Subscribe(string channelName, bool autoRead, ushort prefetchMessages)
        {
            var channel = _c.CreateModel();
            channel.BasicQos(0, prefetchMessages, true);
            var declaration = channel.QueueDeclare(channelName, true, false, false, null);

            var consumer = new CustomEventingBasicConsumer(channel);
            consumer.ChannelNameListener = channelName;

            _subscribes.Add(consumer);
            consumer.Received += OnReceived;
            channel.BasicConsume(channelName, autoRead, consumer);
        }

        public void Unsubscribe(string channelName)
        {
            var subs = _subscribes.Where(x => x.ChannelNameListener == channelName).ToArray();
            foreach (var consumer in subs)
            {
                consumer.Received -= OnReceived;
                _subscribes.Remove(consumer);
            }
        }

        public void UnsubscribeAll()
        {
            _subscribes.ForEach(x => x.Received -= OnReceived);
        }

        public void SendMessage(string channelName, Message message)
        {
            if (!_c.IsOpen) return;

            _taskCount++;

            using (var model = _c.CreateModel())
            {
                model.QueueDeclare(channelName, true, false, false, null);
                model.BasicPublish("", channelName, null, TransportHelper.PackToArray(message));
            }
            _taskCount--;

        }

        public TResponceType SendMessageWithResponceWait<TResponceType>(string channelName, Message message, int timeoutInSeconds = 0)
            where TResponceType : class
        {
            Logger.LogInfo($"Outgoing message {message.GetType().Name}");
            _taskCount++;
            var waitMessage = true;
            TResponceType result = null;

            void Handler(object sender, MessageEventArgs args)
            {
                if (args.Message is TResponceType)
                {
                    result = args.Message as TResponceType;
                    waitMessage = false;
                }
            }

            SendMessage(channelName, message);
            OnMessage += Handler;
            var waitTime = 0;
            while (waitMessage)
            {
                Thread.Sleep(100);
                if (timeoutInSeconds > 0)
                {
                    if (waitTime > timeoutInSeconds * 1000)
                    {
                        break;
                    }
                    waitTime += 100;
                }
            }
            OnMessage -= Handler;
            _taskCount--;
            return result;
        }

        public int SendSplittedMessage(string channelName, Message msg)
        {

            var body = TransportHelper.PackToArray(msg);
            Logger.LogInfo($"Outgoing message {msg.GetType().Name}, message length {(double)body.Length / 1024:F}KBytes");
            Guid messageId = Guid.NewGuid();

            var maxBytes = _maxLengthKB * 1024;
            var totalMessages = (int)Math.Ceiling((decimal)body.Length / maxBytes);
            for (int i = 0, msgNumber = 0; i < body.Length; i += maxBytes, msgNumber++)
            {
                var tinyData = new byte[Math.Min(i + maxBytes, body.Length) - i];
                Array.Copy(body, i, tinyData, 0, Math.Min(i + maxBytes, body.Length) - i);

                var message = new PartialMessage()
                {
                    Body = tinyData,
                    Id = messageId,
                    TotalMessages = totalMessages,
                    MessageNumber = msgNumber + 1
                };

                Logger.LogInfo($"Send splitted message no.{msgNumber}");
                SendMessage(channelName, message);
            }
            return body.Length;
        }
        private void ReadMessage(IModel channel, ulong deliveryTag)
        {
            try
            {
                channel.BasicAck(deliveryTag, false);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error read message. Connection was closed", ex);
            }
        }
        private void DeleteMessage(IModel channel, ulong deliveryTag)
        {
            channel.BasicNack(deliveryTag, false, false);
        }

        private void OnReceived(object sender, BasicDeliverEventArgs e)
        {
            TaskEx.Run(() =>
            {
                _taskCount++;
                var consumer = (sender as EventingBasicConsumer) ?? throw new ArgumentException("Sender must be a EventingBasicConsumer");

                MessageEventArgs args = null;
                try
                {
                    args = new MessageEventArgs(e.Body, e.RoutingKey);
                    if (args.Message is PartialMessage)
                    {
                        var partMsg = args.Message as PartialMessage;
                        Logger.LogInfo($"Get partial message {partMsg.MessageNumber} of {partMsg.TotalMessages}");
                        _messageStorage.Save(partMsg, consumer, e);
                        ReadMessage(consumer.Model, e.DeliveryTag);

                        _taskCount--;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to read message deliveryTag: {e.DeliveryTag} from queue {e.RoutingKey}", ex);
                    ReadMessage(consumer.Model, e.DeliveryTag);
                    return;
                }


                Logger.LogInfo($"Incoming message {args.Message.GetType().Name}");
                OnMessage?.Invoke(consumer, args);
                Proc(args);
                if (args.Handled)
                    ReadMessage(consumer.Model, e.DeliveryTag);

                _taskCount--;
                GC.Collect();
            });
        }

        private void Proc(MessageEventArgs args)
        {
            foreach (var proc in GetProcessors(args.Message.GetType()))
            {
                Logger.LogInfo($"Starting process message: {args.Message.GetType()} from queue: {args.ChannelName}");
                proc.Process(this, args);
                proc.AfterProcess(this);
                Logger.LogInfo($"End process message: {args.Message.GetType()} from queue: {args.ChannelName}");
            }
        }

        private void OnCompleteMessageRecived(Message msg, EventingBasicConsumer consumer, BasicDeliverEventArgs args)
        {
            _taskCount++;
            var msgArgs = new MessageEventArgs(msg, args.RoutingKey);
            Logger.LogInfo($"Incomming message {msg.GetType().Name}");
            OnMessage?.Invoke(consumer, msgArgs);
            Proc(msgArgs);
            _taskCount--;

            GC.Collect();
        }

        public int ActiveTasks => _taskCount;

        private IEnumerable<IMessageProcessor> GetProcessors(Type messageType)
        {
            foreach (var proc in _processors)
            {
                var attr = proc.GetType().GetCustomAttributes(typeof(MessageProcessorAttribute), true)[0] as MessageProcessorAttribute;
                if (attr.MessageTypes.Contains(messageType))
                    yield return proc;
            }
        }

        public event EventHandler<object, MessageEventArgs> OnMessage;

        public void Dispose()
        {
            Logger.LogInfo($"Disposing client...");
            UnsubscribeAll();
            _subscribes = new List<CustomEventingBasicConsumer>();
            _processors = new List<IMessageProcessor>();
            _c?.Dispose();
        }
    }
}