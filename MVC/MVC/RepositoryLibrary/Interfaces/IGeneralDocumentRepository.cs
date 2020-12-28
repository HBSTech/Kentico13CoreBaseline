using CMS.Base;
using Generic.Enums;
using MVCCaching;
using System;
using System.Collections.Generic;

namespace Generic.Repositories.Interfaces
{
    public interface IGeneralDocumentRepository : IRepository
    {
        /// <summary>
        /// Gets Tree Nodes based on the settings, uses current site
        /// </summary>
        /// <param name="SinglePath">The Page or Parent Path, should not contain wildcards such as %</param>
        /// <param name="PathType">The Path Type</param>
        /// <param name="OrderBy">Order by, for Kentico NodeLevel, NodeOrder will follow the tree structure</param>
        /// <param name="WhereCondition">The Where Condition, note that if are you selecting multiple page types, you should limit the where condition to only fields shared by them.</param>
        /// <param name="MaxLevel">Max nesting level of the pages you wish to select</param>
        /// <param name="TopNumber">The Top number of items that you wish to select</param>
        /// <param name="Columns">The Columns you wish to retrieve</param>
        /// <param name="PageTypes">The Page Types (Class Names)</param>
        /// <param name="IncludeCoupledColumns">If you wish to return extra column data that isn't shared, this should be set to true for multiple page types retrieved if you need those columns.</param>
        /// <returns>The ITreeNodes</returns>
        IEnumerable<ITreeNode> GetDocumentsByPath(string SinglePath, PathSelectionEnum PathType, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1, string[] Columns = null, string[] PageTypes = null, bool IncludeCoupledColumns = false, string Culture = null);

        /// <summary>
        /// Gets Tree Nodes based on the settings
        /// </summary>
        /// <param name="SinglePath">The Page or Parent Path, should not contain wildcards such as %</param>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="PathType">The Path Type</param>
        /// <param name="OrderBy">Order by, for Kentico NodeLevel, NodeOrder will follow the tree structure</param>
        /// <param name="WhereCondition">The Where Condition, note that if are you selecting multiple page types, you should limit the where condition to only fields shared by them.</param>
        /// <param name="MaxLevel">Max nesting level of the pages you wish to select</param>
        /// <param name="TopNumber">The Top number of items that you wish to select</param>
        /// <param name="Columns">The Columns you wish to retrieve</param>
        /// <param name="PageTypes">The Page Types (Class Names)</param>
        /// <param name="IncludeCoupledColumns">If you wish to return extra column data that isn't shared, this should be set to true for multiple page types retrieved if you need those columns.</param>
        /// <param name="Culture">The Culture, if not provided will use default culture</param>
        /// <returns>The ITreeNodes</returns>
        IEnumerable<ITreeNode> GetDocumentsByPath(string SinglePath, string SiteName, PathSelectionEnum PathType, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1, string[] Columns = null, string[] PageTypes = null, bool IncludeCoupledColumns = false, string Culture = null);

        /// <summary>
        /// Gets Tree Node based on the settings, uses current site
        /// </summary>
        /// <param name="SinglePath">The Page or Parent Path, should not contain wildcards such as %</param>
        /// <param name="Columns">The Columns you wish to retrieve</param>
        /// <param name="PageType">The Page Type (Class Name), setting this will also automatically include coupled columns</param>
        /// <param name="Culture">The Culture, if not provided will use default culture</param>
        /// <returns>The ITreeNode</returns>
        ITreeNode GetDocumentByPath(string SinglePath, string[] Columns = null, string PageType = null, string Culture = null);

        /// <summary>
        /// Gets Tree Node based on the settings
        /// </summary>
        /// <param name="SinglePath">The Page or Parent Path, should not contain wildcards such as %</param>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="Columns">The Columns you wish to retrieve</param>
        /// <param name="PageType">The Page Type (Class Name), setting this will also automatically include coupled columns</param>
        /// <param name="Culture" > The Culture, if not provided will use default culture</param>
        /// <returns>The ITreeNode</returns>
        ITreeNode GetDocumentByPath(string SinglePath, string SiteName, string[] Columns = null, string PageType = null, string Culture = null);

        /// <summary>
        /// Gets the given Node based on it's Node Guid, uses the current site
        /// </summary>
        /// <param name="NodeGuid">The Node Guid</param>
        /// <param name="Culture">The culture, if a null is passed will use the default culture</param>
        /// <param name="Columns">Columns you wish to retrieve</param>
        /// <param name="PageType">The Classes Page type</param>
        /// <returns>The Node</returns>
        ITreeNode GetDocumentByNodeGuid(Guid NodeGuid, string Culture, string[] Columns = null, string PageType = null);

        /// <summary>
        /// Gets the given Node based on it's Node Guid, use this if the node isn't on the current site.
        /// </summary>
        /// <param name="NodeGuid">The Node Guid</param>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="Culture">The culture, if a null is passed will use the default culture</param>
        /// <param name="Columns">Columns you wish to retrieve</param>
        /// <param name="PageType">The Classes Page type</param>
        /// <returns>The Node</returns>
        ITreeNode GetDocumentByNodeGuid(Guid NodeGuid, string SiteName, string Culture, string[] Columns = null, string PageType = null);


        /// <summary>
        /// Gets the given Node based on it's Node ID
        /// </summary>
        /// <param name="NodeID">The Node ID</param>
        /// <param name="Culture">The culture, if a null is passed will use the default culture</param>
        /// <param name="Columns">Columns you wish to retrieve</param>
        /// <param name="PageType">The Classes Page type</param>
        /// <returns>The Node</returns>
        ITreeNode GetDocumentByNodeID(int NodeID, string Culture, string[] Columns = null, string PageType = null);

        /// <summary>
        /// Gets the given Node based on it's Document Guid
        /// </summary>
        /// <param name="DocumentGuid">The Document Guid</param>
        /// <param name="Columns">Columns you wish to retrieve</param>
        /// <param name="PageType">The Classes Page type</param>
        /// <returns>The Node</returns>
        ITreeNode GetDocumentByDocumentGuid(Guid DocumentGuid, string[] Columns = null, string PageType = null);

        /// <summary>
        /// Gets the given Node based on it's Document Guid
        /// </summary>
        /// <param name="DocumentGuid">The Document Guid</param>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="Columns">Columns you wish to retrieve</param>
        /// <param name="PageType">The Classes Page type</param>
        /// <returns>The Node</returns>
        ITreeNode GetDocumentByDocumentGuid(Guid DocumentGuid, string SiteName, string[] Columns = null, string PageType = null);


        /// <summary>
        /// Gets the given Node based on it's Document ID
        /// </summary>
        /// <param name="DocumentID">The Document ID</param>
        /// <param name="Columns">Columns you wish to retrieve</param>
        /// <param name="PageType">The Classes Page type</param>
        /// <returns>The Node</returns>
        ITreeNode GetDocumentByDocumentID(int DocumentID, string[] Columns = null, string PageType = null);


    }
}