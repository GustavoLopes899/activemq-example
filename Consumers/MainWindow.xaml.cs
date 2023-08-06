using ActiveMQConsumers.VM;
using System.Windows;

namespace ActiveMQConsumers;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowVM();
        Closing += (DataContext as MainWindowVM).OnWindowClosing;
    }
}
