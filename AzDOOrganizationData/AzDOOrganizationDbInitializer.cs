using System;
using System.Collections.Generic;
using AzDOOrganizationData.Model;
using System.Data.Entity;

namespace AzDOOrganizationData
{
    //public class AzDOOrganizationDbInitializer : DropCreateDatabaseAlways<AzDOOrganizationDbContext>
    //public class AzDOOrganizationDbInitializer : DropCreateDatabaseIfModelChanges<AzDOOrganizationDbContext>
    public class AzDOOrganizationDbInitializer : DropCreateDatabaseAlways<AzDOOrganizationDbContext>
    {
        protected override void Seed (AzDOOrganizationDbContext context)
        {
            var organizations = GetOrganizations();
            var neFileds = GetFieldsToBeAdded();
            var newWorkItemTypes = GetNewWorkItemTypes();
            var relationships = GetFieldWIRelationships();
            var exceptions = GetProjectExceptions();
            context.WorkItemTypes.AddRange(newWorkItemTypes);
            context.Organizations.AddRange(organizations);
            context.NewFields.AddRange(neFileds);
            context.FieldItemRelationShips.AddRange(relationships);
            context.ProjectExceptions.AddRange(exceptions);
            context.SaveChanges();
            base.Seed(context);
        }

       


        private List<NewField> GetFieldsToBeAdded()
        {
            var newFields = new List<NewField>
            {
                new NewField {Id = 1, Name = "Type of Work", ReferenceName = "CIOTypeOfWork",
                    IsPickList = true,
                    CreateJSON ="{\"name\":\"Type of Work\",\"referenceName\":\"CIOTypeOfWork\",\"type\":\"string\",\"usage\":\"workItem\",\"readOnly\":false,\"canSortBy\":true,\"isQueryable\":true,\"supportedOperations\":[{\"referenceName\":\"CIOTypeOfWork\",\"name\":\"=\"}],\"isIdentity\":true,\"isPicklist\":true,\"pickListId\":\"9700fa4e-b88b-4b09-8169-bfece1989c9c\",\"isPicklistSuggested\":false,\"url\":null, \"required\":true}",
                    AddJSON =   "{\"name\":\"Type of Work\",\"referenceName\":\"CIOTypeOfWork\",\"type\":\"pickListString\",\"usage\":\"workItem\",\"defaultValue\":\"\",\"readOnly\":false,\"canSortBy\":true,\"isQueryable\":true,\"isPicklist\":true,\"isPicklistSuggested\":false,\"pickListId\":\"9700fa4e-b88b-4b09-8169-bfece1989c9c\",\"url\":null,\"allowedValues\":[\"Accept Service\",\"Plan and Manage Service\",\"Incidents\",\"Service Requests\",\"Change Request\",\"Defects\",\"End User Enablement\",\"Data Patrol and Triage\",\"System Monitoring and Triage\",\"Compliance Vulnerability\",\"Recurring Compliance Work\",\"Functional Collaterals\",\"Security Compliance\",\"Infrastructure, Technical and Tools\",\"Legal & Regulatory\",\"Discretionary Enhancement\",\"Impact Assessment Surveys\",\"Stay Current\",\"Problem/RCA\",\"Identify and Manage to Service Level Indicators\",\"Defining and Managing and Using Error Budgets\",\"Automation\",\"Diagnosis and Trend Analysis\",\"Product Architecture Design Reviews\",\"Deployment Support\",\"Identify and Reduce Tech Debt\",\"Capacity Planning and Management\",\"Data Architecture and Processing\",\"New Capability\"],\"required\":true}",
                    PickListJSON = "{\"id\":null,\"name\":\"TypeOfWorkPickList\",\"type\":\"String\",\"url\":null,\"items\":[\"Accept Service\",\"Plan and Manage Service\",\"Incidents\",\"Service Requests\",\"Change Request\",\"Defects\",\"End User Enablement\",\"Data Patrol and Triage\",\"System Monitoring and Triage\",\"Compliance Vulnerability\",\"Recurring Compliance Work\",\"Functional Collaterals\",\"Security Compliance\",\"Infrastructure, Technical and Tools\",\"Legal & Regulatory\",\"Discretionary Enhancement\",\"Impact Assessment Surveys\",\"Stay Current\",\"Problem/RCA\",\"Identify and Manage to Service Level Indicators\",\"Defining and Managing and Using Error Budgets\",\"Automation\",\"Diagnosis and Trend Analysis\",\"Product Architecture Design Reviews\",\"Deployment Support\",\"Identify and Reduce Tech Debt\",\"Capacity Planning and Management\",\"Data Architecture and Processing\",\"New Capability\"],\"isSuggested\":false,\"required\":true}",
                    }
                ,
                new NewField {Id = 2, Name = "Funding Source", ReferenceName = "CIOFundingSource",
                    IsPickList = true,
                    CreateJSON ="{\"name\":\"Funding Source\",\"referenceName\":\"CIOFundingSource\",\"type\":\"string\",\"usage\":\"workItem\",\"defaultValue\":\"\",\"readOnly\":false,\"canSortBy\":true,\"required\":true,\"isQueryable\":true,\"isPicklist\":true,\"isPicklistSuggested\":false,\"pickListId\":null,\"url\":null,\"allowedValues\":[\"Customer MD&I\",\"Customer Operations\",\"MD&I\",\"Operations\"]}",
                    AddJSON = "{\"name\":\"Funding Source\",\"referenceName\":\"CIOFundingSource\",\"type\":\"pickListString\",\"usage\":\"workItem\",\"defaultValue\":\"\",\"required\":true,\"readOnly\":false,\"canSortBy\":true,\"isQueryable\":true,\"isPicklist\":true,\"isPicklistSuggested\":false,\"pickListId\":null,\"url\":null,\"allowedValues\":[\"Customer MD&I\",\"Customer Operations\",\"MD&I\",\"Operations\"]}",
                    PickListJSON = "{\"id\":null,\"name\":\"FundingSourcePickList\",\"type\":\"String\",\"url\":null,\"items\":[\"Customer MD&I\",\"Customer Operations\",\"MD&I\",\"Operations\"],\"isSuggested\":false}",
                 },
                new NewField {Id = 3, Name = "Product Project ID", ReferenceName = "CIOProductProjectID",
                    IsPickList = false,
                    CreateJSON ="{\"name\":\"Product Project ID\",\"referenceName\":\"CIOProductProjectID\",\"description\":null,\"type\":\"string\",\"usage\":\"workItem\",\"readOnly\":false,\"canSortBy\":true,\"isQueryable\":true,\"supportedOperations\":[{\"referenceName\":\"CIOProductProjectID\",\"name\":\"=\"}],\"isIdentity\":true,\"isPicklist\":false,\"isPicklistSuggested\":false,\"url\":null}",
                    AddJSON = "{\"name\":\"Product Project ID\",\"referenceName\":\"CIOProductProjectID\",\"description\":null,\"type\":\"string\",\"usage\":\"workItem\",\"required\":true,\"readOnly\":false,\"canSortBy\":true,\"isQueryable\":true,\"supportedOperations\":[{\"referenceName\":\"CIOProductProjectID\",\"name\":\"=\"}],\"isIdentity\":true,\"isPicklist\":false,\"isPicklistSuggested\":false,\"url\":null}",
                    PickListJSON=""
                    },

            };

           
            return newFields;
        }

