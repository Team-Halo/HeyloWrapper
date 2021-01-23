using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HeyloWrapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CefSettings settings = new CefSettings();

            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CefCommandLineArgs.Add("enable-speech-input", "1");
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capture", "1");

            Cef.Initialize(settings);
        }
    }
}
