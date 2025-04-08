using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MemApp.Application.Com.Commands.UserConferences
{
    public class CreateMessageTemplateCommand : IRequest<Result>
    {
        public MessageTemplateReq Model { get; set; } = new MessageTemplateReq();
    }

    public class CreateMessageTemplateCommandHandler : IRequestHandler<CreateMessageTemplateCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IMemoryCache _memoryCache;
        public CreateMessageTemplateCommandHandler(IMemDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public async Task<Result> Handle(CreateMessageTemplateCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            try
            {

                var obj = await _context.MessageTemplates
                            .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    obj = new MessageTemplate()
                    {
                        IsActive = true,
                    };
                    _context.MessageTemplates.Add(obj);
                    result.Succeeded = true;
                    result.Messages.Add("Created success");
                }

                obj.Name = request.Model.Name;
                obj.Subject = request.Model.Subject;
                obj.Message = request.Model.Message;
                obj.MessageTypeId = request.Model.MessageTypeId;
                obj.OccasionTypeId = request.Model.OccasionTypeId;

                await _context.SaveChangesAsync(cancellation);

                _memoryCache.Set(StaticData.CacheKey.GetMessageTemplate, request.Model, StaticData.CacheKey.cacheEntryOptions);

            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Messages.Add(ex.Message);
            }
            return result;
        }
    }
}
