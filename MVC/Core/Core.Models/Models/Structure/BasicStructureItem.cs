namespace Core.Models
{

    public class BasicStructureItem<T> : BasicStructureItem
    {

        /// <summary>
        /// Creates a SectionItem model with additional model data.  The itemModel should correlate to the parent section's type and should be checked for the right type upon usage. 
        /// </summary>
        /// <param name="sectionModel"></param>
        public BasicStructureItem(T itemModel)
        {
            ItemModel = itemModel;
        }

        public Maybe<T> ItemModel { get; set; }
    }

        /// <summary>
        /// Basic Section Item model, can use this in many cases but may also want to use your own section item model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BasicStructureItem
    {
        public BasicStructureItem()
        {
        }

        public List<ContentItem> ContentItems { get; set; } = new List<ContentItem>();
        public List<ILinkItem> LinkItems { get; set; } = new List<ILinkItem>();
        public List<IVisualItem> VisualItems { get; set; } = new List<IVisualItem>();

        /// <summary>
        /// If there is a ILinkItem that is of type "General" then returns it.
        /// </summary>
        public Maybe<GeneralLink> GeneralLink { get
            {
                if(LinkItems
                            .FirstOrDefault(x => x.GetLinkType() == LinkType.General)
                            .AsMaybe()
                            .TryGetValue(out var genLinkVal) && genLinkVal is GeneralLink genLinkTyped)
                {
                    return genLinkTyped;
                } else
                {
                    return Maybe.None;
                }
            } 
        }
    }
}
