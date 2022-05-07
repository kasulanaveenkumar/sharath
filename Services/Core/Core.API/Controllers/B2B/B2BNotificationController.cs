using Common.Messages;
using Core.API.Helper;
using Core.API.Services;
using Core.API.Services.B2B;
using Core.API.Swagger.B2B;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Core.API.Controllers.B2B
{
    [TypeFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("apiB2B/v1/notification")]
    public class B2BNotificationController : ControllerBase
    {
        private readonly IB2BService b2bService;
        private readonly IDataService dataService;
        private readonly IErrorService errorService;

        #region Constructor

        public B2BNotificationController(IB2BService b2bService, IDataService dataService, IErrorService errorService)
        {
            this.b2bService = b2bService;
            this.dataService = dataService;
            this.errorService = errorService;
        }

        #endregion

        #region Resend inspection creation email
        /// <summary>
        /// Resend inspection creation email
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resendinspectioninvite/email/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Models.B2B.ErrorMessage), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Models.B2B.ErrorMessage), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ResendWelcomeEmail(int id)
        {
            try
            {
                // Get CompanyGuid from Token
                var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

                // Get Inspection Details
                var inspectionDetails = b2bService.GetInspectionDetails(id, companyGuid);
                if (inspectionDetails != null)
                {
                    var errorMessage = String.Empty;

                    if (inspectionDetails.ApplicationStatus != (int)Models.Enums.ApplicationStatus.Created)
                    {
                        errorMessage = "Inspection Creation Mail can be sent only if the status is Created. " +
                                          "Current ApplicationStatus is " + ((Models.Enums.ApplicationStatus)inspectionDetails.ApplicationStatus);
                        return StatusCode((int)HttpStatusCode.PreconditionFailed, new { Message = errorMessage });
                    }
                    else
                    {
                        // Send Inspection Created Email
                        b2bService.SendInspectionCreatedEmail(id);

                        return Ok(new { Message = CommonMessages.Success_Message });
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, new { Message = "Access denied. Please check the ApplicationId" });
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, id, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Resend inspection creation sms
        /// <summary>
        /// Resend inspection creation sms
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resendinspectioninvite/sms/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Models.B2B.ErrorMessage), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Models.B2B.ErrorMessage), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ResendWelcomeSms(long id)
        {
            try
            {
                // Get CompanyGuid from Token
                var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

                // Get Inspection Details
                var inspectionDetails = b2bService.GetInspectionDetails(id, companyGuid);
                if (inspectionDetails != null)
                {
                    var errorMessage = String.Empty;

                    if (inspectionDetails.ApplicationStatus != (int)Models.Enums.ApplicationStatus.Created)
                    {
                        errorMessage = "Inspection Creation Sms can be sent only if the status is Created. " +
                                          "Current ApplicationStatus is " + ((Models.Enums.ApplicationStatus)inspectionDetails.ApplicationStatus);
                        return StatusCode((int)HttpStatusCode.PreconditionFailed, new { Message = errorMessage });
                    }
                    else
                    {
                        // Send Inspection Created Sms
                        b2bService.SendInspectionCreatedSms(id);

                        return Ok(new { Message = CommonMessages.Success_Message });
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, new { Message = "Access denied. Please check the ApplicationId" });
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, id, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Send Reminder Email
        /// <summary>
        /// Send reminder to seller
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sendinspectionreminder/email/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Models.B2B.ErrorMessage), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Models.B2B.ErrorMessage), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendReminder(long id)
        {
            return Ok();
        }
        #endregion
    }
}
