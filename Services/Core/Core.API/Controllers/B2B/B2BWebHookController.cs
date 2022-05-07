using Common.Messages;
using Core.API.Helper;
using Core.API.Models.B2B;
using Core.API.Services;
using Core.API.Services.B2B;
using Core.API.Swagger.B2B;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Core.API.Controllers.B2B
{
    [TypeFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("apiB2B/v1/webhook")]
    public class B2BWebHookController : ControllerBase
    {
        private readonly IB2BService b2bService;
        private readonly IDataService dataService;
        private readonly IErrorService errorService;

        #region Constructor

        public B2BWebHookController(IB2BService b2bService, IDataService dataService, IErrorService errorService)
        {
            this.b2bService = b2bService;
            this.dataService = dataService;
            this.errorService = errorService;
        }

        #endregion

        #region Subscribe
        /// <summary>
        /// Subsribe web hook
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Subscribe(WebHookSubscribe model)
        {
            // Get CompanyGuid from Token
            var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

            try
            {
                // Get Company Details By Guid
                var companyDetails = dataService.GetCompanyDetails(companyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Subscribe WebHooks 
                b2bService.SubscribeWebHooks(model, companyGuid);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, null, companyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Unsubscribe
        /// <summary>
        /// Unsubsribe a web hook
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Unsubscribe(WebHookUnsubscribe model)
        {
            // Get CompanyGuid from Token
            var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

            try
            {
                // Get Company Details By Guid
                var companyDetails = dataService.GetCompanyDetails(companyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get WebHook By Id
                var webHook = b2bService.GetWebHookById(model.Id);
                if (webHook == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.B2BWebHook_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Unsubscribe WebHooks
                b2bService.UnsubscribeWebHooks(webHook);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, null, companyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region Unsubscribe All
        /// <summary>
        /// Unsubscribe all registered web hooks
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("all")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UnsubscribeAll()
        {
            // Get CompanyGuid from Token
            var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

            try
            {
                // Get Company Details By Guid
                var companyDetails = dataService.GetCompanyDetails(companyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get WebHooks By Company
                var allWebHooks = b2bService.GetWebHooksByCompany(companyGuid);
                if (allWebHooks == null ||
                    allWebHooks.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.B2BWebHook_Error_WebHooksNotAvailable);
                    return BadRequest(ModelState);
                }

                // Unsubscribe All WebHooks
                b2bService.UnsubscribeAllWebHooks(allWebHooks);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, null, companyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion

        #region List of Subscribed Webhooks
        /// <summary>
        /// Get list of all registered web hooks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetList()
        {
            // Get CompanyGuid from Token
            var companyGuid = Convert.ToString(HttpContext.Items["CompanyGuid"]);

            try
            {
                // Get Company Details By Guid
                var companyDetails = dataService.GetCompanyDetails(companyGuid);
                if (companyDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, ConfigMessages.Company_Invalid);
                    return BadRequest(ModelState);
                }

                // Get All WebHooks By Company
                var allWebHooks = b2bService.GetAllWebBooksByCompany(companyGuid);
                if (allWebHooks == null ||
                    allWebHooks.Count() == 0)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.B2BWebHook_Error_WebHooksNotAvailable);
                    return BadRequest(ModelState);
                }

                return Ok(allWebHooks);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, null, companyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }
        #endregion
    }
}
