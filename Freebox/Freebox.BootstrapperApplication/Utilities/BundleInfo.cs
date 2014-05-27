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

using Freebox.BootstrapperApplication.Views;
using Freebox.BootstrapperApplication.Enums;

namespace Freebox.BootstrapperApplication.Utilities
{
    /// <summary>
    /// Contains information of the bundle.
    /// </summary>
    [XmlRoot("BootstrapperApplicationData", IsNullable = false, Namespace = "http://schemas.microsoft.com/wix/2010/BootstrapperApplicationData")]
    public class BundleInfo
    {
        #region Private Members

        // List of packages which will be installed.
        private Collection<PackageInfo> _packages;

        #endregion

        #region Properties

        [XmlElement("WixBundleProperties")]
        public BundleAttributes BundleAttributes { get; set; }

        [XmlElement("WixPackageProperties")]
        public Collection<PackageInfo> Packages
        {
            get
            {
                return _packages;
            }
        }

        #endregion

        #region Constructor

        public BundleInfo()
        {
            _packages = new Collection<PackageInfo>();
        }

        #endregion
    }
}
