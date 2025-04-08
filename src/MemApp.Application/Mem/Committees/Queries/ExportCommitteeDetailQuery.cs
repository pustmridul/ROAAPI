using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace MemApp.Application.Mem.Committees.Queries
{
    public class ExportCommitteeDetailQuery : IRequest<List<ExportCommitteeDetail>>
    {
        public int Id { get; set; }
        public string roothPath { get; set; } 
        public bool HasImage { get; set; }
    }


    public class ExportCommitteeDetailQueryHandler : IRequestHandler<ExportCommitteeDetailQuery, List<ExportCommitteeDetail>>
    {
        private readonly IMemDbContext _context;
      
        //private readonly IUserLogService _userLogService;
        //private readonly ICurrentUserService _currentUserService;
        public ExportCommitteeDetailQueryHandler(IMemDbContext context, ICurrentUserService currentUserService,
            IUserLogService userLogService)
        {
            _context = context;
            //_currentUserService = currentUserService;
            //_userLogService = userLogService;
        }

        public async Task<List<ExportCommitteeDetail>> Handle(ExportCommitteeDetailQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ExportCommitteeDetail>();
            var data = await _context.CommitteeDetails           
                .Where(q=>q.CommitteeId==request.Id).ToListAsync(cancellationToken);

            if (data==null)
            {
               
            }
            else
            {

              

                var a =  request.roothPath;

                //byte[] fileBytes = File.ReadAllBytes(file);
                //string base64Content = Convert.ToBase64String(fileBytes);

                result = data.Select(s=> new ExportCommitteeDetail
                {

                    CommitteeId= s.CommitteeId,
                    Id= s.Id,
                    MemberName= s.MemberName,
                    
                   // IsMasterMember= s.ImgFileUrl!=null? s.ImgFileUrl.Replace("Members/",""): "",
                    ImgFileUrl = request.HasImage? ConvertFileToBase64(request.roothPath, s.ImgFileUrl != null ? s.ImgFileUrl.Replace("Members/", "") : "") : "",

                    Phone =s.Phone ?? "",
                    Email=s.Email ?? "",
                    Designation=s.Designation,
                    MemberShipNo=s.MemberShipNo ?? "",
                    IsMasterMember=s.IsMasterMember == true? "Member" : "Not Member" ,
                    HasImage= request.HasImage
                }).ToList();
            }

            return result;
        }

        private string ConvertFileToBase64(string roothPath, string fileName)
        {
            var files = Directory.GetFiles(Path.Combine(roothPath, "Members")).ToList();

             var file = files.FirstOrDefault(q=>q.Contains(fileName));
            if (file == null)
            {
                return "";
            }
            else
            {
                var filebytes = File.ReadAllBytes(file);

                return Convert.ToBase64String(filebytes);
            }
            
        }
    }
}
