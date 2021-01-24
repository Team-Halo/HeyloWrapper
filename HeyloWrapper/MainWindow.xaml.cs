using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                        cameraOn = false;
                        break;
                    case ClientCommand.TurnOnCamera:
                        cameraOn = true;
                        break;
                }
            });
        }

        private async void window_SourceInitialized(object sender, EventArgs e)
        {
            webcamDetection();
        }

        async void webcamDetection()
        {
            await Task.Run(() =>
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "python";
                    process.StartInfo.Arguments = "emotion.py";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.Start();

                    StreamReader reader = process.StandardOutput;
                    StreamWriter writer = process.StandardInput;

                    bool camera = true;

                    while (true)
                    {
                        Console.WriteLine("Loop");
                        if (cameraOn && !camera)
                        {
                            writer.WriteLine("r");
                            Console.WriteLine("TURN ON CAMERA");
                            camera = true;
                        }
                        else if (!cameraOn && camera)
                        {
                            writer.WriteLine("p");
                            Console.WriteLine("TURN OFF CAMERA");
                            camera = false;
                        }
                        // Synchronously read the standard output of the spawned process.
                        string output = reader.ReadToEnd();
                        Console.WriteLine("heylo: " + output);

                        if (output.Contains("HEYLO_frowning"))
                        {
                            Dispatcher.Invoke(() => browser.ExecuteScriptAsync("window.frowning()"));
                        }
                        else if (output.Contains("HEYLO_sleeping"))
                        {
                            Dispatcher.Invoke(() => browser.ExecuteScriptAsync("window.sleepy()"));
                        }
                        else if (output.Contains("HEYLO_happy"))
                        {
                            Dispatcher.Invoke(() => browser.ExecuteScriptAsync("window.happy()"));
                        }

                        // To pause the face detection:
                        // writer.WriteLine("p");

                        // To resume the face detection:
                        // writer.WriteLine("r");

                        // To quit the face detection:
                        // writer.WRiteLine("q");
                    }

                    process.WaitForExit();
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