        private List<NewFieldWorkItem> GetFieldWIRelationships()
        {
            List<NewFieldWorkItem> relationships = new List<NewFieldWorkItem>
            {
                new NewFieldWorkItem {Id=1,NewFieldId = 1, WorkItemTypeId = 1},
                new NewFieldWorkItem {Id=2,NewFieldId = 1, WorkItemTypeId = 2},
                new NewFieldWorkItem {Id=3,NewFieldId = 2, WorkItemTypeId = 3},
                new NewFieldWorkItem {Id=4,NewFieldId = 3, WorkItemTypeId = 3},
            };
            return relationships;
        }

        private List<WorkItemType> GetNewWorkItemTypes()
        {
            var types = new List<WorkItemType>
            {
                new WorkItemType{Id = 1, Name = "Microsoft.VSTS.WorkItemTypes.UserStory", AddJson = "{\"name\":\"User Story\",\"description\":\"Base User Story With CIO Mandated Fields\",\"color\":\"009CCC\",\"icon\":\"icon_list\",\"isDisabled\":false,\"inheritsFrom\":\"Microsoft.VSTS.WorkItemTypes.UserStory\"}"},
                new WorkItemType{Id = 2, Name = "Microsoft.VSTS.WorkItemTypes.ProductBacklogItem", AddJson ="{\"name\":\"Product Backlog Item\",\"description\":\"Base PBI With CIO Mandated Fields\",\"color\":\"009CCC\",\"icon\":\"icon_list\",\"isDisabled\":false,\"inheritsFrom\":\"Microsoft.VSTS.WorkItemTypes.ProductBacklogItem\"}"},
                new WorkItemType{Id = 3, Name = "Microsoft.VSTS.WorkItemTypes.Epic", AddJson="{\"name\":\"Epic\",\"description\":\"Base Epic With CIO Mandated Fields\",\"color\":\"FF7B00\",\"icon\":\"icon_crown\",\"isDisabled\":false,\"inheritsFrom\":\"Microsoft.VSTS.WorkItemTypes.Epic\"}"},
                new WorkItemType{Id = 4, Name = "Microsoft.VSTS.WorkItemTypes.Feature", AddJson="{\"name\":\"Feature\",\"description\":\"Base Feature With CIO Mandated Fields\",\"color\":\"773B93\",\"icon\":\"icon_trophy\",\"isDisabled\":false,\"inheritsFrom\":\"Microsoft.VSTS.WorkItemTypes.Feature\"}"},
                new WorkItemType{Id = 5, Name = "Microsoft.VSTS.WorkItemTypes.Requirement", AddJson="{\"name\":\"Requirement\",\"description\":\"Base Requirement With CIO Mandated Fields\",\"color\":\"773B93\",\"icon\":\"icon_trophy\",\"isDisabled\":false,\"inheritsFrom\":\"Microsoft.VSTS.WorkItemTypes.Requirement\"}"}
                //
            };
            return types;
        }

