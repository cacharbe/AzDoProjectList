using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzDO.Controller.JsonData
{
    #region json obj classes

    public class QueryResponseJson
    {
        public string queryType { get; set; }
        public string asOf { get; set; }
        public WorkItemJson[] workItems { get; set; }
    }

    public class CreateFieldJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("usage")]
        public string Usage { get; set; }
        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }
        [JsonProperty("canSortBy")]
        public string CanSortBy { get; set; }
        [JsonProperty("isQueryable")]
        public bool IsQueryable { get; set; }
        [JsonProperty("isIdentity")]
        public bool IsIdentity { get; set; }
        [JsonProperty("isPicklist")]
        public bool IsPicklist { get; set; }
        [JsonProperty("pickListId")]
        public string pickListId { get; set; }
        [JsonProperty("isPicklistSuggested")]
        public bool IsPicklistSuggested { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("supportedOperations")]
        public SupportedOperations[] SupportedOperations { get; set; }

    }

    public class AddFieldJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("usage")]
        public string Usage { get; set; }
        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }
        [JsonProperty("canSortBy")]
        public string CanSortBy { get; set; }
        [JsonProperty("isQueryable")]
        public bool IsQueryable { get; set; }


        [JsonProperty("isPicklist")]
        public bool IsPicklist { get; set; }
        [JsonProperty("pickListId")]
        public string pickListId { get; set; }
        [JsonProperty("isPicklistSuggested")]
        public bool IsPicklistSuggested { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("allowedValues")]
        public string[] AllowedValues { get; set; }
    }

    public class PickListsResults
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("value")]
        public PickList[] Lists { get; set; }
    }

    public class PickList
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isSuggested")]
        public bool IsSuggested { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
   
    public class WorkItemJson
    {
        public int id { get; set; }
        public string url { get; set; }
        public WorkItemFieldsJson fields { get; set; }
    }

    public class WorkItemFieldsJson
    {
        [JsonProperty("System.ChangedDate")]
        public string ChangedDate { get; set; }

        [JsonProperty("System.Title")]
        public string Title { get; set; }

        //[JsonProperty("System.ChangedBy")]
        //public string ChangedBy { get; set; }
    }

    public class WorkItemUpdates
    {
        public int count { get; set; }
        public WorkItemUpdateJson[] value { get; set; }
    }

    public class WorkItemUpdateJson
    {
        public int id { get; set; }
        public int workItemId { get; set; }
        public string url { get; set; }
        public WorkItemUpdateFieldJson fields { get; set; }
    }

    public class WorkItemUpdateFieldJson
    {
        [JsonProperty("System.ChangedDate")]
        public ChangedDate ChangedDate { get; set; }
        [JsonProperty("System.RevisedDate")]
        public RevisedDate RevisedDate { get; set; }
        [JsonProperty("System.State")]
        public State State { get; set; }
    }

    [JsonObject("System.RevisedDate")]
    public class RevisedDate
    {
        public string oldValue { get; set; }
        public string newValue { get; set; }
    }

    [JsonObject("System.ChangedDate")]
    public class ChangedDate
    {
        public string oldValue { get; set; }
        public string newValue { get; set; }
    }

    [JsonObject("System.State")]
    public class State
    {
        public string oldValue { get; set; }
        public string newValue { get; set; }
    }

    public class BuildList
    {
        public int count { get; set; }
        public Build[] value { get; set; }
    }
    public class Build
    {
        public int id { get; set; }
        public string queueTime { get; set; }
        public BuildDefinition definition { get; set; }
    }

    public class BuildDefinition
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string uri { get; set; }
        public string path { get; set; }
        public string type { get; set; }
        public string queueStatus { get; set; }
        public string revision { get; set; }
    }
    
    public class WorkItemTypeField
    {
        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }
        [JsonProperty("alwaysRequired")]
        public bool IsRequired { get; set; }
        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
    public class Field
    {

        public string name { get; set; }
        public string referenceName { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string usage { get; set; }
        public string url { get; set; }
        public string customization { get; set; }
    }

    public class FieldList
    {
        public int count { get; set; }
        public Field[] value { get; set; }
    }

    public class Pipeline
    {
        public string id { get; set; }
        public string revision { get; set; }
        public string name { get; set; }
        public string folder { get; set; }
        public string url { get; set; }

    }

    public class PipelineList
    {
        public int count { get; set; }
        public Pipeline[] value { get; set; }
    }

    public class ProjectUpdateJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }

    }

    public class Capabilities
    {
        [JsonProperty("processTemplate")]
        public ProcessTemplateJson ProcessTemplate { get; set; }
    }

    public class ProcessTemplateJson
    {
        [JsonProperty("templateTypeId")]
        public string TemplateTypeId { get; set; }
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public string revision { get; set; }
        public string visibility { get; set; }
        public string lastUpdateTime { get; set; }
        public ProjectPropertyList properties { get; set; }
    }
    public class ProjectList
    {
        public int count { get; set; }
        public Project[] value { get; set; }
    }

    
    public class Process
    {
        public string id { get; set; }
        public string description { get; set; }
        public string isDefault { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string name { get; set; }
    }

    public class CreateProcessJSON
    {
       [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("parentProcessTypeId")]
        public string ParentProcessTypeId { get; set; }
    }
    public class CreateProcessResponse
    {
        [JsonProperty("typeId")]
        public string TypeId { get; set; }

        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("parentProcessTypeId")]
        public string ParentProcessTypeId { get; set; }

        [JsonProperty("isEnabled")]
        public string IsEnabled { get; set; }

        [JsonProperty("customizationType")]
        public string CustomizationType { get; set; }
    }
    public class ProcessType
    {
        public string custom{ get; set; }
        public string inherited { get; set; }
        public string system { get; set; }
    }
    public class UserList
    {
        public int count { get; set; }
        public User[] value { get; set; }
    }
    public class User
    {
        public string displayName { get; set; }

    }
    public class ProcessList
    {
        public int count { get; set; }
        public Process[] value { get; set; }
    }
    [JsonObject("members")]
    public class MemberList
    {
        public Member[] Members { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("continuationToken")]
        public string ContinuationToken { get; set; }
    }
    public class AccessLevel
    {
        [JsonProperty("licensingSource")]
        public string LicensingSource { get; set; }

        [JsonProperty("accountLicenseType")]
        public string AccountLicenseType { get; set; }

        [JsonProperty("msdnLicenseType")]
        public string MsdnLicenseType { get; set; }

        [JsonProperty("licenseDisplayName")]
        public string LicenseDisplayName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        [JsonProperty("assignmentSource")]
        public string AssignmentSource { get; set; }
    }


    public class Member
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("lastAccessedDate")]
        public string LastAccessedDate { get; set; }

        [JsonProperty("accessLevel")]
        public AccessLevel AccessLevel { get; set; }


    }
    public class ProjectPropertyList
    {
        public int count { get; set; }
        public Property[] value { get; set; }
    }
    public class Property
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class WorkItemTypeList
    {
        public int count { get; set; }
        [JsonProperty("value")]
        public WorkItemType[] WorkItemTypes { get; set; }
    }

    public class WorkItemType
    {
        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("customization")]
        public string Customization { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("inherits")]
        public string Inherits { get; set; }
    }

    public class CreateFieldResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("usage")]
        public string Usage { get; set; }

        [JsonProperty("readOnly")]
        public string ReadOnly { get; set; }

        [JsonProperty("canSortBy")]
        public bool CanSortBy { get; set; }

        [JsonProperty("isQueryable")]
        public bool IsQueryable { get; set; }

        [JsonProperty("isIdentity")]
        public string IsIdentity { get; set; }

        [JsonProperty("isPicklist")]
        public string IsPicklist { get; set; }

        [JsonProperty("isPicklistSuggested")]
        public string IsPicklistSuggested { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("supportedOperations")]
        public SupportedOperations[] SupportedOperations { get; set; }
    }

    public class CreatePickListResponse
    {
        [JsonProperty("items")]
        public string[] Items { get; set; }
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("isSuggested")]
        public string IsSuggested { get; set; }

    }
    public class SupportedOperations
    {
        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class CreateControlToGroupJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("isContribution")]
        public bool IsContribution { get; set; }
        [JsonProperty("controlType")]
        public string ControlType { get; set; }
        [JsonProperty("metadata")]
        public string Metadata { get; set; }
        [JsonProperty("inherited")]
        public bool Inherited { get; set; }
        [JsonProperty("overridden")]
        public bool Overridden { get; set; }
        [JsonProperty("watermark")]
        public string Watermark { get; set; }


        [JsonProperty("height")]
        public int Height { get; set; }

    }

    public class CreateControlResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("isContribution")]
        public bool IsContribution { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }
    }

    public class CreateGroupResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("isContribution")]
        public bool IsContribution { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("controls")]
        public Control[] Controls { get; set; }
    }

    public class Control
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }

        [JsonProperty("controlType")]
        public string ControlType { get; set; }


        [JsonProperty("inherited")]
        public bool Inherited { get; set; }

        [JsonProperty("overridden")]
        public bool Overridden { get; set; }

        [JsonProperty("watermark")]
        public string Watermark { get; set; }

        [JsonProperty("visible")]
        public string Visible { get; set; }

        [JsonProperty("isContribution")]
        public bool IsContribution { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }
    public class Layout
    {
        [JsonProperty("pages")]
        public LayoutPage[] Pages { get; set; }
    }

    public class LayoutPage
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("inherited")]
        public bool Inherited { get; set; }
        [JsonProperty("overridden")]
        public bool Overridden { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("pageType")]
        public string PageType { get; set; }
        [JsonProperty("locked")]
        public bool Locked { get; set; }
        [JsonProperty("visible")]
        public bool Visible { get; set; }
        [JsonProperty("isContribution")]
        public bool IsContribution { get; set; }

        [JsonProperty("sections")]
        public LayoutPageSection[] Sections { get; set; }


    }

    public class LayoutPageSection
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("groups")]
        public LayoutPageSectionGroup[] Groups { get; set; }
    }

    public class LayoutPageSectionGroup
    {

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("inherited")]
        public bool Inherited { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("isContribution")]
        public bool IsContribution { get; set; }
        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("controls")]
        public Control[] Controls { get; set; }
    }

    public class LayoutPageSectionGroupControl
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("inherited")]
        public bool Inherited { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("controlType")]
        public string ControlType { get; set; }
    }


    #endregion
}
