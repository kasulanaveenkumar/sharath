using Config.API.Entities;
using Config.API.Models.B2B;

namespace Config.API.Services.B2B
{
    public interface IB2BService
    {
        public Companies GetCompanyDetailsByGuid(string companyGuid);

        public NewInspectionResponse GetNewInspectionDetails(Companies companyDetails, NewInspectionRequest model);
    }
}
