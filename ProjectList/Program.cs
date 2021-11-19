using log4net;
using log4net.Appender;
using log4net.Config;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;
using AzDO.Controller;
using AzDO.Controller.JsonData;
using AzDOOrganizationData;
using AzDOOrganizationData.Model;
using Project = AzDOOrganizationData.Model.Project;

namespace ProjectList
{
    class Program
    {
        static class Settings
        {
            public const bool UPDATEDATA = false;
            public const bool FIELDREPORT = true;
            public const bool REPORTDATA = false;
            public const bool FIELDDATA = false;
            public const bool ADDFIELDS = false;
            public const bool RUNTEST = false;
            public const string BASE = "https://dev.azure.com";
            public const string AUTHUSER = "ads.ccharbeneau@accenture.com";
            public const string PAT = "ox64rp27b6ny2c2rft3w47ca5lfoqpitzixmubm62urajowx6qtq";
            public const string API = "api-version=6.0";
            public static string REPORTFILE = $"AzDO Report - {DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day} - {DateTime.Now.Ticks}.txt";
            public static string ORGREPORTFILE = $"AzDO Orgazniation Project Report - {DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day} - {DateTime.Now.Ticks}.txt";
        }

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            var logger = LogManager.GetLogger(typeof(Program));
            try
            {

                var startTime = DateTime.Now;
                var processedItemsDictionary = new Dictionary<int, int>();
                if (logger.IsInfoEnabled)
                    logger.InfoFormat("{0}: BEGIN ", DateTime.Now.ToShortTimeString());

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", Settings.AUTHUSER, Settings.PAT))));

                var azdoController = new Controller(Settings.BASE, Settings.PAT, Settings.AUTHUSER, logger);
                Database.SetInitializer(new AzDOOrganizationDbInitializer());

                if (Settings.FIELDREPORT)
                {
                    try
                    {
                        CreateNewFieldReport(azdoController, logger);
                    }
                    catch (Exception exception)
                    {

                        logger.ErrorFormat(
                            $"Unable to complete  Creating Field Report {exception.Message} - {exception.StackTrace}");
                        
                    }
                }

                if (Settings.RUNTEST)
                {
                    try
                    {
                        UpdateProjectsForAllOrgs(azdoController, logger);
                        TestExceptionList(azdoController, logger);
                    }
                    catch (Exception exception)
                    {
                        logger.ErrorFormat(
                            $"Unable to complete  Data Collection {exception.Message} - {exception.StackTrace}");
                        throw;
                    }
                }

                if (Settings.UPDATEDATA)
                {
                    try
                    {
                        //GenerateCycleTimeReport(azdoController, logger);
                        //GetUserDataForOrganizations(azdoController, logger);
                        //UpdateOrganizationUserCount(azdoController, logger);
                        // UpdateProcessTemplatesForAllOrgs(azdoController, logger);
                        UpdateProjectsForAllOrgs(azdoController, logger);
                    }
                    catch (Exception exception)
                    {
                        logger.ErrorFormat(
                            $"Unable to complete  Data Collection {exception.Message} - {exception.StackTrace}");
                        throw;
                    }
                }

                if (Settings.FIELDDATA)
                {
                    try
                    {
                        GetAllFieldsForAllOrgs(azdoController, logger);
                    }
                    catch (Exception exception)
                    {
                        logger.ErrorFormat(
                           $"Unable to complete Field Info Update {exception.Message} - {exception.StackTrace}");
                        throw;
                    }
                }

                if (Settings.REPORTDATA)
                {
                    try
                    {

                        //GenerateLastAccessReport(logger);
                        //GenerateLastBuildReport(logger);
                        //GenerateOrgProjReport(azdoController, logger);
                        //GenerateReport(logger);
                        GenerateFieldReport(logger);
                    }
                    catch (Exception exception)
                    {
                        logger.ErrorFormat(
                            $"Unable to complete Reporting {exception.Message} - {exception.StackTrace}");
                        throw;
                    }
                }

                if (Settings.ADDFIELDS)
                {
                    try
                    {
                        AddFieldsToProjects(azdoController, logger);
                    }
                    catch (Exception exception)
                    {
                        logger.ErrorFormat(
                            $"Unable to complete adding fields {exception.Message} - {exception.StackTrace}");
                        throw;
                    }
                }
                var endTime = DateTime.Now;
                var totalRunTime = endTime.Subtract(startTime);

                if (logger.IsInfoEnabled)
                    logger.InfoFormat("Done: Total Run Time {0} Minutes", totalRunTime.TotalMinutes);
                Console.WriteLine("Done: Total Run Time {0} Minutes", totalRunTime.TotalMinutes);
            }
            catch (Exception exception)
            {
                logger.ErrorFormat(
                            $"Unable to complete  Data Collection and Reporting {exception.Message} - {exception.StackTrace}");
            }
        }

