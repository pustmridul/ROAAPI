using MediatR;
using MemApp.Application;
using MemApp.Application.com.Queries.GetAllMessageTemplate;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Application.Mem.Subscription.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MemApp.Infrastructure.CacheData
{
    public class CacheDataLoadHelper : ICacheDataLoadHelper
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISender _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemDbContext _context;
        public CacheDataLoadHelper(ISender mediator, IMemoryCache memoryCache, ICurrentUserService currentUserService, IMemDbContext context)
        {
            _mediator = mediator;
            _memoryCache = memoryCache;
            _currentUserService = currentUserService;
            _context = context;

        }

        public Task<IList<BankReq>> GetAllBanks()
        {
            throw new NotImplementedException();
        }

        public Task<IList<BloodGroupReq>> GetAllBloodGroups()
        {
            throw new NotImplementedException();
        }

        public Task<IList<CollegeDto>> GetAllColleges()
        {
            throw new NotImplementedException();
        }

        public Task<IList<CreditCardReq>> GetAllCredits()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<MemberRes>> GetAllMember()
        {
            if (_memoryCache.TryGetValue(StaticData.CacheKey.GetMemberList, out IList<MemberRes> dataList))
            {
                return dataList;
            }
            else
            {
                var data = await _context.RegisterMembers.Where(q => q.IsActive).AsNoTracking().ToListAsync();
                dataList = data.Select(s => new MemberRes
                {
                    Id = s.Id,
                    MemberShipNo = s.MembershipNo,
                    Name = s.FullName,
                    CadetName = s.CadetName,
                    CadetNo = s.CadetNo,
                    BatchNo = s.BatchNo,
                    Dbo = s.Dob,
                    Phone = s.Phone,
                    CardNo = s.CardNo,
                    Email = s.Email,
                    MemberFullId = s.MemberFullId,
                    JoinDate = s.JoinDate,
                    LeaveDate = s.LeaveDate,
                    PermanentAddress = s.PermanentAddress,
                    NID = s.NID,
                    EmergencyContact = s.EmergencyContact,
                    Anniversary = s.Anniversary,
                    IsMasterMember = s.IsMasterMember,
                }).ToList();

                _memoryCache.Set(StaticData.CacheKey.GetMemberList, dataList, StaticData.CacheKey.cacheEntryOptions);
            }

            //var result = await _mediator.Send(new GetMemberSearchQuery()
            //    {
            //        PageSize =10000,
            //        PageNumber = 1,

            //    });

            //if (result.HasError == true || result == null)
            //{
            //    throw new Exception("Member list not found");
            //}

            //dataList = result.DataList;



            return dataList;

        }

        public Task<IList<MemberProfessionDto>> GetAllMemberProfessions()
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberShipFeeDto>> GetAllMemberShipFees()
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberStatusDto>> GetAllMemberStatuss()
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberTypeDto>> GetAllMemberTypes()
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemServiceDto>> GetAllMemServices()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<MessageTemplateReq>> GetAllMessageTemplate()
        {
            if (_memoryCache.TryGetValue(StaticData.CacheKey.GetMessageTemplate, out IList<MessageTemplateReq> dataList))
            {
                return dataList;
            }
            else
            {
                var result = await _mediator.Send(new GetAllMessageTemplateQuery()
                {

                });

                if (result.HasError == true || result == null)
                {

                    throw new Exception("Message template list not found");
                }


                dataList = result.Data;

                //  _memoryCache.Set(StaticData.CacheKey.GetMessageTemplate, dataList, StaticData.CacheKey.cacheEntryOptions);

                return dataList;
            }
        }

        public Task<IList<MiscItemReq>> GetAllMiscItems()
        {
            throw new NotImplementedException();
        }

        public Task<IList<PaymentMethodRes>> GetAllPaymentMethods()
        {
            throw new NotImplementedException();
        }

        public Task<IList<SubscriptionFeeReq>> GetAllSubscriptionFees()
        {
            throw new NotImplementedException();
        }
    }
}
