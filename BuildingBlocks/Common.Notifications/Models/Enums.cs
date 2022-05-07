namespace Common.Notifications.Models
{
    public enum Events
    {
        User_Registration,
        User_Login_OTP,
        User_Reset_Password,
        User_Broker_Invite,
        User_ExistingUser_Invite,
        User_Broker_JoinExistingCompany,
        User_ImportBroker_Invite,

        Inspection_Created,
        EditInspection_SendReminder,
        Inspection_Rejected,
        Inspection_Submission,
        Inspection_Shared,

        WebApp_Login_OTP,
        WebApp_SendOpenInspection_Ids,

        Company_Data_Changed,
        User_Data_Changed,

        Forward_SMS_To_Email,

        Inspection_SellerStarted,
        Inspection_SellerSubmitted,
        Inspection_AdminRejected,
        Inspection_AdminProcessed
    }
}
