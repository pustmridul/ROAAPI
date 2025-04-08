using MediatR;
using MemApp.Application.Com.Commands.CreateFeedbackCategory;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemApp.Domain.Entities.com;

namespace MemApp.Application.Com.Commands.CreateFeedbackCategory
{
    public class CreateFeedbackCategoryCommand : IRequest<Result>
    {
        public FeedbackCategoryReq Model { get; set; } = new FeedbackCategoryReq();
    }

    public class CreateFeedbackCategoryCommandHandler : IRequestHandler<CreateFeedbackCategoryCommand, Result>
    {
        private readonly IMemDbContext _context;
        public CreateFeedbackCategoryCommandHandler(IMemDbContext context)
        {
            _context = context;
        }
        public async Task<Result> Handle(CreateFeedbackCategoryCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            try
            {

                var obj = await _context.FeedbackCategories
                            .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    obj = new FeedbackCategory()
                    {
                        Id = request.Model.Id,
                        Name=request.Model.Name,
                        TaggedDirectorId = request.Model.TaggedDirectorId,

                    };
                    _context.FeedbackCategories.Add(obj);
                    result.Succeeded = true;
                    result.Messages.Add("Created success");
                }

            
                obj.Name = request.Model.Name;
                obj.TaggedDirectorId = request.Model.TaggedDirectorId;
                


                await _context.SaveChangesAsync(cancellation);

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
