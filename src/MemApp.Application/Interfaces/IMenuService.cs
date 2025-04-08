using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Models.Requests;
using MemApp.Application.Models.Responses;

namespace MemApp.Application.Interfaces
{
    public interface IMenuService
    {
        Task<Result<MenuDto>> CreateMenu(MenuDto menuReq);
        Task<ListResult<MenuDto>> GetAllMenus();
    }
}
