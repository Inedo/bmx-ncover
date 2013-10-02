using System;
using System.IO;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Configurers.Extension;
using Inedo.BuildMaster.Web;
using Microsoft.Win32;

[assembly: ExtensionConfigurer(typeof(Inedo.BuildMasterExtensions.NCover.NCoverConfigurer))]

namespace Inedo.BuildMasterExtensions.NCover
{
    /// <summary>
    /// Extension configurer for the NCover extension.
    /// </summary>
    [CustomEditor(typeof(NCoverConfigurerEditor))]
    public sealed class NCoverConfigurer : ExtensionConfigurerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NCoverConfigurer"/> class.
        /// </summary>
        public NCoverConfigurer()
        {
            this.NCoverPath = GetInstallPath()
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NCover");
        }

        /// <summary>
        /// Gets or sets the path of the NCover installation directory.
        /// </summary>
        [Persistent]
        public string NCoverPath { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the path where NCover is installed on the local machine.
        /// </summary>
        /// <returns>Path where NCover is installed or null if it was not found.</returns>
        private static string GetInstallPath()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Gnoso\NCover", false))
                {
                    if (key == null)
                        return null;

                    return key.GetValue("InstallDir") as string;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
