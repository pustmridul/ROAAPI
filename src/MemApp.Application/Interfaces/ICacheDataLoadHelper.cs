using MemApp.Application.Com.Models;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Application.Mem.Subscription.Model;

namespace MemApp.Application.Interfaces
{
    public interface ICacheDataLoadHelper
    {
        Task<IList<MemberRes>> GetAllMember();
        Task<IList<MessageTemplateReq>> GetAllMessageTemplate();
        Task<IList<BankReq>> GetAllBanks();
        Task<IList<CollegeDto>> GetAllColleges();
        Task<IList<BloodGroupReq>> GetAllBloodGroups();
        Task<IList<CreditCardReq>> GetAllCredits();
        Task<IList<PaymentMethodRes>> GetAllPaymentMethods();
        Task<IList<MemberShipFeeDto>> GetAllMemberShipFees();
        Task<IList<MemberProfessionDto>> GetAllMemberProfessions();
        Task<IList<MemberStatusDto>> GetAllMemberStatuss();
        Task<IList<MemberTypeDto>> GetAllMemberTypes();
        Task<IList<MiscItemReq>> GetAllMiscItems();
        Task<IList<MemServiceDto>> GetAllMemServices();
        Task<IList<SubscriptionFeeReq>> GetAllSubscriptionFees();



    }

}
