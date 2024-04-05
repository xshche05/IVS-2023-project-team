using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Newtonsoft.Json;
using Path = System.IO.Path;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IvsCalc {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application {
        /// <summary>
        /// Application folder
        /// </summary>
        public static readonly string ApplicationFolder = AppDomain.CurrentDomain.BaseDirectory;
        private readonly StreamWriter log;

        /// <summary>
        /// Logs into file
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="caller">Log function caller</param>
        private void Log(string message, [CallerMemberName] string caller = "") {
            log.WriteLine($"[{DateTime.Now}] {caller}: {message}");
            log.Flush();
        }   

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            log = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "IvsCalculator.log"), true);

            Log($"App started");
            Log($"App folder is {AppDomain.CurrentDomain.BaseDirectory}");
        }

        /// <summary>
        /// Event handler for all unhandled exceptions. So that the app won't fall
        /// in App.g.cs debug handler, because in production no debugger is attached.
        /// </summary>
        /// <param name="sender">Exception sender object</param>
        /// <param name="e">Exception arguments</param>
        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e) {
            Log("UNHANDLED EXCEPTION");
            Log($"{{\"Sender\": {JsonConvert.SerializeObject(sender)}, \"Exception\": {JsonConvert.SerializeObject(e)}}}");
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {
            Window window = new MainWindow();
            window.Activate();
        }
    }
}
