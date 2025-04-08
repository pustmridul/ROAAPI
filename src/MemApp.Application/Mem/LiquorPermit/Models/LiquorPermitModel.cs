using MemApp.Application.Extensions;
using Microsoft.AspNetCore.Http;


namespace MemApp.Application.Mem.Communication.Models
{
    public class LiquorPermitModelReq
    {
        public int? Id { get; set; }
        public int MemberId { get; set; }
        public string? Title { get; set; }

  
        public string? FileUrl { get; set; }
        public IFormFile? File { get; set; }
    }



    public class LiquorPermitVm : Result
    {
        public LiquorPermitModelReq Data { get; set; } = new LiquorPermitModelReq();
    }
    public class LiquorPermitListVm : ListResult<LiquorPermitModelReq>
    {
    }
}
