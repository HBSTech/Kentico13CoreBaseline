using CMS.DataEngine;
using Kentico.Forms.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Generic.Models.FormComponents
{
    public class DecimalInputComponentProperties : FormComponentProperties<decimal>
    {
        public decimal Value { get; set; }

        [DefaultValueEditingComponent(TextInputComponent.IDENTIFIER)]
        public override decimal DefaultValue { get => Value; set => Value = value; }

        bool DecimalPrecisionWasSet { get; set; } = false;
        bool DecimalSizeWasSet { get; set; } = false;
        [EditingComponentProperty("DecimalPrecision", 38)]
        public int DecimalPrecision
        {
            get {
                return (DecimalPrecisionWasSet ? Precision : -1);
            } set
            {
                this.Precision = value;
                DecimalPrecisionWasSet = true;
            }
        }

        [EditingComponentProperty("DecimalSize", 38)]
        public int DecimalSize
        {
            get
            {
                return (DecimalSizeWasSet ? Size : -1);
            }
            set
            {
                this.Size = value;
                DecimalSizeWasSet = true;
            }
        }

        // Initializes a new instance of the properties class and configures the underlying database field
        public DecimalInputComponentProperties()
            : base(FieldDataType.Decimal)
        {
        }

    }
}