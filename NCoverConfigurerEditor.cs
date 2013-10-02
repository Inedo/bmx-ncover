using System.Web.UI.WebControls;
using Inedo.BuildMaster.Extensibility.Configurers.Extension;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;

namespace Inedo.BuildMasterExtensions.NCover
{
    /// <summary>
    /// Custom editor for the NCover configurer.
    /// </summary>
    internal sealed class NCoverConfigurerEditor : ExtensionConfigurerEditorBase
    {
        private TextBox txtNCoverPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="NCoverConfigurerEditor"/> class.
        /// </summary>
        public NCoverConfigurerEditor()
        {
        }

        public override void InitializeDefaultValues()
        {
            BindToForm(new NCoverConfigurer());
        }
        public override void BindToForm(ExtensionConfigurerBase extension)
        {
            EnsureChildControls();

            var configurer = (NCoverConfigurer)extension;
            this.txtNCoverPath.Text = configurer.NCoverPath ?? string.Empty;
        }
        public override ExtensionConfigurerBase CreateFromForm()
        {
            EnsureChildControls();

            return new NCoverConfigurer
            {
                NCoverPath = this.txtNCoverPath.Text
            };
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.txtNCoverPath = new TextBox
            {
                Width = 300
            };

            CUtil.Add(this,
                new FormFieldGroup(
                    "NCover Install Path",
                    @"The directory where NCover is installed. This is normally C:\Program Files\NCover.",
                    true,
                    new StandardFormField(
                        "Install Path:",
                        this.txtNCoverPath
                    )
                )
            );
        }
    }
}
