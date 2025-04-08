using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Domain.Entities.Communication;
using MemApp.Domain.Entities.mem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Services
{
    public class FileSaveService : IFileSaveService
    {

        private IHostingEnvironment _hostingEnvironment;
        private IMemDbContext _context;
        //  private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;



        public FileSaveService(IHostingEnvironment hostingEnvironment, IMemDbContext context, IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }




        public async Task<bool> UploadFile(FileSaveModel model)
        {
            model.SubDirectory = model.SubDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, model.SubDirectory);

            

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            FileInformation fileInfo = new FileInformation();

            var file = model.file;

            string fileName = file.FileName;
            string fileExtension = Path.GetExtension(file.FileName).TrimStart('.');
            var existingFile = await _context.FileInformations.Where(q => q.OperationTypeId==model.OperationTypeId && q.OperationId==model.OperationId).AsNoTracking().FirstOrDefaultAsync();
            var isForUpdate = false;
            if (existingFile != null)
            {
                isForUpdate = true;
            }
            fileName = model.OperationTypeId+"-"+model.OperationId +"-"+ file.FileName;
            var filePath = Path.Combine(target, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64Content = Convert.ToBase64String(fileBytes);
            string memorySize = GetMemorySizeString(fileBytes.Length);

            var fileToSave = new FileInformation();
            
            fileToSave.FileContent = base64Content;
            fileToSave.FileSize = memorySize;
            fileToSave.FileName = fileName;
            fileToSave.FileType = fileExtension;
            fileToSave.IsActive = true;
            fileToSave.FileUrl = filePath;
            fileToSave.OperationTypeId = model.OperationTypeId;
            fileToSave.OperationId = model.OperationId;


            if (isForUpdate)
            {
                try
                {
                    if (System.IO.File.Exists(existingFile?.FileUrl))
                    {
                        System.IO.File.Delete(existingFile.FileUrl);
                    }
                    fileToSave.Id = existingFile.Id;
                    _context.FileInformations.Update(fileToSave);
                }
                catch(Exception ex)
                {
                    throw new Exception( ex.Message);
                }
            }
            else
            {
                
                _context.FileInformations.Add(fileToSave);
            }

            using (var stream = new MemoryStream(fileBytes))
            {
                fileInfo.File = stream.ToArray();
            }


            

            if (await _context.SaveAsync() > 0)
            {
                return true;
            }
            else { return false; }

        }



        private string GetMemorySizeString(long byteCount)
        {
            string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double size = byteCount;

            while (size >= 1024 && suffixIndex < sizeSuffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }

            return $"{size:0.##} {sizeSuffixes[suffixIndex]}";
        }


    }
}
