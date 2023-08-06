using ActiveMQProducer.ActiveMQ;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ActiveMQProducer.VM;

public class MainWindowVM : IDisposable
{
    private readonly ActiveMQMyProducer _activeMQproducer;

    public MainWindowVM()
    {
        CreateCommands();
        _activeMQproducer = new ActiveMQMyProducer();
    }

    public ICommand Send { get; private set; }

    private void CreateCommands()
    {
        this.Send = new RelayCommand(
            obj =>
            {
                string message = (obj as string[])[0];
                string queueName = (obj as string[])[1];
                SendMessage(message, queueName);
            }, _ => true);
    }

    private void SendMessage(string message, string queueName)
    {
        _activeMQproducer.SendMessage(message, queueName);
    }

    public void OnWindowClosing(object sender, CancelEventArgs e)
    {
        this.Dispose();
    }

    public void Dispose()
    {
        this._activeMQproducer.Dispose();
    }
}
