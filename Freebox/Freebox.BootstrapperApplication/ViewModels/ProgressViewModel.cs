using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Reflection;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows.Input;
using System.Windows.Threading;

using Freebox.BootstrapperApplication.Views;
using Freebox.BootstrapperApplication.Enums;
using Freebox.BootstrapperApplication.Utilities;
using Freebox.BootstrapperApplication.Properties;

namespace Freebox.BootstrapperApplication.ViewModels
{
    /// <summary>
    /// Modelview class for the Progress view.
    /// </summary>
    public class ProgressViewModel : ViewModelBase
    {
        #region Private Members

        // The entire progress will be calulated based on the cache progress and on the execute progress.
        private int _cacheProgress;
        private int _executeProgress;
        private int _progress;

        private string _packageName;
        private string _packageMessage;
        private string _title;
        private string _subtitle;

        private int _packageProgress;
        private ICommand _cancelCommand;

        #endregion

        #region Constructors

        public ProgressViewModel(MainWindowViewModel mainViewModel)
            : base(mainViewModel)
        {
            BuildStrings();
            MainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;

            InitializeEvents();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title of the page.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title != value)
                {
                    _title = value;
                    base.OnPropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// Gets or sets the subtitle of the page.
        /// </summary>
        public string Subtitle
        {
            get
            {
                return _subtitle;
            }

            set
            {
                if (_subtitle != value)
                {
                    _subtitle = value;
                    base.OnPropertyChanged("Subtitle");
                }
            }
        }

        /// <summary>
        /// Gets or sets the progress of each package
        /// </summary>
        public int PackageProgress
        {
            get
            {
                return _packageProgress;
            }
            set
            {
                _packageProgress = value;
                OnPropertyChanged("PackageProgress");
            }
        }

        /// <summary>
        /// Gets or sets the entire progress.
        /// </summary>
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        /// <summary>
        /// Gets or sets the package message.
        /// </summary>
        public string PackageMessage
        {
            get
            {
                return _packageMessage;
            }

            set
            {
                if (_packageMessage != value)
                {
                    _packageMessage = value;
                    base.OnPropertyChanged("PackageMessage");
                }
            }
        }

        /// <summary>
        /// Gets or sets the package name.
        /// </summary>
        public string PackageName
        {
            get
            {
                return _packageName;
            }

            set
            {
                if (_packageName != value)
                {
                    _packageName = value;
                    base.OnPropertyChanged("PackageName");
                }
            }
        }


        #endregion

        #region Commands

        /// <summary>
        /// Cancels the installation.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {

                    _cancelCommand = new RelayCommand(
                        param =>
                        {
                            lock (this)
                            {
                                Model.Bootstrapper.Dispatcher.InvokeShutdown();
                            }
                        },
                        param => true);
                }