        private static void CreateNewFieldReport(Controller azdoController, ILog logger)
        {
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    var orgs = projectsDb.Organizations.Where(o => o.IncludeInFieldReports == true).ToList();
                    foreach (var org in orgs)
                    {
                        UpdateProjectsForSingleOrg(azdoController, projectsDb, org, logger);
                        var projects = projectsDb.Projects.Where(t => t.OrganizationId == org.Id).ToList();
                        var exceptions = projectsDb.ProjectExceptions.Where(p => p.OrganizationId == org.Id).ToList();
                        foreach (var project in projects)
                        {
                            if (exceptions.All(e => e.ProjectName != project.Name))
                            {
                                //azdoController.DoesFieldExistInProject()
                                var fields = azdoController.GetFieldsFromAzDOForProject(org.Name, project.Name);
                                var newFields = projectsDb.NewFields.ToList();
                                foreach (var newField in newFields)
                                {
                                    if (fields.value.ToList().Any(f => f.name == newField.Name))
                                    {
                                        var localField = fields.value.ToList().First(f => f.name == newField.Name);
                                        
                                    }
                                }
                            }
                            else
                            {
                                logger.InfoFormat($"{project.Name} is on the Exception List");
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {

                logger.ErrorFormat(
                    $"Unable to complete the creation of the New Field Report {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private static void TestExceptionList(Controller azdoController, ILog logger)
        {
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    var orgs = projectsDb.Organizations.Where(o => o.UpdateFields == true).ToList();
                    foreach (var org in orgs)
                    {
                        var projects = projectsDb.Projects.Where(p => p.OrganizationId == org.Id).OrderBy(p => p.Name).ToList();
                        var newFields = projectsDb.NewFields.ToList();
                        var exceptions = projectsDb.ProjectExceptions.Where(p => p.OrganizationId == org.Id).ToList();
                        foreach (var project in projects)
                        {
                            if (!exceptions.Any(e => e.ProjectName == project.Name))
                            {

                            }
                            else
                            {
                                logger.InfoFormat($"{project.Name} is on the Exception List");
                            }
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Failed Adding Fields {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        // End of Main

        #region Reporting
        private static void GenerateOrgProjReport(Controller azdoController, ILog logger)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    foreach (var org in projectsDb.Organizations)
                    {
                        sb.AppendLine($"{org.Name}");
                        var projects = projectsDb.Projects.Where(t => t.OrganizationId == org.Id).ToList();
                        foreach (var proj in projects)
                        {
                            sb.AppendLine($"\t{proj.Name}");
                        }
                    }
                }
                WriteLocalProcessedFile(Settings.ORGREPORTFILE, sb.ToString(), logger);
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Failed Generating Organization / Project Report {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        private static void GenerateCycleTimeReport(Controller azdoController, ILog logger)
        {

            if (logger.IsInfoEnabled)
                logger.InfoFormat("Generating Cycle Time Report");

            try
            {
                var changedDate = azdoController.GetWorkItemActiveDate("accenturecio08", "EnterpriseArchitecture_2641", 1433995);
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Failed Generating User Report {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public static void GenerateUserReport(List<string> Organizations, ILog logger)
        {
            if (logger.IsInfoEnabled)
                logger.InfoFormat("Generating User Report");

            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    foreach (var org in projectsDb.Organizations)
                    {

                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Failed Generating User Report {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        public static void GenerateFieldReport(List<string> fields, ILog logger)
        {
            if (logger.IsInfoEnabled)
                logger.InfoFormat("Generating Field Report");
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var field in fields)
                { sb.AppendLine(field); }
                WriteLocalProcessedFile($"FieldList Report- {DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day} - {DateTime.Now.Ticks}.txt", sb.ToString(), logger);
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Failed Generating Field Report {exception.Message} - {exception.StackTrace}");
                throw;
            }

        }

        public static void GenerateFieldReport(ILog logger)
        {
            if (logger.IsInfoEnabled)
                logger.InfoFormat("Generating Field Report");
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    StringBuilder sb = new StringBuilder();
                    var orgs = projectsDb.Organizations.ToList();
                    List<string> customFields = new List<string>();
                    foreach (var org in orgs)
                    {
                        var projs = projectsDb.Projects.Where(p => p.OrganizationId == org.Id).ToList();
                        foreach (var proj in projs)
                        {
                            var fields = projectsDb.Fields.Where(f => (f.ProjectId == proj.Id)).ToList();
                            foreach (var field in fields)
                            {
                                sb.AppendLine($"{org.Name}\t{proj.Name}\t{field.Name}\t{field.ReferenceName}");
                                if (!field.ReferenceName.Contains("System.") &
                                    field.ReferenceName.Contains("Microsoft."))
                                {
                                    if (!customFields.Contains(field.ReferenceName))
                                        customFields.Add(field.ReferenceName);
                                }
                            }
                        }

                    }
                    sb.AppendLine($"Cutom Fields Across All Orgs {customFields.Count()}");

                    WriteLocalProcessedFile($"FieldList Report- {DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day} - {DateTime.Now.Ticks}.txt", sb.ToString(), logger);
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Failed Generating Report {exception.Message} - {exception.StackTrace}");
                throw;
            }

        }

        private static void GenerateLastAccessReport(ILog logger)
        {
            if (logger.IsInfoEnabled)
                logger.InfoFormat("Generating Last Access Report");
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Organization\tProject\tProject Last updated\tPipline Count\tLast Build Date\tLast WorkItem Changed Date");
                    foreach (var org in projectsDb.Organizations)
                    {
                        var projects = projectsDb.Projects.Where(t => t.OrganizationId == org.Id).ToList();
                        foreach (var proj in projects)
                        {
                            sb.AppendLine($"{org.Name}\t{proj.Name}\t{proj.LastUpdateTime}\t{proj.PipelineCount}\t{proj.LastBuildDate}\t{proj.LastWorkItemChangedDate}");
                        }
                    }

                    WriteLocalProcessedFile($"AzDO Last Access Date Report - {DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day} - {DateTime.Now.Ticks}.txt", sb.ToString(), logger);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat($"Failed Generating Last Access Report {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }

        private static void GenerateLastBuildReport(ILog logger)
        {
            if (logger.IsInfoEnabled)
                logger.InfoFormat("Generating Organization / Project Report");
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Organization\tProject\tProject Last updated\tPipline Count\tLast Build Date");
                    foreach (var org in projectsDb.Organizations)
                    {
                        var projects = projectsDb.Projects.Where(t => t.OrganizationId == org.Id).ToList();
                        foreach (var proj in projects)
                        {
                            sb.AppendLine($"{org.Name}\t{proj.Name}\t{proj.LastUpdateTime}\t{proj.PipelineCount}\t{proj.LastBuildDate}");
                        }
                    }

                    WriteLocalProcessedFile($"AzDO Last Build Date Report - {DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day} - {DateTime.Now.Ticks}.txt", sb.ToString(), logger);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat($"Failed Generating Report {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }
        private static void GenerateReport(ILog logger)
        {
            if (logger.IsInfoEnabled)
                logger.InfoFormat("Generating Organization / Project Report");
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Organizations:\t{projectsDb.Organizations.ToList().Count}");
                    sb.AppendLine($"Number of Unique Process Templates:\t{projectsDb.ProcessTemplates.ToList().Count}");
                    sb.AppendLine($"Number of Projects Across All Orgs:\t{projectsDb.Projects.ToList().Count}");
                    sb.AppendLine($"Number of Teams Across All Orgs and Projects:\t{projectsDb.Projects.Sum(p => p.TeamCount)}");
                    sb.AppendLine($"Number of PipeLines All Orgs and Projects:\t{projectsDb.Projects.Sum(p => p.PipelineCount)}");

                    foreach (var org in projectsDb.Organizations)
                    {
                        sb.AppendLine($"{org.Name}");
                        var templates = projectsDb.ProcessTemplates.Where(t => t.OrganizationId == org.Id).ToList();
                        foreach (var template in templates)
                        {
                            sb.AppendLine($"\t {template.Name}\t{projectsDb.Projects.Count(p => (p.OrganizationId == org.Id && p.ProcessTemplate == template.Name))}");
                        }
                    }

                    WriteLocalProcessedFile(Settings.REPORTFILE, sb.ToString(), logger);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat($"Failed Generating Report {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }

        private static void WriteLocalProcessedFile(string filename, string output, ILog logger)
        {
            try
            {
                TextWriter writer = null;
                try
                {
                    writer = new StreamWriter(filename, false);
                    writer.WriteLine(output);
                }
                finally
                {
                    writer?.Close();
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat($"Unable to Write Local Processed File {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }
        #endregion

        #region GetAzDO Data
        private static void GetUserDataForOrganizations(Controller azdoController, ILog logger)
        {
            try
            {
                if (logger.IsInfoEnabled)
                    logger.InfoFormat("Fetching User Info for All Orgs");

                azdoController.GetOrganizationUserEntitlements("accenturecio08");

            }
            catch (Exception ex)
            {
                logger.ErrorFormat($"Failed User Data {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }

        private static void UpdateOrganizationUserCount(Controller azdoController, ILog logger)
        {
            try
            {
                if (logger.IsInfoEnabled)
                    logger.InfoFormat("Fetching User Info for All Orgs");
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    foreach (var org in projectsDb.Organizations)
                    {
                        int cnt = azdoController.GetOrganizationUserCount(org.Name);
                        org.UserCount = cnt;

                    }
                    projectsDb.SaveChanges();
                }

                //azdoController.GetOrganizationUserCount("accenturecio08");

            }
            catch (Exception ex)
            {
                logger.ErrorFormat($"Failed User Data {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }
        private static string GetProcessIdFromProjectInfo(Controller azdoController, Organization org, Project project)
        {
            string processID = string.Empty;
            var proj = azdoController.GetProjectInfoFromProjId(org.Name, project.AzDoProjectId);
            foreach (var prop in proj.value)
            {
                if (prop.name == "System.ProcessTemplateType")
                {
                    processID = prop.value;
                }
            }

            return processID;
        }

        private static void GetAllFieldsForAllOrgs(Controller azdoController, ILog logger)
        {
            try
            {
                if (logger.IsInfoEnabled)
                    logger.InfoFormat("Fetching Field Data for All Projects in All Orgs");

                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    var orgs = projectsDb.Organizations.ToList();
                    List<string> uniqueFields = new List<string>();
                    foreach (var org in orgs)
                    {
                        var projects = projectsDb.Projects.Where(p => p.OrganizationId == org.Id).ToList();
                        foreach (var project in projects)
                        {
                            if (azdoController.DoesProjectExist(org.Name, project.AzDoProjectId))
                            {
                                if (logger.IsInfoEnabled)
                                    logger.InfoFormat($"Fetching Fields for {org.Name} -  {project.Name}");
                                var fields = azdoController.GetFieldsFromAzDOForProject(org.Name, project.Name);

                                var newFields = fields.value.Select(p =>
                                    new AzDOOrganizationData.Model.Field
                                    {
                                        Description = p.description,
                                        ProjectId = project.Id,
                                        ReferenceName = p.referenceName,
                                        Name = p.name,
                                        Type = p.type,
                                        Usage = p.usage,
                                    }
                                ).ToList();

                                projectsDb.Fields.AddRange(newFields);
                                projectsDb.SaveChanges();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat($"Failed Fields {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }

        private static void UpdateProjectsForAllOrgs(Controller azdoController, ILog logger)
        {
            Console.WriteLine("Updating Project data For All Organizations- ");
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {

                    var orgs = projectsDb.Organizations.ToList();
                    foreach (var org in orgs)
                    {
                        UpdateProjectsForSingleOrg(azdoController, projectsDb, org, logger);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private static void UpdateProjectsForSingleOrg(Controller azdoController, AzDOOrganizationDbContext projectsDb, Organization org, ILog logger)
        {
            if (logger.IsInfoEnabled)
                logger.InfoFormat($"Updating Project Data for {org.Name}");

            try
            {
                //var projectList = azdoController.GetCurrentOrganizationProjectList(org.Name);
                var projectList = azdoController.GetCurrentOrganizationProjectListTest(org.Name);
                if (projectList != null)
                {
                    var templates = projectsDb.ProcessTemplates.Where(p => p.OrganizationId == org.Id).ToList();
                    List<AzDOOrganizationData.Model.Project> dataProjects = CreateNewProjectsForOrg(azdoController, projectsDb, org, projectList, templates, logger);
                    projectsDb.Projects.AddRange(dataProjects);

                    projectsDb.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat(
                           $"Unable to complete  Data Collection and Reporting {exception.Message} - {exception.StackTrace}");

                if (exception.InnerException != null)
                    logger.ErrorFormat(
                           $"INNER EXCEPTION {exception.InnerException.Message} - {exception.InnerException.StackTrace}");

                Console.WriteLine(exception.Message);
                throw;
            }
        }

        private static List<AzDOOrganizationData.Model.Project> CreateNewProjectsForOrg(Controller azdoController, AzDOOrganizationDbContext projectsDb, Organization org, AzDO.Controller.JsonData.ProjectList projectList, List<ProcessTemplate> templates, ILog logger)
        {

            if (logger.IsInfoEnabled)
                logger.InfoFormat($"Updating Project Data for {org.Name}");

            try
            {
                List<AzDOOrganizationData.Model.Project> dataProjects = new List<AzDOOrganizationData.Model.Project>();
                foreach (var proj in projectList.value)
                {
                    if (!projectsDb.Projects.Any(p => p.AzDoProjectId == proj.id))
                    {
                        AzDOOrganizationData.Model.Project dataProj = CreateNewProjectDataObject(azdoController, org, templates, proj, logger);
                        dataProjects.Add(dataProj);
                    }
                }

                return dataProjects;
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Unable to complete CreateNewProjectsForOrg {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private static AzDOOrganizationData.Model.Project CreateNewProjectDataObject(Controller azdoController, Organization org, List<ProcessTemplate> templates, AzDO.Controller.JsonData.Project proj, ILog logger)
        {
            if (logger.IsInfoEnabled)
                logger.InfoFormat($"Creating new Project Data Object for {proj.name}");

            try
            {
                var projectInfo = azdoController.GetProjectInfoFromProjId(org.Name, proj.id);
                proj.properties = projectInfo;
                var processTemplateid = projectInfo.value.First(v => v.name == "System.ProcessTemplateType").value;
                //var processTemplate = processList.value.FirstOrDefault(v => v.id == processTemplateid).name;
                var processTemplate = templates.FirstOrDefault(v => v.UniqueAzDOId == processTemplateid);
                int teamCount = 1;

                if (projectInfo.value.Any(v => v.name == "System.Microsoft.TeamFoundation.Team.Count"))
                {
                    teamCount = int.Parse(projectInfo.value.FirstOrDefault(v => v.name == "System.Microsoft.TeamFoundation.Team.Count").value);
                }

                //int pipelineCount = azdoController.GetProjectPipelineCount(org.Name, proj.name);
                //string changedDate = azdoController.GetMostRecentWorkItemChangedDate(org.Name, proj.name);
                //DateTime lastBuild = GetLastBuildDate(org, proj, client, logger);

                AzDOOrganizationData.Model.Project dataProj = new AzDOOrganizationData.Model.Project
                {
                    AzDoProjectId = proj.id,
                    OrganizationId = org.Id,
                    Name = proj.name,
                    Description = proj.description,
                    LastUpdateTime = proj.lastUpdateTime,
                    //MSPROJ = proj.properties.value.FirstOrDefault(v => v.name == "System.MSPROJ").value,
                    //ProcessTemplate = processTemplate.Name,
                    OriginalProcessTemplateId = projectInfo.value.FirstOrDefault(v => v.name == "System.OriginalProcessTemplateId").value,
                    CurrentProcessTemplateId = projectInfo.value.FirstOrDefault(v => v.name == "System.CurrentProcessTemplateId").value,
                    TeamCount = teamCount,
                    //PipelineCount = pipelineCount,
                    //LastWorkItemChangedDate = changedDate,
                };
                if (processTemplate != null)
                {
                    dataProj.ProcessTemplate = processTemplate.Name;
                }
                //dataProj.LastBuildDate = pipelineCount > 0 ? azdoController.GetLastBuildDate(org.Name, proj.name) : string.Empty;

                return dataProj;
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Unable to complete CreateNewProjectDataObject {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private static string GetMostRecentWorkItemChangedDate(string orgName, string projName, Controller azdoController, ILog logger)
        {
            try
            {
                int workItemId = azdoController.GetMostRecentlyChangedWorkItemId(orgName, projName);
                var changedDate = azdoController.GetWorkItemChangedDate(orgName, projName, workItemId);
                return changedDate;

            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Unable to complete GetMostRecentWorkItemChangedDate {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private static void UpdateProcessTemplatesForAllOrgs(Controller azdoController, ILog logger)
        {
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    var orgs = projectsDb.Organizations.ToList();
                    foreach (var org in orgs)
                    {
                        UpdateProcessTemplatesForSingleOrg(azdoController, projectsDb, org, logger);
                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Unable to complete UpdateProcessTemplatesForSingleOrg {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private static void UpdateProcessTemplatesForSingleOrg(Controller azdoController, AzDOOrganizationDbContext projectsDb, Organization org, ILog logger)
        {
            try
            {
                var processList = azdoController.GetOrganizationProcessTemplates(org.Name);

                if (processList != null)
                {
                    foreach (var pt in processList.value)
                    {
                        logger.InfoFormat(pt.name);
                        if (!projectsDb.ProcessTemplates.Any(t => ((t.Name == pt.name) && (t.OrganizationId == org.Id))))
                        {
                            //Create New Process Template
                            var template = new ProcessTemplate
                            {
                                OrganizationId = org.Id,
                                Description = pt.description,
                                Name = pt.name,
                                Type = pt.type,
                                Url = pt.url,
                                UniqueAzDOId = pt.id
                            };
                            projectsDb.ProcessTemplates.Add(template);
                            projectsDb.SaveChanges();
                            //Add Process Template to List
                        }
                    }
                }
                else { Console.WriteLine($"No Access to {org.Name}"); }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Unable to complete UpdateProcessTemplatesForSingleOrg {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        #endregion
        #region Update AzDO Orgs and Projects
        private static void AddFieldsToProjects(Controller azdoController, ILog logger)
        {
            try
            {
                using (var projectsDb = new AzDOOrganizationDbContext())
                {
                    var orgs = projectsDb.Organizations.Where(o => o.UpdateFields == true).ToList();
                    foreach (var org in orgs)
                    {
                        //Update Projects for Org
                        UpdateProcessTemplatesForSingleOrg(azdoController, projectsDb, org, logger);
                        UpdateProjectsForSingleOrg(azdoController, projectsDb, org, logger);
                        var projects = projectsDb.Projects.Where(p => p.OrganizationId == org.Id).OrderBy(p => p.Name).ToList();
                        var newFields = projectsDb.NewFields.ToList();
                        var exceptions = projectsDb.ProjectExceptions.Where(p => p.OrganizationId == org.Id).ToList();
                        //ManageProcessExceptions(exceptions, azdoController, projectsDb, logger);
                        foreach (var project in projects)
                        {
                            if (!exceptions.Any(e => e.ProjectName == project.Name))// && (project.Name == "EnterpriseArchitecture_2641"))
                            {
                                try
                                {
                                    logger.InfoFormat($"Attemping to Add fields to Process Template: {project.Name}");
                                    int newFieldOrder = 1;
                                    foreach (var newFieldData in newFields)
                                    {

                                        string processID = GetProcessIdFromProjectInfo(azdoController, org, project);


                                        if (newFieldData.IsPickList)
                                        {
                                            var pickListId =
                                                 azdoController.CreatePickLIst(org.Name, newFieldData.PickListJSON);

                                            CreateFieldJson createField = JsonConvert.DeserializeObject<CreateFieldJson>(newFieldData.CreateJSON);
                                            createField.pickListId = pickListId;


                                            var addField =
                                                JsonConvert.DeserializeObject<AddFieldJson>(newFieldData.AddJSON);


                                            addField.pickListId = pickListId;

                                            newFieldData.CreateJSON = JsonConvert.SerializeObject(createField);
                                            newFieldData.AddJSON = JsonConvert.SerializeObject(addField);

                                            projectsDb.SaveChanges();
                                        }


                                        if (!azdoController.DoesFieldExistInProject(newFieldData.ReferenceName, org.Name, project.Name))
                                        {
                                            var createFieldResponse = azdoController.CreateNewWorkItemField(org.Name, project.Name, newFieldData.CreateJSON);
                                        }

                                        var typeIds = projectsDb.FieldItemRelationShips
                                            .Where(r => r.NewFieldId == newFieldData.Id).Select(r => r.WorkItemTypeId)
                                            .ToList();


                                        foreach (var wiTypeId in typeIds)
                                        {
                                            try
                                            {
                                                var wiType = projectsDb.WorkItemTypes.First(t => t.Id == wiTypeId);



                                                if (azdoController.ProcessHasWorkItemType(org.Name, processID, wiType.Name))
                                                {
                                                    logger.InfoFormat($"Updating {wiType.Name} in {project.Name}");
                                                    var derivedWorkItemType = azdoController.CreateDerivedWorkItemType(org.Name, project.Name, processID, wiType.Name, wiType.AddJson);

                                                    var witLayout = azdoController.GetWorkItemTypeLayout(org.Name,
                                                        processID, derivedWorkItemType.ReferenceName);

                                                    string addGroupJSON = "{\"controls\": null,\"id\": null,\"label\": \"Work Alignment\",\"order\": null,\"overridden\": null,\"inherited\": null,\"visible\": true}";
                                                    var newGroupName = azdoController.AddGroupToWorkItemTemplate(org.Name,
                                                        processID, derivedWorkItemType.ReferenceName, witLayout.Pages[0].Id,
                                                        "Section2", "Work Alignment", addGroupJSON);

                                                    var field = azdoController.AddWorkItemFieldToWorkItemType(org.Name, project.Name, processID, derivedWorkItemType.ReferenceName, newFieldData.AddJSON);
                                                    //string createControlToGroupJson = "{\"id\": \"" + field.referenceName + "\",\"order\": \"" + newFieldOrder + "\",\"label\": \"" + newFieldData.Name + "\",\"readOnly\": false,\"visible\": true,\"controlType\": \"pickListString\",\"metadata\": null,\"inherited\": null,\"overridden\": null,\"watermark\": null,\"contribution\": null,\"isContribution\": false,\"height\": null}";
                                                    string createControlToGroupJson = "{\"id\": \"" + field.referenceName + "\", \"order\": \"" + newFieldOrder + "\", \"label\": \"" + newFieldData.Name + "\", \"readOnly\": false, \"visible\": true, \"controlType\": \"pickListString\", \"isContribution\": false }";
                                                    var controlToGroup = azdoController.CreateControlToGroup(org.Name, processID, derivedWorkItemType.ReferenceName, newGroupName, createControlToGroupJson);




                                                }
                                            }
                                            catch (Exception exception)
                                            {

                                                logger.ErrorFormat($"Failed Adding Fields to work item type {wiTypeId}: {exception.Message} - {exception.StackTrace}");
                                            }
                                        }

                                        newFieldOrder++;

                                    }
                                }
                                catch (Exception exception)
                                {
                                    logger.ErrorFormat($"Failed Adding Fields to Project: {project.Name} - {exception.Message} - {exception.StackTrace}");

                                }
                            }
                            else
                            {
                                logger.InfoFormat($"{project.Name} is on the Field Addition Exception List");
                            }
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat($"Failed Adding Fields {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private static void ManageProcessExceptions(List<ProjectException> exceptions, Controller azDoController, AzDOOrganizationDbContext dbContext, ILog logger)
        {
            try
            {
                foreach (ProjectException projectException in exceptions)
                {
                    var org = dbContext.Organizations.First(o => o.Id == projectException.OrganizationId);
                    var project = dbContext.Projects.First(p =>
                        p.OrganizationId == projectException.OrganizationId && p.Name == projectException.ProjectName);
                    var processid = GetProcessIdFromProjectInfo(azDoController, org, project);
                    var template = dbContext.ProcessTemplates.First(t => t.Name == project.ProcessTemplate);
                    //var process = azDoController.GetProcessFromProcessId(org.Name, processid);//template.UniqueAzDOId);
                    if (!template.Type.Equals("inherited") || template.Name == "CIO-Scrum" || template.Name == "CIO-Agile")
                    {
                        CloneProcess(azDoController, template, processid, org, project);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void CloneProcess(Controller azDoController, ProcessTemplate template, string processid,
            Organization org, Project project)
        {
            var createProcess = new CreateProcessJSON
            {
                Name = template.Name + "-NoWorkAlignment",
                Description = "Derived Exception Template to avoid Work Alignment Fields",
                ParentProcessTypeId = processid,
            };
            var newTemplate = azDoController.CreateDerivedProcessTemplate(org.Name, createProcess);
            var updateJson = new ProjectUpdateJson();
            updateJson.Capabilities = new Capabilities
                { ProcessTemplate = new ProcessTemplateJson { TemplateTypeId = newTemplate.TypeId } };

            // Create Child
            // Switch to Child
            azDoController.UpdateProjectData(org.Name, project.AzDoProjectId, updateJson);
        }

        #endregion

    }
}
