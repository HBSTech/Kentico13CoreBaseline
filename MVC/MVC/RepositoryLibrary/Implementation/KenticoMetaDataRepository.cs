using CMS.DocumentEngine;
using Generic.Models;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Http;
using MVC.RepositoryLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.RepositoryLibrary.Implementation
{
    public class KenticoMetaDataRepository : IMetaDataRepository
    {
        public KenticoMetaDataRepository(IPageRetriever pageRetriever, IPageDataContextRetriever pageDataContextRetriever, IHttpContextAccessor httpContextAccessor)
        {
            PageRetriever = pageRetriever;
            PageDataContextRetriever = pageDataContextRetriever;
            HttpContextAccessor = httpContextAccessor;
        }

        public IPageRetriever PageRetriever { get; }
        public IPageDataContextRetriever PageDataContextRetriever { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        private PageMetaData GetMetaDataInternal(TreeNode node, string thumbnail = "")
        {
            
            thumbnail = !string.IsNullOrWhiteSpace(thumbnail) ? thumbnail : "";
            if(node == null)
            {
                return new PageMetaData()
                {
                    Title = "",
                    Keywords = "",
                    Description = "",
                    Thumbnail = GetAbsoluteThumbnailUrl(thumbnail)
                };
            } else
            {
                thumbnail = !string.IsNullOrWhiteSpace(thumbnail) ? thumbnail : node.DocumentCustomData.GetValue("DocumentOGImage")?.ToString();
                PageMetaData metaData = new PageMetaData()
                {
                    Title = !string.IsNullOrWhiteSpace(node.DocumentPageTitle) ? node.DocumentPageTitle : node.DocumentName,
                    Keywords = node.DocumentPageKeyWords,
                    Description = node.DocumentPageDescription,
                    Thumbnail = GetAbsoluteThumbnailUrl(thumbnail)
                };
                return metaData;
            }
        }

        private string GetAbsoluteThumbnailUrl(string thumbnail)
        {
            if(string.IsNullOrWhiteSpace(thumbnail))
            {
                return "";
            } else
            {
                var request = HttpContextAccessor.HttpContext.Request;
                return $"{request.Scheme}://{request.Host.ToUriComponent()}{thumbnail.Replace("~", "")}";
            }
        }


        public async Task<PageMetaData> GetMetaDataAsync(TreeNode node, string thumbnail = "")
        {
            return GetMetaDataInternal(node, thumbnail);
        }

        public async Task<PageMetaData> GetMetaDataAsync(int documentId, string thumbnail = "")
        {

            var page = await PageRetriever.RetrieveAsync<TreeNode>(query => query
            .WhereEquals(nameof(TreeNode.DocumentID), documentId)
            .Columns(nameof(TreeNode.DocumentCustomData), nameof(TreeNode.DocumentPageTitle), nameof(TreeNode.DocumentPageDescription), nameof(TreeNode.DocumentPageKeyWords))
            .TopN(1)
            , cs => cs.Dependencies((result, builder) => builder.Pages(result))
            .Expiration(TimeSpan.FromDays(1))
            );
            return GetMetaDataInternal(page.FirstOrDefault(), thumbnail);
        }

        public async Task<PageMetaData> GetMetaDataAsync(Guid documentGuid, string thumbnail = "")
        {
            var page = await PageRetriever.RetrieveAsync<TreeNode>(query => query
            .WhereEquals(nameof(TreeNode.DocumentGUID), documentGuid)
            .Columns(nameof(TreeNode.DocumentCustomData), nameof(TreeNode.DocumentPageTitle), nameof(TreeNode.DocumentPageDescription), nameof(TreeNode.DocumentPageKeyWords))
            .TopN(1)
            , cs => cs.Dependencies((result, builder) => builder.Pages(result))
            .Expiration(TimeSpan.FromDays(1))
            );
            return GetMetaDataInternal(page.FirstOrDefault(), thumbnail);
        }

        public async Task<PageMetaData> GetMetaDataAsync(string thumbnail = "")
        {
            var curPage = PageDataContextRetriever.Retrieve<TreeNode>();
            return GetMetaDataInternal(curPage?.Page, thumbnail);
        }
    }
}
