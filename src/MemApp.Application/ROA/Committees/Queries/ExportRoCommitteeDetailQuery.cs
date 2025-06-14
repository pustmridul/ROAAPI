﻿using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ResApp.Application.ROA.Committees.Models;

namespace ResApp.Application.ROA.Committees.Queries
{
    public class ExportRoCommitteeDetailQuery : IRequest<List<ExportRoCommitteeDetail>>
    {
        public int Id { get; set; }
        public string? RoothPath { get; set; } 
        public bool HasImage { get; set; }
    }


    public class ExportRoCommitteeDetailQueryHandler : IRequestHandler<ExportRoCommitteeDetailQuery, List<ExportRoCommitteeDetail>>
    {
        private readonly IMemDbContext _context;
      
        //private readonly IUserLogService _userLogService;
        //private readonly ICurrentUserService _currentUserService;
        public ExportRoCommitteeDetailQueryHandler(IMemDbContext context, ICurrentUserService currentUserService,
            IUserLogService userLogService)
        {
            _context = context;
            //_currentUserService = currentUserService;
            //_userLogService = userLogService;
        }

        public async Task<List<ExportRoCommitteeDetail>> Handle(ExportRoCommitteeDetailQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ExportRoCommitteeDetail>();
            var data = await _context.RoCommitteeDetails           
                .Where(q=>q.CommitteeId==request.Id).ToListAsync(cancellationToken);

            if (data==null)
            {
               
            }
            else
            {

              

                

                //byte[] fileBytes = File.ReadAllBytes(file);
                //string base64Content = Convert.ToBase64String(fileBytes);

                result = data.Select(s=> new ExportRoCommitteeDetail
                {

                    CommitteeId= s.CommitteeId,
                    Id= s.Id,
                    MemberName= s.MemberName,
                    
                   // IsMasterMember= s.ImgFileUrl!=null? s.ImgFileUrl.Replace("Members/",""): "",
                    ImgFileUrl = request.HasImage? ConvertFileToBase64(request.RoothPath!, s.ImgFileUrl != null ? s.ImgFileUrl.Replace("uploadsMembers/", "") : "") : "",

                    Phone =s.Phone ?? "",
                    Email=s.Email ?? "",
                    Designation=s.Designation,
                    MembershipNo=s.MembershipNo ?? "",
                 
                    HasImage= request.HasImage
                }).ToList();
            }

            return result;
        }

        private string ConvertFileToBase64(string roothPath, string fileName)
        {
            var files = Directory.GetFiles(Path.Combine(roothPath, "uploadsMembers")).ToList();

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
