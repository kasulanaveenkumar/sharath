using Common.Authorization.Authorize;
using Common.Messages;
using Common.Models.Users;
using Core.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Core.API.Helper;
using static Common.Models.Enums;
using Core.API.Models;
using Common.Validations.Helper;

namespace Core.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminService;
        private readonly IInspectionService inspectionService;
        private readonly IErrorService errorService;
        private readonly IViewRenderService viewRenderService;

        #region Constructor

        private readonly IHostingEnvironment _hostingEnvironment;
        public AdminController(IAdminService adminService, IInspectionService inspectionService,
                               IErrorService errorService, IViewRenderService viewRenderService,
                               IHostingEnvironment hostingEnvironment)
        {
            this.adminService = adminService;
            this.inspectionService = inspectionService;
            this.errorService = errorService;
            this.viewRenderService = viewRenderService;

            _hostingEnvironment = hostingEnvironment;
        }

        #endregion

        #region Get Inspections Filter

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and Simple SupportUser
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
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
                var response = adminService.GetInspectionsFilter();

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Review InspectionsList

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("getreviewinspectionslist")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReviewInspectionsList(AdminInspectionsRequest model)
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
                // Get Review Inspections List
                var response = adminService.GetReviewInspectionsList(model, currentUserDetails.UserGuid, "");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get ReverseTimer Update Status

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpGet]
        [Route("getreversetimerupdatestatus")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReverseTimerUpdateStatus()
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get ReverseTimer Update Status
                var response = adminService.GetReverseTimerUpdateStatus();

                return Ok(new { ReverseTinerUpdateStatus = response });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, null, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Suspend Inspection

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("suspendinspection")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SuspendInspection(SuspendInspectionRequest model)
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
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Suspend Inspection
                adminService.SuspendInspection(model.InspectionId, currentUserDetails.UserId);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Reactive Inspection

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("reactiveinspection")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ReactiveInspection(ReactiveInspectionRequest model)
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
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Reactive Inspection
                adminService.ReactiveInspection(model.InspectionId, currentUserDetails.UserId);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Purge Inspection

        /// <summary>
        /// This API accessed only by SupportTeamAdmin, Reviewer and SimpleSupportUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer,
                (int)UserRoles.SimpleSupportUser
            }
        )]
        [HttpPost]
        [Route("purgeinspection")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PurgeInspection(PurgeInspectionRequest model)
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
                // Get Inspection Details By Id
                var inspectionDetails = inspectionService.GetInspectionDetailsById(model.InspectionId);
                if (inspectionDetails == null)
                {
                    ModelState.AddModelError(CommonMessages.BadRequestKey, CoreMessages.Inspections_Error_InvalidId);
                    return BadRequest(ModelState);
                }

                // Purge Inspection
                adminService.PurgeInspection(model.InspectionId, currentUserDetails.UserId);

                return Ok(new { Message = CommonMessages.Success_Message });
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Completed Inspections List

        /// <summary>
        /// This API accessed only by SupportTeamAdmin and Reviewer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [RoleBasedAuthorization(
            new int[] {
                (int)UserTypes.SupportTeam
            },
            new int[] {
                (int)UserRoles.SupportTeamAdmin,
                (int)UserRoles.Reviewer
            }
        )]
        [HttpPost]
        [Route("getcompletedinspectionslist")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCompletedInspectionsList(AdminInspectionsRequest model)
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
                // Get Completed Inspections List
                var response = adminService.GetCompletedInspectionsList(model, currentUserDetails.UserGuid, "");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, model, currentUserDetails.UserGuid, currentUserDetails.CompanyGuid);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Export to Pdf

        [AllowAnonymous]
        [HttpGet]
        [Route("exporttopdf")]
        public IActionResult ExportToPDF()
        {
            try
            {
                //Initialize HTML to PDF converter 
                //HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
                //WebKitConverterSettings settings = new WebKitConverterSettings();
                //settings.Margin = new Syncfusion.Pdf.Graphics.PdfMargins() { All = 30 };

                //Set WebKit path
                //settings.WebKitPath = Path.Combine(_hostingEnvironment.ContentRootPath, "QtBinariesWindows");
                //Assign WebKit settings to HTML converter
                //htmlConverter.ConverterSettings = settings;

                //Cover Page
                //var result = viewRenderService.RenderToStringAsync("PDFTemplates\\index", null).Result;
                //var baseUrl = Path.Combine(_hostingEnvironment.ContentRootPath, "");
                //PdfDocument document = htmlConverter.Convert(result, baseUrl);

                //MemoryStream stream = new MemoryStream();
                //document.Save(stream);

                //PdfLoadedDocument lDocCoverPage = new PdfLoadedDocument(stream);
                //Identity 
                //var result1 = viewRenderService.RenderToStringAsync("PDFTemplates/coverpage", null).Result;
                //var baseUrl1 = Path.Combine(_hostingEnvironment.ContentRootPath, "Views/PDFTemplates");
                //PdfDocument document1 = htmlConverter.Convert(result, baseUrl); 
                //MemoryStream stream1 = new MemoryStream();
                //document1.Save(stream1);
                //PdfLoadedDocument lDocIdentity = new PdfLoadedDocument(stream);

                //PdfDocument finalDoc = new PdfDocument();
                //finalDoc.ImportPage(lDocCoverPage, 1);
                //finalDoc.ImportPage(lDocIdentity, 2);
                //finalDoc.Save(stream);

                //return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Output.pdf");


                //Sample
                //Initialize HTML to PDF converter
                //HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();

                //WebKitConverterSettings settings = new WebKitConverterSettings();

                //Set WebKit path
                //settings.WebKitPath = Path.Combine(_hostingEnvironment.ContentRootPath, "QtBinariesLinux");

                //Assign WebKit settings to HTML converter
                //htmlConverter.ConverterSettings = settings;

                //Convert URL to PDF
                //PdfDocument document = htmlConverter.Convert("http://www.google.com");

                //MemoryStream stream = new MemoryStream();

                //Save and close the PDF document
                //document.Save(stream);

                //return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Sample.pdf");

                return null;
            }
            catch (Exception ex)
            {
                var errorMsg = ExceptionHelper.GetExceptionMsg(errorService, ex, "", "", "");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = errorMsg });
            }
        }

        #endregion

        #region Get Companies Dashboard Datas

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
        [Route("getcompaniesdashboarddatas")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCompaniesDashboardDatas(GetCompanyDashboardDatasRequest model)
        {
            // Get Current User Details from Token
            var currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];

            try
            {
                // Get Companies Dashboard Datas
                var response = adminService.GetCompaniesDashboardDatas(model);

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
