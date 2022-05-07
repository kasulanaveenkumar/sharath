using Common.Authorization.Authorize;
using Common.Messages;
using Common.Models.Users;
using Common.Payments.Models;
using Common.Validations.Helper;
using Config.API.Helper;
using Config.API.Models.Payment;
using Config.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Common.Models.Enums;

namespace Config.API.Controllers.Payment
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ICompanyService companyService;
        private readonly IErrorService errorService;

        #region Constructor

        public PaymentController(ICompanyService service, IErrorService errorService)
        {
            this.companyService = service;
            this.errorService = errorService;
        }

        #endregion

        #region Get CreditCard Type

        /// <summary>
        /// This API accessed only by BillingResponsible User
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.BillingResponsible
            }
        )]
        [HttpGet]
        [Route("getcreditcardtype/{cardnumber}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCreditCardType(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.GetCreditCard_Error_CardNumber);
                return BadRequest(ModelState);
            }

            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get CreditCard Type
                var response = companyService.GetCreditCardType(cardNumber);
                if (string.IsNullOrEmpty(response))
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.PaymentDetails_Error_InvalidCardNumber);
                    return BadRequest(ModelState);
                }

                return Ok(new { Brand = response.ToLower() });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Save Card Details

        /// <summary>
        /// This API accessed only by BillingResponsible User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.Broker
            },
            new int[] {
                (int)UserRoles.BillingResponsible
            }
        )]
        [HttpPost]
        [Route("savecarddetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.PartialContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveCardDetails(List<PaymentMethod> model)
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
                // Get Company Details By Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Save Card Details
                var result = companyService.SaveCardDetails(companyDetails, model, currentUserDetails.UserId);
                if (string.IsNullOrEmpty(result))
                {
                    return Ok(new { Message = CommonMessages.Success_Message });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PartialContent, result);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Card Details

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
        [Route("getcarddetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCardDetails()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Company Details By Guid
                var companyDetails = companyService.GetCompanyDetails(currentUserDetails.CompanyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get Card Details
                var responses = companyService.GetCardDetails(companyDetails);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Validate Card

        /// <summary>
        /// This API accessed only on Broker Onboarding and Broker PaymentDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.All,
                (int)UserTypes.Broker
            },
            new int[] {
                (int) UserRoles.Onboarding,
                (int) UserRoles.BillingResponsible
            }
        )]
        [HttpPost]
        [Route("validatecard")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ValidateCard(CardDetails model)
        {
            // Check Card Details Data
            if (model == null)
            {
                ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.CardDetails_Required);
                return BadRequest(ModelState);
            }

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
                // Validate Card
                var response = companyService.ValidateCard(model);

                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    // Return invalid carddetails.
                    return BadRequest(new { Message = response.ErrorMessage });
                }

                return Ok(new { Message = "Success" });
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
