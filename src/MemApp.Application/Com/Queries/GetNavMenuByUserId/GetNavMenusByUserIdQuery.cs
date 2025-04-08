using Azure;
using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Queries.GetNavMenuByUserId
{
    public class GetNavMenusByUserIdQuery : IRequest<ListResult<MenuDto>>
    {
        public int UserId { get; set; }
    }

    public class GetNavMenusQueryHandler : IRequestHandler<GetNavMenusByUserIdQuery, ListResult<MenuDto>>
    {
        private readonly IMemDbContext _context;

        public GetNavMenusQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MenuDto>> Handle(GetNavMenusByUserIdQuery request, CancellationToken cancellationToken)
        {
            var response = new ListResult <MenuDto>();

            var menus = await _context.Menus              
                .Include(i => i.Childs.Where(q=>q.IsActive))
                .Where(q=>q.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var userPermission = await _context.UserMenuMaps.Where(q => q.UserId == request.UserId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);


            response.Data = menus.Select(x => new MenuDto()
            {
                Id = x.Id,
                Name = x.Name,
                Url = x.Url ?? "",
                DisplayOrder = x.DisplayOrder,
                Visible = x.Visible,
                NavIcon = x.NavIcon,
                IsChecked = userPermission.Any(q => q.MenuId == x.Id),
                Childs = x.Childs?.Select(x => new MenuDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = x.Url ?? "",
                    DisplayOrder = x.DisplayOrder,
                    Visible = x.Visible,
                    NavIcon = x.NavIcon,
                    IsChecked = userPermission.Any(q => q.MenuId == x.Id)
                }).ToList(),
            }).ToList();
            response.HasError = false;
            response.Count = menus.Count;

            return response;
        }
    }

}