using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;

namespace ResApp.Application.Com.Queries.GetUserById
{


    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public int UserId { get; set; }
       
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IMemDbContext _context;
        public GetUserByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<UserDto>();
        
            var data = await _context.Users.Where(s => s.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
               
                result.Data = new UserDto
                {
                    Id = data.Id,
                    Name = data.Name!,
                    UserName = data.UserName!,
                    EmailId = data.EmailId!,
                    PhoneNo = data.PhoneNo!,
                };
            }
            
           

            return result;
        }
    }
}
