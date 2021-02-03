using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        bool cameraOn = true;

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
                    case ClientCommand.TurnOffCamera:
                        process.StandardInput.WriteLine("p");
                        process.StandardInput.Flush();
                        Console.WriteLine("TURN OFF CAMERA");
                        break;
                    case ClientCommand.TurnOnCamera:
                        process.StandardInput.WriteLine("r");
                        process.StandardInput.Flush();
                        Console.WriteLine("TURN ON CAMERA");
                        break;
                }
            });
        }

        private async void window_SourceInitialized(object sender, EventArgs e)
        {
            webcamDetection();
        }

        Process process;

        async void webcamDetection()
        {
            await Task.Run(() =>
            {
                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                {
                    process = new Process();
                    // preparing ProcessStartInfo
                    process.StartInfo.FileName = "python";
                    process.StartInfo.Arguments = "emotion.py";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardInput = true;
                    StringBuilder outputBuilder = new StringBuilder();
                    try
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            Console.WriteLine(e.Data);
                            if (e.Data == null) return;
                            if (e.Data.Contains("HEYLO_frowning"))
                            {
                                Dispatcher.Invoke(() => browser.ExecuteScriptAsync("window.frowning()"));
                            }
                            else if (e.Data.Contains("HEYLO_sleeping"))
                            {
                                Dispatcher.Invoke(() => browser.ExecuteScriptAsync("window.sleepy()"));
                            }
                            else if (e.Data.Contains("HEYLO_happy"))
                            {
                                Dispatcher.Invoke(() => browser.ExecuteScriptAsync("window.happy()"));
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                    }
                    finally
                    {
                        outputWaitHandle.WaitOne();
                    }
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
        TurnOffCamera,
        TurnOnCamera,
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

        public void turnOffCamera()
        {
            CommandIssued?.Invoke(this, ClientCommand.TurnOffCamera);
        }

        public void turnOnCamera()
        {
            CommandIssued?.Invoke(this, ClientCommand.TurnOnCamera);
        }
    }
}
