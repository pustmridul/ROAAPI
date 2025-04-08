using MemApp.Application.Models;

namespace MemApp.Application.Interfaces
{
    public interface IFileService
    {
        Task<bool> UploadFile(FileUploadModel model);
        Task<bool> UploadBoardMeetingMinuet(BoardMeetingModel model, string webRootPath);

        (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory);
        string SizeConverter(long bytes);
        Task<bool> ProcessFile(string? startFileNo, string? endFileNo, string subDirectory, string webRootPath);
        Task<bool> GenerateFile(string subDirectory, string webRootPath);

        Task<bool> UpdateMemberProfilePicture(MemberProfilePictureUpload model, string webRootPath);
        Task<bool> UploadTicketImage(UploadTicketModel model, string webRootPath);
        Task<bool> UploadDonationFile(DonationUploadModel model, string webRootPath);
    }
}
