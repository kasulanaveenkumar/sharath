using Common.Authorization.Authorize;
using Common.Messages;
using Common.Models.Users;
using Common.Validations.Helper;
using Config.API.Helper;
using Config.API.Models;
using Config.API.Models.Data;
using Config.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Common.Models.Enums;

namespace Config.API.Controllers.Data
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ICompanyService companyService;
        private readonly IInspectionService inspectionService;
        private readonly IAssetService assetService;
        private readonly IDataService dataService;
        private readonly IErrorService errorService;

        #region Constructor

        public DataController(ICompanyService companyService, IInspectionService inspectionService,
                              IDataService dataService, IErrorService errorService, IAssetService assetService)
        {
            this.companyService = companyService;
            this.inspectionService = inspectionService;
            this.dataService = dataService;
            this.errorService = errorService;
            this.assetService = assetService;
        }

        #endregion

        #region Get State Option

        // <summary>
        /// This API accessed only by Broker Admin, Default Admin, Lender Admin
        /// SupportTeam Admin and Support User
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[]
            {
                (int)UserTypes.Broker,
                (int)UserTypes.Lender,
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.BrokerAdmin,
                (int)UserRoles.DefaultAdmin,
                (int)UserRoles.LenderAdmin,
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Support
            }
        )]
        [HttpGet]
        [Route("getstateoption")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetStateoption()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get State Option
                var responses = companyService.GetStateoption();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Get All Assets

        /// <summary>
        /// This API accessed only by Broker and their users, 
        /// SupportTeam Admin, Reviewer and Simple SupportUser
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[]
            {
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
        [Route("getallassets")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllAssets()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Assets List
                var isSupportTeam = (currentUserDetails.UserTypeId == (int)UserTypes.SupportTeam)
                                  ? true
                                  : false;
                var responses = companyService.GetAllAssets(isSupportTeam);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg }); ;
            }
        }

        #endregion

        #region Get All Lenders

        /// <summary>
        /// This API accessed only by Broker and their users, 
        /// SupportTeam Admin, Reviewer and Simple SupportUser
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[]
            {
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
        [Route("getalllenders")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllLenders()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Lenders List
                var isSupportTeam = (currentUserDetails.UserTypeId == (int)UserTypes.SupportTeam)
                                  ? true
                                  : false;
                var responses = companyService.GetAllLenders(isSupportTeam, false);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg }); ;
            }
        }

        #endregion

        #region Get Lenders Work With

        /// <summary>
        /// This API accessed only by Broker and their users
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
        [Route("getlendersworkwith")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLendersWorkWith()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details By Guid
                var companyDetails = companyService.GetCompanyDetailsByGuid(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Lenders Work With
                var responses = companyService.GetLendersWorkWith(companyDetails.Id);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Assets work with

        /// <summary>
        /// This API accessed only by Broker and their users
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
        [Route("getassetsworkwith")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAssetsWorkWith()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details By Guid
                var companyDetails = companyService.GetCompanyDetailsByGuid(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Assets work with
                var responses = companyService.GetAssetsWorkWith(companyDetails.Id, (int)currentUserDetails.UserTypeId);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Company Details

        /// <summary>
        /// This API accessed only by Broker and their users
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
                // Get Company Details By Guid
                var response = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
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

        #region Save NoLender Preference

        [HttpPost]
        [Route("savenolenderpreference")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveNoLenderPreference(SaveNoLenderPreferenceRequest model)
        {

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

                // Save NoLender Preference
                inspectionService.SaveNoLenderPreference(templateSetDetail, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save Users

        [HttpPost]
        [Route("saveusers")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveUsers(List<SaveUsersRequest> model)
        {

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Save Users
                dataService.SaveUsers(model);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("savereguser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveRegUser(List<SaveUsersRequest> model)
        {

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Save Users
                dataService.SaveUsers(model);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get User Profile Company

        [AllowAnonymous]
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

            try
            {
                // Get Company Details By Guid
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
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, companyGuid, null, null);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion
    }
}
