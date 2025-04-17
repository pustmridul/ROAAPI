using MemApp.Application.PaymentGateway.SslCommerz.Model;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.Communication;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Payment;
using MemApp.Domain.Entities.Restaurant;
using MemApp.Domain.Entities.Sale;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Entities.services;
using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Res.Domain.Entities;
using Res.Domain.Entities.RoaCommittee;
using Res.Domain.Entities.ROAPayment;
using Res.Domain.Entities.ROASubscription;
using System.Data;



namespace MemApp.Application.Interfaces.Contexts
{
    public interface IMemDbContext
    {
        #region com
        DbSet<Bank> Banks { get; }
        DbSet<MessageTemplate> MessageTemplates { get; }

        DbSet<EmergencyInfo> EmergencyInfos { get; }
        DbSet<Reply> Replys { get; }
        DbSet<Feedback> Feedbacks { get; }
        DbSet<FeedbackCategory> FeedbackCategories { get; }


        DbSet<SmsLog> SmsLogs { get; }
        DbSet<EmailLog> EmailLogs { get; }


        DbSet<CreditCard> CreditCards { get; }

        DbSet<User> Users { get; }
        DbSet<UserPermission> UserPermissions { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRoleMap> UserRoleMaps { get; }
        DbSet<UserMenuMap> UserMenuMaps { get; }
        DbSet<NotificationToken> NotificationTokens { get; }

        DbSet<Menu> Menus { get; }
        DbSet<BloodGroup> BloodGroups { get; }
        DbSet<PaymentMethod> PaymentMethods { get; }
        DbSet<RolePermissionMap> RolePermissionMaps { get; }
        DbSet<TramsAndCondition> TramsAndConditions { get; }
        DbSet<ReleaseVersion> ReleaseVersions { get; }

        DbSet<SSLCommerzValidator> SSLCommerzValidators { get; }

        #endregion
        #region mem
        DbSet<MessageInbox> MessageInboxs { get; }
        DbSet<Donate> Donates { get; }
        DbSet<Donation> Donations { get; }
        DbSet<AddOnsItem> AddOnsItems { get; }
        DbSet<VenueAddOnsItemDetail> VenueAddOnsItemDetails { get; }

        DbSet<AddOnsPriceHistory> AddOnsPriceHistorys { get; }

        DbSet<MemberType> MemberTypes { get; }
        DbSet<MemberProfession> MemberProfessions { get; }
        DbSet<MemberShipFee> MemberShipFees { get; }
        DbSet<MemberFeesMap> MemberFeesMaps { get; }
        DbSet<MemberStatus> MemberStatuses { get; }
        DbSet<MemberActiveStatus> MemberActiveStatuses { get; }
        DbSet<MemberAddress> MemberAddresses { get; }
        DbSet<RegisterMember> RegisterMembers { get; }
        DbSet<MemberChildren> MemberChildrens { get; }
        DbSet<College> Colleges { get; }
        DbSet<CategoryPattern> CategoryPatterns { get; }
        DbSet<MemService> MemServices { get; }
        DbSet<ServiceTicket> ServiceTickets { get; }
        DbSet<ServiceTicketAvailability> ServiceTicketAvailabilities { get; }

        DbSet<SlotMaster> SlotMasters { get; }
        DbSet<ServiceSlotSettings> ServiceSlotSettingss { get; }


        DbSet<ServiceTicketDetail> ServiceTicketDetails { get; }
        DbSet<ServiceAvailability> ServiceAvailabilities { get; }
        DbSet<ServiceTicketType> ServiceTicketTypes { get; }
        DbSet<SaleMaster> SaleMasters { get; }
        DbSet<SaleTicketDetail> SaleTicketDetails { get; }

        DbSet<SaleLayoutDetail> SaleLayoutDetails { get; }
        DbSet<Attendance> Attendances { get; }
        DbSet<AreaLayout> AreaLayouts { get; }
        DbSet<AreaLayoutDetail> AreaLayoutDetails { get; }
        DbSet<TableSetup> TableSetups { get; }
        DbSet<AreaTableMatrix> AreaTableMatrixs { get; }
        DbSet<Committee> Committees { get; }
        DbSet<CommitteeDetail> CommitteeDetails { get; }
        DbSet<BoardMeetingMinuet> BoardMeetingMinuets { get; }
        DbSet<CommitteeCategory> CommitteeCategories { get; }
        DbSet<MemLedger> MemLedgers { get; }
        DbSet<SerTicketAreaLayout> SerTicketAreaLayouts { get; }
        DbSet<SerTicketAreaLayoutMatrix> SerTicketAreaLayoutMatrices { get; }
        DbSet<UserLog> UserLogs { get; }
        DbSet<UserConference> UserConferences { get; }
        DbSet<MemberTemp> MemberTemps { get; }
        DbSet<MemberEducation> MemberEducations { get; }
        DbSet<ServiceSale> ServiceSales { get; }
        DbSet<ServiceSaleDetail> ServiceSalesDetails { get; }

        DbSet<VenueBooking> VenueBookings { get; }
        DbSet<VenueBookingDetail> VenueBookingDetails { get; }
        DbSet<EventToken> EventTokens { get; }
        DbSet<ServiceSaleAvailability> ServiceSaleAvailabilities { get; }
        DbSet<SaleEventTicket> SaleEventTickets { get; }
        DbSet<SaleEventTicketDetail> SaleEventTicketDetails { get; }
        DbSet<AdminNotification> AdminNotifications { get; }
        DbSet<NotificationEmail> NotificationEmails { get; }
        #endregion
        #region Service
        DbSet<ServiceRecord> ServiceRecords { get; }
        DbSet<ServiceType> ServiceTypes { get; }
        DbSet<Availability> Availabilities { get; }
        DbSet<AvailabilityDetail> AvailabilityDetails { get; }
        DbSet<TicketFiles> TicketFiless { get; }

        DbSet<VenueBlockedSetup> VenueBlockedSetups { get; }


        #endregion

        #region Act
        DbSet<SubscriptionPayment> SubscriptionPayments { get; }
        DbSet<SubscriptionPaymentDetail> SubscriptionPaymentDetails { get; }
        DbSet<SubscriptionPaymentTemp> SubscriptionPaymentTemps { get; }
        DbSet<SubscriptionDueTemp> SubscriptionDueTemps { get; }


        DbSet<TopUp> TopUps { get; }
        DbSet<TopUpDetail> TopUpDetails { get; }

        DbSet<MemTransaction> MemTransactions { get; }
        DbSet<MemFiles> MemFiles { get; }
        DbSet<FileInformation> FileInformations { get; }



        #endregion

        #region subscription
        DbSet<SubscriptionFees> SubscriptionFees { get; }
        DbSet<SubscriptionMode> SubscriptionModes { get; }

        // DbSet<TicketTableChairMatrix> TicketTableChairMatrixs { get; }
        #endregion

        #region misc
        DbSet<MiscItem> MiscItems { get; }
        DbSet<Notice> Notices { get; }
        DbSet<LiquorPermit> LiquorPermits { get; }

        DbSet<MiscSale> MiscSales { get; }
        DbSet<MiscSaleDetail> MiscSaleDetails { get; }
        #endregion

        #region Restaurant
        DbSet<RawMeterialGroup> RawMeterialGroups { get; }
        DbSet<RawMeterial> RawMeterials { get; }
        DbSet<Department> Departments { get; }

        #endregion


        #region Division
        DbSet<Division> Divisions { get; }
        DbSet<District> Districts { get; }
        DbSet<ZoneInfo> ZoneInfos { get; }
        DbSet<Thana> Thanas { get; }
        DbSet<Municipality> Municipalities { get; }
        DbSet<UnionInfo> UnionInfos { get; }
        DbSet<Ward> Wards { get; }
        #endregion

        #region Restaurant Owner Member
        DbSet<MemberRegistrationInfo> MemberRegistrationInfos {  get; }
        DbSet<MultipleOwner> MultipleOwners {  get; }
        #endregion

        #region ROA Subscription

        DbSet<ROASubscriptionFee> ROASubscriptionFees { get; }

        DbSet<RoSubscriptionDueTemp> RoSubscriptionDueTemps { get; }

        DbSet<ROASubscriptionPayment> ROASubscriptionPayments { get; }
        DbSet<RoaMembershipFeePayment> ROAMembershipFeePayments { get; }

        DbSet<ROASubscriptionPaymentDetail> ROASubscriptionPaymentDetail { get; }
        DbSet<RoMemberLedger> RoMemberLedgers { get; }

        #endregion

        #region ROA Committee
        DbSet<RoCommittee> RoCommittees { get; }
        DbSet<RoCommitteeDetail> RoCommitteeDetails { get; }
       // DbSet<BoardMeetingMinuet> BoardMeetingMinuets { get; }
        DbSet<RoCommitteeCategory> RoCommitteeCategories { get; }
        #endregion

        IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        IDbConnection Connection { get; }
        bool HasChanges { get; }
        EntityEntry Entry(object entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveAsync();
        Task<int> SaveAsyncOnly();

        //Task<List<T>> SelectByQueryAsync(string sql, params object[] parameters);
        int Save();

    }
}