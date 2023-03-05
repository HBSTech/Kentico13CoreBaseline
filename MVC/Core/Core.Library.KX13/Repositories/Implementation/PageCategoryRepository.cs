using CMS;
using CMS.Taxonomy;

namespace Generic.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class PageCategoryRepository : IPageCategoryRepository
    {
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ICustomCategoryHelper _customCategoryHelper;

        public PageCategoryRepository(
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            IProgressiveCache progressiveCache,
            ICustomCategoryHelper customCategoryHelper)
        {
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _progressiveCache = progressiveCache;
            _customCategoryHelper = customCategoryHelper;
        }

        public async Task<IEnumerable<CategoryItem>> GetCategoriesByNodeAsync(int nodeID)
        {
            var dictionary = (await GetCategoriesByIdentifiersAsync()).Item1;
            if (dictionary.ContainsKey(nodeID))
            {
                return dictionary[nodeID];
            }
            else
            {
                return Array.Empty<CategoryItem>();
            }
        }

        public async Task<IEnumerable<CategoryItem>> GetCategoriesByNodesAsync(IEnumerable<int> nodeIDs)
        {
            var dictionary = (await GetCategoriesByIdentifiersAsync()).Item1;
            var categoryItems = dictionary
                .Where(x => nodeIDs.Contains(x.Key))
                .SelectMany(x => x.Value)
                .Distinct(new CategoryItemEqualityComparer());
            return categoryItems;
        }

        public async Task<IEnumerable<CategoryItem>> GetCategoryItemsByPathAsync(string path)
        {
            var dictionary = (await GetCategoriesByIdentifiersAsync()).Item2;
            if (dictionary.ContainsKey(path.ToLowerInvariant()))
            {
                return dictionary[path.ToLowerInvariant()];
            }
            else
            {
                return Array.Empty<CategoryItem>();
            }
        }

        /// <summary>
        /// Helper function that gets ALL node/path categories in one query, used by others to quickly retrieve data.
        /// </summary>
        /// <returns></returns>
        private async Task<Tuple<Dictionary<int, IEnumerable<CategoryItem>>, Dictionary<string, IEnumerable<CategoryItem>>>> GetCategoriesByIdentifiersAsync()
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.ObjectType(TreeCategoryInfo.OBJECT_TYPE)
                .ObjectType(CategoryInfo.OBJECT_TYPE);

            return await _progressiveCache.Load(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                };

                // no need to cache this call as caching entire thing
                var query = new ObjectQuery<TreeCategoryInfo>()
                    .Source(x => x.InnerJoin(new QuerySourceTable("CMS_Tree"), $"CMS_Tree.{nameof(TreeNode.NodeID)}", $"{TreeCategoryInfo.OBJECT_TYPE.Replace(".", "_")}.{nameof(TreeCategoryInfo.NodeID)}"))
                    .Source(x => x.InnerJoin<CategoryInfo>($"{TreeCategoryInfo.OBJECT_TYPE.Replace(".", "_")}.{nameof(TreeCategoryInfo.CategoryID)}", $"{CategoryInfo.OBJECT_TYPE.Replace(".", "_")}.{nameof(CategoryInfo.CategoryID)}"))
                    .Columns(new string[] {
                        $"CMS_Tree.{nameof(TreeNode.NodeID)}",
                        $"CMS_Tree.{nameof(TreeNode.NodeAliasPath)}",
                        $"{CategoryInfo.OBJECT_TYPE.Replace(".", "_")}.{nameof(CategoryInfo.CategoryID)}"
                        });
                var retriever = await query.GetEnumerableResultAsync(System.Data.CommandBehavior.Default);

                // Group into two dictionaries
                var items = retriever.Where(x => _customCategoryHelper.GetCategoryCached().ContainsKey((int)x[nameof(CategoryInfo.CategoryID)])).Select(x => new PageCategoryItem(
                    nodeID: (int)x[nameof(TreeNode.NodeID)],
                    path: (string)x[nameof(TreeNode.NodeAliasPath)],
                    categoryItem: _customCategoryHelper.GetCategoryIdentifiertoCategoryCached((int)x[nameof(CategoryInfo.CategoryID)]).Value
                ));

                var dictionaryByNodeID = items.GroupBy(x => x.NodeID).ToDictionary(key => key.Key, value => value.Select(x => x.CategoryItem));
                var dictionaryByPath = items.GroupBy(x => x.Path).ToDictionary(key => key.Key, value => value.Select(x => x.CategoryItem));
                var result = new Tuple<Dictionary<int, IEnumerable<CategoryItem>>, Dictionary<string, IEnumerable<CategoryItem>>>(dictionaryByNodeID, dictionaryByPath);
                return result;
            }, new CacheSettings(60, $"GetCategoriesByIdentifiersAsync"));
        }
    }

    internal record PageCategoryItem
    {
        public PageCategoryItem(int nodeID, string path, CategoryItem categoryItem)
        {
            NodeID = nodeID;
            Path = path;
            CategoryItem = categoryItem;
        }

        public int NodeID { get; set; }
        public string Path { get; set; }
        public CategoryItem CategoryItem { get; set; }
    }
}
