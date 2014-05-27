using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Freebox.BootstrapperApplication.Enums;
using Freebox.BootstrapperApplication.Helpers;
using Freebox.BootstrapperApplication.Properties;
using Freebox.BootstrapperApplication.ViewModels;
using Freebox.BootstrapperApplication.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Freebox.BootstrapperApplication
{
    /// <summary>
    /// View model of the main view.
    /// </summary>
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        #region Private Members

        private FrameworkElement _currentView;

        private Model _model;
        private BurnInstallationState _burnInstallationState;
        private InstallationMode _installationMode;

        private bool _canceled;
        private ICommand _closeCommand;
        private ICommand _minimizeCommand;        

        #endregion

        #region Constructors

        /// <summary>
        ///  Constructor of the main viewmodel.
        /// </summary>
        public MainWindowViewModel(Model model)
        {
            _model = model;

            Hwnd = IntPtr.Zero;
            BurnInstallationState = BurnInstallationState.Initializing;
            InstallationMode = InstallationMode.Undefined;

            MaintenanceWelcomeViewModel = new MaintenanceWelcomeViewModel(this);            
            WelcomeViewModel = new WelcomeViewModel(this);
            InstallViewModel = new InstallViewModel(this);
            ProgressViewModel = new ProgressViewModel(this);
            FinishViewModel = new FinishViewModel(this);

            InitializeEvents();

        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the model of the application
        /// </summary>
        public Model Model
        {
            get
            {
                return _model;
            }
        }

        /// <summary>
        /// Bootstrapper
        /// </summary>
        public BootstrapperApplication Bootstrapper
        {
            get
            {
                return _model.Bootstrapper;
            }
        }

        // Pointer to the window. It will be set from the main view.
        /// <summary>
        /// Hanlde of the current window. It is needed when an action is applied.
        /// </summary>
        public IntPtr Hwnd
        {
            get;
            set;
        }


        // This is the current view (install view, progress view..etc)
        public FrameworkElement CurrentView
        {
            get
            {
                return _currentView;
            }
            set
            {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        /// <summary>
        /// Gets or sets the burn installation state.
        /// </summary>
        public BurnInstallationState BurnInstallationState
        {
            get
            {
                return _burnInstallationState;
            }

            set
            {
                if (_burnInstallationState != value)
                {
                    _burnInstallationState = value;
                    // Notify that the installation state has changed..
                    base.OnPropertyChanged("BurnInstallationState");
                }
            }
        }

        /// <summary>
        /// InstallationMode: Install, Uninstall or Repair
        /// </summary>
        public InstallationMode InstallationMode
        {
            get
            {
                return _installationMode;
            }

            set
            {
                if (_installationMode != value)
                {
                    _installationMode = value;
                    // Notify that the installation state has changed..
                    base.OnPropertyChanged("InstallationMode");
                }
            }
        }

        // Model views of the views
        public MaintenanceWelcomeViewModel MaintenanceWelcomeViewModel
        {
            get;
            private set;
        }

        public WelcomeViewModel WelcomeViewModel
        {
            get;
            private set;
        }

        public InstallViewModel InstallViewModel
        {
            get;
            private set;
        }

        public ProgressViewModel ProgressViewModel
        {
            get;
            private set;
        }

        public FinishViewModel FinishViewModel
        {
            get;
            private set;
        }

        public string LogoPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logo.png");
            }
        }

        public string BackgroundImage
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "background.png");
            }
        }

        public string ProductName
        {
            get
            {
                return Model.BundleInfo.BundleAttributes.DisplayName;
            }
        }        

        public bool Canceled
        {
            get
            {
                return _canceled;
            }

            set
            {
                if (_canceled != value)
                {
                    _canceled = value;
                    InstallationMode = InstallationMode.Cancel;
                    base.OnPropertyChanged("Canceled");

                    if (BurnInstallationState != BurnInstallationState.Applying)
                    {
                        Model.Bootstrapper.Dispatcher.BeginInvoke(
                        new Action(() =>
                        {
                            Bootstrapper.Dispatcher.InvokeShutdown();
                        }));
                    }
                }
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// Launches the shut down.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(
                        param =>
                        {
                            lock (this)
                            {
                                Model.Bootstrapper.Dispatcher.InvokeShutdown();
                            }
                        },
                        param => true);
                }

                return _closeCommand;
            }
        }

        /// <summary>
        /// Mimimizes the window.
        /// </summary>
        public ICommand MinimizeCommand
        {
            get
            {
                if (_minimizeCommand == null)
                {
                    _minimizeCommand = new RelayCommand(
                        param =>
                        {
                            lock (this)
                            {
                                // Find the window that contains the control
                                Window window = Window.GetWindow(CurrentView);

                                // Minimize
                                window.WindowState = WindowState.Minimized;

                            }
                        },
                        param => true);
                }

                return _minimizeCommand;
            }
        }

        #endregion

        #region Boostrapper Engine Methods

        // The methods PlanAction and ApplyAction  prepare and executes tasks like install, uninstall, repair or modify.

        /// <summary>
        /// Tells the Boostrapper enigne to plan the given action.
        /// </summary>
        /// <param name="action"></param>
        public void PlanAction(LaunchAction action)
        {
            Model.PlannedAction = action;
            Model.Bootstrapper.Engine.Plan(action);
        }

        /// <summary>
        /// Tells the Bootrstapper engine to apply the action that was already planed, for the current window (_hwnd)
        /// </summary>
        public void ApplyAction()
        {
            Model.Bootstrapper.Engine.Apply(Hwnd);
        }

        /// <summary>
        /// Tells the Bootstrapper to log the messages.
        /// The log file is located in the %TEMP% - folder.
        /// </summary>
        /// <param name="message"></param>
        public void LogMessage(string message)
        {
            Model.Bootstrapper.Engine.Log(LogLevel.Standard, message);
        }

        #endregion

        #region Bootstrapper Event Hanlders

        private void InitializeEvents()
        {
            // Defining eventhandlers for the events in the bootstrapper is required because we need to do many things:
            //    - set the installation state of our viewModel.
            //    - decide whether to show the installview or the maintenanceview.
            Bootstrapper.DetectBegin += DetectBegin;
            Bootstrapper.DetectComplete += DetectComplete;
            Bootstrapper.Error += ExecuteError;
            

        }

        private void DetectComplete(object sender, DetectCompleteEventArgs e)
        {

            if (BurnInstallationState == Enums.BurnInstallationState.NotPresent)
            {
                Model.Bootstrapper.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     CurrentView = new WelcomeView(WelcomeViewModel);
                 }));
            }
            else
            {
                Model.Bootstrapper.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     CurrentView = new MaintenanceWelcomeView(MaintenanceWelcomeViewModel);
                 }));
            }
        }

        private void DetectBegin(object sender, DetectBeginEventArgs e)
        {
            if (e.Installed)
            {                
                BurnInstallationState = BurnInstallationState.Present;
            }
            else
            {
                BurnInstallationState = BurnInstallationState.NotPresent;
            }
            Model.PlannedAction = LaunchAction.Unknown;
        }

        private void ExecuteError(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ErrorEventArgs e)
        {
            lock (this)
            {
                Model.Bootstrapper.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                    timer.Start();
                    timer.Tick += (a, b) =>
                    {
                        timer.Stop();
                        InstallationMode = InstallationMode.Cancel;
                        this.CurrentView = new FinishView(this.FinishViewModel);
                    };
                }));

                this.ApplyAction();                
                string s = e.ErrorMessage;
            }
        }

        #endregion

        public void WindowOpacity(bool value)
        {

            Window window = Window.GetWindow(CurrentView);
            if (value)
            {

                window.Opacity = 0.4;
            }
            else
            {
                window.Opacity = 1;
            }
        }
    }
}
