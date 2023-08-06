using ActiveMQProducer.VM;
using System.Windows;

namespace ActiveMQProducer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowVM();
        Closing += (DataContext as MainWindowVM).OnWindowClosing;
    }
}
