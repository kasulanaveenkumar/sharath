using Common.Models.Core.Entities;
using Core.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class CancelInspectionResponse
    {
        public AppStakeholders InspectionDetails { get; set; }
    }
}
