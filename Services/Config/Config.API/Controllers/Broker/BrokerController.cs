using Common.Authorization.Authorize;
using Common.Extensions;
using Common.Messages;
using Common.Models.Users;
using Common.Validations.Helper;
using Config.API.Helper;
using Config.API.Models;
using Config.API.Models.Broker;
using Config.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Common.Models.Enums;

namespace Config.API.Controllers.Broker
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BrokerController : ControllerBase
    {
        private readonly ICompanyService companyService;
        private readonly IDataService dataService;
        private readonly IErrorService errorService;

        #region Constructor

        public BrokerController(ICompanyService companyService, IDataService dataService, IErrorService errorService)
        {
            this.companyService = companyService;
            this.dataService = dataService;
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
        [HttpPost]
        [Route("getonboarddetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetOnboardDetails(OnboardDetailsRequest model)
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
                // Get User Details By Email
                var userDetails = dataService.GetUserDetailsByEmail(model.Email, currentUserDetails.Token);
                if (userDetails == null ||
                    userDetails.UserGuid != currentUserDetails.UserGuid)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.User_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Onboard Details
                var responses = companyService.GetOnboardDetails(model);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
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
        [Route("onboardbroker")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> OnboardBroker(BrokerRegisterRequest model)
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
                var companyGuid = string.Empty;
                var invalidCardDetails = string.Empty;

                // Is Company Exists?
                if (model.IsNewCompany)
                {
                    // Get Company Details by Name
                    var company = companyService.GetCompanyByName(model.NewCompanyDetail.CompanyName, "");
                    if (company != null)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.CompanyName_Exists);
                        return BadRequest(ModelState);
                    }
                }

                if (!string.IsNullOrEmpty(model.ExistingCompanyGuid))
                {
                    // Get Company Details By Guid
                    var companyDetails = companyService.GetCompanyDetails(model.ExistingCompanyGuid);
                    if (companyDetails == null)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                        return BadRequest(ModelState);
                    }
                }

                // Check PaymentMethods Data
                if (model.PaymentMethods == null ||
                    model.PaymentMethods.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.PaymentMethods_Error_Required);
                    return BadRequest(ModelState);
                }

                // Check AssetsWorkWith Data
                if (model.AssetsWorkWith == null ||
                    model.AssetsWorkWith.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.AssetsWorkWith_Error_Required);
                    return BadRequest(ModelState);
                }

                // Check LendersWorkWith Data
                if (model.LendersWorkWith == null ||
                    model.LendersWorkWith.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.LendersWorkWith_Error_Required);
                    return BadRequest(ModelState);
                }

                // Get Invalid Assets
                var invalidAssets = companyService.GetInvalidAssetsWorkWith(model.AssetsWorkWith);
                if (!string.IsNullOrEmpty(invalidAssets))
                {
                    var errorMsg = "Asset(s) " + invalidAssets + " invalid in list";
                    ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                    return BadRequest(ModelState);
                }

                // Get Invalid Lenders
                var invalidLenders = companyService.GetInvalidLendersWorkWith(model.LendersWorkWith);
                if (!string.IsNullOrEmpty(invalidLenders))
                {
                    var errorMsg = "Lender(s) " + invalidLenders + " invalid in list";
                    ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                    return BadRequest(ModelState);
                }

                // Onboard Broker
                companyService.OnboardBroker(model, currentUserDetails.UserId, out companyGuid, out invalidCardDetails);

                return Ok(new { CompanyGUID = companyGuid, UserGuid = currentUserDetails.UserGuid, InvalidCardDetails = invalidCardDetails });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save Broker CompanyDetails

        /// <summary>
        /// This API accessed only by BrokerAdmin and DefaultAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.BrokerAdmin,
                (int)UserRoles.DefaultAdmin
            }
        )]
        [HttpPost]
        [Route("savebrokercompanydetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveBrokerCompanyDetails(Models.Broker.SaveCompanyDetailsRequest model)
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
                // Get Company Details by CompanyGuid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Check AssetsWorkWith Data
                if (model.AssetsWorkWith == null ||
                    model.AssetsWorkWith.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.AssetsWorkWith_Error_Required);
                    return BadRequest(ModelState);
                }

                // Check LendersWorkWith Data
                if (model.LendersWorkWith == null ||
                    model.LendersWorkWith.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.LendersWorkWith_Error_Required);
                    return BadRequest(ModelState);
                }

                // Get Invalid Assets
                var invalidAssets = companyService.GetInvalidAssetsWorkWith(model.AssetsWorkWith);
                if (!string.IsNullOrEmpty(invalidAssets))
                {
                    var errorMsg = "Asset(s) " + invalidAssets + " invalid in list";
                    ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                    return BadRequest(ModelState);
                }

                // Get Invalid Lenders
                var invalidLenders = companyService.GetInvalidLendersWorkWith(model.LendersWorkWith);
                if (!string.IsNullOrEmpty(invalidLenders))
                {
                    var errorMsg = "Lender(s) " + invalidLenders + " invalid in list";
                    ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                    return BadRequest(ModelState);
                }

                if (!string.IsNullOrEmpty(model.CompanyLogo))
                {
                    var fileExtn = model.CompanyLogo.GetBase64Extension();
                    if (string.IsNullOrEmpty(fileExtn))
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, CommonMessages.InvalidFileExtension);
                        return BadRequest(ModelState);
                    }
                }

                // Save Company Details
                bool isCompanyExists = false;
                var result = string.Empty;
                companyService.SaveBrokerCompanyDetails(companyDetails, 
                                                        model, 
                                                        companyDetails.CompanyGuid, 
                                                        currentUserDetails.UserId,
                                                        currentUserDetails.Token,
                                                        out isCompanyExists, 
                                                        out result);
                if (isCompanyExists)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.CompanyName_Exists);
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

        #region Get Broker CompanyDetails

        /// <summary>
        /// This API accessed only by BrokerAdmin and DefaultAdmin
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.BrokerAdmin,
                (int)UserRoles.DefaultAdmin
            }
        )]
        [HttpGet]
        [Route("getbrokercompanydetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBrokerCompanyDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by CompanyGuid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Company Details
                var response = companyService.GetBrokerCompanyDetails(companyDetails);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save BrokerAdmin CompanyDetails

        /// <summary>
        /// This API accessed only by BrokerAdmin and DefaultAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.BrokerAdmin,
                (int)UserRoles.DefaultAdmin
            }
        )]
        [HttpPost]
        [Route("savebrokeradmincompanydetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveBrokerAdminCompanyDetails(BrokerAdminCompanyRequest model)
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
                // Get Company Details by Name
                var companyDetails = companyService.GetCompanyByName(model.CompanyName, currentUserDetails.CompanyGuid);
                if (companyDetails != null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, UserMessages.CompanyDetails_Error_CompanyNameExists);
                    return BadRequest(ModelState);
                }
                else
                {
                    // Check AssetsWorkWith Data
                    if (model.Assets == null ||
                        model.Assets.Count() == 0)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.AssetsWorkWith_Error_Required);
                        return BadRequest(ModelState);
                    }

                    // Check LendersWorkWith Data
                    if (model.Lenders == null ||
                        model.Lenders.Count() == 0)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.LendersWorkWith_Error_Required);
                        return BadRequest(ModelState);
                    }

                    // Get Invalid Assets
                    var invalidAssets = companyService.GetInvalidAssetsWorkWith(model.Assets.Select(a => a.TemplateSetGUID).ToList());
                    if (!string.IsNullOrEmpty(invalidAssets))
                    {
                        var errorMsg = "Asset(s) " + invalidAssets + " invalid in list";
                        ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                        return BadRequest(ModelState);
                    }

                    // Get Invalid Lenders
                    var invalidLenders = companyService.GetInvalidLendersWorkWith(model.Lenders.Select(l => l.LenderGUID).ToList());
                    if (!string.IsNullOrEmpty(invalidLenders))
                    {
                        var errorMsg = "Lender(s) " + invalidLenders + " invalid in list";
                        ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                        return BadRequest(ModelState);
                    }

                    if (!string.IsNullOrEmpty(model.CompanyLogo))
                    {
                        var fileExtn = model.CompanyLogo.GetBase64Extension();
                        if (string.IsNullOrEmpty(fileExtn))
                        {
                            ModelState.AddModelError(CommonMessages.BadRequestKey, CommonMessages.InvalidFileExtension);
                            return BadRequest(ModelState);
                        }
                    }

                    // Save Broker Admin Company Details
                    companyService.SaveBrokerAdminCompanyDetails(model, currentUserDetails.CompanyGuid, currentUserDetails.Token,
                                                                 currentUserDetails.UserId);

                    // Get Broker Admin Company Details
                    var response = companyService.GetBrokerAdminCompanyDetails(companyDetails);

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get BrokerAdmin CompanyDetails

        /// <summary>
        /// This API accessed only by BrokerAdmin and DefaultAdmin
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.BrokerAdmin,
                (int)UserRoles.DefaultAdmin
            }
        )]
        [HttpGet]
        [Route("getbrokeradmincompanydetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBrokerAdminCompanyDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by CompanyGuid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);

                // Get Broker Admin Company Details
                var response = companyService.GetBrokerAdminCompanyDetails(companyDetails);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get UserProfile Company 

        /// <summary>
        /// This API accessed only by Broker and SupportTeam
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[]{
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
                (int)UserRoles.Support
            }
        )]
        [HttpGet]
        [Route("getuserprofilecompany")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserProfileCompany()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by CompanyGuid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);

                // Get User Profile Company 
                var response = companyService.GetUserProfileCompany(companyDetails);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        /// <summary>
        /// This API accessed only by SupportTeamAdmin and their supported roles
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[]{
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpGet]
        [Route("getuserprofilecompany/{companyGuid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserProfileCompany(string companyGuid)
        {
            if (string.IsNullOrEmpty(companyGuid))
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.CompanyGuid_Required);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details by CompanyGuid
                var companyDetails = companyService.GetCompanyDetails(companyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                var response = companyService.GetUserProfileCompany(companyDetails);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, companyGuid, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion
    }
}
