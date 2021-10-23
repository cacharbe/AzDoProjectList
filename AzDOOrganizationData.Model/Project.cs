using System;
using System.ComponentModel.DataAnnotations;

namespace AzDOOrganizationData.Model
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public int OrganizationId {get;set;}
        public Organization Organization { get; set; }
    
        public string AzDoProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
        public string Revision { get; set; }
        public string Visibility { get; set; }
        public string LastUpdateTime { get; set; }
        public string CurrentProcessTemplateId { get; set; }
        public string OriginalProcessTemplateId { get; set; }
        public string ProcessTemplateType { get; set; }
        public string MSPROJ { get; set; }
        public string ProcessTemplate { get; set; }
        public string SourceControlCapabilityFlags { get; set; }
        public string SourceControlTfvcEnabled { get; set; }
        public int TeamCount { get; set; }
        public int PipelineCount { get; set; }
        
        public string LastBuildDate { get; set; }
        public string LastWorkItemChangedDate { get; set; }

    }
}
