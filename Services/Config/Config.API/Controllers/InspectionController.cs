using Common.Authorization.Authorize;
using Common.Messages;
using Common.Models.Users;
using Common.Validations.Helper;
using Config.API.Helper;
using Config.API.Models;
using Config.API.Models.Asset;
using Config.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using static Common.Models.Enums;

namespace Config.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InspectionController : ControllerBase
    {
        private readonly ICompanyService companyService;
        private readonly IInspectionService inspectionService;
        private readonly IErrorService errorService;

        #region Constructor

        public InspectionController(ICompanyService companyService, IInspectionService service,
                                                                    IErrorService errorService)
        {
            this.companyService = companyService;
            this.inspectionService = service;
            this.errorService = errorService;
        }

        #endregion

        #region Get TemplatePlans

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
        [Route("templateplans")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTemplatePlans(InspectionPlansRequest model)
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
                // Get Lender Detail By Guid
                var lenderDetail = inspectionService.GetLenderDetailByGuid(model.LenderGuid);
                if (lenderDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CommonMessages.CompanyGuid_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Template Detail By Guid
                var templateSetDetail = inspectionService.GetTemplateSetDetailByGuid(model.TemplateGuid);
                if (templateSetDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.TemplatePlans_Error_TemplateNotExists);
                    return BadRequest(ModelState);
                }

                // Get State Detail By Id
                var StateDetail = inspectionService.GetStateDetailById(model.StateId);
                if (StateDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.StateDetails_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Template Plans
                var responses = inspectionService.GetTemplatePlans(lenderDetail.Id, templateSetDetail.Id, model.StateId);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Get Asset DocumentDetails

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
        [Route("getdocuments")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDocuments(AssetDocListRequest model)
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
                // Get Lender Detail By Guid
                var lenderDetail = inspectionService.GetLenderDetailByGuid(model.LenderGuid);
                if (lenderDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CommonMessages.CompanyGuid_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Template Detail By Guid
                var templateSetDetail = inspectionService.GetTemplateSetDetailByGuid(model.TemplateGuid);
                if (templateSetDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.TemplatePlans_Error_TemplateNotExists);
                    return BadRequest(ModelState);
                }

                if (model.StateId >= 0)
                {
                    if (model.StateId != 0)
                    {
                        // Get State Detail By Id
                        var StateDetail = inspectionService.GetStateDetailById(model.StateId);
                        if (StateDetail == null)
                        {
                            ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.StateDetails_Invalid);
                            return BadRequest(ModelState);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(model.planGuid))
                {
                    // Get Plan Detail By Guid
                    var planDetail = inspectionService.GetPlanDetailByGuid(model.planGuid);
                    if (planDetail == null)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.PlanGuid_Invalid);
                        return BadRequest(ModelState);
                    }
                }

                // Get Asset Document Details
                var response = inspectionService.GetAssetDocDetails(model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Get InspectionDetails

        /// <summary>
        /// This API accessed only by SupportTeamAdmin and SupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Support
            }
        )]
        [HttpGet]
        [Route("getinspectiondetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInspectionDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get All Lenders
                var lendersResponses = companyService.GetAllLenders(true, true);

                // Get All Assets
                var assetsResponses = companyService.GetAllAssets();

                // Get All States
                var statesResponses = companyService.GetStateoption();

                return Ok(new { Lenders = lendersResponses, Assets = assetsResponses, States = statesResponses });
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
