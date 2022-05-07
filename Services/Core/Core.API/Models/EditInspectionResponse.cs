using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class EditInspectionResponse
    {
        public EditInspectionResponse()
        {
            Lenders = new List<Models.Lenders>();

            Assets = new List<Models.TemplateSets>();

            States = new List<Models.States>();

            InspectionProgresses = new List<Models.InspectionProgress>();

            ActivityLogs = new List<Models.ActivityLogs>();

            Reminders = new List<Models.Reminders>();

            ReminderConfigs = new List<Models.CoreConfigsResponse>();

            BrokerUsers = new List<Models.BrokerUsers>();

            Documents = new List<AppDocuments>();

            InspectionPlans = new List<InspectionPlansResponse>();
        }

        public List<Models.Lenders> Lenders { get; set; }

        public List<Models.TemplateSets> Assets { get; set; }

        public List<Models.States> States { get; set; }

        public Models.InspectionDetails InspectionDetails { get; set; }

        public bool ExemptPayment { get; set; }

        public List<Models.InspectionProgress> InspectionProgresses { get; set; }

        public List<Models.ActivityLogs> ActivityLogs { get; set; }

        public List<Models.Reminders> Reminders { get; set; }

        public List<Models.CoreConfigsResponse> ReminderConfigs { get; set; }

        public List<Models.BrokerUsers> BrokerUsers { get; set; }

        public List<Models.InspectionPlansResponse> InspectionPlans { get; set; }

        public bool IsPlanSelected { get; set; }

        public List<AppDocuments> Documents { get; set; }

        public bool IsEditAllowed { get; set; }

        public bool isNotificationAllowed { get; set; }
    }

    public class InspectionDetails
    {
        public Int64 InspectionId { get; set; }

        public AppUser Buyer { get; set; }

        public AppUser Seller { get; set; }

        public string LenderGuid { get; set; }

        public string LenderName { get; set; }

        public string LenderReference { get; set; }

        public string AssetType { get; set; }

        public Int64 StateId { get; set; }

        public string StateCode { get; set; }

        [JsonIgnore]
        public Int64 CreatedBy { get; set; }

        public string CreatedUser { get; set; }

        public DateTime CreatedTime { get; set; }

        public string CompanyName { get; set; }

        [JsonIgnore]
        public string BrokerCompanyGuid { get; set; }

        public string TemplateSetGuid { get; set; }

        public string TemplateSetPlanGuid { get; set; }

        public DateTime? LastActivity { get; set; }

        public string BrokerEmail { get; set; }

        public Int32 ApplicationStatus { get; set; }

        public string LenderPrefix { get; set; }

        public bool IsAllowAwaitedRef { get; set; }

        public bool IsForceLenderRefFormat { get; set; }

        public string WebAppShortLink { get; set; }

        public DateTime? LastNotifiedTime { get; set; }

        public int? DVSStatus { get; set; }

        public string DeviceDetails { get; set; }
    }

    public class InspectionProgress
    {
        [JsonIgnore]
        public Int64 StatusId { get; set; }

        public string Status { get; set; }

        public bool IsProgressed { get; set; }

        public bool IsRejected { get; set; }

        public DateTime? ProcessedTime { get; set; }
    }

    public class AppDocuments
    {
        public long DocId { get; set; }

        public string DocumentName { get; set; }

        public List<AppImages> ImageDetails { get; set; }

        public Models.Enums.DocImageStatus DocStatus { get; set; }

        public bool IsShowDocument { get; set; }
    }

    public class AppImages
    {
        public long ImageId { get; set; }

        public string ImageName { get; set; }

        public string ImageDescription { get; set; }

        public Int16 DocGroup { get; set; }

        public Int16 ImageType { get; set; }

        public Int16 InternalStatus { get; set; }

        public Int16 ImageStatus { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsRejected { get; set; }

        // Will be used in Edit Inspection Screen
        public bool IsImageRejected { get; set; }

        public DateTime? ProcessedTime { get; set; }

        public List<AppImageSelectedReasons> RejectReasons { get; set; }

        public List<AppImageSelectedReasons> FlagReasons { get; set; }

        public bool IsBypassRequested { get; set; }

        public string BypassReason { get; set; }

        public string ImageData { get; set; }

        public bool IsSkipped { get; set; }

        [JsonIgnore]
        public string Extension { get; set; }
    }

    public class AppImageSelectedReasons
    {
        public string Description { get; set; }

        public Int64 ReasonId { get; set; }

        public bool IsSelected { get; set; }
    }

    public class ActivityLogs
    {
        public string Role { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Action { get; set; }

        public DateTime ProcessedTime { get; set; }

        [JsonIgnore]
        public string UserGuid { get; set; }

        [JsonIgnore]
        public Int32 UserType { get; set; }

        [JsonIgnore]
        public bool IsWebAppUser { get; set; }
    }

    public class DownloadActivityLogs
    {
        public string Role { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Action { get; set; }

        public string ProcessedTime { get; set; }
    }

    public class Reminders
    {
        public Int64 Id { get; set; }

        public string RemainderGuid { get; set; }

        public string RemainderTemplate { get; set; }

        public bool IsActive { get; set; }
    }

    public class BrokerUsers
    {
        public string UserGuid { get; set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string CompanyGuid { get; set; }

        public bool IsSelected { get; set; }

        public long UserId { get; set; }
    }
}
