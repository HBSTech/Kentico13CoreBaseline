using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Taxonomy;
using MVCCaching;
using MVCCaching.Base.Core.Interfaces;
using System.Data;

namespace Localization.Repositories.Implementations
{
    public class LocalizedCategoryCachedRepository : ILocalizedCategoryCachedRepository
    {
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly IProgressiveCache _progressiveCache;
        private readonly LocalizationConfiguration _configuration;

        public LocalizedCategoryCachedRepository(ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            IProgressiveCache progressiveCache,
            LocalizationConfiguration configuration)
        {
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _progressiveCache = progressiveCache;
            _configuration = configuration;
        }

        public LocalizedCategoryItem LocalizeCategoryItem(CategoryItem categoryItem, string cultureCode)
        {
            var localizationDictionary = GetLocalizedItemsCached();
            return LocalizeCategoryItem(categoryItem, cultureCode, localizationDictionary);
        }

        public IEnumerable<LocalizedCategoryItem> LocalizeCategoryItems(IEnumerable<CategoryItem> categories, string cultureCode)
        {
            var localizationDictionary = GetLocalizedItemsCached();
            return categories.Select(x => LocalizeCategoryItem(x, cultureCode, localizationDictionary));
        }

        private LocalizedCategoryItem LocalizeCategoryItem(CategoryItem categoryItem, string cultureCode, Dictionary<int, LocalizedCategoryValues> localizationDictionary )
        {
            var localizedCategoryItem = categoryItem.ToLocalizedCategoryItem();
            string cultureOrLanguage = cultureCode.ToLower();
            string language = cultureCode.Split('-')[0];
            string defaultCulture = _configuration.DefaultCulture.ToLower();
            string defaultLanguage = defaultCulture.Split("-")[0];
            if (localizationDictionary.TryGetValue(categoryItem.CategoryID, out var localizationValues))
            {
                var properKey = localizationValues.DisplayNames.Keys.OrderBy(cultureKey =>
                {
                    string categoryLanguage = cultureKey.Split('-')[0];
                    if (cultureKey.Equals(cultureOrLanguage))
                    {
                        return 0;
                    }
                    else if (categoryLanguage.Equals(language))
                    {
                        return 1;
                    }
                    if (cultureKey.Equals(defaultCulture))
                    {
                        return 2;
                    }
                    else if (cultureKey.Equals(defaultLanguage))
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }).FirstOrMaybe();
                var properDescriptionKey = localizationValues.Descriptions.Keys.OrderBy(cultureKey =>
                {
                    string categoryLanguage = cultureKey.Split('-')[0];
                    if (cultureKey.Equals(cultureOrLanguage))
                    {
                        return 0;
                    }
                    else if (categoryLanguage.Equals(language))
                    {
                        return 1;
                    }
                    if (cultureKey.Equals(defaultCulture))
                    {
                        return 2;
                    }
                    else if (cultureKey.Equals(defaultLanguage))
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }).FirstOrMaybe();

                if (properKey.TryGetValue(out var displayNameKey))
                {
                    localizedCategoryItem.CategoryDisplayName = localizationValues.DisplayNames[displayNameKey];
                }
                if (properDescriptionKey.TryGetValue(out var descriptionKey))
                {
                    localizedCategoryItem.CategoryDescription = localizationValues.Descriptions[descriptionKey];
                }
            }
            return localizedCategoryItem;

        }

       
        private Dictionary<int, LocalizedCategoryValues> GetLocalizedItemsCached()
        {
            var builder = _cacheDependencyBuilderFactory.Create()
                .ObjectType(CategoryInfo.OBJECT_TYPE)
                .ObjectType(ResourceStringInfo.OBJECT_TYPE)
                .ObjectType(ResourceTranslationInfo.OBJECT_TYPE);

            return _progressiveCache.Load(cs =>
            {
                var values = new Dictionary<int, LocalizedCategoryValues>();
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }

                var queryName = @"-- Gets the categories and translations, used to build the server-cached object  
select C.CategoryID, RT.TranslationText as CategoryDisplayName, Cu.CultureCode
from CMS_Category C  
left outer join CMS_ResourceString RS on RS.StringKey = TRIM(SUBSTRING(C.CategoryDisplayName, 3, len(C.CategoryDisplayname)-4))
left outer join CMS_ResourceTranslation RT on RT.TranslationStringID = RS.StringID 
left outer join CMS_Culture Cu on Cu.CultureID = RT.TranslationCultureID
where C.CategoryDisplayname like '{$%$}'
union all  
select C.CategoryID, C.CategoryDisplayName, 'en-US' as CultureCode
from CMS_Category C  
where C.CategoryDisplayname not like '{$%$}'";
                var queryDescriptions = @"-- Gets the categories and translations, used to build the server-cached object  
select C.CategoryID, RT.TranslationText as CategoryDescription
from CMS_Category C  
left outer join CMS_ResourceString RS on RS.StringKey = TRIM(SUBSTRING(C.CategoryDescription, 3, len(C.CategoryDescription)-4))  
left outer join CMS_ResourceTranslation RT on RT.TranslationStringID = RS.StringID 
left outer join CMS_Culture Cu on Cu.CultureID = RT.TranslationCultureID
where C.CategoryDescription like '{$%$}'
union all  
select C.CategoryID, C.CategoryparentID,C.CategoryDescription
from CMS_Category C  
where C.CategoryDescription not like '{$%$}'";
                var resultsName = ConnectionHelper.ExecuteQuery(queryName, new QueryDataParameters(), QueryTypeEnum.SQLQuery);
                var resultsDescription = ConnectionHelper.ExecuteQuery(queryDescriptions, new QueryDataParameters(), QueryTypeEnum.SQLQuery);

                foreach (DataRow dr in resultsName.Tables[0].Rows)
                {
                    int categoryID = (int)dr[nameof(CategoryInfo.CategoryID)];
                    string categoryDisplayName = (string)dr[nameof(CategoryInfo.CategoryDisplayName)];
                    Maybe<string> cultureCode = ValidationHelper.GetString(dr["CultureCode"], "").AsNullOrWhitespaceMaybe();

                    if (cultureCode.TryGetValue(out var cultureCodeVal))
                    {
                        if (!values.ContainsKey(categoryID))
                        {
                            values.Add(categoryID, new LocalizedCategoryValues());
                        }
                        var value = values[categoryID];
                        value.DisplayNames.Add(cultureCodeVal.ToLower(), categoryDisplayName);
                    }
                }
                foreach (DataRow dr in resultsDescription.Tables[0].Rows)
                {
                    int categoryID = (int)dr[nameof(CategoryInfo.CategoryID)];
                    Maybe<string> categoryDescription = ValidationHelper.GetString(dr[nameof(CategoryInfo.CategoryDisplayName)], string.Empty).AsNullOrWhitespaceMaybe();
                    Maybe<string> cultureCode = ValidationHelper.GetString(dr["CultureCode"], "").AsNullOrWhitespaceMaybe();

                    if (cultureCode.TryGetValue(out var cultureCodeVal))
                    {
                        if (!values.ContainsKey(categoryID))
                        {
                            values.Add(categoryID, new LocalizedCategoryValues());
                        }
                        var value = values[categoryID];
                        value.Descriptions.Add(cultureCodeVal.ToLower(), categoryDescription);
                    }
                }
                return values;
            }, new CacheSettings(CacheMinuteTypes.VeryLong.ToDouble(), "GetCategoryLocalizedDictionary"));
        }
    }

    public record LocalizedCategoryValues
    {
        public Dictionary<string, string> DisplayNames { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, Maybe<string>> Descriptions { get; set; } = new Dictionary<string, Maybe<string>>();
    }

    public static class CategoryItemExtensions
    {
        public static LocalizedCategoryItem ToLocalizedCategoryItem(this CategoryItem item)
        {
            return new LocalizedCategoryItem(
                categoryID: item.CategoryID,
                categoryName: item.CategoryName,
                categoryGuid: item.CategoryGuid,
                categoryParentID: item.CategoryParentID,
                categoryDisplayName: item.CategoryDisplayName)
            {
                CategoryDescription= item.CategoryDescription
            };
        }
    }
}

