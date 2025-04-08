using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MiscSales.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Queries
{
    public class GetTemplateByMessageTypeId : IRequest<MessageTemplateReq>
    {
        public MiscTemplateSearchModel Model { get; set; } = new MiscTemplateSearchModel();
    }

    public class GetTemplateByMessageTypeIdHandler : IRequestHandler<GetTemplateByMessageTypeId, MessageTemplateReq>
    {
        private readonly IMemDbContext _context;
        public GetTemplateByMessageTypeIdHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MessageTemplateReq> Handle(GetTemplateByMessageTypeId request, CancellationToken cancellationToken)
        {
            var result = new MessageTemplateReq();
            try
            {
                var data = await _context.MessageTemplates.OrderByDescending(c => c.Id).SingleOrDefaultAsync(c => c.MessageTypeId == request.Model.MessageTypeId && c.OccasionTypeId == request.Model.OccasionTypeId);



                if (data == null)
                {
                    return result;
                }
                else
                {


                    result = new MessageTemplateReq
                    {
                        Id = data.Id,
                        Name = data.Name,
                        Subject = data.Subject,
                        Message = data.Message,
                        MessageTypeId = data.MessageTypeId,
                        OccasionTypeId = data.OccasionTypeId,
                    };
                }

            }
            catch (Exception ex)
            {
                throw ex;

            }

            return result;
        }
    }
}
