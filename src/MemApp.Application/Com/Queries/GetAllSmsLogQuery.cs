using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberTypes.Queries
{
    public class GetAllSmsLogQuery : IRequest<SmsLogListVm>
    {
        public SmsLogSearchReq Model { get; set; }
    }

    public class GetAllSmsLogQueryHandler : IRequestHandler<GetAllSmsLogQuery, SmsLogListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllSmsLogQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<SmsLogListVm> Handle(GetAllSmsLogQuery request, CancellationToken cancellationToken)
        {
            var result = new SmsLogListVm();
            var data = await _context.SmsLogs
                .Where(q=> (!string.IsNullOrEmpty(request.Model.SearchText) ? q.PhoneNo.Contains(request.Model.SearchText): true) 
                || (!string.IsNullOrEmpty(request.Model.SearchText) ? q.MemberName.Contains(request.Model.SearchText) : true)
                || (!string.IsNullOrEmpty(request.Model.SearchText) ? q.MemberShipNo.Contains(request.Model.SearchText) : true)
                ).OrderByDescending(o=>o.Id)
                .ToPaginatedListAsync(request.Model.PageNo, request.Model.PageSize,cancellationToken);

            if (data.TotalCount==0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.DataCount=Convert.ToInt32(data.TotalCount);
                result.DataList =data.Data.Select(s=> new SmsLogReq
                {
                    Id = s.Id,
                    SmsDate = s.SmsDate,
                    PhoneNo = s.PhoneNo,
                    MemberName = s.MemberName,
                    
                    MemberShipNo=s.MemberShipNo,
                    Status=s.Status,
                    Message=s.Message,
                }).ToList();
            }

            return result;
        }
    }
}
