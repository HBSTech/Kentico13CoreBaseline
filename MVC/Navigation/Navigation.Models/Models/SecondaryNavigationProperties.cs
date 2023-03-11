namespace Navigation.Models
{


    public record SecondaryNavigationProperties()
    {
        /// <summary>
        /// The Path that the navigation properties build off of.  If empty or not provided, will use the current page's path.
        /// </summary>
        public Maybe<string> Path { get; set; }

        /// <summary>
        /// The level the navigation should start at.  0 = At the root (if LevelIsRelative is false), or 0 = Current page's level (if LevelIsRelative is true)
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// If the given Level is relative to the current page's level.  If true, then the Level dictates what parent is the start point.  A level of 2 will go up 2 levels. 
        /// </summary>
        public bool LevelIsRelative { get; set; } = true;

        /// <summary>
        /// How many levels down from the start of the navigation it should show entries.
        /// </summary>
        public int MinimumAbsoluteLevel { get; set; } = 2;

        /// <summary>
        /// The CSS class that will wrap this navigation.  useful both for styling and for the Navigation Page Selector
        /// </summary>
        public string CssClass { get; set; } = string.Empty;

        /// <summary>
        /// If true, will include the client-side javascript that sets the active
        /// </summary>
        public bool IncludeSecondaryNavSelector { get; set; } = true;
    }
}
