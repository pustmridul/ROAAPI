using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemberTemps.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberTemps.Command
{
    public class RejectMemberTempCommand : IRequest<MemberTempVm>
    {
        public int MemberId { get; set; }
    }

    public class RejectMemberTempCommandHandler : IRequestHandler<RejectMemberTempCommand, MemberTempVm>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public RejectMemberTempCommandHandler(IMemDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<MemberTempVm> Handle(RejectMemberTempCommand request, CancellationToken cancellation)
        {
            var result = new MemberTempVm();
            var obj = await _context.MemberTemps
                .SingleOrDefaultAsync(q => q.Id == request.MemberId);
            if (obj == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found MemberTemp");
                return result;
            }
            else
            {
                obj.Status = "Rejected";              
                result.HasError = false;
                result.Messages.Add("MemberTemp Rejected");
            }

            if (await _context.SaveChangesAsync(cancellation) > 0)
            {

                //result.Data.Anniversary = obj.Anniversary;
                //result.Data.BatchNo = obj.BatchNo;
                //result.Data.SpouseOccupation = obj.SpouseOccupation;
                //result.Data.Spouse = obj.Spouse;
                //result.Data.CadetNo = obj.CadetNo;
                //result.Data.CadetName = obj.CadetName;
                //result.Data.FullName = obj.FullName;
                //result.Data.ClubName = obj.ClubName;
                //result.Data.CollegeId = obj.CollegeId ?? 0;
                //result.Data.Designation = obj.Designation;
                //result.Data.Dob = obj.Dob;
                //result.Data.ImgFileUrl = obj.ImgFileUrl;
                //result.Data.MemberProfessionId = obj.MemberProfessionId;
                result.Data.Email = obj.Email??"";
                result.Data.Status = "Approved";
                result.Data.Phone = obj.Phone ?? "";
                result.Data.Id = obj.Id;
            }
            else
            {
                result.HasError = true;
                result.Messages.Add("something wrong");
            }

            return result;
        }
    }

}
