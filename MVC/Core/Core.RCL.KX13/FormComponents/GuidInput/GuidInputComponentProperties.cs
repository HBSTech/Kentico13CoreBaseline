using CMS.DataEngine;
using Kentico.Forms.Web.Mvc;
using System;

namespace Core.RCL.KX13.Models.FormComponents.GuidInput
{
    public class GuidInputComponentProperties : FormComponentProperties<Guid>
    {
        public Guid Value { get; set; }

        [DefaultValueEditingComponent(TextInputComponent.IDENTIFIER)]
        public override Guid DefaultValue { get => Value; set => Value = value; }

        // Initializes a new instance of the properties class and configures the underlying database field
        public GuidInputComponentProperties()
            : base(FieldDataType.Guid)
        {
        }

    }
}