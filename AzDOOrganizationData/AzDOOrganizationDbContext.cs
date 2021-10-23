using AzDOOrganizationData.Model;
using System.Data.Entity;

namespace AzDOOrganizationData
{
    public class AzDOOrganizationDbContext : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<ProcessTemplate> ProcessTemplates { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<NewField> NewFields { get; set; }
        public DbSet<WorkItemType> WorkItemTypes { get; set; }
        public DbSet<NewFieldWorkItem> FieldItemRelationShips { get; set; }

        public DbSet<ProjectException> ProjectExceptions { get; set; }

    }
}
