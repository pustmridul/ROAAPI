using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Service.Model;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Command
{
    public class GenerateAvailabilityCommand : IRequest<AvailabilityVm>
    {
        public AvailabilityRes Model { get; set; } = new AvailabilityRes();
    }

    public class GenerateAvailabilityCommandHandler : IRequestHandler<GenerateAvailabilityCommand, AvailabilityVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        public GenerateAvailabilityCommandHandler(IMemDbContext context, IMediator mediator,  ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }
        public async Task<AvailabilityVm> Handle(GenerateAvailabilityCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            bool isNewObj = false;
            var result = new AvailabilityVm();

            if (DateTime.Parse(request.Model.AvailabilityDate) >( (request.Model.AvailabilityEndDate == null || request.Model.AvailabilityEndDate == "") ? null : DateTime.Parse(request.Model.AvailabilityEndDate)))
            {
                result.HasError = true;
                result.Messages.Add("End Date is less then From Date");
                return result;
            }
            for (DateTime currentDate =DateTime.Parse(request.Model.AvailabilityDate); currentDate <= ((request.Model.AvailabilityEndDate == null || request.Model.AvailabilityEndDate == "") ? null : DateTime.Parse(request.Model.AvailabilityEndDate)); currentDate = currentDate.AddDays(1))
            {
                try
                {

                    var obj = await _context.Availabilities
                        .Include(i => i.AvailabilityDetails)
                    .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                    if (obj == null)
                    {
                        isNewObj = true;
                        obj = new Availability();
                        obj.IsActive = true;
                        obj.Name = request.Model.Name;
                        obj.IsLifeTime = request.Model.IsLifeTime;
                        obj.AvailabiltyDate = currentDate;
                        _context.Availabilities.Add(obj);
                        result.HasError = false;
                        result.Messages.Add("New Availability created");
                    }
                    else
                    {
                        result.HasError = false;
                        result.Messages.Add("Availability Updated");
                    }

                    if (isNewObj)
                    {
                        if (await _context.SaveChangesAsync(cancellation) > 0)
                        {
                            foreach (var ad in request.Model.AvailabilityDetailVms)
                            {
                                var objDetail = new AvailabilityDetail()
                                {
                                    AvailabilityId = obj.Id,
                                    StartTime = DateTimeGen(currentDate, ad.StartTime),
                                    EndTime = DateTimeGen(currentDate, ad.EndTime),
                                    IsActive = true,
                                    IsWholeDay = ad.IsWholeDay,
                                    IsChecked = ad.IsChecked,
                                    Title = ad.Title
                                };
                                _context.AvailabilityDetails.Add(objDetail);
                            }
                        }
                        else
                        {
                            result.HasError = true;
                            result.Messages.Add("opoos!!!!!");
                        }

                    }
                    else
                    {
                        foreach (var ad in request.Model.AvailabilityDetailVms)
                        {
                            var objDetail = obj.AvailabilityDetails.SingleOrDefault(q => q.AvailabilityId == obj.Id && q.IsActive && q.Id == ad.Id);

                            if (objDetail == null)
                            {
                                objDetail = new AvailabilityDetail()
                                {
                                    AvailabilityId = obj.Id,
                                    IsActive = true,

                                };
                                _context.AvailabilityDetails.Add(objDetail);
                            }
                            objDetail.StartTime = ad.StartTime;
                            objDetail.EndTime = ad.EndTime;
                            objDetail.IsWholeDay = ad.IsWholeDay;
                            objDetail.IsChecked = ad.IsChecked;
                            objDetail.Title = ad.Title;



                        }
                        foreach (var ad in obj.AvailabilityDetails)
                        {
                            var exist = request.Model.AvailabilityDetailVms.SingleOrDefault(q => q.Id == ad.Id);
                            if (exist == null)
                            {
                                ad.IsActive = false;
                            }
                        }
                    }

                    if (await _context.SaveChangesAsync(cancellation) > 0)
                    {
                        result.Data.Id = obj.Id;
                    }
                    else
                    {
                        result.HasError = true;
                        result.Messages.Add("something wrong");
                    }

                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.Messages.Add("Exception" + ex.Message);ToString();
                }
            }
           
            return result;
        }
    
    
        private string DateTimeGen(DateTime forDate, string forTime)
        {
            return (forDate.Date + DateTime.Parse(forTime).TimeOfDay).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
    }

}