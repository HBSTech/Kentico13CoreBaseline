namespace Localization.Repositories
{
    public interface ILocalizedCategoryCachedRepository
    {
        LocalizedCategoryItem LocalizeCategoryItem(CategoryItem categoryItem, string cultureCode);
        IEnumerable<LocalizedCategoryItem> LocalizeCategoryItems(IEnumerable<CategoryItem> categories, string cultureCode);
    }
}
