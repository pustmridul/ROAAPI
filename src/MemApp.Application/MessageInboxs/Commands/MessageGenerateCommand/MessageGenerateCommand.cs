using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MemApp.Application.MessageInboxs.Commands.MessageGenerateCommand;


public class MessageGenerateCommand : IRequest<Result>
{
    public MessageInboxCreateDto Model { get; set; } = new MessageInboxCreateDto();
}

public class MessageGenerateCommandHandler : IRequestHandler<MessageGenerateCommand, Result>
{
    private readonly IMemDbContext _context;
    private readonly IMediator _mediator;
    private readonly IUserLogService _userLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionHandler _permissionHandler;
    public MessageGenerateCommandHandler(IMemDbContext context, IMediator mediator, IUserLogService userLogService, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
    {
        _context = context;
        _mediator = mediator;
        _userLogService = userLogService;
        _currentUserService = currentUserService;
        _permissionHandler = permissionHandler;
    }
    public async Task<Result> Handle(MessageGenerateCommand request, CancellationToken cancellation)
    {
        try
        {
            var result = new Result();

            if (request.Model.IsAllMember == true)
            {
                var objList = new List<MessageInbox>();
                var memberList = await _context.RegisterMembers.Where(q => q.IsActive)
               .Select(s => new { s.Id }).AsNoTracking()
               .ToListAsync(cancellation);

                foreach (var member in memberList)
                {
                    var obj = new MessageInbox();
                    obj.IsRead = request.Model.IsRead;
                    obj.Message = request.Model.Message;
                    obj.TypeId = request.Model.TypeId;
                    obj.MemberId = member.Id;
                    obj.IsRead = false;
                    obj.Title = GetEnumNameWithSpaces((MessageInboxTypeEnum)request.Model.TypeId);
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = _currentUserService.UserId;
                    obj.CreatedByName = _currentUserService.Username;
                    objList.Add(obj);
                }

                _context.MessageInboxs.AddRange(objList);
            }
            else
            {
                var obj = new MessageInbox();
                obj.IsRead = false;
                obj.Message = request.Model.Message;
                obj.TypeId = request.Model.TypeId;
                obj.Title = GetEnumNameWithSpaces((MessageInboxTypeEnum)request.Model.TypeId);
                obj.MemberId= (int)request.Model.MemberId!;
                _context.MessageInboxs.Add(obj);

            }
            await _context.SaveAsyncOnly();

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
    public static string GetEnumNameWithSpaces(Enum enumValue)
    {
        var enumName = enumValue.ToString();
        return Regex.Replace(enumName, "(?<!^)([A-Z])", " $1");
    }
}