using Common.Authorization.Authorize;
using Common.Messages;
using Common.Models.Users;
using Common.Validations.Helper;
using Core.API.Helper;
using Core.API.Models;
using Core.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using static Common.Models.Enums;

namespace Core.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LenderConfigurationController : ControllerBase
    {
        private readonly ILenderConfigurationService lenderConfigurationService;
        private readonly IDataService dataService;
        private readonly IErrorService errorService;

        #region Constructor

        public LenderConfigurationController(ILenderConfigurationService lenderConfigurationService,
                                             IDataService dataService,
                                             IErrorService errorService)
        {
            this.lenderConfigurationService = lenderConfigurationService;
            this.dataService = dataService;
            this.errorService = errorService;
        }

        #endregion

        #region Get LenderDetails

        /// <summary>
        /// This API accessed only by LenderAdmin, SupportTeamAdmin and SupportUser
        /// </summary>
        /// <param name="companyGuid"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
           new int[] {
                (int)UserTypes.Lender,
                (int)UserTypes.SupportTeam
           },
           new int[] {
                (int)UserRoles.LenderAdmin,
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
            // If CompanyGuid is null or empty
            if (string.IsNullOrEmpty(companyGuid))
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.CompanyGuid_Required);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Lender Details
                var response = lenderConfigurationService.GetLenderDetails(companyGuid);
                if (response == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Lender_Invalid);
                    return BadRequest(ModelState);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, companyGuid, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save LenderDetails

        /// <summary>
        /// This API accessed only by LenderAdmin, SupportTeamAdmin and SupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
           new int[] {
                (int)UserTypes.All,
                (int)UserTypes.Lender,
                (int)UserTypes.SupportTeam
           },
           new int[] {
                (int)UserRoles.Onboarding,
                (int)UserRoles.LenderAdmin,
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
        public async Task<IActionResult> SaveLenderDetails(LenderConfigurationRequest model)
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
                // Get Company Details
                var companyDetails = dataService.GetCompanyDetails(model.LenderCompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.LenderConfigurations_Invalid);
                    return BadRequest(ModelState);
                }

                // Save Lender Details
                lenderConfigurationService.SaveLenderDetails(model);

                // Get Lender Details
                var response = lenderConfigurationService.GetLenderDetails(model.LenderCompanyGuid);

                return Ok(response);
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
