using MemApp.Application.Interfaces;
using MemApp.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace MemApp.WebApi.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        #region Property
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public FileController(IFileService fileService, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Upload
        [HttpPost(nameof(UploadMemberFile))]
        public async Task<IActionResult> UploadMemberFile(IFormFile formFile, string subDirectory, string title, int memberId)
        {
            try
            {
                var model = new FileUploadModel();
                model.Title = title;
                model.SubDirectory = subDirectory;
                model.WebRootPath = _webHostEnvironment.WebRootPath;
                model.MemberId = memberId;
                model.file = formFile;
                await _fileService.UploadFile(model);

                return Ok(new { formFile, Size = _fileService.SizeConverter(formFile.Length) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Download File
        [HttpGet(nameof(Download))]
        public IActionResult Download([Required] string subDirectory)
        {

            try
            {
                var (fileType, archiveData, archiveName) = _fileService.DownloadFiles(subDirectory);

                return File(archiveData, fileType, archiveName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        #endregion

        #region Process File
        [HttpGet(nameof(ProcessFile))]
        public async Task<IActionResult> ProcessFile(string? startFileNo, string? endFileNo, [Required] string subDirectory)
        {

            try
            {
                var a = await _fileService.ProcessFile(startFileNo, endFileNo, subDirectory, _webHostEnvironment.WebRootPath);
                return Ok(a);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet(nameof(GenerateFile))]
        public async Task<IActionResult> GenerateFile( [Required] string subDirectory)
        {
            try
            {
                var a = await _fileService.GenerateFile(subDirectory, _webHostEnvironment.WebRootPath);
                return Ok(a);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region BoardMeetingMinuet
        [HttpPost(nameof(UploadBoardMeetingMinuet))]
        public async Task<IActionResult> UploadBoardMeetingMinuet([FromForm] BoardMeetingModel model)
        {
            try
            {   
                await _fileService.UploadBoardMeetingMinuet(model, _webHostEnvironment.WebRootPath);

                return Ok(new { model.file, Size = _fileService.SizeConverter(model.file.Length) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        #region UpdateMemberPicture
        [HttpPost(nameof(UpdateMemberPicture))]
        public async Task<IActionResult> UpdateMemberPicture([FromForm] MemberProfilePictureUpload model)
        {
            if(model.file == null)
            {
                return BadRequest();
            }


                return Ok(await _fileService.UpdateMemberProfilePicture(model, _webHostEnvironment.WebRootPath));
          
        }
        #endregion
       
        #region UploadTicketImage
        [HttpPost(nameof(UploadTicketImage))]
        public async Task<IActionResult> UploadTicketImage([FromForm] UploadTicketModel model)
        {
            if (model.file == null)
            {
                return BadRequest();
            }

            try
            {
                await _fileService.UploadTicketImage(model, _webHostEnvironment.WebRootPath);

                return Ok(new { model.file, Size = _fileService.SizeConverter(model.file.Length) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

    }
}
