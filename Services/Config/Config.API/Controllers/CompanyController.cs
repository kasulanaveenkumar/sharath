using Common.Authorization.Authorize;
using Common.EventMessages;
using Common.Messages;
using Common.Models.Users;
using Common.Payments.Helper;
using Common.Validations.Helper;
using Config.API.Helper;
using Config.API.Models;
using Config.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Common.Models.Enums;

namespace Config.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService companyService;
        private readonly IErrorService errorService;
        private readonly StripeIntegrationHelper stripeIntegrationHelper;
        private readonly IMessageSender messageSender;

        #region Constructor

        public CompanyController(ICompanyService service, IErrorService errorService, IMessageSender messageSender)
        {
            this.companyService = service;
            this.errorService = errorService;
            this.messageSender = messageSender;

            stripeIntegrationHelper = new StripeIntegrationHelper(Startup.AppConfiguration.GetSection("AppSettings")
                                                                                          .GetSection("StripeApiKey").Value);
        }

        #endregion

        #region Get CompaniesList

        /// <summary>
        /// This API accessed only by SupportTeamAdmin and SupportUser
        /// </summary>
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
        [Route("getcompanieslist")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCompaniesList()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Companies List
                var responses = companyService.GetCompaniesList();
                if (responses != null &&
                    responses.Count > 0)
                {
                    return Ok(responses);
                }
                else
                {
                    return Ok(new { Message = ConfigMessages.Companies_NotFound });
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save Admin CompanyDetails

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
        [HttpPost]
        [Route("saveadmincompanydetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveAdminCompanyDetails(SaveCompanyDetailsRequest model)
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
                bool isCompanyExists = false;
                string companyGuid = string.Empty;
                string errorMsg = string.Empty;

                if (!string.IsNullOrEmpty(model.ExistingCompanyGuid))
                {
                    // Get Company Details By CompanyGuid
                    var companyDetails = companyService.GetCompanyDetails(model.ExistingCompanyGuid);
                    if (companyDetails == null)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                        return BadRequest(ModelState);
                    }
                }

                // Check BrokerUsers Data
                if (model.BrokerUsers == null ||
                    model.BrokerUsers.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.BrokerUsersList_Error_Required);
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
                    var validationMsg = "Asset(s) " + invalidAssets + " invalid in list";
                    ModelState.AddModelError(CommonMessages.BadRequestKey, validationMsg);
                    return BadRequest(ModelState);
                }

                // Get Invalid Lenders
                var invalidLenders = companyService.GetInvalidLendersWorkWith(model.LendersWorkWith);
                if (!string.IsNullOrEmpty(invalidLenders))
                {
                    var validationMsg = "Lender(s) " + invalidLenders + " invalid in list";
                    ModelState.AddModelError(CommonMessages.BadRequestKey, validationMsg);
                    return BadRequest(ModelState);
                }

                // Save Admin Company Details
                var response = companyService.SaveAdminCompanyDetails(model,
                                                                      currentUserDetails.Token,
                                                                      currentUserDetails.UserId,
                                                                      out isCompanyExists,
                                                                      out companyGuid,
                                                                      out errorMsg);
                if (isCompanyExists)
                {
                    // Company name already exists.
                    return BadRequest(new { ExistCompany = CoreMessages.CompanyName_Exists });
                }

                if (response == -1)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                    return BadRequest(ModelState);
                }

                return Ok(new { CompanyGUID = companyGuid, UserGuid = currentUserDetails.UserGuid });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Admin CompanyDetails

        /// <summary>
        /// This API accessed only by SupportTeamAdmin and SupportUser
        /// </summary>
        /// <param name="companyGuid"></param>
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
        [Route("getadmincompanydetails/{companyGuid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAdminCompanyDetails(string companyGuid)
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
                // Get Company Details By CompanyGuid
                var companyDetails = companyService.GetCompanyDetails(companyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Admin Company Details By CompanyGuid
                var response = companyService.GetAdminCompanyDetails(companyDetails, currentUserDetails.Token);
                if (response == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save UserProfile Company

        /// <summary>
        /// This API accessed only by SupportTeam and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.AllUserRoles
            }
        )]
        [HttpPost]
        [Route("saveuserprofilecompany")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveUserProfileCompany(UserProfileCompanyRequest model)
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
                // Get Company Details By CompanyGuid
                var companyDtls = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDtls == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get User Profile Company
                var response = companyService.GetUserProfileCompany(companyDtls);
                if (response != null)
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
                        // Save User Profile Company
                        companyService.SaveUserProfileCompany(model, currentUserDetails.CompanyGuid, currentUserDetails.UserId);

                        return Ok(new { Status = CommonMessages.Success_Message });
                    }
                }
                else
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get LendersList

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
        [Route("getlenderslist")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLendersList(LendersListRequest model)
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Lenders List
                var responses = companyService.GetLendersList(model);
                if (responses != null &&
                    responses.Count > 0)
                {
                    return Ok(responses);
                }
                else
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, UserMessages.LendersList_NotFound);
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Lender Details

        /// <summary>
        /// This API accessed only by SupportTeamAdmin and SupportUser
        /// </summary>
        /// <param name="companyGuid"></param>
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
        [Route("getlenderdetails/{companyGuid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLenderDetails(string companyGuid)
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
                // Get Lender Details By CompanyGuid
                var response = companyService.GetLenderDetails(companyGuid, currentUserDetails.Token);
                if (response == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Lender_Invalid);
                    return BadRequest(ModelState);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save Lender Details

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
        [HttpPost]
        [Route("savelenderdetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveLenderDetails(LenderCompanyRequest model)
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
                var lenderCompanyGuid = string.Empty;

                // Get Lender Details by Name
                var lenderDetails = companyService.GetLenderByName(model.LenderName, model.LenderCompanyGuid);
                if (lenderDetails != null)
                {
                    // Lender already exists.
                    return BadRequest(new { ExistLender = UserMessages.Lender_Error_LenderNameExists });
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.LenderCompanyGuid))
                    {
                        // Get Company Details By CompanyGuid
                        var companyDetails = companyService.GetCompanyDetails(model.LenderCompanyGuid);
                        if (companyDetails == null)
                        {
                            ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Lender_Invalid);
                            return BadRequest(ModelState);
                        }
                    }

                    // Check CompanyContacts Data
                    if (model.CompanyContacts == null ||
                        model.CompanyContacts.Count() == 0)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.CompanyContacts_Error_Required);
                        return BadRequest(ModelState);
                    }

                    // Check AssetsWorkWith Data
                    if (model.Assets == null ||
                        model.Assets.Count() == 0)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.AssetsWorkWith_Error_Required);
                        return BadRequest(ModelState);
                    }

                    // Check LenderConfigurations Data
                    if (model.LenderConfigurations == null)
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.LenderConfigurations_NotFound);
                        return BadRequest(ModelState);
                    }

                    // Get Invalid Company Contact Types
                    var invalidCompanyContactTypes = companyService.GetInvalidCompanyContactTypes(model.CompanyContacts);
                    if (!string.IsNullOrEmpty(invalidCompanyContactTypes))
                    {
                        var errorMsg = "Company Contact Type(s) " + invalidCompanyContactTypes + " invalid in list";
                        ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                        return BadRequest(ModelState);
                    }

                    // Get Invalid Assets
                    var invalidAssets = companyService.GetInvalidAssetsWorkWith(model.Assets);
                    if (!string.IsNullOrEmpty(invalidAssets))
                    {
                        var errorMsg = "Asset(s) " + invalidAssets + " invalid in list";
                        ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                        return BadRequest(ModelState);
                    }

                    // Save Lender Details
                    var errorMessage = "";
                    companyService.SaveLenderDetails(model, currentUserDetails.UserId, currentUserDetails.Token,
                                                     ref lenderCompanyGuid, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        ModelState.AddModelError(CommonMessages.BadRequestKey, errorMessage);
                        return BadRequest(ModelState);
                    }

                    // Get Lender Details
                    var response = companyService.GetLenderDetails(lenderCompanyGuid, currentUserDetails.Token);

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

        #region Get All LendersDetails

        /// <summary>
        /// This API accessed only by SupportTeamAdmin and SupportUser
        /// </summary>
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
        [Route("getalllendersdetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllLendersDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get All Lenders Details
                var responses = companyService.GetAllLendersDetails();
                if (responses != null &&
                    responses.Count > 0)
                {
                    return Ok(responses);
                }
                else
                {
                    return Ok(new { Message = ConfigMessages.Users_NotFound });
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Import Brokers

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
        [HttpPost]
        [Route("importbrokers")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ImportBrokers()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Import Brokers
                var importErrorCount = 0;
                companyService.ImportBrokers(currentUserDetails.Token, out importErrorCount);
                if (importErrorCount > 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, "There is an error in Import process");
                    return BadRequest(ModelState);
                }

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion
    }
}
