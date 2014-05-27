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
    public class PopupViewModel : UserControl
    {
        #region Private Members
        private ICommand _nextCommand;        
        private ICommand _cancelCommand;       

        #endregion

        #region Constructors

        public PopupViewModel(Object c)
            
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
                                //MainWindowViewModel.WindowOpacity(true);                                                             
                                
                                if (System.Windows.MessageBox.Show(String.Format(Resources.strCancelText), String.Format(Resources.strCancel), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    //MainWindowViewModel.Canceled = true;
                                }
                                else
                                {
                                    //MainWindowViewModel.Canceled = false;
                                    //MainWindowViewModel.WindowOpacity(false);  
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

        #region Bootstrapper Event Hanlders

        private void InitializeEvents()
        {           

            //Bootstrapper.Error += ExecuteError;            

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
