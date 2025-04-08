using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models
{
    public class FileModel
    {
    }
    public class FileUploadModel
    {
        public string Title { get; set; }
        public int MemberId { get; set; }
        public string SubDirectory { get; set; }
        public string WebRootPath { get; set; }
        public IFormFile file { get; set; }
    }
    public class BoardMeetingModel
    {
        public string Title { get; set; }
        public DateTime MeetingDate { get; set; }
        public string SubDirectory { get; set; }
        public string? Note { get; set; }
        public int? CommitteeId { get; set; }
        public string? CommitteeTitle { get; set; }
        public IFormFile file { get; set; } 
    }

    public class MemberProfilePictureUpload
    {
        public int MemberId { get; set; }
      
        public IFormFile file { get; set; }
    }
    public class UploadTicketModel
    {
        public int TicketId { get; set; }
        public string SubDirectory { get; set; }
        public IFormFile file { get; set; }
    }
    public class DonationUploadModel
    {
        public int DonationId { get; set; }
        public string SubDirectory { get; set; } = "Donation";
        public IFormFile file { get; set; }
    }
}

