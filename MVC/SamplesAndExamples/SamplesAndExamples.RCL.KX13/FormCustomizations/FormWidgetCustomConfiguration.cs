using CMS;
using CMS.DataEngine;
using Generic.Library;
using Kentico.Forms.Web.Mvc;
using Kentico.Forms.Web.Mvc.Widgets;

[assembly: RegisterModule(typeof(FormWidgetCustomConfiguration))]

namespace Generic.Library
{
    /// <summary>
    /// This can be modified to adjust the Form Builder Widget, the current configuration is to make it render
    /// forms that work with the Bootstrap Layout Tool.  You can modify as you wish!
    /// </summary>
    public class FormWidgetCustomConfiguration : Module
    {
        // Module class constructor, the system registers the module under the name "CustomInit"
        public FormWidgetCustomConfiguration()
            : base("FormWidgetCustomConfiguration")
        {
        }

        // Contains initialization code that is executed when the application starts
        protected override void OnInit()
        {
            base.OnInit();
            // Modifies the default FormFieldRenderingConfiguration for the 'Form' widget
            // Specifying a new FormFieldRenderingConfiguration instance completely replaces the default Kentico configuration
            FormFieldRenderingConfiguration.Widget.RootConfiguration = null;
            FormFieldRenderingConfiguration.Widget.LabelWrapperConfiguration = null;
            FormFieldRenderingConfiguration.Widget.EditorWrapperConfiguration = null;
            FormFieldRenderingConfiguration.Widget.ComponentWrapperConfiguration = null;
            FormFieldRenderingConfiguration.Widget.ExplanationTextWrapperConfiguration = null;
            FormFieldRenderingConfiguration.Widget.ColonAfterLabel = false;
            FormFieldRenderingConfiguration.Widget.ExplanationTextWrapperConfiguration = new ElementRenderingConfiguration
            {
                ElementName = "small",
                HtmlAttributes = { { "class", "form-text text-muted" } } // can drop the "form-text" if doing inline
            };

            // Sets a new rendering configuration for the 'Form' widget, adding attributes
            // to the form element and the submit button and wrapping the form in two 'div' blocks
            FormWidgetRenderingConfiguration.Default = new FormWidgetRenderingConfiguration
            {
                // Submit button HTML attributes
                SubmitButtonHtmlAttributes = { { "class", "btn btn-primary" } },

            };

            // Customizations here
            FormFieldRenderingConfiguration.GetConfiguration.Execute += BootstrapifyRendering;

            FormFieldRenderingConfiguration.GetConfiguration.Execute += RequiredAttributeRendering;
        }

        private void RequiredAttributeRendering(object? sender, GetFormFieldRenderingConfigurationEventArgs e)
        {
            if (e.FormComponent.BaseProperties.Required)
            {
                e.Configuration.EditorHtmlAttributes.Add("required", "required");
                e.FormComponent.BaseProperties.Placeholder += "*";
            }
        }

        private void BootstrapifyRendering(object? sender, GetFormFieldRenderingConfigurationEventArgs e)
        {
            string formName = e.FormComponent.GetBizFormComponentContext().FormInfo.FormName;
            string formFieldName = e.FormComponent.Name;

            switch (formName)
            {
                // https://getbootstrap.com/docs/4.0/components/forms/#form-grid
                default:
                    // Set Bootstrap 4 Layout properties to:
                    // form-group Automation: On Column (or none if you do not want to use form groups)
                    // Columns: any #
                    // Set column sizes to whatever you wish
                    // Put various form elements within the columns
                    break;

                // https://getbootstrap.com/docs/4.0/components/forms/#horizontal-form
                // Set Bootstrap 4 Layout properties to:
                // form-group Automation: On Row
                // Columns: 1
                // 1st Column Size: Do not render column div
                // (put various form elements within the div)
                case "Example_LabelInputSideBySide":
                    switch (formFieldName)
                    {
                        case "FirstName":
                        case "LastName":

                            e.Configuration.LabelHtmlAttributes["class"] = "col-sm-2";
                            e.Configuration.EditorWrapperConfiguration = new ElementRenderingConfiguration
                            {
                                ElementName = "div",
                                HtmlAttributes = { { "class", "col-sm-4" } }
                            };
                            break;
                        default:
                            e.Configuration.LabelHtmlAttributes["class"] = "col-sm-2";
                            e.Configuration.EditorWrapperConfiguration = new ElementRenderingConfiguration
                            {
                                ElementName = "div",
                                HtmlAttributes = { { "class", "col-sm-2" } }
                            };
                            break;
                    }
                    break;
                // https://getbootstrap.com/docs/4.0/components/forms/#inline-forms
                // Set Bootstrap 4 Layout properties to:
                // form-group Automation: none
                // Container CSS: form-inline
                // Columns: 1
                // 1st Column Size: Do not render column div
                // (put various form elements within the div)
                case "Example_FormInline":
                    e.Configuration.LabelHtmlAttributes["class"] = "sr-only";
                    e.Configuration.EditorHtmlAttributes["class"] = "form-control mb-2 mr-sm-2";
                    break;
            }

            // https://getbootstrap.com/docs/4.0/components/forms/#select-menu
            if (e.FormComponent is DropDownComponent)
            {
                if (e.Configuration.EditorHtmlAttributes.ContainsKey("class"))
                {
                    e.Configuration.EditorHtmlAttributes["class"] += " custom-select";
                }
                else
                {
                    e.Configuration.EditorHtmlAttributes["class"] = " custom-select";
                }
            }

        }
    }

}