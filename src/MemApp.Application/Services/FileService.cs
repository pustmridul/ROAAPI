using AutoMapper.Execution;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.IO.Compression;

namespace MemApp.Application.Services
{     
    public class FileService : IFileService
    {
        #region Property
        private IHostingEnvironment _hostingEnvironment;
        private IMemDbContext _context;
      //  private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        #endregion

        #region Constructor
        public FileService(IHostingEnvironment hostingEnvironment, IMemDbContext context, IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        #endregion

        #region Upload File
        public async Task<bool> UploadFile(FileUploadModel model)
        {
            model.SubDirectory = model.SubDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, model.SubDirectory);

            Directory.CreateDirectory(target);
            List<MemFiles> memFiles = new List<MemFiles>();

                var file = model.file;         

                var filePath = Path.Combine(target, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileName = file.FileName;
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string base64Content = Convert.ToBase64String(fileBytes);
                string memorySize = GetMemorySizeString(fileBytes.Length);
              
                string fileExtension = Path.GetExtension(file.FileName).TrimStart('.');
                var memFileObj= await _context.MemFiles.FirstOrDefaultAsync(q=>q.MemberId == model.MemberId 
                                && q.FileName==fileName 
                                && q.FileType== fileExtension                                  
                                );
               
                if (memFileObj == null)
                {
                    memFileObj = new MemFiles()
                    {
                        MemberId = model.MemberId,
                        FileContent = base64Content,
                        FileSize = memorySize,
                        FileName = fileName,
                        FileType=fileExtension,
                        IsActive=true,
                        Titile=model.Title,
                        FileUrl= filePath,

                    };

                    memFiles.Add(memFileObj);
                }

                using (var stream = new MemoryStream(fileBytes))
                {
                    memFileObj.MemImage = stream.ToArray();
                }


            _context.MemFiles.AddRange(memFiles);

            if (await _context.SaveAsync() > 0)
            {
                return true;
            }
            else { return false; }
        
            }
        #endregion


        #region Download File
        public async Task<bool> GenerateFile(string subDirectory, string webRootPath)
        {
            var dataList = await _context.MemFiles.Where(q => q.IsActive).ToListAsync();
            foreach(var member in dataList)
            {
                var content = new System.IO.MemoryStream(member.MemImage);

                string fileName = member.FileUrl.Replace("Members/", "");
                var nextPath = Path.Combine(webRootPath, "Members1", fileName);
                await CopyStream(content, nextPath);
            }
            

            return false;
        }

        public  async Task<bool> ProcessFile(string? startFileNo, string? endFileNo, string subDirectory, string webRootPath    )
        {
            string FName = "";
            var files = Directory.GetFiles(Path.Combine(webRootPath, subDirectory)).ToList();
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;

            var dalaList = await  _context.RegisterMembers.Select(s=> new {s.MembershipNo, s.CardNo, s.FullName, s.Id}).ToListAsync();

            List<MemFiles> fileList = new List<MemFiles>();

            files.ForEach(async file =>
            {
                string fileFullName = Path.GetFileName(file);
                string fileName = Path.GetFileNameWithoutExtension(file);
                byte[] fileBytes = File.ReadAllBytes(file);
                string base64Content = Convert.ToBase64String(fileBytes);

                string memorySize = GetMemorySizeString(fileBytes.Length);

                fileName = fileName.PadLeft(5, '0');
               

                var member = dalaList.FirstOrDefault(q => q.MembershipNo == fileName);
                if(member != null)
                {
                    string fileExtension = Path.GetExtension(file).TrimStart('.');
                    FName = "Members/" + member.Id.ToString() + "-" + member.MembershipNo + "." + fileExtension;
                    var memFile = new MemFiles()
                    {
                        Titile = member.FullName,                  
                        FileContent =fileBytes.ToString(),
                        FileName = fileName,
                        FileUrl = "Members/" + member.Id.ToString() + "-" + member.MembershipNo + "." + fileExtension,
                        FileSize = memorySize,
                        MemberId = member.Id,
                        FileType = fileExtension,
                        IsActive = true,
                       
                    };
                    var content = new System.IO.MemoryStream(fileBytes);

                    var nextPath= Path.Combine(webRootPath, "Members", member.Id.ToString() +"-"+ member.MembershipNo+"."+ fileExtension);
                    await CopyStream(content, nextPath);
                  
                    using (var stream = new MemoryStream(fileBytes))
                    {
                      //  fileBytes.CopyTo(stream);
                        memFile.MemImage = stream.ToArray();
                    }
                    fileList.Add(memFile);
                }
               
                
            });
            _context.MemFiles.AddRange(fileList);
            await _context.SaveAsync();
            return true;
        }

        public async Task CopyStream(Stream stream, string downloadPath)
        {
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
        public (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory)
            {
                var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

                var files = Directory.GetFiles(Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory)).ToList();

                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        files.ForEach(file =>
                        {
                            var theFile = archive.CreateEntry(file);
                            using (var streamWriter = new StreamWriter(theFile.Open()))
                            {
                                streamWriter.Write(File.ReadAllText(file));
                            }

                        });
                    }

                    return ("application/zip", memoryStream.ToArray(), zipName);
                }
            }


