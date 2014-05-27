using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

using Freebox.BootstrapperApplication.Enums;
using Freebox.BootstrapperApplication.Properties;

namespace Freebox.BootstrapperApplication.ViewModels
{
    /// <summary>
    /// Modelview class for the finish view.
    /// </summary>
    public class FinishViewModel : ViewModelBase
    {
        #region Private Members

        private string _title;
        private string _subtitle;

        private ICommand _finishCommand;

        #endregion

        #region Constructors

        public FinishViewModel(MainWindowViewModel mainViewModel)
            : base(mainViewModel)
        {

            BuildStrings();
            MainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;
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

        #endregion

        #region Commands

        /// <summary>
        /// Launches the shut down.
        /// </summary>
        public ICommand FinishCommand
        {
            get
            {
                if (_finishCommand == null)
                {
                    // Create a command for planing the install action into the engine. 
                    // The command can be executed only if there was no other installation of the current product on the computer.
                    _finishCommand = new RelayCommand(
                        param =>
                        {
                            Model.Bootstrapper.Dispatcher.InvokeShutdown();
                        },
                        param => true);
                }

                return _finishCommand;
            }
        }

        #endregion

        #region Private Methods

        private void BuildStrings()
        {
            switch (InstallationMode)
            {
                case InstallationMode.Install:
                    {
                        Title = Resources.strInstallComplete;
                        Subtitle = String.Format(Resources.strInstallThankyou, Model.BundleInfo.BundleAttributes.DisplayName);
                        break;
                    }
                case InstallationMode.Uninstall:
                    {
                        Title = Resources.strUninstallComplete;
                        Subtitle = String.Format(Resources.strUninstallThankyou, Model.BundleInfo.BundleAttributes.DisplayName);
                        ;
                        break;
                    }
                case InstallationMode.Repair:
                    {
                        Title = Resources.strRepairComplete;
                        Subtitle = String.Format(Resources.strRepairThankyou, Model.BundleInfo.BundleAttributes.DisplayName); ;
                        break;
                    }
                case InstallationMode.Cancel:
                    {
                        Title = Resources.strActionCanceled;
                        Subtitle = "";
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

        private void MainWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Whenever the InstallationState changes, we have to inform the UI that the Message property also has changed.
            if ("InstallationMode" == e.PropertyName)
            {
                BuildStrings();
            }
        }

        #endregion
    }
}
