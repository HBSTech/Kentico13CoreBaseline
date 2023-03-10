using CMS.DataEngine;
using Kentico.Forms.Web.Mvc;

namespace Core.RCL.KX13.Models.FormComponents.DoubleInput
{
    public class DoubleInputComponentProperties : FormComponentProperties<double>
    {
        public double Value { get; set; }

        [DefaultValueEditingComponent(TextInputComponent.IDENTIFIER)]
        public override double DefaultValue { get => Value; set => Value = value; }

        // Initializes a new instance of the properties class and configures the underlying database field
        public DoubleInputComponentProperties()
            : base(FieldDataType.Double)
        {
        }

    }
}