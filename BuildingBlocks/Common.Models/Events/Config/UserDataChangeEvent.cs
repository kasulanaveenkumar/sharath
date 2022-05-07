using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Events.Config
{
    public class UserDataChangeEvent : BaseEvent
    {
        public UserDataChangeEvent()
        {
            Event = Events.User_Data_Changed;
        }

        public Int64 UserId { get; set; }

        public string UserGuid { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string CompanyGuid { get; set; }

        public string Mobile { get; set; }

        public long UserTypeId { get; set; }

        public bool IsActive { get; set; }

        public DateTime UpdatedTime { get; set; }

        public bool IsDeleted { get; set; }

        public Int64 UpdatedBy { get; set; }

        public bool IsAdmin { get; set; }

        public Int32 UsersCount { get; set; }
    }
}
