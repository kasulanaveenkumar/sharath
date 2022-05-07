namespace Config.API.Models.Broker
{
    public class BrokerUserResponse
    {
        public long UserId { get; set; }

        public string UserGuid { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsBillingResponsible { get; set; }

        public bool IsPrimaryContact { get; set; }

        public bool IsEmailVerified { get; set; }

        public int EmailStatus { get; set; }
    }
}
