using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeyloWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientAPIProxy clientProxy;

        public MainWindow()
        {
            InitializeComponent();


            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            browserSettings.BackgroundColor = Colors.DarkGray.ToArgb();
            browser.BrowserSettings = browserSettings;

            clientProxy = new ClientAPIProxy();
            browser.JavascriptObjectRepository.Register("heyloClient", clientProxy, true);
            clientProxy.CommandIssued += ClientProxy_CommandIssued; ;
        }

        private void ClientProxy_CommandIssued(ClientAPIProxy proxy, ClientCommand command)
        {
            Dispatcher.Invoke(() =>
            {
                Storyboard sb;
                switch (command)
                {
                    case ClientCommand.HeightIncrease:
                        sb = FindResource("IncreaseHeight") as Storyboard;
                        sb.Begin();
                        break;
                    case ClientCommand.HeightDecrease:
                        sb = FindResource("DecreaseHeight") as Storyboard;
                        sb.Begin();
                        break;
                    case ClientCommand.WidthIncrease:
                        sb = FindResource("IncreaseWidth") as Storyboard;
                        sb.Begin();
                        break;
                    case ClientCommand.WidthDecrease:
                        sb = FindResource("DecreaseWidth") as Storyboard;
                        sb.Begin();
                        break;
                }
            });
        }
    }

    enum ClientCommand
    {
        HeightIncrease,
        HeightDecrease,
        WidthIncrease,
        WidthDecrease,
    }

    class ClientAPIProxy
    {
        public delegate void CommandHandler(ClientAPIProxy proxy, ClientCommand command);
        public event CommandHandler CommandIssued;

        public void increaseHeight()
        {
            CommandIssued?.Invoke(this, ClientCommand.HeightIncrease);
        }

        public void decreaseHeight()
        {
            CommandIssued?.Invoke(this, ClientCommand.HeightDecrease);
        }

        public void increaseWidth()
        {
            CommandIssued?.Invoke(this, ClientCommand.WidthIncrease);
        }

        public void decreaseWidth()
        {
            CommandIssued?.Invoke(this, ClientCommand.WidthDecrease);
        }
    }
}
