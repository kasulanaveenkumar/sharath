using System.Collections.Generic;

namespace Core.API.Models.B2B
{
    public class InspectionDetailResponse : InspectionResponse
    {
        public List<Documents> Documents { get; set; }

        public List<Users> SharedUsers { get; set; }
    }

    public class Documents
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Status DocumentStatusId { get; set; }

        public string DocumentStatus { get; set; }

        public List<Images> Images { get; set; }
    }

    public class Images
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageDataBase64 { get; set; }

        public Status ImageStatusId { get; set; }

        public string ImageStatus { get; set; }

        public List<string> RejectReasons { get; set; }

        public bool IsBypassRequested { get; set; }

        public string BypassReason { get; set; }
    }

    public enum Status
    {
        Pending = 1,
        Uploaded = 2,
        Rejected = 3,
        Completed = 4,
        Deleted = 5,
        PreUploaded = 6
    }
}