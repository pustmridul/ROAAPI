using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Committees.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.ROA.Committees.Models;

namespace ResApp.Application.ROA.Committees.Queries
{
   
    public class ExportRoCommitteeInfoQuery : IRequest<ExportRoCommitteeInfo>
    {
        public int Id { get; set; }
    }


    public class ExportRoCommitteeInfoQueryHandler : IRequestHandler<ExportRoCommitteeInfoQuery, ExportRoCommitteeInfo>
    {
        private readonly IMemDbContext _context;
        //private readonly IUserLogService _userLogService;
        //private readonly ICurrentUserService _currentUserService;
        public ExportRoCommitteeInfoQueryHandler(IMemDbContext context, ICurrentUserService currentUserService, IUserLogService userLogService)
        {
            _context = context;
            //_currentUserService = currentUserService;
            //_userLogService = userLogService;
        }

        public async Task<ExportRoCommitteeInfo> Handle(ExportRoCommitteeInfoQuery request, CancellationToken cancellationToken)
        {
            var result = new ExportRoCommitteeInfo();
            var data = await _context.RoCommittees.Include(x=>x.CommitteeCategory)
                .FirstOrDefaultAsync(q => q.Id == request.Id && q.IsActive, cancellationToken);

            var committeeCategory = await _context.RoCommitteeCategories.ToListAsync(cancellationToken);

            if (data == null)
            {

            }
            else
            {
                result.Title = data.Title;
                result.CommitteeType = data.CommitteeType;
                result.CommitteeYear = data.CommitteeYear;

                result.CommitteeDate = data.CommitteeDate.ToString("yyyy-mm-dd");
                result.CommitteeCategoryName = data.CommitteeCategory?.Title!;
                result.DivisionName = data.DivisionId != null ? _context.Divisions.FirstOrDefault(x => x.Id == data.DivisionId)?.EnglishName : "";
                result.DistrictName = data.DistrictId != null ? _context.Districts.FirstOrDefault(x => x.Id == data.DistrictId)?.EnglishName : "";
                result.ZoneName = data.Zone != null ? _context.ZoneInfos.FirstOrDefault(x => x.Id == data.ZoneId)?.EnglishName : "";
                result.ThanaName = data.ThanaId != null ? _context.Thanas.FirstOrDefault(x => x.Id == data.ThanaId)?.EnglishName : "";
                result.MunicipalityName = data.MunicipalityId != null ? _context.Municipalities.FirstOrDefault(x => x.Id == data.MunicipalityId)?.EnglishName : "";
                result.UnionName = data.UnionInfoId != null ? _context.UnionInfos.FirstOrDefault(x => x.Id == data.UnionInfoId)?.EnglishName : "";
                result.WardName = data.WardId != null ? _context.Wards.FirstOrDefault(x => x.Id == data.WardId)?.EnglishName : "";
              //  result.CommitteeCategoryName = data.CommitteeCategoryId.GetValueOrDefault() > 0 ? committeeCategory.SingleOrDefault(s => s.Id == data.CommitteeCategoryId).Title : "";
            }

            return result;
        }
    }
}
