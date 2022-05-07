using Common.Authorization.Authorize;
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
    public class LenderController : ControllerBase
    {
        private readonly ILenderService lenderService;
        private readonly IErrorService errorService;

        #region Constructor

        public LenderController(ILenderService lenderService, IErrorService errorService)
        {
            this.lenderService = lenderService;
            this.errorService = errorService;
        }

        #endregion

        #region Get Inspections Filter

        /// <summary>
        /// This API accessed only by Lender and their supported roles
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
           new int[] {
               (int)UserTypes.Lender
           },
           new int[] {
               (int)UserRoles.AllUserRoles
           }
        )]
        [HttpGet]
        [Route("getinspectionsfilter")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInspectionsFilter()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Inspections Filter
                var response = lenderService.GetInspectionsFilter();

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, currentUserDetails.CompanyGuid, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get InspectionsList

        /// <summary>
        /// This API accessed only by Lender and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
           new int[] {
                (int)UserTypes.Lender
           },
           new int[] {
                (int)UserRoles.AllUserRoles
           }
        )]
        [HttpPost]
        [Route("getinspectionslist")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInspectionsList(LenderInspectionsRequest model)
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
                // Get Inspections List
                var response = lenderService.GetInspectionsList(model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

                return Ok(response);
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
        /// This API accessed only by Lender and their supported roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
           new int[] {
                (int)UserTypes.Lender
           },
           new int[] {
                (int)UserRoles.AllUserRoles
           }
        )]
        [HttpPost]
        [Route("getcompletedinspectionslist")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCompletedInspectionsList(LenderInspectionsRequest model)
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
                // Get Completed Inspections List
                var response = lenderService.GetCompletedInspectionsList(model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);

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
