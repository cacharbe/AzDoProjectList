using System.ComponentModel.DataAnnotations;

namespace AzDOOrganizationData.Model
{
    public class Field
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReferenceName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Usage { get; set; }
        
        public int ProjectId { get; set; }

        public Project Project { get; set; }


    }
}
