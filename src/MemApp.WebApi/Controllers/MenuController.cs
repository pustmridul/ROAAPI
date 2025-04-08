using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces;
using MemApp.Application.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuservice;

        public MenuController(IMenuService menuService)
        {
            _menuservice = menuService;
        }
        // [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMenu(MenuDto menuReq)
        {
            var response = await _menuservice.CreateMenu(menuReq);
            if (response.HasError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllMeuns()
        {
            var response = await _menuservice.GetAllMenus();
            if (response.HasError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
