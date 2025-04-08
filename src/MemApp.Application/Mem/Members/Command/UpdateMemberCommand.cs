using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.Members.Command
{
    public class UpdateMemberCommand : IRequest<MemberUpdateVm>
    {
        public MemberUpdateVm Model { get; set; } = new MemberUpdateVm();
    }

    public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberUpdateVm>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateMemberCommandHandler(IMemDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<MemberUpdateVm> Handle(UpdateMemberCommand request, CancellationToken cancellation)
        {
            try
            {

                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }
                var result = new MemberUpdateVm();
                var obj = await _context.RegisterMembers
                              .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Member Not Found");
                }
                else
                {
                    obj.Dob = (request.Model.Dob == null || request.Model.Dob == "") ? null : DateTime.Parse(request.Model.Dob);
                    obj.FullName = request.Model.FullName;
                    obj.Phone = request.Model.Phone;
                    obj.Email = request.Model.Email;
                    obj.Organaization = request.Model.Organaization;
                    obj.Designation = request.Model.Designation;
                    obj.HomeAddress = request.Model.HomeAddress;
                    obj.OfficeAddress = request.Model.OfficeAddress;
                    obj.Spouse = request.Model.Spouse;
                    obj.SpouseOccupation = request.Model.SpouseOccupation;
                    obj.Address = request.Model.Address;

                    await _context.SaveChangesAsync(cancellation);

                    result.Id = obj.Id;
                    result.FullName = obj.FullName;
                    result.Phone = obj.Phone;
                    result.Email = obj.Email;
                    result.Organaization = obj.Organaization;
                    result.Designation = obj.Designation;
                    result.HomeAddress = obj.HomeAddress;
                    result.OfficeAddress = obj.OfficeAddress;
                    result.Spouse = obj.Spouse;
                    result.SpouseOccupation = obj.SpouseOccupation;
                    result.Address = obj.Address;
                    result.Dob = obj?.Dob == null ? "" : obj.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    result.HasError = false;
                    result.Messages.Add("Member Updated Successfully");

                }

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
    }
}
