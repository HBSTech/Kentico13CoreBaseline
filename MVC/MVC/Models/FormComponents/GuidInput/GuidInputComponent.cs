using Generic.Models.FormComponents;
using Kentico.Forms.Web.Mvc;
using System;


// Registers a form component for use in the form builder
[assembly: RegisterFormComponent(GuidInputComponent.IDENTIFIER, typeof(GuidInputComponent), "Guid Value", Description = "Receives a Guid typed value.", IconClass = "icon-octothorpe")]
namespace Generic.Models.FormComponents
{
    /// <summary>
    /// Basic Guid form component
    /// </summary>
    public class GuidInputComponent : FormComponent<GuidInputComponentProperties, Guid>
    {
        public const string IDENTIFIER = "GuidInputFormComponent";

        [BindableProperty]
        public string Value { 
            get {
                return GuidValue == null ? "" : GuidValue.ToString();
            } set {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    GuidValue = new Guid(value);
                }
            } 
        }

        public Guid GuidValue { get; set; }

        // Gets the value of the form field instance passed from a view where the instance is rendered
        public override Guid GetValue()
        {
            return GuidValue;
        }

        public override void SetValue(Guid value)
        {
            GuidValue = value;
        }
    }
}