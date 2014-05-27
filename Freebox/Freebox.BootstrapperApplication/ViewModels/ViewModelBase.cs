using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

using Freebox.BootstrapperApplication.Helpers;
using Freebox.BootstrapperApplication.Enums;

namespace Freebox.BootstrapperApplication.ViewModels
{
    /// <summary>
    /// Base class for the viewmodels of the usercontrols (views).
    /// This class provides access to the BootstrapperApplication/Engine/Command.
    /// </summary>
    public class ViewModelBase : NotifyPropertyChanged
    {
        #region Private Members

        private MainWindowViewModel _mainViewModel;

        #endregion

        #region Constructors

        public ViewModelBase(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Model
        /// </summary>
        public Model Model
        {
            get { return _mainViewModel.Model; }
        }

        /// <summary>
        /// Gets the BootstrapperApplication
        /// </summary>
        public BootstrapperApplication Bootstrapper
        {
            get { return _mainViewModel.Model.Bootstrapper; }
        }

        /// <summary>
        /// Gets the command from the BootstrapperApplication
        /// </summary>
        public Command Command
        {
            get { return _mainViewModel.Model.Bootstrapper.Command; }
        }

        /// <summary>
        /// Gets the Engine of the BootstrapperApplication
        /// </summary>
        public Engine Engine
        {
            get { return _mainViewModel.Model.Bootstrapper.Engine; }
        }

        /// <summary>
        /// Gets or sets the BurnInstallationState
        /// </summary>
        public BurnInstallationState BurnInstallationState
        {
            get
            {
                return _mainViewModel.BurnInstallationState;
            }
            set
            {
                _mainViewModel.BurnInstallationState = value;
            }
        }

        /// <summary>
        /// Gets or sets the InstallationMode
        /// </summary>
        public InstallationMode InstallationMode
        {
            get
            {
                return _mainViewModel.InstallationMode;
            }
            set
            {
                _mainViewModel.InstallationMode = value;
            }
        }

        /// <summary>
        /// Gets the view model of the main window.
        /// </summary>
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return _mainViewModel;
            }
        }
        #endregion
    }
}
