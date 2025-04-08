using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Com.Queries.GetNavMenuByUserId;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Commands.SaveUserPermission
{
    public class SaveUserMenuMapCommand : IRequest<int>
    {


    }

    public class SaveUserMenuMapCommandHandler : IRequestHandler<SaveUserMenuMapCommand, int>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;

        public SaveUserMenuMapCommandHandler(
            IMemDbContext context,
            IMediator mediator
            )
        {
            _context = context;
            _mediator = mediator;
           
        }


        public async Task<int> Handle(SaveUserMenuMapCommand request, CancellationToken cancellation)
        {
            return 0;
            //var menuList = new List<UserMenuMap>();
            //try
            //{
            //    foreach (var m in request.models)
            //    {

            //        if (m.SubMenuId == 0)
            //        {
            //            var menu = await _context.UserMenuMaps.SingleOrDefaultAsync(q => q.UserId == userId && q.MenuId == m.MenuId);
            //            if (menu == null)
            //            {
            //                menu = new UserMenuMap();
            //                menu.MenuId = m.MenuId;
            //                menu.SubMenuId = 0;
            //                menu.UserId = userId;
            //                menuList.Add(menu);
            //            }

            //        }
            //        else
            //        {
            //            var menu = await _context.UserMenuMaps.SingleOrDefaultAsync(q => q.UserId == userId && q.MenuId == m.MenuId && q.SubMenuId == m.SubMenuId);
            //            if (menu == null)
            //            {
            //                menu = new UserMenuMap();
            //                menu.MenuId = m.MenuId;
            //                menu.SubMenuId = m.SubMenuId;
            //                menu.UserId = userId;
            //                menuList.Add(menu);
            //            }

            //        }

            //    }
            //    _context.UserMenuMaps.AddRange(menuList);

            //    var currentUserMenus = await _context.UserMenuMaps
            //        .Where(up => up.UserId == userId).ToListAsync(cancellation);

            //    foreach (var cp in currentUserMenus)
            //    {
            //        if (request.models.Any(q => q.MenuId == cp.MenuId && q.SubMenuId == cp.SubMenuId))
            //            continue;

            //        _context.UserMenuMaps.Remove(cp);

            //    }

            //    await _context.SaveChangesAsync(cancellation);


            //    return await _mediator.Send(new GetNavMenusByUserIdQuery()
            //    {
            //        UserId = userId
            //    });
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
           
        }
    }

}