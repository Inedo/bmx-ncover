using System.Web.UI.WebControls;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.NCover
{
    internal sealed  class CodeCoverageReportActionEditor : ActionEditorBase
    {
        private ValidatingTextBox txtExeFile;
        private ValidatingTextBox txtReportName;
        private TextBox txtExeArguments;
        private DropDownList ddlReportType;
        private TextBox txtReportFile;
        private TextBox txtAssemblyIncludes;
        private TextBox txtAssemblyExcludes;
        private TextBox txtAttributeIncludes;
        private TextBox txtAttributeExcludes;
        private TextBox txtSourceIncludes;
        private TextBox txtSourceExcludes;
        private TextBox txtClassIncludes;
        private TextBox txtClassExcludes;

        public CodeCoverageReportActionEditor()
        {
        }

        public override bool DisplaySourceDirectory
        {
            get { return true; }
        }
        public override bool DisplayTargetDirectory
        {
            get { return true; }
        }

        public override void BindToForm(ActionBase extension)
        {
            EnsureChildControls();

            var action = (CodeCoverageReportAction)extension;
            this.txtExeFile.Text = action.ExePath ?? "";
            this.txtExeArguments.Text = action.Arguments ?? "";
            this.ddlReportType.SelectedValue = action.ReportType ?? "";
            this.txtReportName.Text = action.OutputName ?? "";
            this.txtAssemblyIncludes.Text = action.AssemblyIncludes ?? "";
            this.txtAssemblyExcludes.Text = action.AssemblyExcludes ?? "";
            this.txtAttributeIncludes.Text = action.AttributeIncludes ?? "";
            this.txtAttributeExcludes.Text = action.AttributeExcludes ?? "";
            this.txtSourceIncludes.Text = action.SourceIncludes ?? "";
            this.txtSourceExcludes.Text = action.SourceExcludes ?? "";
            this.txtClassIncludes.Text = action.ClassIncludes ?? "";
            this.txtClassExcludes.Text = action.ClassExcludes ?? "";
        }
        public override ActionBase CreateFromForm()
        {
            EnsureChildControls();

            return new CodeCoverageReportAction
            {
                ExePath = this.txtExeFile.Text,
                Arguments = this.txtExeArguments.Text,
                ReportType = this.ddlReportType.SelectedValue,
                OutputName = this.txtReportName.Text,
                AssemblyIncludes = this.txtAssemblyIncludes.Text,
                AssemblyExcludes = this.txtAssemblyExcludes.Text,
                AttributeIncludes = this.txtAttributeIncludes.Text,
                AttributeExcludes = this.txtAttributeExcludes.Text,
                SourceIncludes = this.txtSourceIncludes.Text,
                SourceExcludes = this.txtSourceExcludes.Text,
                ClassIncludes = this.txtClassIncludes.Text,
                ClassExcludes = this.txtClassExcludes.Text
            };
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.txtExeFile = new ValidatingTextBox
            {
                Required = true,
                Width = 300
            };

            this.txtReportName = new ValidatingTextBox
            {
                Width = 300
            };

            this.txtExeArguments = new TextBox { Width = 300 };

            this.ddlReportType = new DropDownList();
            foreach (var reportType in ReportType.All)
                this.ddlReportType.Items.Add(new ListItem(reportType.FriendlyName, reportType.Name));

            this.txtReportFile = new TextBox { Width = 300 };

            Func<TextBox> newBigBox = () => new TextBox { Width = 300, TextMode = TextBoxMode.MultiLine, Rows = 3 };

            this.txtAssemblyIncludes = newBigBox();
            this.txtAssemblyExcludes = newBigBox();
            this.txtAttributeIncludes = newBigBox();
            this.txtAttributeExcludes = newBigBox();
            this.txtSourceIncludes = newBigBox();
            this.txtSourceExcludes = newBigBox();
            this.txtClassIncludes = newBigBox();
            this.txtClassExcludes = newBigBox();

            CUtil.Add(this,
                new RenderJQueryDocReadyDelegator(w => w.WriteLine("$('#" + this.txtReportFile.ClientID + "').inedobm_defaulter({defaultText:'default'});")),
                new FormFieldGroup(
                    "Application to Profile",
                    "Provide the executable to profile with NCover and any command line arguments to pass to it. The executable path may either be absolute or relative to the source directory.",
                    false,
                    new StandardFormField(
                        "Executable:",
                        this.txtExeFile
                    ),
                    new StandardFormField(
                        "Arguments:",
                        this.txtExeArguments
                    )
                ),
                new FormFieldGroup(
                    "Report Type",
                    "Specify the type of report to generate. Consult NCover documentation for information about the types of reports.",
                    false,
                    new StandardFormField(
                        "Report Type:",
                        this.ddlReportType
                    )
                ),
                new FormFieldGroup(
                    "Data File",
                    "Specify the profiler data file to generate, relative to the target path. If a report file is not specified, a temporary file is used.",
                    false,
                    new StandardFormField(
                        "Data File:",
                        this.txtReportFile
                    )
                ),
                new FormFieldGroup(
                    "Report Name",
                    "Provide the name of the report as it will appear in BuildMaster. This is optional.",
                    false,
                    new StandardFormField(
                        "Report Name (Optional):",
                        this.txtReportName
                    )
                ),
                new FormFieldGroup(
                    "Includes and Excludes",
                    "Specify the rules to apply for including or excluding code from analysis. Rules are semicolon-separated Perl-compatible regular expressions.",
                    true,
                    new StandardFormField(
                        "Assemblies to Include:",
                        this.txtAssemblyIncludes
                    ),
                    new StandardFormField(
                        "Assemblies to Exclude:",
                        this.txtAssemblyExcludes
                    ),
                    new StandardFormField(
                        "Attributes to Include:",
                        this.txtAttributeIncludes
                    ),
                    new StandardFormField(
                        "Attributes to Exclude:",
                        this.txtAttributeExcludes
                    ),
                    new StandardFormField(
                        "Source Files to Include:",
                        this.txtSourceIncludes
                    ),
                    new StandardFormField(
                        "Source Files to Exclude:",
                        this.txtSourceExcludes
                    ),
                    new StandardFormField(
                        "Types to Include:",
                        this.txtClassIncludes
                    ),
                    new StandardFormField(
                        "Types to Exclude:",
                        this.txtClassExcludes
                    )
                )
            );
        }
    }
}
