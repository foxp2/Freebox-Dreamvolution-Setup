using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Freebox.BootstrapperApplication.Enums
{
    /// <summary>
    /// Enum with posibile states of the Burn installation.
    /// </summary>
    public enum BurnInstallationState
    {
        /// <summary>
        /// Installation of the product is beeing initialized.
        /// </summary>
        Initializing,

        /// <summary>
        /// The installation of the product already exists on the computer.
        /// </summary>
        Present,

        /// <summary>
        /// The product is not installed on the computer.
        /// </summary>
        NotPresent,

        /// <summary>
        /// A newer version of the product was already installed on this computer.
        /// </summary>
        Newer,

        /// <summary>
        /// The installation of the product is running.
        /// </summary>
        Applying,

        /// <summary>
        /// The installation of the product is done.
        /// </summary>
        Applied,

        /// <summary>
        /// The installation has failed.
        /// </summary>
        Failed

    }
}
