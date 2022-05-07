using System;

namespace Core.API.Models.Data
{
    public class SaveUsersRequest
    {
        public Int64 UserId { get; set; }

        public string UserGuid { get; set; }

        public string CompanyGuid { get; set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public Int64 UserTypeId { get; set; }

        public bool IsActive { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsDeleted { get; set; }
    }
}