        #endregion
        #region UploadBoardMeetingMinuet
        public async Task<bool> UploadBoardMeetingMinuet(BoardMeetingModel model, string webRootPath)
        {

            if (!await _permissionHandler.HasRolePermissionAsync(3501) || !await _permissionHandler.HasRolePermissionAsync(3502))
            {
                return false;
            }
            //BoardMeeting
            model.SubDirectory = model.SubDirectory ?? string.Empty;
          
            var target = Path.Combine(webRootPath, model.SubDirectory);

            Directory.CreateDirectory(target);
           

            var file = model.file;

            var filePath = Path.Combine(target, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string fileName = file.FileName;
            byte[] fileBytes = File.ReadAllBytes(filePath);
          //  string base64Content = Convert.ToBase64String(fileBytes);
            string memorySize = GetMemorySizeString(fileBytes.Length);

            string fileExtension = Path.GetExtension(file.FileName).TrimStart('.');

            var memFileObj = new BoardMeetingMinuet()
            {
                FileName = fileName,
                FileType = fileExtension,        
                IsActive = true,
                Title = model.Title,
                MeetingDate = model.MeetingDate,
                Note= model.Note,   
                FileSize= memorySize,
                FileUrl =  model.SubDirectory + "/" + fileName,
            };
            memFileObj.CommitteeId = model.CommitteeId;
            memFileObj.CommitteeTitle= model.CommitteeTitle;


            _context.BoardMeetingMinuets.Add(memFileObj);

            using (var stream = new MemoryStream(fileBytes))
            {
                //  fileBytes.CopyTo(stream);
                memFileObj.FileContent = stream.ToArray();
            }

            if(await _context.SaveAsync() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
        #region Size Converter
        public string SizeConverter(long bytes)
            {
                var fileSize = new decimal(bytes);
                var kilobyte = new decimal(1024);
                var megabyte = new decimal(1024 * 1024);
                var gigabyte = new decimal(1024 * 1024 * 1024);

                switch (fileSize)
                {
                    case var _ when fileSize < kilobyte:
                        return $"Less then 1KB";
                    case var _ when fileSize < megabyte:
                        return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                    case var _ when fileSize < gigabyte:
                        return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                    case var _ when fileSize >= gigabyte:
                        return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                    default:
                        return "n/a";
                }
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
        #endregion
   


        #region UpdateMemberProfilePicture
        public async Task<bool> UpdateMemberProfilePicture(MemberProfilePictureUpload model, string webRootPath)
        {
            if(!await _permissionHandler.HasRolePermissionAsync(2301) || !await _permissionHandler.HasRolePermissionAsync(2302))
            {
                return false;
            }


            var file = model.file;

            string fileExtension = Path.GetExtension(file.FileName).TrimStart('.');

           // model.SubDirectory = model.SubDirectory ?? string.Empty;

            var memObj = await _context.RegisterMembers.SingleOrDefaultAsync(q => q.Id == model.MemberId);

            if(memObj == null)
            {
                return false;
            }
            var fileName = memObj.Id.ToString() + "-" + memObj.MembershipNo + "." + fileExtension;
            
            var filePath = Path.Combine(webRootPath , "Members", fileName);

            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
           
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string memorySize = GetMemorySizeString(fileBytes.Length);


            memObj.ImgFileUrl = "Members/" + fileName;
            
            var memFile = await _context.MemFiles.SingleOrDefaultAsync(s=>s.MemberId == model.MemberId && s.FileUrl == memObj.ImgFileUrl);

            if (memFile== null)
            {
                memFile = new MemFiles()
                {
                    Titile = memObj.FullName,                
                    FileName = fileName,
                    FileUrl = "Members/" + memObj.Id.ToString() + "-" + memObj.MembershipNo + "." + fileExtension,
                    MemberId = memObj.Id,
                    FileType = fileExtension,
                    IsActive = true,

                };
                _context.MemFiles.Add(memFile);
            }
            memFile.FileContent=fileBytes.ToString();
            memFile.FileSize = memorySize;

            using (var stream = new MemoryStream(fileBytes))
            {               
                memFile.MemImage = stream.ToArray();
            }
            if(await _context.SaveAsync() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
        public async Task<bool> UploadDonationFile(DonationUploadModel model, string webRootPath)
        {
            var file = model.file;

            string fileExtension = Path.GetExtension(file.FileName).TrimStart('.');


            var target = Path.Combine(webRootPath, model.SubDirectory);

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            

            model.SubDirectory = model.SubDirectory ?? string.Empty;

            var obj = await _context.Donations.SingleOrDefaultAsync(q => q.Id == model.DonationId && q.IsActive);

            if (obj == null)
            {
                return false;
            }
            var fileName =  obj.Id + "." + fileExtension;
            var filePath = Path.Combine(webRootPath, model.SubDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            byte[] fileBytes = File.ReadAllBytes(filePath);
            string memorySize = GetMemorySizeString(fileBytes.Length);


            obj.FileUrl = model.SubDirectory + "/" + fileName;

 
            if (await _context.SaveAsync() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        #region UploadTicketImage
        public async Task<bool> UploadTicketImage(UploadTicketModel model, string webRootPath)
        {
            var file = model.file;

            string fileExtension = Path.GetExtension(file.FileName).TrimStart('.');

            model.SubDirectory = model.SubDirectory ?? string.Empty;

            var obj = await _context.ServiceTickets.SingleOrDefaultAsync(q => q.Id == Convert.ToInt32(model.TicketId) && q.IsActive);

            if (obj == null)
            {
                return false;
            }
            var fileName = obj.Id.ToString() + "-" + obj.MemServiceId + "-" + obj.MemServiceTypeId + "-" + obj.Title + "." + fileExtension;
            var filePath = Path.Combine(webRootPath, model.SubDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
          //  ResizeAndSaveImage(model.file, filePath);
        

            byte[] fileBytes = File.ReadAllBytes(filePath);
            string memorySize = GetMemorySizeString(fileBytes.Length);


            obj.ImgFileUrl = model.SubDirectory + "/" + fileName;

            var tocketFile = await _context.TicketFiless.SingleOrDefaultAsync(s => s.TicketId == model.TicketId
            && s.FileUrl == obj.ImgFileUrl
            && s.IsActive);

            if (tocketFile == null)
            {
                tocketFile = new TicketFiles()
                {
                    Titile = obj.Title,
                    FileName = fileName,
                    FileUrl = obj.ImgFileUrl,
                    TicketId = obj.Id,
                    FileType = fileExtension,
                    IsActive = true,

                };
                _context.TicketFiless.Add(tocketFile);
            }
            tocketFile.FileContent = fileBytes.ToString();
            tocketFile.FileSize = memorySize;


            using (var stream = new MemoryStream(fileBytes))
            {
                tocketFile.TicketImage = stream.ToArray();
            }
            if(await _context.SaveAsync() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

            
        }

        //private void ResizeAndSaveImage(IFormFile imageFile, string filepath)
        //{
        //    // Resize the image to a target size (e.g., 2MB)
        //    var targetSizeBytes = 4 * 1024 ; // 2MB
        //    using (var image = Image.Load(imageFile.OpenReadStream()))
        //    {
        //        //image.Mutate(x => x
        //        //    .Resize(new ResizeOptions
        //        //    {
        //        //        Size = CalculateTargetSize(image, imageFile.Length, targetSizeBytes)
        //        //    }));

        //        // Save the resized image to a file directory

        //        var filePath = filepath;
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            image.Save(stream, new JpegEncoder());
        //        }
        //    }

        //}
        //private Size CalculateTargetSize(Image image, long originalSizeBytes, long targetSizeBytes)
        //{
        //    // Calculate the target size based on the original and desired size
        //    var scaleFactor = Math.Sqrt((double)originalSizeBytes / targetSizeBytes);
        //    var width = (int)(image.Width / scaleFactor);
        //    var height = (int)(image.Height / scaleFactor);

        //    return new Size(width, height);
        //}
        #endregion
    }

}
