using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Domain.Entities.mem;
using MemApp.Application.Mem.ServiceSlottSettings.Models;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Mem.ServiceSlottSettings.Queries;

namespace MemApp.Application.Mem.ServiceSlottSettings.Commands
{
    public class ServiceSlotSettingsCommand : IRequest<ServiceSlotSettingsVm>
    {
        public ServiceSlotSettingsReq Model { get; set; } = new ServiceSlotSettingsReq();
    }

    public class ServiceSlotSettingsCommandHandler : IRequestHandler<ServiceSlotSettingsCommand, ServiceSlotSettingsVm>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        public ServiceSlotSettingsCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<ServiceSlotSettingsVm> Handle(ServiceSlotSettingsCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            try
            {
                var obj = await _context.SlotMasters
                    .Include(i => i.ServiceSlots)
                    .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellationToken);
                if (obj == null)
                {
                    obj = new SlotMaster()
                    {
                        ServiceId = request.Model.ServiceId,
                        ServiceText = request.Model.ServiceText,
                    };
                    _context.SlotMasters.Add(obj);
                }
                await _context.SaveChangesAsync(cancellationToken);
                foreach (var d in request.Model.SlotList)
                {
                    //var objDetail = await _context.ServiceSlotSettingss.SingleOrDefaultAsync(q => q.Id == d.Id, cancellationToken);
                    //if (objDetail == null)
                    //{
                    //    objDetail = new ServiceSlotSettings()
                    //    {
                    //        SlotMasterId = obj.Id,
                    //        Day = d.DayText,
                    //        StartTime = d.StartTime,
                    //        EndTime = d.EndTime,
                    //        IsActive = true,
                    //        IsWholeDay = d.IsWholeDay,
                    //    };
                    //    _context.ServiceSlotSettingss.Add(objDetail);
                    //}
                    foreach (var v in d.SlotList)
                    {
                        var objDetail = await _context.ServiceSlotSettingss.SingleOrDefaultAsync(q => q.Id == v.Id, cancellationToken);
                        if (objDetail == null)
                        {
                            objDetail = new ServiceSlotSettings()
                            {
                                SlotMasterId = obj.Id,
                                Day = v.DayText,
                                IsActive = true,
                            };
                            _context.ServiceSlotSettingss.Add(objDetail);
                        }
                        objDetail.StartTime = v.StartTime;
                        objDetail.EndTime = v.EndTime;
                        objDetail.IsWholeDay = v.IsWholeDay;

                    }
                }
                    var objTobeDeleted = new List<ServiceSlotSettings>();

                    foreach (var d in obj.ServiceSlots)
                    {
                        if (d.Id == 0) continue;

                        var has = request.Model.SlotList.Any(q => q.Id == d.Id);
                        if (!has)
                        {
                            var has1 = true;
                            foreach (var d1 in request.Model.SlotList)
                            {

                                has1 = d1.SlotList.Any(q => q.Id == d.Id);
                                if (has1) break;
                            }
                            if (!has1)
                            {
                                objTobeDeleted.Add(d);
                            }
                        }

                    }
                    _context.ServiceSlotSettingss.RemoveRange(objTobeDeleted);
                    await _context.SaveChangesAsync(cancellationToken);
                    return await _mediator.Send(new GetServiceSlotSettingsByIdQuery() { Id = obj.Id });
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            //var existingRecords = await _context.ServiceSlotSettingss.ToListAsync(cancellationToken);
            //var incomingSlots = request.Model.SlotList.SelectMany(day => day.SlotList).ToList();

            //var incomingIds = incomingSlots.Where(slot => slot.Id != null && slot.Id != 0).Select(slot => slot.Id).ToList();
            //var recordsToDelete = existingRecords.Where(record => !incomingIds.Contains(record.Id)).ToList();
            //recordsToDelete.ForEach(record => record.IsActive = false);

            //var newRecords = new List<ServiceSlotSettings>();
            //foreach (var slot in incomingSlots)
            //{
            //    if (slot.Id == null || slot.Id == 0)
            //    {
            //        // New record
            //        newRecords.Add(new ServiceSlotSettings
            //        {
            //            StartTime = slot.StartTime,
            //            EndTime = slot.EndTime,
            //            Day = slot.DayText,
            //            IsWholeDay = slot.IsWholeDay,
            //            IsActive = true,
            //            CreatedBy = _currentUserService.UserId,
            //            CreatedByName = _currentUserService.Username,
            //            CreatedOn = DateTime.Now
            //        });
            //    }
            //    else
            //    {
            //        // Existing record
            //        var existingRecord = existingRecords.SingleOrDefault(record => record.Id == slot.Id);
            //        if (existingRecord != null)
            //        {
            //            existingRecord.StartTime = slot.StartTime;
            //            existingRecord.EndTime = slot.EndTime;
            //            existingRecord.Day = slot.DayText;
            //            existingRecord.IsWholeDay = slot.IsWholeDay;
            //            existingRecord.IsActive = true;
            //            existingRecord.LastModifiedBy = _currentUserService.UserId;
            //            existingRecord.LastModifiedByName = _currentUserService.Username;
            //            existingRecord.LastModifiedOn = DateTime.Now;
            //        }
            //    }
            //}

            //// Apply changes to the database
            //if (recordsToDelete.Any())
            //{
            //    _context.ServiceSlotSettingss.UpdateRange(recordsToDelete);
            //}

            //if (newRecords.Any())
            //{
            //    await _context.ServiceSlotSettingss.AddRangeAsync(newRecords, cancellationToken);
            //}

            //await _context.SaveChangesAsync(cancellationToken);

           
        }
    }
}
