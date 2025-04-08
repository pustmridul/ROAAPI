using MemApp.Application.Extensions;
using MemApp.Domain.Entities;

namespace MemApp.Application.Models.DTOs
{
    public class UserDetailsDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? EmailId { get; set; }
        
        public string? PhoneNo { get; set; }

        public string? CompanyName { get; set; }
        public string? TradeLicense { get; set; }
        public string? UserNID { get; set; }


        //public UserDTO(User user)
        //{
        //    Id = user.Id;
        //    Name = user.Name;
        //    EmailId = user.EmailId;      
        //}
    }

    public class UserLogReq
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LogDate { get; set; }
        public string LogText { get; set; }
    }
    public class UserLogVm : Result
    {
        public UserLogReq Date { get; set; }
    }
    public class UserLogListVm : Result
    {
        public long DataCount { get; set; }
        public List<UserLogReq> DataList { get; set; }= new List<UserLogReq>();
    }
}