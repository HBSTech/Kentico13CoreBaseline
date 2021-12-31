using Generic.Libraries.Attributes;
using Generic.Libraries.Helpers;
using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Services.Implementations
{
    public class ModelStateService : IModelStateService
    {
        public void MergeModelState(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState, ITempDataDictionary tempData)
        {
            string key = typeof(ModelStateTransfer).FullName;
            var serialisedModelState = tempData[key] as string;

            if (serialisedModelState != null)
            {
                var retrievedModelState = ModelStateHelpers.DeserialiseModelState(serialisedModelState);
                // Merge
                if (modelState.Keys.Any())
                {
                    retrievedModelState.Merge(modelState);
                }
                else
                {
                    // Populate
                    foreach (string modelStateKey in retrievedModelState.Keys)
                    {
                        var stateItem = retrievedModelState[modelStateKey];
                        modelState.SetModelValue(modelStateKey, stateItem.RawValue, stateItem.AttemptedValue);
                        foreach (var error in stateItem.Errors)
                        {
                            modelState.AddModelError(modelStateKey, error.ErrorMessage);
                        }
                    }
                }
            }
        }
    }
}