        private List<Organization> GetOrganizations()
        {
            var orgs = new List<Organization>
            {
                new Organization { Id = 1, Name="accenture", UpdateFields = false},
                new Organization { Id = 2, Name="accenturecio", UpdateFields = false},
                new Organization { Id = 3, Name="accenturecio01", UpdateFields = false},
                new Organization { Id = 4, Name="accenturecio02", UpdateFields = false},
                new Organization { Id = 5, Name="accenturecio03", UpdateFields = false},
                new Organization { Id = 6, Name="accenturecio04", UpdateFields = false},
                new Organization { Id = 7, Name="accenturecio05", UpdateFields = false},
                new Organization { Id = 8, Name="accenturecio06", UpdateFields = false},
                new Organization { Id = 9, Name="accenturecio07", UpdateFields = false},
                new Organization { Id = 10, Name="accenturecio08", UpdateFields = false},
                new Organization { Id = 11, Name="accenturecio09", UpdateFields = false},
                new Organization { Id = 12, Name="accenturecio10", UpdateFields = false},
                new Organization { Id = 13, Name="accenturecio11", UpdateFields = false},
                new Organization { Id = 14, Name="accentureciostg", UpdateFields = false},
                new Organization { Id = 15, Name="accentureciodev", UpdateFields = true},
            };

            return orgs;
        }

