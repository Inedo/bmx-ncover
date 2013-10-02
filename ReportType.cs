namespace Inedo.BuildMasterExtensions.NCover
{
    /// <summary>
    /// Represents a report type supported by NCover.
    /// </summary>
    internal sealed class ReportType
    {
        /// <summary>
        /// Gets or sets the name which identifies the report type.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets or sets a descriptive name of the report type.
        /// </summary>
        public string FriendlyName { get; private set; }

        /// <summary>
        /// An array containing all supported report types.
        /// </summary>
        public static readonly ReportType[] All =
        {
            new ReportType { Name = "FullCoverageReport", FriendlyName = "Full Coverage Report" },
            new ReportType { Name = "Summary", FriendlyName = "Summary" },
            new ReportType { Name = "UncoveredCodeSections", FriendlyName = "Uncovered Code Sections" },
            new ReportType { Name = "SymbolCCByGroup", FriendlyName = "Classes By Cyclomatic Complexity" }
        };

        /// <summary>
        /// Returns a report type with a specified name.
        /// </summary>
        /// <param name="name">Name of the report to get.</param>
        /// <returns>Report with this specified name or null if not found.</returns>
        public static ReportType Get(string name)
        {
            foreach (var type in All)
            {
                if (type.Name == name)
                    return type;
            }

            return null;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.FriendlyName;
        }
    }
}
