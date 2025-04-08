using AutoMapper;
using MemApp.Application.App.Models;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.Service.Model;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Entities.subscription;

namespace MemApp.Application.Helper
{
    public class AutomapperProfile : Profile
    {
        //source --->destination
        public AutomapperProfile()
        {
            CreateMap<MemberChildrenReq, MemberChildren>();
            CreateMap<RegisterMember, RegisterMemberRes>();
            CreateMap<SpouseRegistrationReq, RegisterMember>();
            CreateMap<MemberFeesMapReq, MemberFeesMap>();
            CreateMap<SubscriptionFees, SubscriptionFeeDto>();
            CreateMap<RegisterMember, MemberLoginRes>();
            CreateMap<RegisterMember, MemberSearchRes>();
            CreateMap<AvailabilityDetail,AvailabilityDetailVm > ();


        }
    }
}
