using System.ComponentModel.DataAnnotations;

namespace AzDOOrganizationData.Model
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        public string AzDOId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int UserCount { get; set; }
        public bool UpdateFields { get; set; }

    }
}
