using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace ActiveMQConsumers.ActiveMQ;

public class ActiveMQMyConsumers : Notification, IDisposable
{
    private IConnection _connection;
    private ISession _session;
    private List<IMessageConsumer> _consumers;
    private readonly int _queues = 3;
    private readonly object _lock = new();

    private ObservableCollection<string> _queue1;
    private ObservableCollection<string> _queue2;
    private ObservableCollection<string> _queue3;

    private ObservableCollection<string> _topic;

    public ActiveMQMyConsumers()
    {
        _consumers = new List<IMessageConsumer>();
        Queue1 = new ObservableCollection<string>();
        Queue2 = new ObservableCollection<string>();
        Queue3 = new ObservableCollection<string>();
        Topic = new ObservableCollection<string>();
        Init();
    }

    #region Queues
    public ObservableCollection<string> Queue1
    {
        get
        {
            return _queue1;
        }
        set
        {
            _queue1 = value;
            BindingOperations.EnableCollectionSynchronization(_queue1, _lock);
            NotifyPropertyChanged(nameof(Queue1));
        }
    }

    public ObservableCollection<string> Queue2
    {
        get
        {
            return _queue2;
        }
        set
        {
            _queue2 = value;
            BindingOperations.EnableCollectionSynchronization(_queue2, _lock);
            NotifyPropertyChanged(nameof(Queue2));
        }
    }

    public ObservableCollection<string> Queue3
    {
        get
        {
            return _queue3;
        }
        set
        {
            _queue3 = value;
            BindingOperations.EnableCollectionSynchronization(_queue3, _lock);
            NotifyPropertyChanged(nameof(Queue3));
        }
    }
    #endregion

    #region Topic
    public ObservableCollection<string> Topic
    {
        get
        {
            return _topic;
        }
        set
        {
            _topic = value;
            BindingOperations.EnableCollectionSynchronization(_topic, _lock);
            NotifyPropertyChanged(nameof(Topic));
        }
    }
    #endregion

    private void Init()
    {
        Uri connecturi = new Uri("activemq:tcp://localhost:61616");
        ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);
        _connection = connectionFactory.CreateConnection();
        _connection.Start();
        _session = _connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
        for (int i = 1; i <= _queues; i++)
        {
            CreateQueueConsumer($"Queue{i}");
            CreateTopicConsumer("Topic", $"Topic{i}");
        }
    }

    private void CreateQueueConsumer(string queue)
    {
        IMessageConsumer consumer = _consumers
            .Where(c => (c as MessageConsumer).ConsumerInfo.Destination.PhysicalName == queue)
            .FirstOrDefault();

        if (consumer is null)
        {
            IDestination destination = _session.GetQueue(queue);
            consumer = _session.CreateConsumer(destination);
            consumer.Listener += ConsumerQueueListener;
            _consumers.Add(consumer);
        }
    }

    private void CreateTopicConsumer(string topicName, string consumerId)
    {
        IMessageConsumer consumer = _consumers
            .Where(c => (c as MessageConsumer).ConsumerInfo.SubscriptionName == topicName)
            .FirstOrDefault();

        if (consumer is null)
        {
            ActiveMQTopic topic = new ActiveMQTopic(topicName);
            consumer = _session.CreateDurableConsumer(topic, consumerId);
            consumer.Listener += ConsumerTopicListener;
            _consumers.Add(consumer);
        }
    }

    private void ConsumerQueueListener(IMessage message)
    {
        string queueName = (message as Message).Destination.PhysicalName;
        ITextMessage txtMessage = message as ITextMessage;
        switch (queueName)
        {
            case "Queue1":
                Queue1.Add(txtMessage.Text);
                break;
            case "Queue2":
                Queue2.Add(txtMessage.Text);
                break;
            case "Queue3":
                Queue3.Add(txtMessage.Text);
                break;
        }
        NotifyPropertyChanged(queueName);
    }

    private void ConsumerTopicListener(IMessage message)
    {
        string topicName = (message as Message).Destination.PhysicalName;
        ITextMessage txtMessage = message as ITextMessage;
        switch (topicName)
        {
            case "Topic":
                Topic.Add(txtMessage.Text);
                break;
        }
        NotifyPropertyChanged(topicName);
    }

    public void Dispose()
    {
        _consumers = null;
        _session.Close();
        _connection.Close();
    }
}
