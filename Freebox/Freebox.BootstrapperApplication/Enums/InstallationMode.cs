using System;

namespace Freebox.BootstrapperApplication.Enums
{
    /// <summary>
    /// Enum with posibile mode of an installation(Install, Uninstall, Repair).
    /// </summary>
    public enum InstallationMode
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined,
        /// <summary>
        /// User has choosen to install the product
        /// </summary>
        Install,

        /// <summary>
        /// User has choosen to uninstall the product
        /// </summary>
        Uninstall,

        /// <summary>
        /// User has choosen to repair the product
        /// </summary>
        Repair,

        /// <summary>
        /// User has choosen to cancel the installation
        /// </summary>
        Cancel
    }
}
