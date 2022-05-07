using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models.ImageData
{
    public class ImageType01_IDFront
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string FamilyName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string LicenseNumber { get; set; }

        public string Address { get; set; }

        public string StateOfIssue { get; set; }

        public bool IsFaceMatched { get; set; }
        public bool? IsFaceLocked { get; set; }
        public bool IsOwner { get; set; }

        public int RelationId { get; set; }

        public string CustomRelation { get; set; }

        public string FaceMatchPercentage { get; set; }

        public string IdScanGoErrorMessage { get; set; }
    }
}
