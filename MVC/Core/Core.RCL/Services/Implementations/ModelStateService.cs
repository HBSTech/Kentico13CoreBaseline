using Core.Attributes;
using Core.Helpers;
using Core.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MVCCaching;
using System.Text.Json;

namespace Core.Services.Implementations
{
    [AutoDependencyInjection]
    public class ModelStateService : IModelStateService
    {
        public void MergeModelState(ModelStateDictionary modelState, ITempDataDictionary tempData)
        {
            string key = typeof(ModelStateTransfer).FullName ?? typeof(ModelStateTransfer).Name;
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
                        if (stateItem != null)
                        {
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

        public void StoreViewModel<TModel>(ITempDataDictionary tempData, TModel viewModel)
        {
            tempData.Put<TModel>($"GetViewModel_{typeof(TModel).FullName}", viewModel);
        }

        public Result<TModel> GetViewModel<TModel>(ITempDataDictionary tempData)
        {
            var obj = tempData.Get<TModel>($"GetViewModel_{typeof(TModel).FullName}");
            return obj != null ? (TModel)obj : Result.Failure<TModel>("No model found");
        }

        public void ClearViewModel<TModel>(ITempDataDictionary tempData)
        {
            tempData.Remove($"GetViewModel_{typeof(TModel).FullName}");
        }
    }

    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value)
        {
            tempData[key] = JsonSerializer.Serialize<T>(value);
        }

        public static T? Get<T>(this ITempDataDictionary tempData, string key)
        {
            if (tempData.TryGetValue(key, out var o))
            {
                return o == null ? default : JsonSerializer.Deserialize<T>((string)o);
            }
            return default;
        }
    }
}
