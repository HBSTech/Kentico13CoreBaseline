using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace Core.Helpers
{
    /// <summary>
    /// https://andrewlock.net/post-redirect-get-using-tempdata-in-asp-net-core/
    /// </summary>
    public static class ModelStateHelpers
    {
        public static string SerialiseModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Select(kvp => new ModelStateTransferValue(kvp.Key)
                {
                    AttemptedValue = kvp.Value?.AttemptedValue,
                    RawValue = kvp.Value?.RawValue,
                    ErrorMessages = kvp.Value != null ? kvp.Value.Errors.Select(err => err.ErrorMessage).ToList() : Array.Empty<string>(),
                });

            return JsonSerializer.Serialize(errorList);
        }

        public static ModelStateDictionary DeserialiseModelState(string serialisedErrorList)
        {
            var errorList = JsonSerializer.Deserialize<List<ModelStateTransferValue>>(serialisedErrorList);
            var modelState = new ModelStateDictionary();

            if (errorList != null)
            {
                foreach (var item in errorList)
                {
                    modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                    foreach (var error in item.ErrorMessages)
                    {
                        modelState.AddModelError(item.Key, error);
                    }
                }
            }
            return modelState;
        }
    }

    public class ModelStateTransferValue
    {
        public ModelStateTransferValue(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
        public string? AttemptedValue { get; set; }
        public object? RawValue { get; set; }
        public ICollection<string> ErrorMessages { get; set; } = new List<string>();
    }
}
