using System;
using System.IO;
using System.Text;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Extensibility.Actions.Reporting;
using Inedo.BuildMaster.Web;
using Inedo.BuildMaster.Data;

namespace Inedo.BuildMasterExtensions.NCover
{
    [ActionProperties(
        "Generate Code Coverage Report",
        "Executes NCover to generate a code coverage report for an application.",
        "NCover")]
    [CustomEditor(typeof(CodeCoverageReportActionEditor))]
    public class CodeCoverageReportAction : ReportingActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCoverageReportAction"/> class.
        /// </summary>
        public CodeCoverageReportAction()
        {
        }

        /// <summary>
        /// Gets or sets the coverage data file to produce.
        /// </summary>
        /// <remarks>
        /// If this is null or empty, a temporary file is generated.
        /// </remarks>
        [Persistent]
        public string CoverageDataFile { get; set; }
        /// <summary>
        /// Gets or sets the type of code coverage report to generate.
        /// </summary>
        /// <remarks>
        /// See the values in <see cref="ReportType.All"/> for a list of valid report types.
        /// </remarks>
        [Persistent]
        public string ReportType { get; set; }
        /// <summary>
        /// Gets or sets the assemblies to include in analysis.
        /// </summary>
        [Persistent]
        public string AssemblyIncludes { get; set; }
        /// <summary>
        /// Gets or sets the assemblies to exclude from analysis.
        /// </summary>
        [Persistent]
        public string AssemblyExcludes { get; set; }
        /// <summary>
        /// Gets or sets the attributes to include in analysis.
        /// </summary>
        [Persistent]
        public string AttributeIncludes { get; set; }
        /// <summary>
        /// Gets or sets the attributes to exclude from analysis.
        /// </summary>
        [Persistent]
        public string AttributeExcludes { get; set; }
        /// <summary>
        /// Gets or sets the source files to include in analysis.
        /// </summary>
        [Persistent]
        public string SourceIncludes { get; set; }
        /// <summary>
        /// Gets or sets the source files to exclude from analysis.
        /// </summary>
        [Persistent]
        public string SourceExcludes { get; set; }
        /// <summary>
        /// Gets or sets the types to include in analysis.
        /// </summary>
        [Persistent]
        public string ClassIncludes { get; set; }
        /// <summary>
        /// Gets or sets the types to exclude from analysis.
        /// </summary>
        [Persistent]
        public string ClassExcludes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Run NCover on {0} ({1}) and generate a {2} report.",
                this.ExePath,
                this.Arguments,
                Inedo.BuildMasterExtensions.NCover.ReportType.Get(this.ReportType)
            );
        }

        protected override void Execute()
        {
            var application = StoredProcs.Applications_GetApplication(this.Context.ApplicationId).ExecuteDataRow();
            var output = ExecuteRemoteCommand("ncover", (string)application[TableDefs.Applications_Extended.Application_Name]);
            var decoded = Convert.FromBase64String(output);
            this.OutputName = Util.CoalesceStr(this.OutputName, "NCover " + Inedo.BuildMasterExtensions.NCover.ReportType.Get(this.ReportType).FriendlyName);
            SubmitReport(decoded, ReportFormats.ZippedHtml);
        }
        protected override string ProcessRemoteCommand(string name, string[] args)
        {
            var configurer = (NCoverConfigurer)GetExtensionConfigurer();
            var ncoverConsolePath = Path.Combine(configurer.NCoverPath, "NCover.Console.exe");
            var ncoverReportingPath = Path.Combine(configurer.NCoverPath, "NCover.Reporting.exe");
            string coverageFilePath;
            if (!string.IsNullOrEmpty(this.CoverageDataFile))
                coverageFilePath = Path.Combine(this.RemoteConfiguration.TargetDirectory, this.CoverageDataFile);
            else
                coverageFilePath = Path.GetTempFileName();

            var applicationName = args[0];

            var consoleArgs = string.Format("\"{0}\" {1} {2}", this.ExePath, this.Arguments, GetNCoverConsoleArgs(coverageFilePath, applicationName));

            try
            {
                LogInformation(string.Format("Profiling {0} with NCover...", this.ExePath));
                ExecuteCommandLine(ncoverConsolePath, consoleArgs, this.RemoteConfiguration.SourceDirectory);

                var reportPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                try
                {
                    Directory.CreateDirectory(reportPath);

                    var reportingArgs = string.Format("\"{0}\" {1}", coverageFilePath, GetNCoverReportingArgs(reportPath, applicationName));
                    ExecuteCommandLine(ncoverReportingPath, reportingArgs, this.RemoteConfiguration.SourceDirectory);

                    return ZipAndEncodeDirectory(reportPath, this.ReportType.ToLower() + ".html");
                }
                finally
                {
                    Util.Files.DeleteFolder(reportPath);
                }
            }
            finally
            {
                if (string.IsNullOrEmpty(this.CoverageDataFile))
                    Util.Files.DeleteFile(coverageFilePath);
            }
        }

        private string GetNCoverConsoleArgs(string outputPath, string applicationName)
        {
            var buffer = new StringBuilder();
            buffer.Append("//bi ");
            buffer.Append(this.Context.BuildNumber);

            buffer.Append(" //p \"");
            buffer.Append(applicationName);
            buffer.Append('\"');

            AddString append = (a, v) =>
                {
                    if(!string.IsNullOrEmpty(v))
                    {
                        buffer.Append(' ');
                        buffer.Append(a);
                        buffer.Append(" \"");
                        buffer.Append(v);
                        buffer.Append('\"');
                    }
                };

            append("//ias", this.AssemblyIncludes);
            append("//eas", this.AssemblyExcludes);
            append("//ia", this.AttributeIncludes);
            append("//ea", this.AttributeExcludes);
            append("//if", this.SourceIncludes);
            append("//ef", this.SourceExcludes);
            append("//it", this.ClassIncludes);
            append("//et", this.ClassExcludes);

            buffer.Append(" //x \"");
            buffer.Append(outputPath);
            buffer.Append('\"');

            return buffer.ToString();
        }
        private string GetNCoverReportingArgs(string outputPath, string applicationName)
        {
            var buffer = new StringBuilder();
            buffer.Append("//bi ");
            buffer.Append(this.Context.BuildNumber);

            buffer.Append(" //p \"");
            buffer.Append(applicationName);
            buffer.Append('\"');

            buffer.Append(" //op \"");
            buffer.Append(outputPath);
            buffer.Append('\"');

            buffer.Append(" //or ");
            buffer.Append(this.ReportType);

            return buffer.ToString();
        }

        private delegate void AddString(string arg, string value);
    }
}
