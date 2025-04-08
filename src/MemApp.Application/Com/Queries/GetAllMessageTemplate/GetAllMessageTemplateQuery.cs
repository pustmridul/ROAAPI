using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MiscSales.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.com.Queries.GetAllMessageTemplate
{
    public class GetAllMessageTemplateQuery : IRequest<ListResult<MessageTemplateReq>>
    {
        public MiscTemplateSearchModel Model { get; set; } = new MiscTemplateSearchModel();
    }

    public class GetAllMessageTemplateQueryHandler : IRequestHandler<GetAllMessageTemplateQuery, ListResult<MessageTemplateReq>>
    {
        private readonly IMemDbContext _context;
        public GetAllMessageTemplateQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MessageTemplateReq>> Handle(GetAllMessageTemplateQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MessageTemplateReq>();
            try
            {
                var data = await _context.MessageTemplates
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                if (data != null)
                {
                    if (request.Model.MessageTypeId != null)
                    {
                        data = data.Where(c => c.MessageTypeId == request.Model.MessageTypeId).ToList();
                    }
                    if (request.Model.OccasionTypeId != null)
                    {
                        data = data.Where(c => c.OccasionTypeId == request.Model.OccasionTypeId).ToList();
                    }
                }

                if (data.Count == 0)
                {
                    result.HasError = false;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.Count;
                    result.Data = data.Select(s => new MessageTemplateReq
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Subject = s.Subject,
                        Message = s.Message,
                        MessageTypeId = s.MessageTypeId,
                        OccasionTypeId = s.OccasionTypeId,
                    }).ToList();
                }
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }
    }
}
