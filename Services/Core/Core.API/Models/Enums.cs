using System.ComponentModel;

namespace Core.API.Models
{
    public class Enums
    {
        public enum ApplicationStatus
        {
            [Description("Created")]
            Created = 1,
            [Description("Started")]
            Started = 2,
            [Description("Submitted")]
            Submitted = 3,
            [Description("Rejected")]
            Rejected = 4,
            [Description("Completed")]
            Completed = 5,
            [Description("Delayed")]
            Delayed = 6,
            [Description("Suspended")]
            Suspended = 7,
            [Description("Cancelled")]
            Cancelled = 8,
            [Description("Bypass Requested")]
            ByPassRequested = 9
        }
        public enum DocStatus
        {
            Pending = 1,
            Uploaded = 2,
            Rejected = 3,
            Completed = 4,
            Deleted = 5,
            PreUploaded = 6
        }
        public enum DocImageStatus
        {
            Pending = 1,
            Uploaded = 2,
            Rejected = 3,
            Completed = 4,
            Deleted = 5,
            PreUploaded = 6
        }
        public enum DocInternalStatus
        {
            Pending = 0,
            Uploaded = 1,
            Accepted = 2,
            Rejected = 3,
            Flagged = 4
        }
        public enum Role
        {
            Buyer = 1,
            Seller = 2
        }

        public enum NotificationEvents
        {
            Created = 1,
            Submitted = 2,
            Rejected = 3,
            Processed = 4,
            Payment = 5
        }

        public enum AppActivityLogs
        {
            InspectionCreated = 1,
            InspectionStarted = 2,
            InspectionSubmitted = 3,
            InspectionRejected = 4,
            InspectionCompleted = 5,
            PaymentProcessed = 6,
            PaymentRejected = 7,
            PlanUpgraded = 8,
            BypassRequested = 9,
            InspectionShared = 10
        }

        public enum ReasonTypes
        {
            RejectReason = 1,
            FlagReason = 2
        }

        public enum ImageTypes
        {
            [Description("ID Front")]
            LicenseFront = 1,
            [Description("ID Back")]
            LicenseBack = 2,
            [Description("Vin")]
            CarVin = 3,
            [Description("Car Plate")]
            CarPlate = 4,
            [Description("Front View")]
            CarFront = 5,
            [Description("Back View")]
            CarBack = 6,
            [Description("Driver Side")]
            DriverSide = 7,
            [Description("Passenger Side")]
            PassengerSide = 8,
            [Description("Odometer/HourMeter")]
            Odometer = 9,
            [Description("Rego Paper Front")]
            RegoPaperFront = 10,
            [Description("Bank Statements")]
            BankStatements = 11,
            [Description("PPSR Documents")]
            PPSRCheck = 12,
            [Description("Face Recognition")]
            FaceRecognition = 13,
            [Description("Rego Paper Back")]
            RegoPaperBack = 14,
            [Description("Roadworthy Certificate")]
            RoadworthyCertificate = 15,
            [Description("OtherVehicleImages")]
            OtherVehicleImages = 16,
            [Description("Payout Letter")]
            PayoutLetter = 17,
            [Description("Invoice")]
            Invoice = 18,
            [Description("HIN")]
            HIN = 19,
            [Description("Registration Number")]
            PPSRRegistrationNumber = 20,
            [Description("Boat Plate")]
            BoatPlate = 21,
            [Description("Boat Front View")]
            BoatFront = 22,
            [Description("Boat Back View")]
            BoatBack = 23,
            [Description("Boat Right Side")]
            BoatRightSide = 24,
            [Description("Boat Left Side")]
            BoatLeftSide = 25,
            [Description("Motor Photo")]
            MotorPhoto = 26,
            [Description("Motor SerialNumber")]
            MotorSerialNumber = 27,
            [Description("Trailer Vin")]
            TrailerVin = 28,
            [Description("Trailer Plate")]
            TrailerPlate = 29,
            [Description("HourMeter")]
            HourMeter = 30,
            [Description("PPSR Boat")]
            PPSRBoat = 31,
            [Description("PPSR Motor")]
            PPSRMotor = 32,
            [Description("PPSR Trailer")]
            PPSRTrailer = 33,
            [Description("Proof of Ownership")]
            ProofofOwnership = 34,
            [Description("Private Sale Invoice")]
            PrivateSaleInvoice = 35,
            [Description("Renewal Notice 1")]
            RenewalNotice = 36,
            [Description("Renewal Notice 2")]
            RenewalNotice_1 = 37,
            [Description("Payout Letter 1")]
            PayoutLetter_BankStatement_1 = 38,
            [Description("Payout Letter 2")]
            PayoutLetter_BankStatement_2 = 39,
            [Description("Gas Certificate")]
            Gas_Certificate = 40
        }

        public enum InspectionTypes
        {
            New = 1,
            Upgrade = 2
        }

        public enum PaymentStatus
        {
            Success = 1,
            Failed = -1,
            Pending = 0
        }

        // This group is only for processlist service
        public enum DocGroup
        {
            PhotoIdentification = 1,
            VehicleDetails = 2,
            VehicleRegoPapers = 3,
            BankStatements = 4,
            PPSRcheck = 5,
            AdditionalDocument = 6,
            RoadworthyCertificate = 7,
            BoatDetails = 8,
            TrailerDetails = 9,
            MotorPhotos = 10,
            TrailerRego = 11,
            BoatRego = 12,
            ProofofOwnership = 13,
            PrivateSaleInvoice = 14,
            TrailerRWC = 15
        }

        public enum PingPPSRStatus
        {
            [Description("Ping Failed")]
            PingFailed = 0,
            [Description("Ping Passed PDF Report not found - Verification Status")]
            PingPassedPDFReportNotFound = 1,
            [Description("Ping Passed Issue in Report Generation")]
            PingPassedIssueInReportGeneration = 2,
            [Description("Ping Passed and PDF Passed")]
            PingPassedAndPDFPassed = 3
        }
    }
}
