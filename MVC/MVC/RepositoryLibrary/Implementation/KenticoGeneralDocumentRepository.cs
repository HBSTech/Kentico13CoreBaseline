using CMS.Base;
using CMS.DocumentEngine;
using Generic.Enums;
using Generic.Repositories.Interfaces;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Repositories.Implementations
{
    public class KenticoGeneralDocumentRepository : IGeneralDocumentRepository
    {
        private IRepoContext _repoContext;
        private readonly ISiteService siteService;

        public KenticoGeneralDocumentRepository(IRepoContext repoContext, ISiteService siteService)
        {
            _repoContext = repoContext;
            this.siteService = siteService;
        }

        [CacheDependency("node|##SITENAME##|{0}|childnodes")]
        [CacheDependency("node|##SITENAME##|{0}")]
        public IEnumerable<ITreeNode> GetDocumentsByPath(string SinglePath, PathSelectionEnum PathType, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1, string[] Columns = null, string[] PageTypes = null, bool IncludeCoupledColumns = false, string Culture = null)
        {
            return GetDocumentsByPathInternal(SinglePath, siteService.CurrentSite.SiteName, PathType, OrderBy, WhereCondition, MaxLevel, TopNumber, Columns, PageTypes, IncludeCoupledColumns, (!string.IsNullOrWhiteSpace(Culture) ? Culture : _repoContext.CurrentCulture()));
        }

        [CacheDependency("node|{1}|{0}|childnodes")]
        [CacheDependency("node|{1}|{0}")]
        public IEnumerable<ITreeNode> GetDocumentsByPath(string SinglePath, string SiteName, PathSelectionEnum PathType, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1, string[] Columns = null, string[] PageTypes = null, bool IncludeCoupledColumns = false, string Culture = null)
        {
            return GetDocumentsByPathInternal(SinglePath, (!string.IsNullOrWhiteSpace(SiteName) ? SiteName : siteService.CurrentSite.SiteName), PathType, OrderBy, WhereCondition, MaxLevel, TopNumber, Columns, PageTypes, IncludeCoupledColumns, (!string.IsNullOrWhiteSpace(Culture) ? Culture : _repoContext.CurrentCulture()));
        }

        private IEnumerable<ITreeNode> GetDocumentsByPathInternal(string SinglePath, string SiteName, PathSelectionEnum PathType, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1, string[] Columns = null, string[] PageTypes = null, bool IncludeCoupledColumns = false, string Culture = null)
        {
            MultiDocumentQuery Query = new MultiDocumentQuery();
            if (PageTypes != null && PageTypes.Length > 0)
            {
                if (PageTypes.Length == 1)
                {
                    Query.Type(PageTypes[0]);
                }
                else
                {
                    Query.Types(PageTypes);
                }
            }
            if (IncludeCoupledColumns)
            {
                Query.ExpandColumns();
            }

            // Handle culture and versioning and site
            Query.Culture(Culture)
                .CombineWithDefaultCulture()
                .CombineWithAnyCulture()
                .Published(!_repoContext.PreviewEnabled())
                .LatestVersion(_repoContext.PreviewEnabled())
                .OnSite(SiteName);

            PathTypeEnum KenticoPathType = PathTypeEnum.Explicit;
            switch (PathType)
            {
                case PathSelectionEnum.ChildrenOnly:
                    KenticoPathType = PathTypeEnum.Children;
                    break;
                case PathSelectionEnum.ParentAndChildren:
                    KenticoPathType = PathTypeEnum.Section;
                    break;
                case PathSelectionEnum.ParentOnly:
                    KenticoPathType = PathTypeEnum.Single;
                    break;
            }
            Query.Path(SinglePath, KenticoPathType);

            if (!string.IsNullOrWhiteSpace(OrderBy))
            {
                Query.OrderBy(OrderBy);
            }
            if (!string.IsNullOrWhiteSpace(WhereCondition))
            {
                Query.Where(WhereCondition);
            }
            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            if (MaxLevel >= 0)
            {
                Query.NestingLevel(MaxLevel);
            }
            if (TopNumber >= 0)
            {
                Query.TopN(TopNumber);
            }
            return Query.TypedResult;
        }

        [CacheDependency("node|##SITENAM##|{0}")]

        public ITreeNode GetDocumentByPath(string SinglePath, string[] Columns = null, string PageType = null, string Culture = null)
        {
            return GetDocumentByPathInternal(SinglePath, siteService.CurrentSite.SiteName, Columns, PageType, (!string.IsNullOrWhiteSpace(Culture) ? Culture : _repoContext.CurrentCulture()));
        }

        [CacheDependency("node|{1}|{0}")]
        public ITreeNode GetDocumentByPath(string SinglePath, string SiteName, string[] Columns = null, string PageType = null, string Culture = null)
        {
            return GetDocumentByPathInternal(SinglePath, (!string.IsNullOrWhiteSpace(SiteName) ? SiteName : siteService.CurrentSite.SiteName), Columns, PageType, (!string.IsNullOrWhiteSpace(Culture) ? Culture : _repoContext.CurrentCulture()));
        }

        private ITreeNode GetDocumentByPathInternal(string SinglePath, string SiteName, string[] Columns = null, string PageType = null, string Culture = null)
        {
            DocumentQuery Query = (!string.IsNullOrWhiteSpace(PageType) ? new DocumentQuery(PageType) : new DocumentQuery());

            Query.Culture(Culture)
                .CombineWithDefaultCulture()
                .CombineWithAnyCulture()
                .Published(!_repoContext.PreviewEnabled())
                .LatestVersion(_repoContext.PreviewEnabled())
                .OnSite(SiteName);
            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            Query.Path(SinglePath, PathTypeEnum.Single);
            return Query.FirstOrDefault();
        }

        [CacheDependency("nodeguid|##SITENAME##|{0}")]
        public ITreeNode GetDocumentByNodeGuid(Guid NodeGuid, string Culture, string[] Columns = null, string PageType = null)
        {
            return GetDocumentByNodeGuidInternal(NodeGuid, siteService.CurrentSite.SiteName, (!string.IsNullOrWhiteSpace(Culture) ? Culture : _repoContext.CurrentCulture()), Columns, PageType);
        }

        [CacheDependency("nodeguid|{1}|{0}")]
        public ITreeNode GetDocumentByNodeGuid(Guid NodeGuid, string SiteName, string Culture, string[] Columns = null, string PageType = null)
        {
            return GetDocumentByNodeGuidInternal(NodeGuid, (!string.IsNullOrWhiteSpace(SiteName) ? SiteName : siteService.CurrentSite.SiteName), (!string.IsNullOrWhiteSpace(Culture) ? Culture : _repoContext.CurrentCulture()), Columns, PageType);
        }

        private ITreeNode GetDocumentByNodeGuidInternal(Guid NodeGuid, string SiteName, string Culture, string[] Columns = null, string PageType = null)
        {
            DocumentQuery Query = (!string.IsNullOrWhiteSpace(PageType) ? new DocumentQuery(PageType) : new DocumentQuery());
            
            Query.Culture(Culture)
                .CombineWithDefaultCulture()
                .CombineWithAnyCulture()
                .Published(!_repoContext.PreviewEnabled())
                .LatestVersion(_repoContext.PreviewEnabled())
                .OnSite(SiteName);
            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            Query.WhereEquals("NodeGuid", NodeGuid);
            return Query.FirstOrDefault();
        }

        [CacheDependency("nodeid|{0}")]
        public ITreeNode GetDocumentByNodeID(int NodeID, string Culture, string[] Columns = null, string PageType = null)
        {
            DocumentQuery Query = (!string.IsNullOrWhiteSpace(PageType) ? new DocumentQuery(PageType) : new DocumentQuery());

            string CultureToUse = (!string.IsNullOrWhiteSpace(Culture) ? Culture : _repoContext.CurrentCulture());

            Query.Culture(CultureToUse)
                .CombineWithDefaultCulture()
                .CombineWithAnyCulture()
                .Published(!_repoContext.PreviewEnabled())
                .LatestVersion(_repoContext.PreviewEnabled());

            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            Query.WhereEquals("NodeID", NodeID);
            return Query.FirstOrDefault();
        }

        [CacheDependency("documentguid|##SITENAME##|{0}")]
        public ITreeNode GetDocumentByDocumentGuid(Guid DocumentGuid, string[] Columns = null, string PageType = null)
        {
            DocumentQuery Query = (!string.IsNullOrWhiteSpace(PageType) ? new DocumentQuery(PageType) : new DocumentQuery());

            Query.Published(!_repoContext.PreviewEnabled())
                .LatestVersion(_repoContext.PreviewEnabled())
                .OnCurrentSite();

            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            Query.WhereEquals("DocumentGuid", DocumentGuid);
            return Query.FirstOrDefault();
        }

        [CacheDependency("documentguid|{1}|{0}")]
        public ITreeNode GetDocumentByDocumentGuid(Guid DocumentGuid, string SiteName, string[] Columns = null, string PageType = null)
        {
            DocumentQuery Query = (!string.IsNullOrWhiteSpace(PageType) ? new DocumentQuery(PageType) : new DocumentQuery());
            string SiteToUse = (!string.IsNullOrWhiteSpace(SiteName) ? SiteName : siteService.CurrentSite.SiteName);
            Query.Published(!_repoContext.PreviewEnabled())
                .LatestVersion(_repoContext.PreviewEnabled())
                .OnSite(SiteToUse);

            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            Query.WhereEquals("DocumentGuid", DocumentGuid);
            return Query.FirstOrDefault();
        }

        [CacheDependency("documentid|{0}")]
        public ITreeNode GetDocumentByDocumentID(int DocumentID, string[] Columns = null, string PageType = null)
        {
            DocumentQuery Query = (!string.IsNullOrWhiteSpace(PageType) ? new DocumentQuery(PageType) : new DocumentQuery());

            Query.Published(!_repoContext.PreviewEnabled())
                .LatestVersion(_repoContext.PreviewEnabled());

            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            Query.WhereEquals("DocumentID", DocumentID);
            return Query.FirstOrDefault();
        }
    }
}