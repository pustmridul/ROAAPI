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
using Hangfire;
using MemApp.Application.MessageInboxs.Commands.MessageGenerateCommand;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Application.Extensions;
using Microsoft.AspNetCore.Http;

namespace MemApp.Application.Mem.Communication.Command
{
    public class CreateNoticeCommand : IRequest<Result<NoticeModelDto>>
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int? AttachmentType { get; set; }
        public string? TextContent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? FileUrl { get; set; }
        public IFormFile? File { get; set; }
        public string webHostEnvironment { get; set; }= string.Empty;
    }

    public class CreateNoticeCommandHandler : IRequestHandler<CreateNoticeCommand, Result<NoticeModelDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IFileSaveService _fileService;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;


        public CreateNoticeCommandHandler(IFileSaveService fileService,IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService,
            IPermissionHandler permissionHandler, IBackgroundJobClientV2 backgroundJobClientV2)
        {
            _context = context;
            _mediator = mediator;
            _backgroundJobClientV2 = backgroundJobClientV2;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
            _fileService = fileService;
        }
        public async Task<Result<NoticeModelDto>> Handle(CreateNoticeCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result<NoticeModelDto>();
            var isUpdate = false;

            var obj = await _context.Notices
                .SingleOrDefaultAsync(q => q.Id == request.Id);

            if (obj == null)
            {
                obj = new Notice();

                _context.Notices.Add(obj);
                result.HasError = false;
                result.Messages.Add("New MiscItem Created");
            }
            else
            {
                isUpdate = true;
            }
            obj.Title = request.Title;
            obj.AttachmentType = request.AttachmentType;
            obj.IsActive = true;
            obj.TextContent = request.TextContent;
            obj.StartDate = request.StartDate;
            obj.EndDate = request.EndDate;




            try
            {
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {

                    if (request.Id == null || request.Id == 0)
                    {
                        var messageObj = new MessageInboxCreateDto
                        {
                            Message = $"Notice: {obj.TextContent}",
                            TypeId = MessageInboxTypeEnum.NoticeBroadcast,
                            IsAllMember = true

                        };
                        _backgroundJobClientV2.Enqueue(() => ProcessMessage(new MessageGenerateCommand() { Model = messageObj }));
                    }

                    result.Data.Title = obj.Title;
                    result.Data.Id = obj.Id;


                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("Something wrong");
                }


                if (request.File != null)
                {
                    if (request.AttachmentType == 1)
                    {
                        obj.FileUrl = "Notice/Image/" + (int)OperationTypeEnum.Notice + "-" + obj.Id + "-" + request.File.FileName;
                    }
                    if (request.AttachmentType == 2)
                    {
                        obj.FileUrl = "Notice/Pdf/" + (int)OperationTypeEnum.Notice + "-" + obj.Id + "-" + request.File.FileName;
                    }

                }

                if (request.File != null)
                {
                    var model = new FileSaveModel();
                    if (request.AttachmentType == 1)
                        model.SubDirectory = "wwwroot/Notice/Image";
                    if (request.AttachmentType == 2)
                        model.SubDirectory = "wwwroot/Notice/Pdf";

                    model.WebRootPath = request.webHostEnvironment;
                    model.file = request.File;

                    model.OperationTypeId = (int)OperationTypeEnum.Notice;
                    model.OperationId = obj.Id;

                    await _fileService.UploadFile(model);
                }
                _context.SaveChangesAsync(cancellation);


            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong", ex);
            }
           

            return result;
        }
        public async Task ProcessMessage(MessageGenerateCommand command)
        {
            await _mediator.Send(command);
        }
    }
}
