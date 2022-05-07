namespace Core.API.Services
{
    public interface ILenderService
    {
        public Models.LenderInspectionsFilterResponse GetInspectionsFilter();

        public Models.LenderInspectionsResponse GetInspectionsList(Models.LenderInspectionsRequest model, string userGuid, string companyGuid);

        public Models.LenderCompletedInspectionsResponse GetCompletedInspectionsList(Models.LenderInspectionsRequest model, 
                                                                                     string userGuid, string companyGuid);
    }
}
