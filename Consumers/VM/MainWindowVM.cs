using ActiveMQConsumers.ActiveMQ;
using System;
using System.ComponentModel;

namespace ActiveMQConsumers.VM;

public class MainWindowVM : Notification, IDisposable
{
    public MainWindowVM()
    {
        ActiveMQMyConsumers = new ActiveMQMyConsumers();
    }

    public ActiveMQMyConsumers ActiveMQMyConsumers { get; private set; }

    public void OnWindowClosing(object sender, CancelEventArgs e)
    {
        Dispose();
    }

    public void Dispose()
    {
        ActiveMQMyConsumers.Dispose();
    }
}
