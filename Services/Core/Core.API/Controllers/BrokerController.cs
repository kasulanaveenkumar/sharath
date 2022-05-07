using Common.Authorization.Authorize;
using Common.Messages;
using Common.Models.Users;
using Common.Validations.Helper;
using Core.API.Helper;
using Core.API.Models;
using Core.API.Services;
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
    public class BrokerController : ControllerBase
    {
        private readonly IInspectionService inspectionService;
        private readonly IErrorService errorService;

        #region Constructor

        public BrokerController(IInspectionService inspectionService, IErrorService errorService)
        {
            this.inspectionService = inspectionService;
            this.errorService = errorService;
        }

        #endregion

        #region Get IllionDetails

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
        [Route("getilliondetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetIllionDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Illion Details
                var response = inspectionService.GetIllionDetails(currentUserDetails.CompanyGuid);

                return Ok(new { IllionDetails = response });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Update IllionDetails

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
        [Route("updateilliondetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateIllionDetails(UpdateIllionDetailsRequest model)
        {
            if (!model.IsActive)
            {
                ValidationHelper.RemoveModelError(ModelState, "ReferralCode");
            }

            if (!ModelState.IsValid)
            {
                var result = ValidationHelper.GetModelValidationResult(ModelState);
                return UnprocessableEntity(result);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Save Illion Details
                inspectionService.SaveIllionDetails(currentUserDetails.CompanyGuid, model);

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
