using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Core.Services
{
    public interface IModelStateService
    {
        /// <summary>
        /// Merges the Model State (Validation state and errors) from the TempData, this should be called in Post-Redirect-Get methedology so the model state can persist between the POST action and the redirected GET.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="tempData"></param>
        void MergeModelState(ModelStateDictionary modelState, ITempDataDictionary tempData);

        /// <summary>
        /// Stores the View model in the Temp Data, this is used in Post-Redirect-Get when the controller's POST modifies the view model and then redirects to the original request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tempData"></param>
        /// <param name="viewModel"></param>
        void StoreViewModel<T>(ITempDataDictionary tempData, T viewModel);

        /// <summary>
        /// Gets the View Model from the TempData, used in Post-Redirect-Get when redirecting back to the original request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tempData"></param>
        /// <returns></returns>
        Result<TModel> GetViewModel<TModel>(ITempDataDictionary tempData);

        /// <summary>
        /// Removes the View Model from TempData, this should be called if you are redirecting away from the calling view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tempData"></param>
        void ClearViewModel<T>(ITempDataDictionary tempData);
    }
}
