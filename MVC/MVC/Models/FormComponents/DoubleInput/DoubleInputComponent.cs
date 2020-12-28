using Generic.Models.FormComponents;
using Kentico.Forms.Web.Mvc;


// Registers a form component for use in the form builder
[assembly: RegisterFormComponent(DoubleInputComponent.IDENTIFIER, typeof(DoubleInputComponent), "Double Value", Description = "Receives a Double typed value.", IconClass = "icon-octothorpe")]

namespace Generic.Models.FormComponents
{
    public class DoubleInputComponent : FormComponent<DoubleInputComponentProperties, double>
    {
        public const string IDENTIFIER = "DoubleInputFormComponent";
        
        
        [BindableProperty]
        public double Value { get; set; }


        // Gets the value of the form field instance passed from a view where the instance is rendered
        public override double GetValue()
        {
            return Value;
        }

        public override void SetValue(double value)
        {
            Value = value;
        }
    }
}