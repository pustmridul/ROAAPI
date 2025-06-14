﻿using MediatR;
using MemApp.Application.Mem.MemberTypes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResApp.Application.Com.Commands.Division.Update;
using ResApp.Application.Com.Commands.UpdateThana;
using ResApp.Application.Com.Queries.GetDivisionQuery;
using ResApp.Application.ROA.Zone.Command;
using ResApp.Application.ROA.Zone.Queries;

namespace Res.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DivisionController : ApiNewControllerBase
    {
        public DivisionController(ISender sender) : base(sender) 
        {
        }

       // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllDivision()
        {
            return Ok(await Mediator.Send(new GetAllDivisionQuery()
            {
              //  UserId = userId
            }));
          
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDistrictByDivisionId(int DivisionId, int pageNo, int pageSize, string appId, string? searchText, bool? IsDrop)
        {
            return Ok(await Mediator.Send(new GetDistrictDivisionIdQuery()
            {
                   DivId = DivisionId,

                    PageNo = pageNo,
                    PageSize = pageSize,
                    AppId = appId,
                    SearchText = searchText,
                    IsDropDown = IsDrop
            }));

        }

        [HttpGet]
        public async Task<IActionResult> GetDivisionDetailsById(int DivisionId)
        {
            return Ok(await Mediator.Send(new GetDivisionDetailsQuery()
            {
                DivisionId = DivisionId,
            }));

        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictDetailsById(int DistrictId)
        {
            return Ok(await Mediator.Send(new GetDistrictDetailsQuery()
            {
                DistrictId = DistrictId,
            }));

        }

        [HttpGet]
        public async Task<IActionResult> GetAllThanaByDistrictId(int DistricId, int pageNo, int pageSize, string appId, string? searchText)
        {
            return Ok(await Mediator.Send(new GetThanaByDistrictIdQuery()
            {
                DistrictId = DistricId,
                PageNo = pageNo,
                PageSize = pageSize,
                AppId = appId,
                SearchText = searchText
            }));

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateThana(UpdateThanaInfoCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateDivision(UpdateDivisionCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateDistrict(UpdateDistrictCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveZone(CreateZoneCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [HttpGet]
        public async Task<IActionResult> GetAllZoneByDistrictId(int DistricId, int pageNo, int pageSize, string appId, string? searchText)
        {
            return Ok(await Mediator.Send(new GetZoneByDistrictIdQuery()
            {
                DistrictId = DistricId,
                PageNo = pageNo,
                PageSize = pageSize,
                AppId = appId,
                SearchText = searchText
            }));

        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveZone(int id)
        {
            var result = await Mediator.Send(new DeleteZoneCommand() { Id = id });
            return Ok(result);
        }



    }
}
