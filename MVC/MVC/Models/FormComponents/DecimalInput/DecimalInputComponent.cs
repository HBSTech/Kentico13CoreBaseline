using Generic.Models.FormComponents;
using Kentico.Forms.Web.Mvc;
using System;


// Registers a form component for use in the form builder
[assembly: RegisterFormComponent(DecimalInputComponent.IDENTIFIER, typeof(DecimalInputComponent), "Decimal Value", Description = "Receives a Decimal typed value.", IconClass = "icon-octothorpe")]

namespace Generic.Models.FormComponents
{
    /// <summary>
    /// Basic Decimal Form Component, has two EditingComponentProperties of DecimalSize and DecimalPrecision which will alter the inputed value to match.
    /// </summary>
    public class DecimalInputComponent : FormComponent<DecimalInputComponentProperties, decimal>
    {
        public const string IDENTIFIER = "DecimalInputFormComponent";


        public decimal _Value;

        [BindableProperty]
        public decimal Value { get
            {
                return _Value;
            }
            set
            {
                // Handle custom precision and size
                decimal NewVal = value;
                if(Properties.DecimalPrecision >= 0)
                {
                    NewVal = decimal.Round(NewVal, Properties.DecimalPrecision);
                }
                if(Properties.DecimalSize > 0)
                {
                    int Size = Properties.DecimalSize;
                    if(Properties.DecimalPrecision >= 0)
                    {
                        Size -= Properties.DecimalPrecision;
                    }
                    
                    if(NewVal >= Convert.ToDecimal(Math.Pow(10, Size)))
                    {
                        string[] DecimalParts = NewVal.ToString().Split('.');
                        NewVal = Convert.ToDecimal(string.Join(".", new string[] { DecimalParts[0].Substring(DecimalParts[0].Length - Size), (DecimalParts.Length > 1 ? DecimalParts[1] : "0") }));
                    }
                }

                _Value = NewVal;
            }
        }

        // Gets the value of the form field instance passed from a view where the instance is rendered
        public override decimal GetValue()
        {
            return Value;
        }

        public override void SetValue(decimal value)
        {
            Value = value;
        }
    }
}