using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

using Freebox.BootstrapperApplication.Helpers;
using Freebox.BootstrapperApplication.ViewModels;
using Freebox.BootstrapperApplication.Views;
using Freebox.BootstrapperApplication.Utilities;

namespace Freebox.BootstrapperApplication
{
    /// <summary>
    /// Model class for the main window and for the other windows.
    /// </summary>
    public class Model
    {
        #region Private Members

        private BundleInfo _bundleInfo;
        private string _installDir;

        #endregion

        #region Constructors

        /// <summary>
        ///  Constructor of the main model.
        /// </summary>
        public Model(BootstrapperApplication bootstrapper)
        {
            Bootstrapper = bootstrapper;
        }

        #endregion

        #region Properties

        /// <summary>
        /// BootstrapperApplication
        /// </summary>
        public BootstrapperApplication Bootstrapper
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the command from the BootstrapperApplication
        /// </summary>
        public Command Command
        {
            get { return Bootstrapper.Command; }
        }

        /// <summary>
        /// Gets the Engine of the BootstrapperApplication
        /// </summary>
        public Engine Engine
        {
            get { return Bootstrapper.Engine; }
        }

        /// <summary>
        /// Stores the return value of the Burn engine after the installation.
        /// </summary>
        public int FinalResult
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the bundle info from the 
        /// </summary>
        public BundleInfo BundleInfo
        {
            get
            {
                if (_bundleInfo == null)
                {
                    _bundleInfo = BundleInfoLoader.Load();
                }
                return _bundleInfo;
            }
        }

        /// <summary>
        /// Gets or sets the planned action of the bootsrtaper engine.
        /// </summary>
        public LaunchAction PlannedAction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InstallDir
        {
            get
            {
                if (String.IsNullOrEmpty(_installDir))
                {
                    if (Engine.StringVariables.Contains("INSTALLDIR"))
                    {
                        _installDir = Engine.StringVariables["INSTALLDIR"];
                    }
                }
                return _installDir.Trim('"');
            }
            set
            {
                _installDir = value;
                // Update also the install dir from the engine 
                if (Engine.StringVariables.Contains("INSTALLDIR"))
                {
                    Engine.StringVariables["INSTALLDIR"] = "\"" + _installDir + "\"";
                }
            }
        }
        #endregion
    }
}
