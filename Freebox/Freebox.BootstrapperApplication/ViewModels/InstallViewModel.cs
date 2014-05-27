using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

using Freebox.BootstrapperApplication.Views;
using Freebox.BootstrapperApplication.Enums;
using Freebox.BootstrapperApplication.Properties;

namespace Freebox.BootstrapperApplication.ViewModels
{
    /// <summary>
    /// Modelview class for the InstallView view.
    /// </summary>
    public class InstallViewModel : ViewModelBase
    {
        #region Private Members

        private ICommand _installCommand;
        private ICommand _folderSelectionDialogCommand;
        private ICommand _cancelCommand;

        private bool _licenseAccepted;
        private string _installDir;

        #endregion

        #region Constructors

        public InstallViewModel(MainWindowViewModel mainViewModel)
            : base(mainViewModel)
        {
            InitializeEvents();
        }

        #endregion

        #region Properties

        public string LicenseText
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string InstallSelection
        {
            get;
            set;
        }

        public bool LicenseAccepted
        {
            get
            {
                return _licenseAccepted;
            }
            set
            {
                _licenseAccepted = value;
                OnPropertyChanged("LicenseAccepted");
                OnPropertyChanged("InstallEnabled");
            }
        }

        public string InstallDir
        {
            get
            {
                if (String.IsNullOrEmpty(_installDir))
                {
                    // Default path of the installation dir is the one from the model.
                    _installDir = Engine.FormatString(Model.InstallDir);
                }
                return _installDir;
            }
            set
            {
                _installDir = value;
                Model.InstallDir = _installDir;
                // Notify the textbox bound to the installDir to update its ui.
                OnPropertyChanged("InstallDir");
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Launches the install action of the bootstrapper.
        /// The command can be executed only if the user has accepted the license.
        /// </summary>
        public ICommand InstallCommand
        {
            get
            {
                if (_installCommand == null)
                {
                    // Create a command for planing the install action into the engine. 
                    // The command can be executed only if the user has accepted the license.
                    _installCommand = new RelayCommand(
                        param =>
                        {
                            InstallationMode = InstallationMode.Install;
                            MainWindowViewModel.PlanAction(LaunchAction.Install);
                            MainWindowViewModel.PlanAction(LaunchAction.Help);

                        },
                        param => LicenseAccepted == true);
                }

                return _installCommand;
            }
        }

        /// <summary>
        /// Gets a value indicating, wether the install command can be executed or not.
        /// </summary>
        public bool InstallEnabled
        {
            get { return this.InstallCommand.CanExecute(this); }
        }

        /// <summary>
        /// Opens the FolderBrowserDialog.
        /// </summary>
        public ICommand FolderSelectionDialogCommand
        {
            get
            {
                if (_folderSelectionDialogCommand == null)
                {
                    // Create a command for planing the install action into the engine. 
                    // The command will be executed allways.
                    _folderSelectionDialogCommand = new RelayCommand(
                        param =>
                        {
                            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                            {
                                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                                {
                                    InstallDir = folderBrowserDialog.SelectedPath;
                                }
                            }
                        },
                        param => true);
                }

                return _folderSelectionDialogCommand;
            }
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

        #region Bootstrapper Event Hanlders

        private void InitializeEvents()
        {
            // Defining eventhandlers for the events of bootstrapper is required because we need to do many things:
            //    - check if the installation was cancelled and in that case tell the bootstrapper to invoke the shut down
            //    - after planing complete, apply the install and change the CurrentView to the ProgressView
            Bootstrapper.PlanComplete += PlanComplete;

            Bootstrapper.Error += ExecuteError;

            Title = String.Format(Resources.strInstallLicenseTerms, Model.BundleInfo.BundleAttributes.DisplayName);

            InstallSelection = String.Format(Resources.strInstallSelection);

            string licenseFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LicenseTerms.txt");

            Encoding isoLatin1 = Encoding.GetEncoding(28591);

            LicenseText = File.ReadAllText(licenseFile, isoLatin1);

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
                //System.Windows.MessageBox.Show(s);
            }
        }

        #endregion

        #endregion

    }
}
