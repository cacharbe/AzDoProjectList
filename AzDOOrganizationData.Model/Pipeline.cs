using System.ComponentModel.DataAnnotations;

namespace AzDOOrganizationData.Model
{
    public class Pipeline
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Organization { get; set; }
        public string UniqueAzDOId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