                return _cancelCommand;
            }
        }
        #endregion

        #region Private Methods

        private void MainWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Whenever the InstallationMode changes, we have to inform the UI that the strings had also changed.
            if (e.PropertyName == "InstallationMode")
            {
                BuildStrings();
            }
        }

        private void BuildStrings()
        {
            switch (InstallationMode)
            {
                case InstallationMode.Install:
                    {
                        Title = Resources.strInstalling;
                        Subtitle = String.Format(Resources.strPleaseWaitWhileInstalling, Model.BundleInfo.BundleAttributes.DisplayName);
                        break;
                    }
                case InstallationMode.Uninstall:
                    {
                        Title = Resources.strUninstalling;
                        Subtitle = String.Format(Resources.strPleaseWaitWhileUninstalling, Model.BundleInfo.BundleAttributes.DisplayName);
                        break;
                    }
                case InstallationMode.Repair:
                    {
                        Title = Resources.strRepairing;
                        Subtitle = String.Format(Resources.strPleaseWaitWhileRepairing, Model.BundleInfo.BundleAttributes.DisplayName);
                        break;
                    }
                case InstallationMode.Cancel:
                    {
                        Title = Resources.strCanceling;
                        Subtitle = Resources.strPleaseWaitWhileCanceling;
                        break;
                    }
                default:
                    {
                        Title = Resources.strInstallationStateUnknown;
                        Subtitle = "";
                        break;
                    }
            }
        }

        #region Bootstrapper Event Hanlders

        private void InitializeEvents()
        {
            // Defining eventhandlers for the events in the bootstrapper is required because we need to do many things:
            //    - set the installation state of our viewModel.
            //    - set the final state of the installation in the model.
            //    - check if the installation was cancelled and in that case tell the bootstrapper to invoke the shut down
            //    - check the progress.
            Bootstrapper.ApplyBegin += ApplyBegin;
            Bootstrapper.ApplyComplete += ApplyComplete;

            Bootstrapper.CacheAcquireProgress += OnCacheAcquireProgress;
            Bootstrapper.CacheAcquireComplete += CacheAcquireComplete;

            Bootstrapper.ExecutePackageBegin += ExecutePackageBegin;
            Bootstrapper.ExecutePackageComplete += ExecutePackageComplete;

            Bootstrapper.ExecuteMsiMessage += ExecuteMsiMessage;
            Bootstrapper.ExecuteProgress += OnExecuteProgress;
            Bootstrapper.Progress += OnProgress;

            Bootstrapper.Error += ExecuteError;
        }

        #region Apply - Used for initialization and finalization (changing view)

        private void ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            lock (this)
            {
                PackageMessage = Resources.strMessageInitialize;
                PackageName = "";
                BurnInstallationState = BurnInstallationState.Applying;
            }
        }

        private void ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            lock (this)
            {
                Model.FinalResult = e.Status;

                Model.Bootstrapper.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                    timer.Start();
                    timer.Tick += (a, b) =>
                    {
                        timer.Stop();
                        MainWindowViewModel.CurrentView = new FinishView(MainWindowViewModel.FinishViewModel);
                    };
                }));
            }
        }

        #endregion

        #region Cache - Used for Progress

        private void OnCacheAcquireProgress(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.CacheAcquireProgressEventArgs e)
        {
            lock (this)
            {

                //_cacheProgress = e.OverallPercentage;
                // HACK for bugfix overallPercentage > 100. Fix will come in 3.8
                _cacheProgress = e.OverallPercentage / Model.Bootstrapper.Engine.PackageCount;
                Progress = (_cacheProgress + _executeProgress) / 2;

                e.Result = MainWindowViewModel.Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void CacheAcquireComplete(object sender, CacheAcquireCompleteEventArgs e)
        {
            lock (this)
            {
                _cacheProgress = 100;
                Progress = (_cacheProgress + _executeProgress) / 2;
            }
        }

        #endregion

        #region ExecutePackage - Used for Message and Cancel

        private void ExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
        {
            lock (this)
            {
                foreach (PackageInfo package in Model.BundleInfo.Packages)
                {
                    if (package.Id == e.PackageId)
                    {
                        PackageName = package.DisplayName;
                    }
                }
                if (MainWindowViewModel.Canceled)
                {
                    e.Result = Result.Cancel;
                }
            }
        }

        private void ExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
        {
            lock (this)
            {
                if (MainWindowViewModel.Canceled)
                {
                    e.Result = Result.Cancel;
                }
            }
        }

        #endregion

        #region Execute - Used for Message and Progress

        private void ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            lock (this)
            {

                switch (e.MessageType)
                {
                    case InstallMessage.ActionStart:
                        {
                            string[] entireMessage = e.Message.Split(':');
                            string[] actionMessage = entireMessage[3].Split('.');
                            if (actionMessage[1].Trim() != "")
                            {
                                PackageMessage = actionMessage[1];
                            }
                            break;
                        }
                }

                e.Result = MainWindowViewModel.Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void OnExecuteProgress(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ExecuteProgressEventArgs e)
        {
            lock (this)
            {
                _executeProgress = e.OverallPercentage;
                Progress = (_cacheProgress + _executeProgress) / 2;

                PackageProgress = e.ProgressPercentage;

                e.Result = MainWindowViewModel.Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void OnProgress(object sender, ProgressEventArgs e)
        {
            // Shows you the total overall progress and is very easy to use for a single progress bar but 
            // the progress bar will not move very smoothly
            lock (this)
            {
                e.Result = MainWindowViewModel.Canceled ? Result.Cancel : Result.Ok;
            }
        }
        #endregion

        private void ExecuteError(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ErrorEventArgs e)
        {
            lock (this)
            {
                string s = e.ErrorMessage;
                //MessageBox.Show(s);
            }
        }


        #endregion

        #endregion

    }
}
