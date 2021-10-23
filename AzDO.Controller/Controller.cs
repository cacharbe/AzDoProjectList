using AzDO.Controller.JsonData;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AzDO.Controller
{
    public class Controller
    {
        private readonly ILog _logger;
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        //private string 
        #region Constructors
        public Controller()
        {
            throw new NotImplementedException();
        }

        public Controller(string baseAzDOUrl, HttpClient client, ILog logger)
        {
            _logger = logger;
            _client = client;
            _baseUrl = baseAzDOUrl;
        }

        public Controller(string baseAzDOUrl, string PAT, string AuthenticationUser, ILog logger)
        {
            _baseUrl = baseAzDOUrl;
            _logger = logger;
            _client = CreateHttpClient(PAT, AuthenticationUser);

        }

        private HttpClient CreateHttpClient(string PAT, string AuthenticationUser)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", AuthenticationUser, PAT))));
            return client;
        }
        #endregion

        #region Create New Fields

        public string CreatePickLIst(string organization, string createPickListJson)
        {
            try
            {
                var createPickListFields =
                    JsonConvert.DeserializeObject<CreatePickListResponse>(createPickListJson);

                var pickListId = FindPickLIst(organization, createPickListFields.Name);
                if (String.IsNullOrEmpty(pickListId))
                {
                     var content = new StringContent(createPickListJson, Encoding.UTF8, "application/json");
                    //https://dev.azure.com/accentureciostg/_apis/work/processes/lists?api-version=6.0-preview.1
                    string uri = String.Join("?",
                        String.Join("/", _baseUrl, organization, "_apis/work/processes/lists"),
                        "api-version=6.0-preview.10");
                    var result = PostRequest(uri, content).Result;
                    var response = JsonConvert.DeserializeObject<CreatePickListResponse>(result);
                    return response.Id;
                }

                return pickListId;

            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete CreatePickLIst {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public string FindPickLIst(string organization, string pickListName)
        {
            try
            {
                
                //https://dev.azure.com/accentureciostg/_apis/work/processes/lists?api-version=6.0-preview.1
                string uri = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/work/processes/lists"), "api-version=6.0-preview.1");
                var result = SendRequest(uri).Result;
                var response = JsonConvert.DeserializeObject<PickListsResults>(result);
                if (response.Lists.Any(l=>l.Name == pickListName))
                {
                    return response.Lists.First(l => l.Name == pickListName).Id;
                }
                return string.Empty;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete FindPickLIst {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        public CreateFieldResponse CreateNewWorkItemField(string orgName, string projectName, string createFieldJson)
        {
            try
            {
                var content = new StringContent(createFieldJson, Encoding.UTF8, "application/json");
                //POST https://dev.azure.com/{organization}/_apis/wit/fields?api-version=6.0
                string uri = String.Join("?", String.Join("/", _baseUrl, orgName, "_apis/wit/fields"), "api-version=6.0");
                var result = PostRequest(uri, content).Result;
                var response = JsonConvert.DeserializeObject<CreateFieldResponse>(result);
                return response;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete CreateNewWorkItemField on {orgName} - {projectName} - {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public string CreateControlToGroup(string organization, string processId, string workItemRefName, string groupId, string createControlJson)
        {
            try
            {
                var createJson = JsonConvert.DeserializeObject<CreateControlToGroupJson>(createControlJson);
                var ctrlId = DoesControlExistOnTemplate(organization, processId, workItemRefName, createJson.Label);
                if (!string.IsNullOrEmpty(ctrlId))
                    return ctrlId;

                var content = new StringContent(createControlJson, Encoding.UTF8, "application/json");

                // POST https://dev.azure.com/{organization}/_apis/work/processes/{processId}/workItemTypes/{witRefName}/layout/groups/{groupId}/controls?api-version=5.0-preview.1
                string uri = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/work/processes",processId, "workItemTypes", workItemRefName, "layout", "groups", groupId, "controls" ), "api-version=6.0-preview.1");
                var result = PostRequest(uri, content).Result;
                var response = JsonConvert.DeserializeObject<CreateControlResponse>(result);
                
                return response.Id;

            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete CreateControlToGroup {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public Field AddWorkItemFieldToWorkItemType(string orgName, string projName, string processId, string workItemType, string addFieldJson)
        {
            try
            {
                var content = new StringContent(addFieldJson, Encoding.UTF8, "application/json");
                //POST https://dev.azure.com/{organization}/_apis/work/processes/{processId}/workItemTypes/{witRefName}/fields?api-version=6.0-preview.2
                string postUri = String.Join("?", String.Join("/", _baseUrl, orgName, "_apis/work/processes", processId, "workItemTypes", workItemType, "fields"), "api-version=6.0-preview.2");

                var result = PostRequest(postUri, content).Result;
                var response = JsonConvert.DeserializeObject<Field>(result);
                

                ////https://dev.azure.com/{organization}/_apis/work/processes/{processId}/workItemTypes/{witRefName}/fields/{fieldRefName}?api-version=6.0-preview.2
                //string patchUri = String.Join("?", String.Join("/", _baseUrl, orgName, "_apis/work/processes", processId, "workItemTypes", workItemType, "fields", response.referenceName), "api-version=6.0-preview.2");
                //result = PatchRequest(patchUri, content).Result;
                return response;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete CreateNewWorkItemField {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

       

        private string GetCreateFieldJson()
        {
            string json = "";
            return json;
        }
        #endregion


        #region GetAzDO Data

        public string GetWorkItemStateChangeDate(string orgName, string projName, int workItemId, string state)
        {
            try
            {
                //https://dev.azure.com/{organization}/{project}/_apis/wit/workItems/{id}/updates?api-version=6.0
                string uri = String.Join("?", String.Join("/", _baseUrl, orgName, projName, "_apis/wit/workitems", workItemId, "updates"), "&api-version=6.0");
                var result = SendRequest(uri).Result;
                var wi = JsonConvert.DeserializeObject<WorkItemUpdates>(result);
                foreach (var update in wi.value)
                {
                    if (update.fields != null && update.fields.State != null)
                    {
                        if (update.fields.State.newValue == state)
                        {
                            return update.fields.RevisedDate.newValue;
                        }
                    }
                }

                return String.Empty;

            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetWorkItemActiveDate {exception.Message} - {exception.StackTrace}");
                throw;
            }

        }
        public string GetWorkItemActiveDate(string orgName, string projName, int workItemId)
        {
            try
            {
                return GetWorkItemStateChangeDate(orgName, projName, workItemId, "Active");

            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetWorkItemActiveDate {exception.Message} - {exception.StackTrace}");
                throw;
            }

        }
        public string GetMostRecentWorkItemChangedDate(string orgName, string projName)
        {
            try
            {
                int workItemId = GetMostRecentlyChangedWorkItemId(orgName, projName);
                var changedDate = GetWorkItemChangedDate(orgName, projName, workItemId);
                return changedDate;

            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetMostRecentWorkItemChangedDate {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        public int GetMostRecentlyChangedWorkItemId(string orgName, string projName)
        {
            try
            {
                string wiql = "{\"query\": \"Select[System.Id] From WorkItems order by [System.ChangedDate] desc\"}";
                var content = new StringContent(wiql, Encoding.UTF8, "application/json");
                string uri = String.Join("?", String.Join("/", _baseUrl, orgName, projName, "_apis/wit/wiql"), "$top=1&api-version=6.0");
                var result = PostRequest(uri, content).Result;
                var response = JsonConvert.DeserializeObject<QueryResponseJson>(result);
                return response.workItems.Length > 0 ? response.workItems[0].id : 0;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetMostRecentlyChangedWorkItemId {exception.Message} - {exception.StackTrace}");
                throw;
            }

        }

        public string GetWorkItemChangedDate(string orgName, string projectName, int workItemId)
        {
            //ttps://dev.azure.com/{organization}/{project}/_apis/wit/workitems/{id}?api-version=6.0
            try
            {
                string uri = String.Join("?", String.Join("/", _baseUrl, orgName, projectName, "_apis/wit/workitems", workItemId), "$top=1&api-version=6.0");
                var result = SendRequest(uri).Result;
                var wi = JsonConvert.DeserializeObject<WorkItemJson>(result);
                return wi.fields.ChangedDate;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetWorkItemChangedDate {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public string GetLastBuildDate(string orgName, string projName)
        {
            try
            {
                var builds = GetBuildInfoFromAzDOForProject(orgName, projName);

                return builds.count > 0 ? builds.value[0].queueTime : string.Empty;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetLastBuildDate {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }


        private void GetWorkItem(string organization, string project)
        {
            /*
             Get Most Recent Build: https://docs.microsoft.com/en-us/rest/api/azure/devops/build/latest/get?view=azure-devops-rest-6.0
            GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/{id}?api-version=6.0
        https://dev.azure.com/{org}accenturecio08/{proj}/_apis/build/builds?$top=1&queryOrder=queueTimeDescending&api-version=6.1-preview.6
            */
            try
            {
                string buildsUrl = String.Join("?", String.Join("/", _baseUrl, organization, project, "_apis/build/builds"), String.Join("&", "$top=1", "queryOrder=queueTimeDescending", "api -version=6.0-preview.1"));
                string result = SendRequest(buildsUrl).Result;
                var buildList = JsonConvert.DeserializeObject<BuildList>(result);

            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetWorkItem {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private BuildList GetBuildInfoFromAzDOForProject(string organization, string project)
        {
            /*
             Get Most Recent Build: https://docs.microsoft.com/en-us/rest/api/azure/devops/build/latest/get?view=azure-devops-rest-6.0
            https://dev.azure.com/{org}accenturecio08/{proj}/_apis/build/builds?$top=1&queryOrder=queueTimeDescending&api-version=6.1-preview.6
             */
            try
            {
                string buildsUrl = String.Join("?", String.Join("/", _baseUrl, organization, project, "_apis/build/builds"), String.Join("&", "$top=1", "queryOrder=queueTimeDescending", "api -version=6.0-preview.1"));
                string result = SendRequest(buildsUrl).Result;
                var buildList = JsonConvert.DeserializeObject<BuildList>(result);
                return buildList;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetBuildInfoFromAzDOForProject {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }


        public int GetProjectPipelineCount(string orgName, string projId)
        {
            try
            {
                var pipeLines = GetPipelineListFromProjectId(orgName, projId);

                return pipeLines.count;

            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetProjectPipelineCount {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private PipelineList GetPipelineListFromProjectId(string orgName, string projId)
        {
            try
            {
                //ttps://docs.microsoft.com/en-us/rest/api/azure/devops/pipelines/pipelines/list?view=azure-devops-rest-6.0#pipelineconfiguration
                //https://dev.azure.com/{organization}/{project}/_apis/pipelines?api-version=6.0-preview.1
                //string projectUrl = String.Join("?", String.Join("/", AzDO.BASE, AzDO.ORG, "_apis/projects", id, "properties"),String.Join("&", "api-version=6.0-preview.1", "keys=*ProcessTemplate*"));
                string projectUrl = String.Join("?", String.Join("/", _baseUrl, orgName, projId, "_apis/pipelines"), "api-version=6.0-preview.1");
                string result = SendRequest(projectUrl).Result;
                var pipelines = JsonConvert.DeserializeObject<PipelineList>(result);

                return pipelines;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetPipelineListFromProjectId {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private ProcessList ProcessOrganization(string organization)
        {
            var processList = GetOrganizationProcessTemplates(organization);
            return processList;
        }

        public ProjectList GetCurrentOrganizationProjectList(string org)
        {
            //GET https://dev.azure.com/{organization}/_apis/projects?api-version=6.0
            string projectsUrl = String.Join("?", String.Join("/", _baseUrl, org, "_apis/projects"), "?ContinuationToken=&api-version=6.0");

            //Console.WriteLine(projectsUrl);
            try
            {
                string result = SendRequest(projectsUrl).Result;
                ProjectList projectList = JsonConvert.DeserializeObject<ProjectList>(result);
                return projectList;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetCurrentOrganizationProjectList {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public ProjectList GetCurrentOrganizationProjectListTest(string org)
        {
            //GET https://dev.azure.com/{organization}/_apis/projects?api-version=6.0
            string projectsUrl = String.Join("?", String.Join("/", _baseUrl, org, "_apis/projects"), "api-version=6.0");

            //Console.WriteLine(projectsUrl);
            try
            {
                var results = SendLargeRequest(projectsUrl, "0").Result;
                List<ProjectList> projects = new List<ProjectList>();
                foreach (string result in results)
                {
                    ProjectList projList = JsonConvert.DeserializeObject<ProjectList>(result);
                    projects.Add(projList);

                }
                ProjectList projectList = new ProjectList();
                projectList.count = projects.Sum(p => p.count);
                projectList.value = new Project[projectList.count];
                int cnt = 0;
                foreach (var proj in projects)
                {
                    foreach (var prop in proj.value)
                    {
                        projectList.value[cnt] = prop;
                        cnt++;
                    }
                }

                return projectList;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetCurrentOrganizationProjectList {org} - {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public ProjectPropertyList GetProjectInfoFromProjId(string organization, string id)
        {
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/core/projects/get%20project%20properties?view=azure-devops-rest-6.0
            //https://dev.azure.com/{organization}/_apis/projects/{projectId}/properties?api-version=6.0-preview.1
            //string projectUrl = String.Join("?", String.Join("/", AzDO.BASE, AzDO.ORG, "_apis/projects", id, "properties"),String.Join("&", "api-version=6.0-preview.1", "keys=*ProcessTemplate*"));
            try
            {
                string projectUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/projects", id, "properties"), String.Join("&", "api-version=6.0-preview.1"));
                string result = SendRequest(projectUrl).Result;
                ProjectPropertyList props = JsonConvert.DeserializeObject<ProjectPropertyList>(result);
                return props;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetProjectInfoFromProjId {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public void GetOrganizationUsers(string organization)
        {
            //https://vssps.dev.azure.com/{organization}/_apis/graph/users?api-version=6.0-preview.1
            try
            {
                string processUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/graph/users"), "api-version=6.0-preview.1");

                string result = SendRequest(processUrl).Result;
                var processList = JsonConvert.DeserializeObject<ProcessList>(result);
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetOrganizationUsers {exception.Message} - {exception.StackTrace}");
                //return null;
            }

        }

        public int GetOrganizationUserCount(string organization)
        {
            try
            {
                var memberList = GetOrganizationUserEntitlements(organization);
                return memberList.TotalCount;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetOrganizationUserCount {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        public MemberList GetOrganizationUserEntitlements(string organization)
        {
            //https://vsaex.dev.azure.com/$OrganizationName/_apis/userentitlements?api-version=5.1-preview.2
            //api-version=6.0-preview.3
            try
            {
                string processUrl = String.Join("?", String.Join("/", "https://vsaex.dev.azure.com", organization, "_apis/userentitlements"), "api-version=5.1-preview.2");
                string result = SendRequest(processUrl).Result;
                var memberList = JsonConvert.DeserializeObject<MemberList>(result);
                return memberList;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetOrganizationUsers {exception.Message} - {exception.StackTrace}");
                return null;
            }

        }

        public Process GetProcessFromProcessId (string organization, string processId)
        {
            //GET https://dev.azure.com/{organization}/_apis/process/processes/{processId}?api-version=6.0
            try
            {
                string processUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/process/processes", processId), "api-version=6.0");
                string result = SendRequest(processUrl).Result;
                var process= JsonConvert.DeserializeObject<Process>(result);
                return process;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetProcessFromProcessIDstring {exception.Message} - {exception.StackTrace}");
                return null;
            }
        }
        public ProcessList GetOrganizationProcessTemplates(string organization)
        {
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/core/processes/list?view=azure-devops-rest-6.0
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/templates/get?view=azure-devops-rest-6.0
            //https://dev.azure.com/{organization}/_apis/process/processes?api-version=6.0
            try
            {
                string processUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/process/processes"), "api-version=6.0");

                string result = SendRequest(processUrl).Result;
                var processList = JsonConvert.DeserializeObject<ProcessList>(result);

                return processList;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetOrganizationProcessTemplates {exception.Message} - {exception.StackTrace}");
                return null;
            }
        }

        public bool DoesProjectExist(string organization, string project)
        {
            try
            {
                string testUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/projects", project, "properties"), String.Join("&", "api-version=6.0-preview.1"));
                var testResult = SendRequest(testUrl);
                var res = testResult.Result;
                var good = testResult.IsCompletedSuccessfully;
                return true;
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("is an Invalid Url"))
                    return false;
                _logger.ErrorFormat($"Unable to complete DoesProjectExist {exception.Message} - {exception.StackTrace}");
                throw;
            }

        }

        public void UpdateProjectData(string organization, string projectId, ProjectUpdateJson updateJson)
        {
            try
            {
                //https://docs.microsoft.com/en-us/rest/api/azure/devops/core/projects/update?view=azure-devops-rest-6.0
                // PATCH https://dev.azure.com/{organization}/_apis/projects/{projectId}?api-version=6.0
                string updateUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/projects", projectId), String.Join("&", "api-version=6.0"));
                var content = new StringContent(JsonConvert.SerializeObject(updateJson), Encoding.UTF8, "application/json");
                var result = PatchRequest(updateUrl, content).Result;
                
            } 
            catch (Exception exception)
            {

                _logger.ErrorFormat($"Unable to complete UpdateProjectData {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        public CreateProcessResponse CreateDerivedProcessTemplate(string organization, CreateProcessJSON templateJson)
        {
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/processes/processes/create?view=azure-devops-rest-6.0
            // POST https://dev.azure.com/{organization}/_apis/work/processes?api-version=6.0-preview.2
            try
            {
                string createProcessUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/work", "processes"), String.Join("&", "api-version=6.0-preview.1"));
                string json = JsonConvert.SerializeObject(templateJson);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = PostRequest(createProcessUrl, content).Result;
                //CreateProcessResponse
                var createResponse = JsonConvert.DeserializeObject<CreateProcessResponse>(result);
                return createResponse;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete CreateDerivedProcessTemplate {exception.Message} - {exception.StackTrace}");
                throw;
            }

        }

      public WorkItemType CreateDerivedWorkItemType(string organization, string project, string processID, string baseWorkItemTypeName, string createJSON)
        {
            try
            {
                var types = GetWorkItemTypeList(organization, processID);

                if (types.WorkItemTypes.Any(t => t.Inherits == baseWorkItemTypeName))
                    return types.WorkItemTypes.First(t => t.Inherits == baseWorkItemTypeName);

                //POST https://dev.azure.com/{organization}/_apis/work/processes/{processId}/workitemtypes?api-version=6.0-preview.2

                string createWITUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/work/processes", processID, "workitemtypes"), "api-version=5.1-preview.2");
                var content = new StringContent(createJSON, Encoding.UTF8, "application/json");
                var result = PostRequest(createWITUrl, content).Result;
                var createResponse = JsonConvert.DeserializeObject<WorkItemType>(result);

                return createResponse;
            }
            catch (Exception exception)
            {

                _logger.ErrorFormat($"Unable to complete CreateDerivedWorkItemType {organization} - {project} - {baseWorkItemTypeName} - {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public Layout GetWorkItemTypeLayout(string organization, string processId, string workItemReferenceName)
        {
            try
            {
                //GET https://dev.azure.com/{organization}/_apis/work/processdefinitions/{processId}/workItemTypes/{witRefName}/layout?api-version=4.1-preview.1

                string getWITLayout = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/work/processdefinitions", processId, "workitemtypes", workItemReferenceName, "layout"), "api-version=4.1-preview.1");
                string result = SendRequest(getWITLayout).Result;
                var layout = JsonConvert.DeserializeObject<Layout>(result);
                return layout;
            }
            catch (Exception exception)
            {

                _logger.ErrorFormat($"Unable to complete GetWorkItemTypeLayout {exception.Message} - {exception.StackTrace}");
                throw; ;
            }
        }

        public string  AddGroupToWorkItemTemplate(string organization, string processId, string workItemReferenceName, string pageId, string sectionId, string groupLabel, string groupJson)
        {
            try
            {
                var exists = DoesGroupExistOnTemplate(organization, processId, workItemReferenceName, groupLabel);
                if (exists == String.Empty )
                {
                    //POST https://dev.azure.com/{organization}/_apis/work/processes/{processId}/workItemTypes/{witRefName}/layout/pages/{pageId}/sections/{sectionId}/groups?api-version=6.0-preview.1
                    string addGroupUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/work/processes", processId, "workitemtypes", workItemReferenceName, "layout", "pages", pageId, "sections", sectionId, "groups"), "api-version=6.0-preview.1");
                    var content = new StringContent(groupJson, Encoding.UTF8, "application/json");
                    var addResponse = PostRequest(addGroupUrl, content).Result;
                    var createResponse = JsonConvert.DeserializeObject<CreateGroupResponse>(addResponse);
                    return createResponse.Id; 
                }
                else
                {

                    return exists;
                }

            }
            catch (Exception exception)
            {

                _logger.ErrorFormat($"Unable to complete AddGroupToWorkItemTemplate {exception.Message} - {exception.StackTrace}");
                throw; ;
            }
        }

        public string  DoesGroupExistOnTemplate(string organization, string processId, string workItemReferenceName, string groupLabel)
        {

            try
            {
                var layout = GetWorkItemTypeLayout(organization, processId, workItemReferenceName);
                foreach (var page in layout.Pages)
                {
                    foreach (var section in page.Sections)
                    {
                        if (section.Groups.Any(g => g.Label == groupLabel))
                        {
                            var group = section.Groups.First(g => g.Label == groupLabel);
                            return group.Id;
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to determine DoesGroupExistOnTemplate {organization} - {processId} - {workItemReferenceName} - {groupLabel} - {exception.Message} - {exception.StackTrace}");
                throw;
            }
            return string.Empty;
        }

        public string DoesControlExistOnTemplate(string organization, string processId, string workItemReferenceName, string controlName)
        {
            try
            {

                var layout = GetWorkItemTypeLayout(organization, processId, workItemReferenceName);
                foreach (var page in layout.Pages)
                {
                    foreach (var section in page.Sections)
                    {
                        foreach (var group in section.Groups)
                        {
                            if (group.Controls.Any(g => g.Label == controlName))
                            {

                                var ctrl = group.Controls.First(g => g.Label == controlName);
                                return ctrl.Label;
                            }

                        }

                    }
                }
                return string.Empty;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to determine DoesControlExistOnTemplate  {organization} - {processId} - {workItemReferenceName} - {controlName} - {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public WorkItemTypeList GetWorkItemTypeList(string organization, string processID)
        {
            // GET https://dev.azure.com/{organization}/_apis/work/processes/{processId}/workitemtypes?api-version=5.1-preview.2
            string getWITListUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/work/processes", processID, "workitemtypes"), "api-version=5.1-preview.2");
            string result = SendRequest(getWITListUrl).Result;
            var processList = JsonConvert.DeserializeObject<WorkItemTypeList>(result);
            return processList;
        }

        public bool ProcessHasWorkItemType(string organization, string processID, string workItemTypeRefName)
        {
            // GET https://dev.azure.com/{organization}/_apis/work/processes/{processId}/workitemtypes?api-version=5.1-preview.2
           var types = GetWorkItemTypeList(organization, processID);

                if (types.WorkItemTypes.Any(t => t.ReferenceName == workItemTypeRefName) || (types.WorkItemTypes.Any(t => t.Inherits == workItemTypeRefName)))
                    return true;

                return false;
        }

        public bool DoesFieldExistInProject(string field, string organization, string project)
        {
            try
            {
                var fields = GetFieldsFromAzDOForOrganization(organization);
                var exists = (fields.value.ToList()).Any(i => i.name == field || i.referenceName == field);
                return exists;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete DoesFieldExistInProject {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }
        public FieldList GetFieldsFromAzDOForProject(string organization, string project)
        {
            /*
             https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/fields?view=azure-devops-rest-4.1
            GET https://dev.azure.com/{organization}/{project}/_apis/wit/fields?api-version=4.1
             */
            try
            {
                string fieldsUrl = String.Join("?", String.Join("/", _baseUrl, organization, project, "_apis/wit/fields"), "api-version=4.1");
                string result = SendRequest(fieldsUrl).Result;
                var fieldList = JsonConvert.DeserializeObject<FieldList>(result);
                return fieldList;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetFieldsFromAzDOForProject {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public FieldList GetFieldsFromAzDOForOrganization(string organization)
        {
            /*
             https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/fields?view=azure-devops-rest-4.1
            GET https://dev.azure.com/{organization}/{project}/_apis/wit/fields?api-version=4.1
             */
            try
            {
                string fieldsUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/wit/fields"), "api-version=6.0");
                string result = SendRequest(fieldsUrl).Result;
                var fieldList = JsonConvert.DeserializeObject<FieldList>(result);
                return fieldList;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetFieldsFromAzDOForProject {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        public void GetConnectedOrganizations(string organization)
        {
            //https://stackoverflow.com/questions/54762368/get-all-organizations-in-azure-devops-using-rest-api
            //Post https://dev.azure.com/{organization1}/_apis/Contribution/HierarchyQuery?api-version=5.0-preview.1

            try
            {
                string organizationsUrl = String.Join("?", String.Join("/", _baseUrl, organization, "_apis/Contribution/HierarchyQuery"), "api-version=5.0-preview.1");
                string postData = "{\"contributionIds\": [\"ms.vss-features.my-organizations-data-provider\"],\"dataProviderContext\":{\"properties\":{}}}";
                var content = new StringContent(postData, Encoding.UTF8, "application/json");

                var result = PostRequest(organizationsUrl, content).Result;
                var orgsList = JsonConvert.DeserializeObject(result);
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete GetConnectedOrganizations {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }


        #endregion

        #region HTTP Methods
        private async Task<string> PostRequest(string uri, StringContent content)
        {
            try
            {
                using (HttpResponseMessage response = await _client.PostAsync(uri, content))
                {
                    response.EnsureSuccessStatusCode();
                    return (await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception exception)
            {
                var json = content.ReadAsStringAsync();
                _logger.ErrorFormat($"Unable to complete Post Request {uri} - {content.ToString()} -  {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private async Task<string> PatchRequest(string uri, StringContent content)
        {
            try
            {
                using (HttpResponseMessage response = await _client.PatchAsync(uri, content))
                {
                    response.EnsureSuccessStatusCode();
                    return (await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception exception)
            {
                var json = content.ReadAsStringAsync();
                _logger.ErrorFormat($"Unable to complete Post Request {uri} - {content.ToString()} -  {exception.Message} - {exception.StackTrace}");
                throw;
            }
        }

        private async Task<string> SendRequest(string uri)
        {
            try
            {
                using (HttpResponseMessage response = await _client.GetAsync(uri))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        return (await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        throw new ArgumentException($"{uri} is an Invalid Url");
                    }
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete Send Request {exception.Message} - {exception.StackTrace}");
                throw;
            }
        } // End of Send 

        private async Task<IEnumerable<string>> SendLargeRequest(string uri, string continueNumber)
        {
            try
            {
                string continuationUrl = String.Join("&", uri, $"ContinuationToken={continueNumber}");
                using (HttpResponseMessage response = await _client.GetAsync(continuationUrl))
                {
                    IEnumerable<string> cont;
                    List<string> responses = new List<string>();
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        try
                        {
                            cont = response.Headers.GetValues("x-ms-continuationtoken");
                        }
                        catch (InvalidOperationException)
                        {

                            cont = null;
                        }
                        responses.Add(responseBody);
                        if (cont != null)
                        {
                            var r = SendLargeRequest(uri, cont.First()).Result;
                            responses.AddRange(r);
                        }
                        return (responses);
                    }
                    else
                    {
                        throw new ArgumentException($"{uri} is an Invalid Url");
                    }
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.ErrorFormat($"Unable to complete Send Request {exception.Message} - {exception.StackTrace}");
                throw;
            }
        } // End of Send 


        #endregion
    }
}
