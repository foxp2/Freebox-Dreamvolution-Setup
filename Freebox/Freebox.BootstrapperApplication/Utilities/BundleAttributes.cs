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
    /// Contains attributes info of the bundle.
    /// </summary>
    public class BundleAttributes
    {
        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; }

    }
}
