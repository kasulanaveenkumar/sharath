using Common.Extensions;
using Core.API.Models;
using Core.API.Repositories;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Core.API.Services
{
    public class LenderService : ILenderService
    {
        private readonly ILenderRepository lenderRepository;

        #region Constructor

        public LenderService(ILenderRepository repository)
        {
            this.lenderRepository = repository;
        }

        #endregion

        #region Get Inspections Filter

        public LenderInspectionsFilterResponse GetInspectionsFilter()
        {
            var response = new LenderInspectionsFilterResponse();

            // Get All Assets
            response.Assets = lenderRepository.GetAllAssets();

            // Get All Brokers
            response.Brokers = lenderRepository.GetAllBrokers();

            // Get Application Statuses
            response.ApplicationStatuses = lenderRepository.GetApplicationStatuses();

            return response;
        }

        #endregion

        #region Get Inspections List

        public LenderInspectionsResponse GetInspectionsList(LenderInspectionsRequest model, string userGuid, string companyGuid)
        {
            var response = new Models.LenderInspectionsResponse();

            // Get All Inspections List
            var inspectionsList = lenderRepository.GetInspectionsList(model, userGuid, companyGuid);
            inspectionsList.ForEach(i =>
            {
                i.Status = ((Enums.ApplicationStatus)(int)i.ApplicationStatus).GetEnumDescriptionAttributeValue();
            });
            response.InspectionsList = inspectionsList
                                       .Skip(model.SkipData)
                                       .Take(model.LimitData)
                                       .ToList();

            var recordsCount = inspectionsList.Count();
            response.TotalRecords = recordsCount > 0
                                  ? recordsCount
                                  : 0;

            return response;
        }

        #endregion

        #region Get Completed Inspections List

        public LenderCompletedInspectionsResponse GetCompletedInspectionsList(LenderInspectionsRequest model, string userGuid, string companyGuid)
        {
            var response = new LenderCompletedInspectionsResponse();

            // Get Compeleted Inspections List
            var inspectionsList = lenderRepository.GetCompletedInspectionsList(model, userGuid, companyGuid);
            inspectionsList.ForEach(i =>
            {
                i.Status = ((Enums.ApplicationStatus)(int)i.ApplicationStatus).GetEnumDescriptionAttributeValue();
            });
            response.InspectionsList = inspectionsList
                                       .Skip(model.SkipData)
                                       .Take(model.LimitData)
                                       .ToList();

            var recordsCount = inspectionsList.Count();
            response.TotalRecords = recordsCount > 0
                                  ? recordsCount
                                  : 0;

            return response;
        }

        #endregion
    }
}