        private List<ProjectException> GetProjectExceptions()
        {
            var exceptions = new List<ProjectException>
            {
                new ProjectException{Id= 1,OrganizationId= 1,ProjectName="ACN-Mobility-Test"},
               new ProjectException{Id= 2,OrganizationId= 1,ProjectName="RAD_Scrum"},
               new ProjectException{Id= 3,OrganizationId= 1,ProjectName="ASW Agile"},
               new ProjectException{Id= 4,OrganizationId= 1,ProjectName="RAD_Agile"},
               new ProjectException{Id= 5,OrganizationId= 1,ProjectName="SandboxAgile"},
               new ProjectException{Id= 6,OrganizationId= 1,ProjectName="UpwardlyGlobal"},
               new ProjectException{Id= 7,OrganizationId= 1,ProjectName="AccentureWayPoint"},
               new ProjectException{Id= 8,OrganizationId= 1,ProjectName="SandboxScrum_Git"},
               new ProjectException{Id= 9,OrganizationId= 1,ProjectName="CIO BETA"},
               new ProjectException{Id= 10,OrganizationId= 1,ProjectName="ASW Scrum"},
               new ProjectException{Id= 11,OrganizationId= 1,ProjectName="ContractManagement_64"},
               new ProjectException{Id= 12,OrganizationId= 1,ProjectName="SandboxScrum"},
               new ProjectException{Id= 13,OrganizationId= 1,ProjectName="SandboxAgile_Git"},
               new ProjectException{Id= 14,OrganizationId= 1,ProjectName="Hack-a-thon"},
               new ProjectException{Id= 15,OrganizationId= 1,ProjectName="AccentureWayPoint_Voceras"},
               new ProjectException{Id= 16,OrganizationId= 1,ProjectName="SQLlint_4090_GIT"},
               new ProjectException{Id= 17,OrganizationId= 1,ProjectName="ContractManagement_Scrum_2_4"},
               new ProjectException{Id= 18,OrganizationId= 1,ProjectName="ProjectOrleansHackathon"},
               new ProjectException{Id= 19,OrganizationId= 1,ProjectName="CIODocker2OctopusPoC"},
               new ProjectException{Id= 20,OrganizationId= 2,ProjectName="TFS_Test"},
               new ProjectException{Id= 21,OrganizationId= 2,ProjectName="CodeAnalysis"},
               new ProjectException{Id= 22,OrganizationId= 2,ProjectName="HelloWorldReadyRoll"},
               new ProjectException{Id= 23,OrganizationId= 2,ProjectName="DoNetDockerDeploy"},
               new ProjectException{Id= 24,OrganizationId= 2,ProjectName="AgileSandbox"},
               new ProjectException{Id= 25,OrganizationId= 2,ProjectName="ABC_Project_Destination"},
               new ProjectException{Id= 26,OrganizationId= 3,ProjectName="ABC_Project"},
               new ProjectException{Id= 26,OrganizationId= 3,ProjectName="TakeAction_7291"},
               new ProjectException{Id= 27,OrganizationId= 3,ProjectName="TSCCENTRAL_38066"},
               new ProjectException{Id= 28,OrganizationId= 3,ProjectName="PosterDigital_7723"},
               new ProjectException{Id= 29,OrganizationId= 3,ProjectName="ANZOrion_7618"},
               new ProjectException{Id= 30,OrganizationId= 3,ProjectName="TFOAMTPortal_7310"},
               new ProjectException{Id= 31,OrganizationId= 3,ProjectName="CHNISAOrchestrator_7548"},
               new ProjectException{Id= 32,OrganizationId= 3,ProjectName="DOSS_35779"},
               new ProjectException{Id= 33,OrganizationId= 3,ProjectName="AIA004rLAIR_163446"},
               new ProjectException{Id= 34,OrganizationId= 3,ProjectName="FutureSysDiagnostic_59219"},
               new ProjectException{Id= 35,OrganizationId= 3,ProjectName="asgbahn_10416"},
               new ProjectException{Id= 36,OrganizationId= 3,ProjectName="MicroFocusSuite_7165"},
               new ProjectException{Id= 37,OrganizationId= 3,ProjectName="YeomanTest"},
               new ProjectException{Id= 38,OrganizationId= 4,ProjectName="AIA000uMyLearning_1720"},
               new ProjectException{Id= 39,OrganizationId= 4,ProjectName="ClientTeamPortal_4309_Stage"},
               new ProjectException{Id= 40,OrganizationId= 4,ProjectName="ICS_123322"},
               new ProjectException{Id= 41,OrganizationId= 4,ProjectName="ContractForecast_135522"},
               new ProjectException{Id= 42,OrganizationId= 4,ProjectName="AIA002zQSMAnalytics_3107"},
               new ProjectException{Id= 43,OrganizationId= 4,ProjectName="CodingDojoGit"},
               new ProjectException{Id= 44,OrganizationId= 4,ProjectName="AIA0034IntelFabric_108938"},
               new ProjectException{Id= 45,OrganizationId= 4,ProjectName="AIA002vTQRA_103998"},
               new ProjectException{Id= 46,OrganizationId= 4,ProjectName="AIA002sMCDataLake_103875"},
               new ProjectException{Id= 47,OrganizationId= 5,ProjectName="ITPortal_4369"},
               new ProjectException{Id= 48,OrganizationId= 5,ProjectName="IntegratedDeliveryPlatformAO_4699"},
               new ProjectException{Id= 49,OrganizationId= 5,ProjectName="MyHighPerfDP_4587"},
               new ProjectException{Id= 50,OrganizationId= 5,ProjectName="myWizardUploader_13808"},
               new ProjectException{Id= 51,OrganizationId= 5,ProjectName="myWizardENS_11533"},
               new ProjectException{Id= 52,OrganizationId= 5,ProjectName="AIA0058dibt_155994"},
               new ProjectException{Id= 53,OrganizationId= 5,ProjectName="myWizardAgile_4628"},
               new ProjectException{Id= 54,OrganizationId= 5,ProjectName="Newsletter_4709"},
               new ProjectException{Id= 55,OrganizationId= 5,ProjectName="SIDeliveryWizard_4577"},
               new ProjectException{Id= 56,OrganizationId= 5,ProjectName="OperDataStore_4557"},
               new ProjectException{Id= 57,OrganizationId= 5,ProjectName="CIODesignAg_31577"},
               new ProjectException{Id= 58,OrganizationId= 5,ProjectName="SolutionOptimizer_4822"},
               new ProjectException{Id= 59,OrganizationId= 5,ProjectName="ViGDN_4517"},
               new ProjectException{Id= 60,OrganizationId= 5,ProjectName="DeliveryPerf_ 4620"},
               new ProjectException{Id= 61,OrganizationId= 6,ProjectName="DIPFlex_3533"},
               new ProjectException{Id= 62,OrganizationId= 6,ProjectName="UIDesignStudio_5067"},
               new ProjectException{Id= 63,OrganizationId= 6,ProjectName="ADMEstimator_4087"},
               new ProjectException{Id= 64,OrganizationId= 6,ProjectName="Missouri_4986"},
               new ProjectException{Id= 65,OrganizationId= 6,ProjectName="SCA_SAST"},
               new ProjectException{Id= 66,OrganizationId= 6,ProjectName="TGPReporting_3612"},
               new ProjectException{Id= 67,OrganizationId= 6,ProjectName="DEI-IntegratedTools_4135"},
               new ProjectException{Id= 68,OrganizationId= 6,ProjectName="IDCQuality_3269"},
               new ProjectException{Id= 69,OrganizationId= 6,ProjectName="IACAssessmentPortal_86422"},
               new ProjectException{Id= 70,OrganizationId= 6,ProjectName="ALMA_4538"},
               new ProjectException{Id= 71,OrganizationId= 6,ProjectName="OperationsNavigator_3683"},
               new ProjectException{Id= 72,OrganizationId= 6,ProjectName="Harvest_4949"},
               new ProjectException{Id= 73,OrganizationId= 6,ProjectName="ProductivityHub_5024"},
               new ProjectException{Id= 74,OrganizationId= 6,ProjectName="SystemAccessMgr_4183"},
               new ProjectException{Id= 75,OrganizationId= 6,ProjectName="ContentApps"},
               new ProjectException{Id= 76,OrganizationId= 6,ProjectName="Test"},
               new ProjectException{Id= 77,OrganizationId= 6,ProjectName="AzureDevopsTest"},
               new ProjectException{Id= 78,OrganizationId= 6,ProjectName="OpsNavigator_3683"},
               new ProjectException{Id= 79,OrganizationId= 6,ProjectName="MultiSkill_5141"},
               new ProjectException{Id= 80,OrganizationId= 7,ProjectName="ICP_34883"},
               new ProjectException{Id= 81,OrganizationId= 7,ProjectName="GBI_1639"},
               new ProjectException{Id= 82,OrganizationId= 7,ProjectName="AppServForecasting_6396"},
               new ProjectException{Id= 83,OrganizationId= 7,ProjectName="Moxis_7103"},
               new ProjectException{Id= 84,OrganizationId= 7,ProjectName="ProdIllustrator_5186"},
               new ProjectException{Id= 85,OrganizationId= 7,ProjectName="AetnaHealth_5211"},
               new ProjectException{Id= 86,OrganizationId= 7,ProjectName="AppMetadataRegistry_39515"},
               new ProjectException{Id= 87,OrganizationId= 7,ProjectName="DPCognitive_6483"},
               new ProjectException{Id= 88,OrganizationId= 7,ProjectName="SplunkSIEM_27601"},
               new ProjectException{Id= 89,OrganizationId= 7,ProjectName="BusAdvisorExp_5021"},
               new ProjectException{Id= 90,OrganizationId= 7,ProjectName="myWizardRDDA_7354"},
               new ProjectException{Id= 91,OrganizationId= 7,ProjectName="ToolsAndAutomation"},
               new ProjectException{Id= 92,OrganizationId= 7,ProjectName="Boardwalk_4464"},
               new ProjectException{Id= 93,OrganizationId= 7,ProjectName="HighPerfSolPfm_5364"},
               new ProjectException{Id= 94,OrganizationId= 8,ProjectName="VirtualAgents_6837"},
               new ProjectException{Id= 95,OrganizationId= 8,ProjectName="Missouri_4986Git"},
               new ProjectException{Id= 96,OrganizationId= 8,ProjectName="EnvDelForAnP_7693"},
               new ProjectException{Id= 97,OrganizationId= 8,ProjectName="KnowledgeNavigator_4724"},
               new ProjectException{Id= 98,OrganizationId= 8,ProjectName="TFS_2641_VSTSTest"},
               new ProjectException{Id= 99,OrganizationId= 8,ProjectName="MSGlobalRoster_6831"},
               new ProjectException{Id= 100,OrganizationId= 8,ProjectName="Aurum_6716"},
               new ProjectException{Id= 101,OrganizationId= 8,ProjectName="ClientDataProtection_4009"},
               new ProjectException{Id= 102,OrganizationId= 8,ProjectName="MSPSDevelopment_5242"},
               new ProjectException{Id= 103,OrganizationId= 9,ProjectName="QSM_Reporting_3107"},
               new ProjectException{Id= 104,OrganizationId= 9,ProjectName="StoryAnalyzer_10957"},
               new ProjectException{Id= 105,OrganizationId= 9,ProjectName="myWizardDDR_6819"},
               new ProjectException{Id= 106,OrganizationId= 9,ProjectName="IberiaDashboard_11205"},
               new ProjectException{Id= 107,OrganizationId= 9,ProjectName="AutoPacksforSAP_10364"},
               new ProjectException{Id= 108,OrganizationId= 9,ProjectName="AICore_29256"},
               new ProjectException{Id= 109,OrganizationId= 9,ProjectName="SvcMgmtAPI_10345"},
               new ProjectException{Id= 110,OrganizationId= 9,ProjectName="myWizardSICustom_30852"},
               new ProjectException{Id= 111,OrganizationId= 9,ProjectName="DeliveryMethods_6951"},
               new ProjectException{Id= 112,OrganizationId= 9,ProjectName="InnateIntelligentMiner_8125"},
               new ProjectException{Id= 113,OrganizationId= 9,ProjectName="GetADT_4036"},
               new ProjectException{Id= 114,OrganizationId= 9,ProjectName="DigitalDeliveryPlatform_25854"},
               new ProjectException{Id= 115,OrganizationId= 9,ProjectName="GTRA_11822"},
               new ProjectException{Id= 116,OrganizationId= 9,ProjectName="Navigator_10230"},
               new ProjectException{Id= 117,OrganizationId= 9,ProjectName="SLAMeter_9992"},
               new ProjectException{Id= 118,OrganizationId= 10,ProjectName="TTR_Testing_4471"},
               new ProjectException{Id= 119,OrganizationId= 10,ProjectName="TestProject"},
               new ProjectException{Id= 120,OrganizationId= 10,ProjectName="TFSAdapterTest_4587"},
               new ProjectException{Id= 121,OrganizationId= 10,ProjectName="Cartridge_Management"},
               new ProjectException{Id= 122,OrganizationId= 10,ProjectName="Cartridge_Pilot"},
               new ProjectException{Id= 123,OrganizationId= 10,ProjectName="Cartridge_Generate_5555"},
               new ProjectException{Id= 124,OrganizationId= 10,ProjectName="myWizardTA_4587"},
               new ProjectException{Id= 125,OrganizationId= 10,ProjectName="ManageMyVendor_7989"},
               new ProjectException{Id= 126,OrganizationId= 10,ProjectName="TestAzDoSecurityModel_58902002"},
               new ProjectException{Id= 127,OrganizationId= 10,ProjectName="GovReImagined_11883"},
               new ProjectException{Id= 128,OrganizationId= 10,ProjectName="Sample - Software Engineering and Architecture"},
               new ProjectException{Id= 129,OrganizationId= 10,ProjectName="myWizardESB_5092"},
               new ProjectException{Id= 130,OrganizationId= 10,ProjectName="TemplateTest"},
               new ProjectException{Id= 131,OrganizationId= 10,ProjectName="DevSampleProject"},
               new ProjectException{Id= 132,OrganizationId= 10,ProjectName="AutomationProcess_29697"},
               new ProjectException{Id= 133,OrganizationId= 11,ProjectName="ITPortal_4369"},
               new ProjectException{Id= 134,OrganizationId= 11,ProjectName="DIIPMetricsReporting_4136"},
               new ProjectException{Id= 135,OrganizationId= 11,ProjectName="ClientServiceReporting_3902"},
               new ProjectException{Id= 136,OrganizationId= 11,ProjectName="Discoverer_4368"},
               new ProjectException{Id= 137,OrganizationId= 11,ProjectName="AOAnalyticsEngine_3901"},
               new ProjectException{Id= 138,OrganizationId= 11,ProjectName="FedRMS_Project"},
               new ProjectException{Id= 139,OrganizationId= 11,ProjectName="DeliveryMgmtSystem_2121"},
               new ProjectException{Id= 140,OrganizationId= 11,ProjectName="OPI_Project"},
               new ProjectException{Id= 141,OrganizationId= 11,ProjectName="TicketResolutionPlatform_4250"},
               new ProjectException{Id= 142,OrganizationId= 11,ProjectName="LeadershipDashboard_3918"},
               new ProjectException{Id= 143,OrganizationId= 11,ProjectName="IndustryPacks_4371"},
               new ProjectException{Id= 144,OrganizationId= 11,ProjectName="CaptureDART_3499"},
               new ProjectException{Id= 145,OrganizationId= 11,ProjectName="DPNextGenAnalytics_4268"},
               new ProjectException{Id= 146,OrganizationId= 11,ProjectName="Crowdsourcing_4406"},
               new ProjectException{Id= 147,OrganizationId= 11,ProjectName="TGP_RMQ_LsPortal_Project"},
               new ProjectException{Id= 148,OrganizationId= 11,ProjectName="Salesforce_2711"},
               new ProjectException{Id= 149,OrganizationId= 11,ProjectName="TGPIntegration_2121"},
               new ProjectException{Id= 150,OrganizationId= 11,ProjectName="acenturecio01"},
               new ProjectException{Id= 151,OrganizationId= 11,ProjectName="SolutionFactoryMgmtTool_3919"},
               new ProjectException{Id= 152,OrganizationId= 11,ProjectName="AssetsIncubator_3031"},
               new ProjectException{Id= 153,OrganizationId= 11,ProjectName="TestingAnalyticsEngine_3901"},
               new ProjectException{Id= 154,OrganizationId= 11,ProjectName="TGP_RMQ_Reports_3107"},
               new ProjectException{Id= 155,OrganizationId= 11,ProjectName="AO_Mobilization_3112"},
               new ProjectException{Id= 156,OrganizationId= 12,ProjectName="FedMP_Project"},
               new ProjectException{Id= 157,OrganizationId= 12,ProjectName="IQ_Project"},
               new ProjectException{Id= 158,OrganizationId= 12,ProjectName="mDART2.0_Project"},
               new ProjectException{Id= 159,OrganizationId= 12,ProjectName="Innovation_ADM_Project"},
               new ProjectException{Id= 160,OrganizationId= 12,ProjectName="PhilsLAD_EMTS_Project"},
               new ProjectException{Id= 161,OrganizationId= 12,ProjectName="CIO-EDM_SIS_Project"},
               new ProjectException{Id= 162,OrganizationId= 12,ProjectName="TESystems_ARTES_Project"},
               new ProjectException{Id= 163,OrganizationId= 12,ProjectName="QCR_Project"},
               new ProjectException{Id= 164,OrganizationId= 12,ProjectName="GlobalSuspense_Project"},
               new ProjectException{Id= 165,OrganizationId= 12,ProjectName="Anticipatory_Visa_System_Project"},
               new ProjectException{Id= 166,OrganizationId= 12,ProjectName="CIO_EDM_TQR_Project"},
               new ProjectException{Id= 167,OrganizationId= 12,ProjectName="ADAMS_Project"},
               new ProjectException{Id= 168,OrganizationId= 12,ProjectName="IO_ReportingDashboard_Project"},
               new ProjectException{Id= 169,OrganizationId= 12,ProjectName="SMS_POC_Project"},
               new ProjectException{Id= 170,OrganizationId= 12,ProjectName="CIO_EDC_Project"},
               new ProjectException{Id= 171,OrganizationId= 12,ProjectName="DCNMMEIntegration_DCNWebService_Project"},
               new ProjectException{Id= 172,OrganizationId= 12,ProjectName="BPO_QCSC2S_Reporting_Project"},
               new ProjectException{Id= 173,OrganizationId= 12,ProjectName="Innovaccion_Project"},
               new ProjectException{Id= 174,OrganizationId= 12,ProjectName="EnterpriseSOA_Project"},
               new ProjectException{Id= 175,OrganizationId= 12,ProjectName="MCIM_MEET_Project"},
               new ProjectException{Id= 176,OrganizationId= 12,ProjectName="myLearningX_Project"},
               new ProjectException{Id= 177,OrganizationId= 12,ProjectName="NDA_Project"},
               new ProjectException{Id= 178,OrganizationId= 12,ProjectName="Support_Project"},
               new ProjectException{Id= 179,OrganizationId= 12,ProjectName="DFG_SEFS_Project"},
               new ProjectException{Id= 180,OrganizationId= 12,ProjectName="Telepresence_Project"},
               new ProjectException{Id= 181,OrganizationId= 14,ProjectName="CIO-Fields-Test"},//WorkAlignment_Test
               new ProjectException{Id= 182,OrganizationId= 15,ProjectName="WorkAlignment_Test"},//WorkAlignment_Test

            };
            return exceptions;
        }
    }
}
