using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Authorization.Authorize;
using Core.API.Services;
using System.Net;
using Common.Models.Users;
using static Common.Models.Enums;
using Core.API.Helper;
using Core.API.Models;
using Common.Validations.Helper;
using Common.Messages;

namespace Core.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService notificationsService;
        private readonly IErrorService errorService;

        #region Constructor

        public NotificationsController(INotificationsService notificationsService, IErrorService errorService)
        {
            this.notificationsService = notificationsService;
            this.errorService = errorService;
        }

        #endregion

        #region Get NotificationMappings

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
        [Route("getnotificationmappings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificationMappings()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get NotificationsMappings
                var responses = notificationsService.GetNotificationsMappings(currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                if (responses != null &&
                    responses.Count() > 0)
                {
                    return Ok(responses);
                }

                return Ok("");
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save NotificationMappings

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
        [Route("savenotificationmappings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveNotificationMappings(List<NotificationsRequest> model)
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
                var invalidNotificationGuids = notificationsService.GetInvalidNotificationGuids(model);
                if (!string.IsNullOrEmpty(invalidNotificationGuids))
                {
                    var errorMsg = "NotificationGuid(s) " + invalidNotificationGuids + " invalid in request";
                    ModelState.AddModelError(CommonMessages.BadRequestKey, errorMsg);
                    return BadRequest(ModelState);
                }

                // Save NotificationsMappings
                notificationsService.SaveNotificationMappings(model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                // Get NotificationsMappings
                var responses = notificationsService.GetNotificationsMappings(currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                return Ok(responses);
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
