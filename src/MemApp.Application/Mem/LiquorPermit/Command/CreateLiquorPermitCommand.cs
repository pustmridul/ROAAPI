using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Communication.Models;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Application.Mem.MiscSales.Command;
using MemApp.Domain.Entities.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.Communication;
using MemApp.Application.Services;
using AutoMapper.Execution;
using MemApp.Application.Models;
using Microsoft.AspNetCore.Http.Internal;
using MemApp.Domain.Enums;
using MemApp.Domain.Entities;

namespace MemApp.Application.Mem.Communication.Command
{
    public class CreateLiquorPermitCommand : IRequest<LiquorPermitVm>
    {
        public LiquorPermitModelReq Model { get; set; } = new LiquorPermitModelReq();
        public string webHostEnvironment { get; set; } = string.Empty;
    }

    public class CreateLiquorPermitCommandHandler : IRequestHandler<CreateLiquorPermitCommand, LiquorPermitVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IFileSaveService _fileService;

        public CreateLiquorPermitCommandHandler(IFileSaveService fileService, IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
            _fileService = fileService;
        }
        public async Task<LiquorPermitVm> Handle(CreateLiquorPermitCommand request, CancellationToken cancellation)
        {
            //if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            //{
            //    throw new UnauthorizedAccessException();
            //}
            var result = new LiquorPermitVm();
            var isUpdate = false;

            var obj = await _context.LiquorPermits
                .SingleOrDefaultAsync(q => q.MemberId == request.Model.MemberId);

            if (obj == null)
            {
                obj = new LiquorPermit();

                _context.LiquorPermits.Add(obj);
                result.HasError = false;
                result.Messages.Add("New Liquor Permit Created");
            }
            else
            {
                isUpdate = true;
            }
            obj.Title = request.Model.Title;

            obj.IsActive = true;
            obj.MemberId = request.Model.MemberId;





            try
            {
                await _context.SaveChangesAsync(cancellation);

                if (request.Model.File != null)
                {
                    obj.FileUrl = "LiquorPermit/Image/" + (int)OperationTypeEnum.LiquorPermit + "-" + obj.Id + "-" + request.Model.File.FileName;
                }
                if (request.Model.File != null)
                {
                    var model = new FileSaveModel();
                    model.SubDirectory = "wwwroot/LiquorPermit/Image";

                    model.WebRootPath = request.webHostEnvironment;
                    model.file = request.Model.File;

                    model.OperationTypeId = (int)OperationTypeEnum.LiquorPermit;  
                    model.OperationId = obj.Id;

                    await _fileService.UploadFile(model);
                }


                await _context.SaveChangesAsync(cancellation);

                result.Data.Title = obj.Title;
                result.Data.Id = obj.Id;
                result.Data.FileUrl = obj.FileUrl;

                result.HasError = false;
                result.Succeeded = true;
                result.Messages.Add("Update Successfull");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }
    }
}
