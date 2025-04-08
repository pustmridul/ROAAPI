using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Services
{
    public class PermissionHandler : IPermissionHandler
    {
        private IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public PermissionHandler(IMemDbContext context, ICurrentUserService currentUserService) {
            _context = context; 
            _currentUserService = currentUserService;
        }
        public bool HasPermission(int permissionId)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            bool hasPermission = _context.UserPermissions
                .Any(up => up.UserId == _currentUserService.UserId
                           && up.PermissionNo == permissionId);
            return hasPermission;
        }
        public bool HasRolePermission(int permissionId)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            bool hasPermission = false;

            var roleIds = _currentUserService?.Current()?.Roles?.ToList();
            if (roleIds?.Count == 0 || roleIds == null)
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                if (roleIds.Any())
                {
                    hasPermission = _context.RolePermissionMaps
                      .Any(q => roleIds.Contains(q.RoleId) && q.PermissionNo == permissionId && q.IsActive);
                }
            }
           
            return hasPermission;
        }

        public async Task<bool> HasPermissionAsync(int permissionId)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            bool hasPermission = await _context.UserPermissions
                .AnyAsync(up => up.UserId == _currentUserService.UserId
                                && up.PermissionNo == permissionId);
            return hasPermission;
        }

        public async Task<bool> IsTempMember()
        {
            bool hasPermission = false;
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            if (_currentUserService.Current().MemberId > 0)
            {
                hasPermission = await _context.RegisterMembers
               .AnyAsync(q => q.Id == _currentUserService.Current().MemberId && q.MemberTypeId == 77);
            }
            else
            {
                var user = await _context.Users.SingleOrDefaultAsync(q=>q.Id== _currentUserService.UserId);
                if(user != null)
                {
                    hasPermission = await _context.RegisterMembers
                            .AnyAsync(q => q.Id == user.MemberId && q.MemberTypeId == 77);
                }             
            }

            return hasPermission;
        }
        public async Task<bool> HasRolePermissionAsync(int permissionId)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            bool hasPermission = false;
           
            var roleIds = _currentUserService?.Current()?.Roles?.ToList();
            if(roleIds?.Count == 0 || roleIds==null)
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                if (roleIds.Any())
                {
                    hasPermission = await _context.RolePermissionMaps
                      .AnyAsync(q => roleIds.Contains(q.RoleId) && q.PermissionNo == permissionId && q.IsActive);
                }
            }
           
            return hasPermission;
        }
    }
}
