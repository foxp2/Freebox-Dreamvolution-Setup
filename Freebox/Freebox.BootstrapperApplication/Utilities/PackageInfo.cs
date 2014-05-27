using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

using Freebox.BootstrapperApplication.Views;
using Freebox.BootstrapperApplication.Enums;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Reflection;

namespace Freebox.BootstrapperApplication.Utilities
{
    /// <summary>
    /// Contains information about the packages whitch will be installed.
    /// </summary>
    public class PackageInfo
    {
        [XmlAttribute("Package")]
        public string Id { get; set; }

        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }
    }
}
