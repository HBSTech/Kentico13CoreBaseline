namespace Core.Repositories
{
    /// <summary>
    /// Kentico agnostic version of the IPageBuilderContext, only used to separate library usage
    /// </summary>
    public interface IBaselinePageBuilderContext
    {
        bool IsPreviewMode { get; }

        //
        // Summary:
        //     True if XperienceCommunity.PageBuilderUtilities.IPageBuilderContext.IsLivePreviewMode
        //     and XperienceCommunity.PageBuilderUtilities.IPageBuilderContext.IsEditMode is
        //     false. Also the opposite of XperienceCommunity.PageBuilderUtilities.IPageBuilderContext.IsPreviewMode
        //
        // Remarks:
        //     True if the Page Builder Mode cannot be determined (ex no HttpContext, Xperience
        //     features not initialized)
        bool IsLiveMode { get; }

        //
        // Summary:
        //     True if the current request is being made for a preview version of the Page with
        //     editing disabled
        //
        // Remarks:
        //     False if the Page Builder Mode cannot be determined (ex no HttpContext, Xperience
        //     features not initialized)
        bool IsLivePreviewMode { get; }

        //
        // Summary:
        //     True if the current request is being made for the Page Builder experience
        //
        // Remarks:
        //     False if the Page Builder Mode cannot be determined (ex no HttpContext, Xperience
        //     features not initialized)
        bool IsEditMode { get; }

        //
        // Summary:
        //     The current Mode as a XperienceCommunity.PageBuilderUtilities.PageBuilderMode
        //     value
        BaselinePageBuilderMode Mode { get; }

        //
        // Summary:
        //     The value of XperienceCommunity.PageBuilderUtilities.IPageBuilderContext.Mode
        //     as a string
        string ModeName();
    }
}
