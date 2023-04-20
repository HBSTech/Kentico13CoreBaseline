namespace Core.Services
{
	public interface ILinkItemService
	{
        /// <summary>
        /// Gets the Link Beginning/End HTML given the Link item and optional title/css
        /// </summary>
        /// <param name="linkItem"></param>
        /// <param name="title"></param>
        /// <param name="cssClass"></param>
        /// <returns>The Link HTML Start (Item1) and the Link HTML End (Item2) in Tuple form</returns>
        Tuple<string, string> GetLinkBeginningEnd(ILinkItem linkItem, string? title = null, string? cssClass = null);

        /// <summary>
        /// Get the full link HTML given the Link Item and optional title/css
        /// </summary>
        /// <param name="linkItem"></param>
        /// <param name="title"></param>
        /// <param name="cssClass"></param>
        /// <returns>the full link 'a' tag</returns>
        string GetLinkHtml(ILinkItem linkItem, string? title, string? cssClass);
    }
}
