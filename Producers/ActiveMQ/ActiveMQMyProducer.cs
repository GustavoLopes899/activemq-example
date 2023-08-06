using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActiveMQProducer.ActiveMQ;

public class ActiveMQMyProducer : IDisposable
{
    private IConnection _connection;
    private ISession _session;
    private List<IMessageProducer> _producers;

    public ActiveMQMyProducer()
    {
        _producers = new List<IMessageProducer>();
        Init();
    }

    public void SendMessage(string message, string queueName)
    {
        IMessageProducer producer = GetProducer(queueName);
        ITextMessage messageText = _session.CreateTextMessage(message);
        producer.Send(messageText);
    }

    private IMessageProducer GetProducer(string name)
    {
        if (name.StartsWith("Queue"))
        {
            return GetProducerQueue(name);
        }

        return GetProducerTopic(name);
    }

    private IMessageProducer GetProducerQueue(string queueName)
    {
        IMessageProducer producer = _producers
            .Where(p => (p as MessageProducer).ProducerInfo.Destination.PhysicalName == queueName)
            .FirstOrDefault();

        if (producer == null)
        {
            IDestination destination = this._session.GetQueue(queueName);
            producer = _session.CreateProducer(destination);
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            _producers.Add(producer);
        }

        return producer;
    }

    private IMessageProducer GetProducerTopic(string topicName)
    {
        IMessageProducer producer = _producers
            .Where(p => (p as MessageProducer).ProducerInfo.Destination.PhysicalName == topicName)
            .FirstOrDefault();

        if (producer == null)
        {
            IDestination destination = this._session.GetTopic(topicName);
            producer = _session.CreateProducer(destination);
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            _producers.Add(producer);
        }

        return producer;
    }

    private void Init()
    {
        Uri connecturi = new Uri("activemq:tcp://localhost:61616");
        ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);
        this._connection = connectionFactory.CreateConnection();
        this._connection.Start();
        this._session = _connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
    }

    public void Dispose()
    {
        _producers = null;
        _session.Close();
        _connection.Close();
    }
}
