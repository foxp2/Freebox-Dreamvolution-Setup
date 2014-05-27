using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

using Freebox.BootstrapperApplication.Views;
using Freebox.BootstrapperApplication.Enums;

namespace Freebox.BootstrapperApplication.ViewModels
{
    /// <summary>
    /// Modelview class for the MaintenanceWelcomeView view.
    /// </summary>
    public class MaintenanceWelcomeViewModel : ViewModelBase
    {
        #region Private Members

        private ICommand _repairCommand;
        private ICommand _uninstallCommand;
        private ICommand _cancelCommand;

        #endregion

        #region Constructors

        public MaintenanceWelcomeViewModel(MainWindowViewModel mainViewModel)
            : base(mainViewModel)
        {
            MainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;

            InitializeEvents();
        }

        #endregion

        #region Properties
        #endregion

        #region Commands

        /// <summary>
        /// Launches the repair action of the bootstrapper.
        /// The command can be executed only if there was already an installation found on the computer.
        /// </summary>
        public ICommand RepairCommand
        {
            get
            {
                if (_repairCommand == null)
                {
                    // Create a command for planing the repair action into the engine. 
                    // The command can be executed only if there was already an installation found on the computer.
                    _repairCommand = new RelayCommand(
                        param =>
                        {
                            InstallationMode = InstallationMode.Repair;
                            MainWindowViewModel.PlanAction(LaunchAction.Repair);
                        },
                        param => BurnInstallationState == BurnInstallationState.Present);
                }

                return _repairCommand;
            }
        }

        /// <summary>
        /// Gets a value indicating, wether the repair command can be executed or not.
        /// </summary>
        public bool RepairEnabled
        {
            get { return this.RepairCommand.CanExecute(this); }
        }

        /// <summary>
        /// Launches the uninstall action of the bootstrapper
        /// The command can be executed only if there was already an installation found on the computer.
        /// </summary>
        public ICommand UninstallCommand
        {
            get
            {
                if (_uninstallCommand == null)
                {
                    // Create a command for planing the uninstall action into the engine. 
                    // The command can be executed only if there was found an installation of the current product on the computer.
                    _uninstallCommand = new RelayCommand(
                        param =>
                        {
                            InstallationMode = InstallationMode.Uninstall;
                            MainWindowViewModel.PlanAction(LaunchAction.Uninstall);
                        },
                        param => BurnInstallationState == BurnInstallationState.Present);
                }

                return _uninstallCommand;
            }
        }

        /// <summary>
        /// Gets a value indicating, wether the uninstall command can be executed or not.
        /// </summary>
        public bool UninstallEnabled
        {
            get { return this.UninstallCommand.CanExecute(this); }
        }

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
                                if (System.Windows.MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    MainWindowViewModel.Canceled = true;
                                }
                                else
                                {
                                    MainWindowViewModel.Canceled = false;
                                }
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
            if ("BurnInstallationState" == e.PropertyName)
            {
                base.OnPropertyChanged("Title");
                base.OnPropertyChanged("RepairEnabled");
                base.OnPropertyChanged("UninstallEnabled");
            }
        }


        #region Bootstrapper Event Hanlders

        private void InitializeEvents()
        {
            // Defining eventhandlers for the events of bootstrapper is required because we need to do many things:
            //    - check if the installation was cancelled and in that case tell the bootstrapper to invoke the shut down
            //    - after planing complete, apply the repair or uninstall action and change the CurrentView to the ProgressView
            Bootstrapper.PlanComplete += PlanComplete;

            Bootstrapper.Error += ExecuteError;
        }


        private void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (MainWindowViewModel.Canceled)
            {
                Bootstrapper.Dispatcher.InvokeShutdown();
                return;
            }

            Model.Bootstrapper.Dispatcher.BeginInvoke(
            new Action(() =>
            {
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                timer.Start();
                timer.Tick += (a, b) =>
                {
                    timer.Stop();
                    MainWindowViewModel.CurrentView = new ProgressView(MainWindowViewModel.ProgressViewModel);
                };
            }));

            MainWindowViewModel.ApplyAction();
        }

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
