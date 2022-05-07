namespace Common.Models.Users
{
    public class AuthenticateResponse
    {
        public string Guid { get; set; }

        public string UserType { get; set; }

        public string Name { get; set; }
        
        public string SurName { get; set; }
        
        public string Email { get; set; }
        
        public string Token { get; set; }

        public string CompanyGuid { get; set; }

        public string CompanyName { get; set; }

        public string CompanyLogo { get; set; }

        public string RefreshToken { get; set; }

        public bool IsAdmin { get; set; }
        
        public bool IsBillingResponsible { get; set; }

        public bool IsPrimaryContact { get; set; }

        public bool IsReviewer { get; set; }

        public bool IsSupport { get; set; }

        public bool IsNew { get; set; }

        public bool IsAssociatedToMultipleCompanies { get; set; }

        public string[] MenuPermissions { get; set; }

        public string RedirectUrl { get; set; }
    }
}
