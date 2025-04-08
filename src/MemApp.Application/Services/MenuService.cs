using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Services
{
    public class MenuService : IMenuService
    {

        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public MenuService(IMemDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<MenuDto>> CreateMenu(MenuDto menuReq)
        {
            var response = new Result<MenuDto>();
            var obj = await _context.Menus.SingleOrDefaultAsync(q => q.Id == menuReq.Id);
            if (obj == null)
            {
                obj = new Menu();
                _context.Menus.Add(obj);
            }

            obj.Name = menuReq.Name;
            obj.Url = menuReq.Url;
            obj.DisplayOrder = menuReq.DisplayOrder;
            obj.Visible = menuReq.Visible;
            obj.NavIcon = menuReq.NavIcon;

            if (await _context.SaveAsync() > 0)
            {
                response.Data = new MenuDto
                {
                    Id = obj.Id
                };

                response.HasError = false;
                //response.Id = obj.Id;
                //response.Name = obj.Name;
                //response.Url = obj.Url;
                //response.DisplayOrder = obj.DisplayOrder;
                //response.Visible = obj.Visible;
                //response.NavIcon = obj.NavIcon;
                response.Messages.Add("Save Success");
            }
            else
            {
                response.HasError = true;
                response.Messages.Add("something went wrong");
            }

            return response;
        }

        public async Task<ListResult<MenuDto>> GetAllMenus()
        {
            var response = new ListResult<MenuDto>();
            var menus = await _context.Menus
                .Include(i => i.Childs)
                .ToListAsync();


            if (menus == null)
            {
                response.HasError = true;
                response.Messages.Add("Something went wrong.");
            }
            else
            {
                response.Data = menus.Select(x => new MenuDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = x.Url ?? "",
                    DisplayOrder = x.DisplayOrder,
                    Visible = x.Visible,
                    NavIcon = x.NavIcon,
                    Childs = x.Childs.Select(x => new MenuDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Url = x.Url ?? "",
                        DisplayOrder = x.DisplayOrder,
                        Visible = x.Visible,
                        NavIcon = x.NavIcon,
                    }).ToList(),
                }).ToList();
                response.HasError = false;
                response.Count = menus.Count;
            }
            return response;
        }

        public async Task<Result> RemoveMenu(int id)
        {
            var response = new Result();
            var obj = await _context.Menus.SingleOrDefaultAsync(q => q.Id == id);
            if (obj == null)
            {
                response.HasError = true;
                response.Messages.Add("Something went wrong.");
            }
            else
            {
                obj.IsActive = false;
                response.HasError = false;
                response.Messages.Add("Success.");
            }
            await _context.SaveAsync();
            return response;
        }
    }
}
