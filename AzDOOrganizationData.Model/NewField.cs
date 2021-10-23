using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzDOOrganizationData.Model
{
    public class NewField
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReferenceName { get; set; }
        public bool IsPickList { get; set; }
        public string CreateJSON { get; set; }
        public string AddJSON { get; set; }

        public string PickListJSON { get; set; }
        
    }

    public class WorkItemType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string AddJson { get; set; }
    }

    public class NewFieldWorkItem
    {
        [Key]
        public int Id { get; set; }
        public int NewFieldId { get; set; }

        
        public int WorkItemTypeId{ get; set; }
        public string PageId { get; set; }
        public string SectionId { get; set; }
    }
}
