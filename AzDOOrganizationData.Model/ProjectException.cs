
using System;
using System.ComponentModel.DataAnnotations;

namespace AzDOOrganizationData.Model
{
    public class ProjectException
    {
        [Key]
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string ProjectName { get; set; }

    }
}
