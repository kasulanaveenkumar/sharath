using Common.Messages;
using Core.API.Helper;
using Core.API.Models.B2B;
using Core.API.Services;
using Core.API.Services.B2B;
using Core.API.Swagger.B2B;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Core.API.Controllers.B2B
{
    [TypeFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("apiB2B/v1/applications")]
    public class B2BInspectionController : ControllerBase
    {
        private readonly IB2BService b2bService;
        private readonly IErrorService errorService;

        #region Constructor

        public B2BInspectionController(IB2BService b2bService, IErrorService errorService)
        {
            this.b2bService = b2bService;
            this.errorService = errorService;
        }

        #endregion

        #region Get All Inspections
        /// <summary>
        /// Get all applications created for the company 
        /// </summary>
        /// <returns></returns>
        /// <param name="email">Buyer / Seller Email</param>
        /// <param name="externalRef">External Reference</param>
        /// <response code="200">Returns list of inspection details</response>
        /// <response code="400">Returns error message</response>
        /// <response code="500">Returns the internal server error with error id</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllInspections([FromQuery(Name = "email")] string email = "",
                                                                [FromQuery(Name = "externalRef")] string externalRef = "")
        {
            try
            {
                // Get CompanyGuid from Token
                var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

                // Get Inspections List
                var model = new Models.B2B.AllInspectionRequest();
                model.CompanyGuid = companyGuid;
                if (!string.IsNullOrEmpty(email))
                {
                    model.Email = email.Replace("\"", "");
                }
                if (!string.IsNullOrEmpty(externalRef))
                {
                    model.ExternalRef = externalRef.Replace("\"", "");
                }
                var responses = b2bService.GetInspectionsList(model);

                if (responses.Count() == 0)
                {
                    return BadRequest("Inspections not found");
                }

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Get Inspection Details
        /// <summary>
        /// Get an application details using Buyer / Seller Email or External Reference
        /// </summary>
        /// <param name="includeAllDocuments">To include all documents</param>
        /// <param name="includeRejectedDocuments">To get only rejected documents</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInspectionDetails([FromQuery(Name = "includeAllDocuments")] bool includeAllDocuments = false,
                                                                [FromQuery(Name = "includeRejectedDocuments")] bool includeRejectedDocuments = false)
        {
            try
            {
                var response = new List<Models.B2B.InspectionDetailResponse>();
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Create Inspections
        /// <summary>
        /// Create new inspection
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateNewInspection(CreateInspectionRequest model)
        {
            try
            {
                // Get CompanyGuid from Token
                var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

                // Create New Inspection
                var paymentFailedReason = string.Empty;
                Exception errorMessage = null;
                var response = b2bService.CreateNewInspection(model, companyGuid, out paymentFailedReason, out errorMessage);

                // Invalid Template
                if (response == -1)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.TemplatePlans_Error_TemplateNotExists);
                    return BadRequest(ModelState);
                }

                // Invalid Lender Configuration
                else if (response == -2)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Lender_Invalid);
                    return BadRequest(ModelState);
                }

                // Invalid Lender Company
                else if (response == -3)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.LenderCompany_Invalid);
                    return BadRequest(ModelState);
                }

                // Invalid Broker Company
                else if (response == -4)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.BrokerCompany_Invalid);
                    return BadRequest(ModelState);
                }

                // Payment Method not exists
                else if (response == -5)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.PaymentMethods_Error_Required);
                    return BadRequest(ModelState);
                }

                // If Error Message is returned
                else if (errorMessage != null)
                {
                    var errorId = errorService.SaveError(errorMessage, model, null, null);
                    return StatusCode((int)HttpStatusCode.InternalServerError, string.Format(CommonMessages.Error_Message, errorId));
                }

                // If payment failed due to some reasons
                else if (!string.IsNullOrEmpty(paymentFailedReason))
                {
                    return BadRequest(new { Message = paymentFailedReason });
                }

                return Ok(new { Message = CoreMessages.Application_Created, InspectionId = response });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Update Inspection Details

        /// <summary>
        /// Edit existing inspection
        /// </summary>
        /// <remarks>
        /// If inspection status is created then Buyer Details, Seller Details, Lender Reference or External Reference can be edited.
        /// 
        /// If inspection status is started but not processed then only Lender Reference or External Reference can be edited.
        /// 
        /// If inspection status is submitted / cancelled then inspection details cannot be edited.
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.PreconditionFailed)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateInspectionDetails(UpdateInspectionRequest model)
        {
            try
            {
                long userId = 160;

                // Get CompanyGuid from Token
                var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

                // Get Inspection Details
                var inspectionDetails = b2bService.GetInspectionDetails(model.Id, companyGuid);
                if (inspectionDetails != null)
                {
                    var errorMessage = String.Empty;

                    // If inputs are empty
                    if (model.Buyer == null &&
                        model.Seller == null &&
                        string.IsNullOrEmpty(model.RefNumber) &&
                        string.IsNullOrEmpty(model.ExternalRefNumber))
                    {
                        errorMessage = "Kindly enter inputs for Update Process";
                        return StatusCode((int)HttpStatusCode.PreconditionFailed, new { Message = errorMessage });
                    }

                    // If ApplicationStatus is Completed return Error Message
                    if (inspectionDetails.ApplicationStatus == (int)Models.Enums.ApplicationStatus.Completed)
                    {
                        errorMessage = "ApplicationStatus is already in completed.You cannot make further changes";
                        return StatusCode((int)HttpStatusCode.PreconditionFailed, new { Message = errorMessage });
                    }

                    // If Application Status is not Created return Error Message
                    // Else perform Update Inspection and AppUsers
                    if (inspectionDetails.ApplicationStatus != (int)Models.Enums.ApplicationStatus.Created)
                    {
                        // If Buyer Details provided or Seller Derails provided return Error Message
                        // Else perform Update Inspection
                        if (model.Buyer != null ||
                            model.Seller != null)
                        {
                            errorMessage = "Application can be updated only if the status is Created. " +
                                           "Current ApplicationStatus is " + ((Models.Enums.ApplicationStatus)inspectionDetails.ApplicationStatus);
                            return StatusCode((int)HttpStatusCode.PreconditionFailed, new { Message = errorMessage });
                        }
                        else
                        {
                            // Update Inspection
                            b2bService.UpdateInspection(inspectionDetails, model, userId);

                            return Ok(new { Message = CommonMessages.Success_Message });
                        }
                    }
                    else
                    {
                        // Update Inspection
                        b2bService.UpdateInspection(inspectionDetails, model, userId);

                        // Update AppUsers
                        var appUsers = b2bService.GetAppUsers(model.Id);
                        b2bService.UpdateAppUsers(appUsers, model);

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
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Cancel Inspection
        /// <summary>
        /// Cancel inspection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.PreconditionFailed)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CancelInspection(Models.B2B.CancelInspectionRequest model)
        {
            try
            {
                // Get CompanyGuid from Token
                var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

                // Get Inspection Details
                var inspectionDetails = b2bService.GetInspectionDetails(model.Id, companyGuid);
                if (inspectionDetails != null)
                {
                    // If Application Status is not Created return Error Message
                    // Else perform Cancel Inspection
                    if (inspectionDetails.ApplicationStatus != (int)Models.Enums.ApplicationStatus.Created)
                    {
                        string errorMessage = "Application can be cancelled only if the status is Created. " +
                                              "Current ApplicationStatus is " + ((Models.Enums.ApplicationStatus)inspectionDetails.ApplicationStatus);

                        return StatusCode((int)HttpStatusCode.PreconditionFailed, new { Message = errorMessage });
                    }
                    else
                    {
                        b2bService.CancelInspection(model, inspectionDetails);

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
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Download Report By Id
        /// <summary>
        /// Download report in pdf format using inspection id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns the reqested report in pdf</response>
        /// <response code="204">Returns if not content is available</response>
        /// <response code="500">Returns the internal server error with error id</response>
        [HttpGet]
        [Route("{id}/pdf")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DownloadReportById(long id)
        {
            // Get CompanyGuid from Token
            var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

            try
            {
                // Get Inspection Details by InspectionId and CompanyGuid
                var inspectionDetails = b2bService.GetInspectionDetails(id, companyGuid);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Get Report Data
                var reportData = b2bService.GetReport(inspectionDetails);
                if (string.IsNullOrEmpty(reportData))
                {
                    return StatusCode((int)HttpStatusCode.NoContent, new { Message = CommonMessages.Report_NotAvailable });
                }

                // Return Pdf File
                var fileContents = Convert.FromBase64String(reportData);
                var fileName = string.Format("Verimoto_InspectionReport_{0}.pdf", inspectionDetails.Id);
                return File(fileContents, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, id, null, companyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Download Report By External Ref
        /// <summary>
        /// Download report in pdf format using inspection external reference
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the reqested report in pdf</response>
        /// <response code="204">Returns if not content is available</response>
        /// <response code="500">Returns the internal server error with error id</response>
        [HttpGet]
        [Route("ext/{externalRef}/pdf")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DownloadReportByExtRef(string externalRef)
        {
            // Get CompanyGuid from Token
            var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

            try
            {
                // Get Inspection Details by InspectionId and CompanyGuid
                var inspectionDetails = b2bService.GetInspectionDetails(0, companyGuid, externalRef);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Get Report Data
                var reportData = b2bService.GetReport(inspectionDetails);
                if (string.IsNullOrEmpty(reportData))
                {
                    return StatusCode((int)HttpStatusCode.NoContent, new { Message = CommonMessages.Report_NotAvailable });
                }

                // Return Pdf File
                var fileContents = Convert.FromBase64String(reportData);
                var fileName = string.Format("Verimoto_InspectionReport_{0}.pdf", inspectionDetails.Id);
                return File(fileContents, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, externalRef, null, companyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion
    }
}
