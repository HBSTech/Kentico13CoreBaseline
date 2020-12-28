using CMS.DataEngine;
using Kentico.Forms.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models.FormComponents
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