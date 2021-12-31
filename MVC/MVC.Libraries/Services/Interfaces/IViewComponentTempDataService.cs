using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MVCCaching;
namespace Generic.Services.Interfaces
{
    public interface IModelStateService : IService
    {
        void MergeModelState(ModelStateDictionary modelState, ITempDataDictionary tempData);
    }
}
