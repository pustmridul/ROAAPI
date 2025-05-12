using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Queries.GetUserMenus;


public class GetUserMenuQuery : IRequest<ListResult<MenuDto>>
{
    public int UserId { get; set; }
}

internal class GetUserMenuQueryHandler : IRequestHandler<GetUserMenuQuery, ListResult<MenuDto>>
{
    private readonly IMemDbContext _context;
    public GetUserMenuQueryHandler(IMemDbContext context)
    {
        _context = context;
    }

    public async Task<ListResult<MenuDto>> Handle(GetUserMenuQuery request, CancellationToken cancellationToken)
    {
        var response = new ListResult<MenuDto>();

        try
        {
            var menus = await _context.Menus.Where(i => i.ParentId == null)
            .Include(i => i!.Childs!.Where(i => i.IsActive))
            .Where(i => i.IsActive)
            .ToListAsync(cancellationToken);

            var userPermission = await _context.UserMenuMaps.Where(q => q.UserId == request.UserId).ToListAsync(cancellationToken);

            if (userPermission.Count < 1)
            {
                response.HasError = true;
                response.Count = 0;





                return response;
            }

            response.Data = menus.Select(x => new MenuDto()
            {
                Id = x.Id,
                Name = x.Name,
                ModuleName = x.ModuleName!,
                Url = x.Url ?? "",
                DisplayOrder = x.DisplayOrder,
                Visible = x.Visible,
                NavIcon = x.NavIcon,
                IsChecked = userPermission.Any(q => q.MenuId == x.Id),
                ParentId = x.ParentId,
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

            // response.Data= response.Data.Remove(x=>x.)
            response.HasError = false;
            response.Count = menus.Count;
        }
        catch (Exception ex)
        {

        }


        

        return response;
    }
}
