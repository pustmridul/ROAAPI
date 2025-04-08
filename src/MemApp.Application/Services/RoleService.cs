using MemApp.Application.Models.Requests;
using MemApp.Domain.Entities;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Models.Responses;
using MemApp.Application.Extensions;
using System.Data;

namespace MemApp.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMemDbContext _memDbContext;
        public RoleService(IMemDbContext memDbContext)
        {           
            _memDbContext = memDbContext;
        }

        public async Task<Result<RoleDto>> CreateRole(RoleDto req)
        {
            
            var result= new Result<RoleDto>();
            if (string.IsNullOrEmpty(req.Name))
            {
                result.HasError = true;
                result.Messages.Add("Name can't be null");
                return result;
            }
            var role = _memDbContext.Roles.SingleOrDefault(q=>q.Name== req.Name);
            if (role != null)
            {
                result.HasError = true;
                result.Messages.Add("This role does exist already!!!");
                return result;
            }

            if (role == null)
            {
                role = new Role()
                {               
                };
                role.IsActive= true;
                _memDbContext.Roles.Add(role);
            }
                role.Name = req.Name;

            if (await _memDbContext.SaveAsync()>0)
            {
                result.HasError = false;
                //result.Data.Id = role.Id;
                //result.Data.Name= role.Name;
                result.Messages.Add("Save Success");
            }
            else
            {
                result.HasError= true;
                result.Messages.Add("something wrong");             
            }

            return result;
        }

        public async Task<ListResult<RoleDto>> GetAllRole()
        {
            var result = new ListResult<RoleDto>();
            var roles = await _memDbContext.Roles.Select(x => new RoleDto()
            {
                Id = x.Id,
                Name = x.Name,
                RoleUsers= _memDbContext.UserRoleMaps.Count(userRole => userRole.RoleId == x.Id)
            }).ToListAsync();
            result.Count = roles.Count;
            result.Data = roles;
            return result;
        }
    }
}