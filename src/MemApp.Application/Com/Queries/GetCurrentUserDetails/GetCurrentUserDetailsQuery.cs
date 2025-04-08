using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Queries.GetCurrentUserDetails
{
    public class GetCurrentUserDetailsQuery : IRequest<Result<UserDetailsDto>>
    {
       // public int MemberId {  get; set; }
    }

    public class GetMemberRegInfoQueryHandler : IRequestHandler<GetCurrentUserDetailsQuery, Result<UserDetailsDto>>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public GetMemberRegInfoQueryHandler(IMemDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<UserDetailsDto>> Handle(GetCurrentUserDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<UserDetailsDto>();
            var checkUser = _currentUserService.Current();
            var data = await _context.Users.
                Where(x=> x.UserName== checkUser.UserName).FirstOrDefaultAsync(cancellationToken);

            var memberReg=await _context.MemberRegistrationInfos.FirstOrDefaultAsync(x=>x.Id==data!.MemberInfoId);

            if (memberReg == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
               // result.Data = data;
                result.Data = new UserDetailsDto
                {
                    Id = memberReg!.Id,
                    CompanyName = memberReg.InstituteNameEnglish,
                    EmailId = memberReg.Email,
                    Name = memberReg.Name,
                    PhoneNo = memberReg.PhoneNo,
                    TradeLicense = memberReg.MemberTradeLicense,
                    UserNID = memberReg.MemberNID,
                    
                };
                
            }

            return result;
        }
    }
}
