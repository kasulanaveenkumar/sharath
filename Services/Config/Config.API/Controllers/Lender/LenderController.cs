using Common.Authorization.Authorize;
using Common.Messages;
using Common.Models.Users;
using Common.Validations.Helper;
using Config.API.Helper;
using Config.API.Models;
using Config.API.Models.Lender;
using Config.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Common.Models.Enums;

namespace Config.API.Controllers.Lender
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LenderController : ControllerBase
    {
        private readonly ILenderService lenderService;
        private readonly ICompanyService companyService;
        private readonly IInspectionService inspectionService;
        private readonly IErrorService errorService;

        #region Constructor

        public LenderController(ILenderService lenderService, ICompanyService companyService, 
                                IInspectionService inspectionService, IErrorService errorService)
        {
            this.lenderService = lenderService;
            this.companyService = companyService;
            this.inspectionService = inspectionService;
            this.errorService = errorService;
        }

        #endregion

        #region Get Onboard details

        /// <summary>
        /// This API accessed only on Onboarding
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.All
            },
            new int[] {
                (int)UserRoles.Onboarding
            }
        )]
        [HttpGet]
        [Route("getonboarddetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetOnboardDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Onboard Details
                var responses = companyService.GetOnboardDetails();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion 

        #region Save Onboard CompanyDetails

        /// <summary>
        /// This API accessed only on Onboarding
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.All
            },
            new int[] {
                (int)UserRoles.Onboarding
            }
        )]
        [HttpPost]
        [Route("onboardlender")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> OnboardLender(LenderRegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                string companyGuid = "";
                string result = "";

                // Get Company Details by Name
                var company = companyService.GetCompanyByName(model.NewCompanyDetail.CompanyName, "");
                if (company != null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.CompanyName_Exists);
                    return BadRequest(ModelState);
                }

                // Onboard Lender
                companyService.OnboardLender(model, currentUserDetails.UserId, currentUserDetails.Token, 
                                             out companyGuid, out result);

                return Ok(new { CompanyGUID = companyGuid, ErrorMessage = result });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Mapped Assets And States

        /// <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpGet]
        [Route("getmappedassetsstates")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMappedAssetsStates()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get MappedAssets States
                var responses = lenderService.GetMappedAssetsStates(companyDetails.CompanyGuid);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get InspectionPlans

        /// <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpPost]
        [Route("getinspectionplans")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInspectionPlans(InspectionPlanDetailRequest model)
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
                // Get Template Detail By Guid
                var templateSetDetail = inspectionService.GetTemplateSetDetailByGuid(model.TemplateSetGuid);
                if (templateSetDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.TemplatePlans_Error_TemplateNotExists);
                    return BadRequest(ModelState);
                }

                // Get Company Details By Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Asset Document Details
                var responses = lenderService.GetAssetDocumentDetails(templateSetDetail, model.TemplateSetGuid, model.IsApplyToAllStates, 
                                                                      model.IsUseDbValue, companyDetails.CompanyGuid);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save InspectionPlans

        /// <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpPost]
        [Route("saveinspectionplans")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveInspectionPlans(InspectionPlanDetailRequest model)
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
                // Get Template Detail By Guid
                var templateSetDetail = inspectionService.GetTemplateSetDetailByGuid(model.TemplateSetGuid);
                if (templateSetDetail == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.TemplatePlans_Error_TemplateNotExists);
                    return BadRequest(ModelState);
                }

                // Get Company Details By Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Save Inspection Plans
                lenderService.SaveInspectionPlans(templateSetDetail, model, companyDetails.CompanyGuid);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get CompanyDetails

        /// <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpGet]
        [Route("getcompanydetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCompanyDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Lender Company Details
                var response = companyService.GetLenderCompanyDetails(companyDetails, currentUserDetails.CompanyGuid, currentUserDetails.Token);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save CompanyDetails

        /// <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpPost]
        [Route("savecompanydetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveCompanyDetails(Models.Lender.LenderCompanyRequest model)
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
                var errorMessage = "";

                // Get Lender Details by Name
                var lenderDetails = companyService.GetLenderByName(model.LenderName, currentUserDetails.CompanyGuid);
                if (lenderDetails != null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, UserMessages.Lender_Error_LenderNameExists);
                    return BadRequest(ModelState);
                }
                else
                {
                    // Get Company Details by Guid
                    var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                    if (companyDetails == null)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                        return BadRequest(ModelState);
                    }

                    // Save Lender Company Details
                    companyService.SaveLenderCompanyDetails(companyDetails, model, currentUserDetails.CompanyGuid, 
                                                            currentUserDetails.Token, currentUserDetails.UserId, 
                                                            out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        // Return Error Message.
                        return StatusCode((int)HttpStatusCode.InternalServerError, errorMessage);
                    }

                    // Get Company Details
                    var response = companyService.GetLenderCompanyDetails(companyDetails, currentUserDetails.CompanyGuid, 
                                                                          currentUserDetails.Token);

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get PaymentDetails

        /// <summary>
        /// This API accessed only by BillingResponsible User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.BillingResponsible
            }
        )]
        [HttpGet]
        [Route("getpaymentdetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPaymentDetails(GetPaymentDetailsRequest model)
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                if (model.IsPaymentByCard)
                {
                    // Get Card Details
                    var responses = companyService.GetCardDetails(companyDetails);

                    return Ok(responses);
                }
                else
                {
                    return Ok(new { Message = "Success" });
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save PaymentDetails

        /// <summary>
        /// This API accessed only by BillingResponsible User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.BillingResponsible
            }
        )]
        [HttpPost]
        [Route("savepaymentdetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SavePaymentDetails(SavePaymentDetailsRequest model)
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
                // Get Company Details by Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                if (model.PaymentMethods != null &&
                    model.PaymentMethods.Count() > 0)
                {
                    // Save Card Details
                    var result = companyService.SaveCardDetails(companyDetails, model.PaymentMethods, currentUserDetails.UserId);
                    if (string.IsNullOrEmpty(result))
                    {
                        return Ok(new { Message = CommonMessages.Success_Message });
                    }
                    else
                    {
                        return StatusCode(206, result);
                    }
                }
                else
                {
                    return Ok(new { Message = "Success" });
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Company Visibility 

        /// <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpGet]
        [Route("getcompanyvisibility")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCompanyVisibility()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Company Visibility
                var response = lenderService.GetCompanyVisibility(companyDetails);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save Company Visibility

        /// <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpPost]
        [Route("savecompanyvisibility")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveCompanyVisibility(SaveCompanyVisibilityRequest model)
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
                // Get Company Details by Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Save Company Visibility
                var result = lenderService.SaveCompanyVisibility(companyDetails, model);
                if (result == -1)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.CompanyVisibility_Error_SameVisibility);
                    return BadRequest(ModelState);
                }

                // Get Company Visibility
                var response = lenderService.GetCompanyVisibility(companyDetails);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Broker Companies

        /// <summary>
        /// This API accessed only by LenderAdmin and SupportTeamAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender,
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.LenderAdmin,
                (int)UserRoles.SupportTeamAdmin
            }
        )]
        [HttpGet]
        [Route("getbrokercompanies")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBrokerCompanies()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Broker Companies
                var responses = lenderService.GetBrokerCompanies(companyDetails.CompanyGuid);
                if (responses.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.BrokerCompanies_Error_NotFound);
                    return BadRequest(ModelState);
                }

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Update LenderVisibility

        /// <summary>
        /// This API accessed only by LenderAdmin and SupportTeamAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender,
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.LenderAdmin,
                (int)UserRoles.SupportTeamAdmin
            }
        )]
        [HttpPost]
        [Route("updatelendervisibility")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateLenderVisibility(List<SaveLenderVisibilityRequest> model)
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
                // Get Company Details by Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Update LenderVisibility
                lenderService.UpdateLenderVisibility(companyDetails, model);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Blocked BrokerUsers

        // <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpGet]
        [Route("getblockedbrokerusers/{brokerCompanyGuid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBlockedBrokerUsers(string brokerCompanyGuid)
        {
            if (string.IsNullOrEmpty(brokerCompanyGuid))
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.CompanyGuid_Required);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Broker Company Details by Guid
                var brokerCompanyDetails = companyService.GetCompanyDetails(brokerCompanyGuid);
                if (brokerCompanyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.BrokerCompany_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Lender Company Details by Guid
                var lenderCompanyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (lenderCompanyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.LenderCompany_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Blocked BrokerUsers
                var responses = lenderService.GetBlockedBrokerUsers(brokerCompanyDetails.CompanyGuid, lenderCompanyDetails.CompanyGuid);
                if (responses.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.BrokerUsers_Error_NotFound);
                    return BadRequest(ModelState);
                }

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save Blocked BrokerUsers

        // <summary>
        /// This API accessed only by LenderAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Lender
            },
            new int[] {
                (int)UserRoles.LenderAdmin
            }
        )]
        [HttpPost]
        [Route("saveblockedbrokerusers")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveBlockedBrokerUsers(LenderBlockedBrokerUsersRequest model)
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
                // Get Broker Company Details by Guid
                var brokerCompanyDetails = companyService.GetCompanyDetails(model.BrokerCompanyGuid);
                if (brokerCompanyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.BrokerCompany_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Lender Company Details by Guid
                var lenderCompanyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (lenderCompanyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.LenderCompany_Invalid);
                    return BadRequest(ModelState);
                }

                // Save Blocked BrokerUsers
                lenderService.SaveBlockedBrokerUsers(model, currentUserDetails.CompanyGuid);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion
    }
}
