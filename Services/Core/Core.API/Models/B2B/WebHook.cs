namespace Core.API.Models.B2B
{
    public class WebHookSubscribed
    {
        public long Id { get; set; }

        public string TargetUrl { get; set; }

        public string Event { get; set; }
    }

    public class WebHookSubscribe
    {
        /// <summary>
        /// URL to be invoked for the registered events
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// Allowed Events - Created | Started | Processed | Completed | Cancelled
        /// </summary>
        public string Event { get; set; }
    }

    public class WebHookUnsubscribe
    {
        public long Id { get; set; }
    }


}
