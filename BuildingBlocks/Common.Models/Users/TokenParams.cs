namespace Common.Models.Users
{
    public class TokenParams
    {
        //public string TokenType { get; set; }

        public string Token { get; set; }

        public long UserId { get; set; }

        public string CompanyGuid { get; set; }

        public long UserTypeId { get; set; }

        public string UserGuid { get; set; }

        public int[] UserRoles { get; set; }
    }
}
