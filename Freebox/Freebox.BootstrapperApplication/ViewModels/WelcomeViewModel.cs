using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Documents;
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
    public class WelcomeViewModel : ViewModelBase
    {
        #region Private Members
        private ICommand _nextCommand;        
        private ICommand _cancelCommand;       

        #endregion

        #region Constructors

        public WelcomeViewModel(MainWindowViewModel mainViewModel)
            : base(mainViewModel)
        {
            InitializeEvents();
        }

        #endregion

        #region Properties

        public string Title
        {
            get;
            set;
        }

        #endregion

        #region Commands

        /// <summary>
        /// go to ReadMe Window
        /// </summary>

		public ICommand NextCommand
		{
			get
			{
				if (_nextCommand == null)
				{
					_nextCommand = new RelayCommand(
						param =>
						{
							lock (this)
							{
								Model.Bootstrapper.Dispatcher.BeginInvoke(
                                new Action(() =>
                                {
                                   var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                                   timer.Start();
                                   timer.Tick += (sender, args) =>
                                   {
                                       timer.Stop();
                                       MainWindowViewModel.CurrentView = new InstallView(MainWindowViewModel.InstallViewModel);
                                   };             
                                   
                                }));
							}
						},
						param => true);
				}
				return _nextCommand;
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

            Bootstrapper.Error += ExecuteError;

            Title = String.Format(Resources.strWelcometext, Model.BundleInfo.BundleAttributes.DisplayName);

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
