using Common.Messages;
using Config.API.Helper;
using Config.API.Models.B2B;
using Config.API.Services;
using Config.API.Services.B2B;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Config.API.Controllers.B2B
{
    [ApiController]
    [Route("apiB2B/v1")]
    public class B2BController : ControllerBase
    {
        private readonly IB2BService b2bService;
        private readonly IErrorService errorService;

        #region Constructor

        public B2BController(IB2BService service, IErrorService errorService)
        {
            this.b2bService = service;
            this.errorService = errorService;
        }

        #endregion

        #region Get New Inspection Details

        [HttpPost]
        [Route("getnewinspectiondetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNewInspectionDetails(NewInspectionRequest model)
        {
            try
            {
                // Get Company Details By Guid
                var companyDetails = b2bService.GetCompanyDetailsByGuid(model.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get New Inspection Details
                var responses = b2bService.GetNewInspectionDetails(companyDetails, model);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion
    }
}
