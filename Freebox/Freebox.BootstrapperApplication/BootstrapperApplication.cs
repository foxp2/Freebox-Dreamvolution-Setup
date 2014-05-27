using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml;
using System.Windows.Media.Animation;

namespace Freebox.BootstrapperApplication
{
    /// <summary>
    /// BootstrapperApplication.
    /// </summary>
    public class BootstrapperApplication : Microsoft.Tools.WindowsInstallerXml.Bootstrapper.BootstrapperApplication, IDisposable
    {
        /// <summary>
        /// Gets the global dispatcher.
        /// </summary>
        public Dispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// Required to release burn ui gracefully
        /// </summary>
        /// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                // dispose other managed objects here
            }
        }


        /// <summary>
        /// Starts the main instance of the bootstrapper
        /// </summary>
        protected override void Run()
        {

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Start();
            timer.Tick += (a, b) =>
            {
                timer.Stop();
                Engine.Elevate(IntPtr.Zero);                
                Engine.CloseSplashScreen();

            };

            // set the global message dispatcher
            Dispatcher = Dispatcher.CurrentDispatcher;

            // Create the model, view model and the view of the main window.
            // The view model handles the enitre ui.
            Model model = new Model(this);

            MainWindowViewModel viewModel = new MainWindowViewModel(model);

            MainWindow mainWindow = new MainWindow(viewModel);

            Engine.Detect();

            // Create a Window to show UI.
            Engine.Log(Microsoft.Tools.WindowsInstallerXml.Bootstrapper.LogLevel.Verbose, "Creating UI");

            mainWindow.Show();


            // run the threading dispatcher
            Dispatcher.Run();

            // Finalize the installation (with the finalResult) and quit the process.
            Engine.Quit(model.FinalResult);

        }
    }
}
