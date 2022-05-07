using Common.Authorization.Authorize;
using Core.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Common.EventMessages;
using Common.Messages;
using Common.Models.Users;
using System.IO;
using Common.Identity.Models.DVS;
using static Common.Models.Enums;
using Core.API.Helper;
using Core.API.Models;
using Common.Validations.Helper;

namespace Core.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InspectionController : ControllerBase
    {
        private readonly IInspectionService inspectionService;
        private readonly IDataService dataService;
        private readonly IErrorService errorService;

        #region Constructor

        public InspectionController(IInspectionService inspectionService,
                                    IDataService dataService,
                                    IErrorService errorService,
                                    IMessageSender messageSender)
        {
            this.inspectionService = inspectionService;
            this.dataService = dataService;
            this.errorService = errorService;
        }

        #endregion

        #region New Inspection Details

        /// <summary>
        /// This API accessed only by Broker, Lender and their supported roles
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker,
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpGet]
        [Route("newinspectiondetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> NewInspectionDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get New Inspection Details
                var response = inspectionService.NewInspectionDetails(currentUserDetails.Token, currentUserDetails.UserTypeId,
                                                                      currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                return Ok(response);
            }

            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Create New Inspection

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
           new int[] {
                (int)UserTypes.Broker
           },
           new int[] {
                (int)UserRoles.AllUserRoles
           }
        )]
        [HttpPost]
        [Route("new")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateInspection(NewInspectionRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Create New Inspection
                var paymentFailedReason = string.Empty;
                Exception errorMessage = null;
                var response = inspectionService.CreateNewInspection(model, currentUserDetails.UserId, currentUserDetails.Token,
                                                                     currentUserDetails.CompanyGuid, currentUserDetails.UserGuid,
                                                                     currentUserDetails.UserTypeId, out paymentFailedReason,
                                                                     out errorMessage);

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
                    var errorId = errorService.SaveError(errorMessage, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                    return StatusCode((int)HttpStatusCode.InternalServerError, string.Format(CommonMessages.Error_Message, errorId));
                }

                // If payment failed due to some reasons
                else if (!string.IsNullOrEmpty(paymentFailedReason))
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, paymentFailedReason);
                    return BadRequest(ModelState);
                }

                return Ok(new { Message = CoreMessages.Application_Created, InspectionId = response });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Cancel Inspection

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("cancelinspection")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CancelInspection(CancelInspectionRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Verify Inspection Permission
                var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(model.InspectionId, currentUserDetails.UserGuid);
                if (verifiedInspectionPermission == false)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                    return BadRequest(ModelState);
                }

                // Cancel Inspection
                inspectionService.CancelInspection(model.InspectionId, currentUserDetails.UserId);

                return Ok(new { Message = CoreMessages.Inspections_Cancelled });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Inspection Statuses

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
           new int[] {
                (int)UserTypes.Broker,
                (int)UserTypes.SupportTeam
           },
           new int[] {
                (int)UserRoles.AllUserRoles
           }
       )]
        [HttpGet]
        [Route("getinspectionstatuses")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInspectionStatuses()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Statuses
                var isSupportTeam = (currentUserDetails.UserTypeId == (int)UserTypes.SupportTeam)
                                  ? true
                                  : false;
                var responses = inspectionService.GetInspectionStatuses(isSupportTeam);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get All InspectionsList

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("getallinspectionslist")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllInspectionsList(BrokerInspectionsRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get User Mapped Inspection Details Count
                var result = inspectionService.GetUserMappedInspectionDetailsCount(currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                if (result > 0)
                {
                    // Get All Inspections List
                    var response = inspectionService.GetAllInspectionsList(model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                    return Ok(response);
                }

                return Ok(new { Message = CommonMessages.NewUser_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Completed InspectionsList

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("getcompletedinspectionslist")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCompletedInspectionsList(BrokerInspectionsRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get User Mapped Inspection Details Count
                var result = inspectionService.GetUserMappedInspectionDetailsCount(currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                if (result > 0)
                {
                    // Get Completed Inspections List
                    var response = inspectionService.GetCompletedInspectionsList(model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                    return Ok(response);
                }

                return Ok(new { Message = CommonMessages.NewUser_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Edit Inspection Details

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("editinspectiondetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EditInspectionDetails(EditInspectionRequest model)
        {
            ValidationHelper.RemoveModelError(ModelState, "LenderGuid");
            ValidationHelper.RemoveModelError(ModelState, "LenderRef");
            ValidationHelper.RemoveModelError(ModelState, "TemplateSetGuid");
            ValidationHelper.RemoveModelError(ModelState, "TemplateSetPlanGuid");
            ValidationHelper.RemoveModelError(ModelState, "StateCode");

            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Verify Inspection Permission
                var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(model.InspectionId, currentUserDetails.UserGuid);
                if (verifiedInspectionPermission == false)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                    return BadRequest(ModelState);
                }

                // Get Edit Inspection Details
                var response = inspectionService.EditInspectionDetails(model, currentUserDetails.Token, currentUserDetails.UserGuid,
                                                                       currentUserDetails.CompanyGuid);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Edit Inspection

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("editinspection")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EditInspection(EditInspectionRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Verify Inspection Permission
                var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(model.InspectionId, currentUserDetails.UserGuid);
                if (verifiedInspectionPermission == false)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                    return BadRequest(ModelState);
                }

                var applicationStatus = string.Empty;

                // Edit Inspection
                var paymentFailedReason = string.Empty;
                var response = inspectionService.EditInspection(inspectionDetails, model, currentUserDetails.Token, currentUserDetails.UserId,
                                                                currentUserDetails.UserGuid, (int)currentUserDetails.UserTypeId,
                                                                out paymentFailedReason, out applicationStatus);

                // Application Status is not Created
                if (response == -1)
                {
                    return BadRequest(new { Message = string.Format(CoreMessages.EditInspection_Update_Validate_ApplicationStatus, applicationStatus) });
                }
                // If payment failed due to some reasons
                else if (response == -2)
                {
                    return BadRequest(new { Message = paymentFailedReason });
                }

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Download ActivityLogs

        /// <summary>
        /// This API accessed only by Broker, SupportTeam and their supported roles
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker,
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpGet]
        [Route("downloadactivitylogs/{inspectionId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DownloadActivityLogs(long inspectionId)
        {
            if (inspectionId == 0)
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_ZeroInspectionId);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(inspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                if (currentUserDetails.UserTypeId != (int)UserTypes.SupportTeam)
                {
                    // Verify Inspection Permission
                    var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(inspectionId, currentUserDetails.UserGuid);
                    if (verifiedInspectionPermission == false)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                        return BadRequest(ModelState);
                    }
                }

                // Get Activity Logs Data
                var activityLogsDatas = inspectionService.GetActivityLogsData(inspectionId);
                if (!string.IsNullOrEmpty(activityLogsDatas))
                {
                    return Ok(activityLogsDatas);
                }
                else
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.EditInspection_ActivityLogs_NotFound);
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Send Reminder

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("sendreminder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendReminder(ReminderRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Verify Inspection Permission
                var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(model.InspectionId, currentUserDetails.UserGuid);
                if (verifiedInspectionPermission == false)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                    return BadRequest(ModelState);
                }

                // Send Reminder
                var response = inspectionService.SendReminder(model, currentUserDetails.UserId);

                // If there are no Pending Documents For Selected Inspection
                if (response == -1)
                {
                    return BadRequest(new { Message = CoreMessages.EditInspection_Reminder_Error_NoPendingDocs });
                }
                // If there are no Rejected Documents For Selected Inspection
                else if (response == -2)
                {
                    return BadRequest(new { Message = CoreMessages.EditInspection_Reminder_Error_NoRejectedDocs });
                }
                else
                {
                    return Ok(new { Message = CommonMessages.Success_Message });
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Inspection BypassRequest

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("inspectionbypassrequest")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> InspectionBypassRequest(InspectionBypassRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Verify Inspection Permission
                var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(model.InspectionId, currentUserDetails.UserGuid);
                if (verifiedInspectionPermission == false)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                    return BadRequest(ModelState);
                }

                // Get AppImage Detail
                var imageDetail = inspectionService.GetAppImageDetail(model.ImageId, model.InspectionId);
                if (imageDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidInspectionIdOrImageId);
                    return BadRequest(ModelState);
                }

                // Update Bypass Reason
                inspectionService.UpdateBypassReason(imageDetail, model, (int)currentUserDetails.UserTypeId,
                                                     currentUserDetails.UserGuid, currentUserDetails.UserId);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Image Data

        /// <summary>
        /// This API accessed only by BrokerAdmin, DefaultAdmin, BillingResponsible, PrimaryContact, SimpleBroker,
        /// SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker,
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.BrokerAdmin,
                (int)UserRoles.DefaultAdmin,
                (int)UserRoles.BillingResponsible,
                (int)UserRoles.PrimaryContact,
                (int)UserRoles.SimpleBroker,
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpGet]
        [Route("getimagedata/{inspectionId}/{imageId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetImageData(long inspectionId, long imageId)
        {
            // If Inspection Id is zero
            if (inspectionId == 0 &&
                imageId > 0)
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.InspectionId_Error_Required);
                return BadRequest(ModelState);
            }

            // If Image Id is zero
            if (inspectionId > 0 &&
                imageId == 0)
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.ImageId_Error_Required);
                return BadRequest(ModelState);
            }

            // If Inspection Id and Image Id is zero
            if (inspectionId == 0 &&
                imageId == 0)
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.GetImageData_Error_InspectionId_ImageId);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(inspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                if (currentUserDetails.UserTypeId != (int)UserTypes.SupportTeam)
                {
                    // Verify Inspection Permission
                    var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(inspectionId, currentUserDetails.UserGuid);
                    if (verifiedInspectionPermission == false)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                        return BadRequest(ModelState);
                    }
                }

                // Get AppImage Detail
                var imageDetail = inspectionService.GetAppImageDetail(imageId, inspectionId);
                if (imageDetail == null)
                {
                    return Conflict(new { Message = CoreMessages.Inspections_Error_InvalidInspectionIdOrImageId });
                }

                // Get Image Data
                var response = inspectionService.GetImageData(imageDetail, inspectionId, imageId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Review Data

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpGet]
        [Route("getreviewdata/{inspectionId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReviewData(long inspectionId)
        {
            if (inspectionId == 0)
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.InspectionId_Error_Required);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(inspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Get Review Data
                var response = inspectionService.GetReviewData(inspectionId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save Review Data

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("savereviewdata")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveReviewData(ReviewInspectionSaveRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Save Review Data
                var errorMessage = string.Empty;
                var response = inspectionService.SaveReviewData(model, currentUserDetails.UserGuid, currentUserDetails.UserId, out errorMessage);

                // If Error Message found
                // ElseIf Lender Configurations not found
                // ElseIf Report Email not configured
                // ElseIf Report not available
                if (response == -1)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, errorMessage);
                    return BadRequest(ModelState);
                }
                if (response == -2)
                {
                    return StatusCode((int)HttpStatusCode.NoContent, new { Message = CoreMessages.LenderConfigurations_NotFound });
                }
                else if (response == -3)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.SendSubmissonReport_Error_ReportEmailNotConfigured);
                    return BadRequest(ModelState);
                }
                else if (response == -4)
                {
                    return StatusCode((int)HttpStatusCode.NoContent, new { Message = CommonMessages.Report_NotAvailable });
                }

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Rotate Image

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("rotateimage")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RotateImage(ReviewInspectionRotateImageRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get AppImage Detail By ImageId
                var imageDetail = inspectionService.GetAppImageDetail(model.ImageId);
                if (imageDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidImageId);
                    return BadRequest(ModelState);
                }

                // Rotate Image
                var response = inspectionService.RotateImage(imageDetail, model, currentUserDetails.UserId);

                return Ok(new { RotatedImage = response });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Move Image

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("moveimage")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MoveImage(ReviewInspectionMoveImageRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                if (inspectionDetails.ApplicationStatus != (int)Enums.ApplicationStatus.Submitted)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey,
                                             string.Format(CoreMessages.ReviewInspection_MoveImage_Error_CheckApplicationStatus,
                                                           inspectionDetails.ApplicationStatus));
                    return BadRequest(ModelState);
                }

                // Move Image
                var response = inspectionService.MoveImage(model, currentUserDetails.UserId);

                // Source Image not found
                if (response == -1)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.ReviewInspection_MoveImage_Error_SourceImageNotFound);
                    return BadRequest(ModelState);
                }

                // Destination Image not found
                if (response == -2)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.ReviewInspection_MoveImage_Error_DestinationImageNotFound);
                    return BadRequest(ModelState);
                }

                // Image Details not found
                if (response == -3)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.ReviewInspection_MoveImage_Error_InvalidImageDetails);
                    return BadRequest(ModelState);
                }

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Notifications Details

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("notificationsdetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> NotificationDetails(NotificationsDetailsRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get User Mapped Inspection Details Count
                var result = inspectionService.GetUserMappedInspectionDetailsCount(currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                if (result > 0)
                {
                    // Get Notifications List
                    var response = inspectionService.GetNotificationsList(model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                    if (response.NotificationsList.Count() == 0)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Notifications_NotFound);
                        return BadRequest(ModelState);
                    }

                    return Ok(response);
                }

                return Ok(new { Message = CommonMessages.NewUser_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Notifications Count

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpGet]
        [Route("getnotificationscount")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificationsCount()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Notifications Count
                var response = inspectionService.GetNotificationsCount(currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                return Ok(new { NotificationsCount = response });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Update NotificationStatus

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("updatenotificationstatus")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateNotificationStatus()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Update Notification Status
                inspectionService.UpdateNotificationStatus(currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region DVS Check

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("dvscheck")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> DVSCheck(CheckDVSRequest model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Check DVS
                var response = inspectionService.CheckDVS(model, currentUserDetails.UserId);

                // DVSCheck PingFailed
                if (response == -1)
                {
                    return BadRequest(new { DVSStatus = -1, Message = CoreMessages.DVSCheck_Error_PingFailed });
                }

                // DVSCheck Rejected
                else if (response == 0)
                {
                    return BadRequest(new { DVSStatus = 0, Message = CoreMessages.DVSCheck_Rejected_Msg });
                }

                // DVSCheck Accepted
                else
                {
                    return Ok(new { DVSStatus = 1, Message = CoreMessages.DVSCheck_Accepted_Msg });
                }
            }

            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region PPSR Check

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("ppsrcheck")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PPSRCheck(PPSRModel model)
        {
            // Validate Model Inputs
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Ping PPSR
                var result = "";
                var httpStatusCode = new HttpStatusCode();
                inspectionService.PingPPSR(model, out result, out httpStatusCode);
                if (httpStatusCode == HttpStatusCode.OK)
                {
                    return Ok(new { Message = result });
                }
                else
                {
                    return StatusCode((int)httpStatusCode, result);
                }
            }

            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Send Inspection Report

        /// <summary>
        /// This API accessed only by Broker and their supported roles
        /// </summary>
        /// <param name="inspectionGuid"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("sendinspectionreport/{inspectionGuid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendInspectionReport(string inspectionGuid)
        {
            // If InspectionGuid is null or empty
            if (string.IsNullOrEmpty(inspectionGuid))
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.InspectionGuid_Error_NotFound);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details by Guid
                var inspectionDetails = inspectionService.GetInspectionDetailsByGuid(inspectionGuid);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.InspectionGuid_Invalid);
                    return BadRequest(ModelState);
                }

                // Verify Inspection Permission
                var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(inspectionDetails.Id, currentUserDetails.UserGuid);
                if (verifiedInspectionPermission == false)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                    return BadRequest(ModelState);
                }

                // Get User Details By User Id
                // Get Seller Details by Inspection Id
                // Get Lender Details by CompanyGuid
                var userDetails = dataService.GetUserDetailsByUserId(inspectionDetails.CreatedBy);
                var sellerDetails = inspectionService.GetAppUsers(inspectionDetails.Id)
                                    .FirstOrDefault(au => au.Role == (int)Enums.Role.Seller);
                var lenderDetails = dataService.GetCompanyDetails(inspectionDetails.LenderCompanyGuid);

                // Send Submission Report
                var errorMessage = string.Empty;
                var response = inspectionService.SendSubmissionReport(inspectionDetails, userDetails, sellerDetails, lenderDetails,
                                                                      inspectionGuid, false, true, out errorMessage);

                // If Error Message found
                // ElseIf Lender Configurations not found
                // ElseIf Report Email not configured
                // ElseIf Report not available
                if (response == -1)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, errorMessage);
                    return BadRequest(ModelState);
                }
                if (response == -2)
                {
                    return StatusCode((int)HttpStatusCode.NoContent, new { Message = CoreMessages.LenderConfigurations_NotFound });
                }
                else if (response == -3)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.SendSubmissonReport_Error_ReportEmailNotConfigured);
                    return BadRequest(ModelState);
                }
                else if (response == -4)
                {
                    return StatusCode((int)HttpStatusCode.NoContent, new { Message = CommonMessages.Report_NotAvailable });
                }

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Download Report

        /// <summary>
        /// This API accessed only by Broker, SupportTeam and their supported roles
        /// </summary>
        /// <param name="inspectionGuid"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker,
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("downloadreport/{inspectionGuid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DownloadReport(string inspectionGuid)
        {
            if (string.IsNullOrEmpty(inspectionGuid))
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.InspectionGuid_Error_NotFound);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspection Details By Guid
                var inspectionDetails = inspectionService.GetInspectionDetailsByGuid(inspectionGuid);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.InspectionGuid_Invalid);
                    return BadRequest(ModelState);
                }

                if (currentUserDetails.UserTypeId != (int)UserTypes.SupportTeam)
                {
                    var verifiedInspectionPermission = inspectionService.VerifyInspectionPermission(inspectionDetails.Id, currentUserDetails.UserGuid);
                    if (verifiedInspectionPermission == false)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspection_InvalidPermissionAccess);
                        return BadRequest(ModelState);
                    }
                }

                // Get Report Data
                long inspectionId = 0;
                var reportData = inspectionService.GetReport(ref inspectionId, inspectionGuid);
                if (string.IsNullOrEmpty(reportData))
                {
                    return StatusCode((int)HttpStatusCode.NoContent, new { Message = CommonMessages.Report_NotAvailable });
                }

                var byteData = System.Convert.FromBase64String(reportData);
                using (MemoryStream ms = new MemoryStream(byteData))
                {
                    return Ok(ms.ToArray());
                }

                // Return Pdf File
                //return File(System.Convert.FromBase64String(reportData), "application/pdf", string.Format("Verimoto_InspectionReport_{0}.pdf", inspectionId));
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion
    }
}
