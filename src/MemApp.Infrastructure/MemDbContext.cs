using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using MemApp.Domain.Core.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities.mem;
using MemApp.Application.Interfaces;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.services;
using MemApp.Domain.Entities.subscription;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Entities.Payment;
using MemApp.Domain.Entities.Sale;
using Serilog;
using Serilog.Context;
using MemApp.Domain.Entities.Communication;
using MemApp.Application.Mem.Communication.Models;
using MemApp.Domain.Entities.Restaurant;
using Res.Domain.Entities;
using Res.Domain.Entities.ROAPayment;
using Res.Domain.Entities.ROASubscription;
using Res.Domain.Entities.RoaCommittee;

namespace MemApp.Infrastructure
{
    public class MemDbContext : DbContext, IMemDbContext
    {
        private readonly ICurrentUserService _authenticatedUser;



        public MemDbContext(DbContextOptions<MemDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _authenticatedUser = currentUserService;

        }

        #region com
        public DbSet<Bank> Banks => Set<Bank>();
        public DbSet<MessageTemplate> MessageTemplates => Set<MessageTemplate>();
        public DbSet<EmergencyInfo> EmergencyInfos => Set<EmergencyInfo>();
        public DbSet<Reply> Replys => Set<Reply>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<FeedbackCategory> FeedbackCategories => Set<FeedbackCategory>();
        

        public DbSet<SmsLog> SmsLogs => Set<SmsLog>();
        public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

        public DbSet<CreditCard> CreditCards => Set<CreditCard>();
        public DbSet<User> Users => Set<User>();

        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRoleMap> UserRoleMaps => Set<UserRoleMap>();
        public DbSet<UserMenuMap> UserMenuMaps => Set<UserMenuMap>();


        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<BloodGroup> BloodGroups => Set<BloodGroup>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<RolePermissionMap> RolePermissionMaps => Set<RolePermissionMap>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<TramsAndCondition> TramsAndConditions => Set<TramsAndCondition>();
        public DbSet<SSLCommerzValidator> SSLCommerzValidators => Set<SSLCommerzValidator>();



        #endregion
        #region mem
        public DbSet<AddOnsItem> AddOnsItems => Set<AddOnsItem>();
        public DbSet<MessageInbox> MessageInboxs => Set<MessageInbox>();
        public DbSet<Donation> Donations => Set<Donation>();
        public DbSet<Donate> Donates => Set<Donate>();


        public DbSet<VenueAddOnsItemDetail> VenueAddOnsItemDetails => Set<VenueAddOnsItemDetail>();

        public DbSet<AddOnsPriceHistory> AddOnsPriceHistorys => Set<AddOnsPriceHistory>();
        public DbSet<MemberType> MemberTypes => Set<MemberType>();

        public DbSet<MemberProfession> MemberProfessions => Set<MemberProfession>();
        public DbSet<MemberStatus> MemberStatuses => Set<MemberStatus>();
        public DbSet<MemberAddress> MemberAddresses => Set<MemberAddress>();
        public DbSet<RegisterMember> RegisterMembers => Set<RegisterMember>();
        public DbSet<MemberChildren> MemberChildrens => Set<MemberChildren>();
        public DbSet<CategoryPattern> CategoryPatterns => Set<CategoryPattern>();
        public DbSet<College> Colleges => Set<College>();
        public DbSet<MemberActiveStatus> MemberActiveStatuses => Set<MemberActiveStatus>();
        public DbSet<MemberShipFee> MemberShipFees => Set<MemberShipFee>();
        public DbSet<MemberFeesMap> MemberFeesMaps => Set<MemberFeesMap>();

        public DbSet<ServiceRecord> ServiceRecords => Set<ServiceRecord>();
        public DbSet<MemService> MemServices => Set<MemService>();
        public DbSet<ServiceTicket> ServiceTickets => Set<ServiceTicket>();
        public DbSet<ServiceTicketAvailability> ServiceTicketAvailabilities => Set<ServiceTicketAvailability>();
        public DbSet<ServiceTicketDetail> ServiceTicketDetails => Set<ServiceTicketDetail>();
        public DbSet<SubscriptionMode> SubscriptionModes => Set<SubscriptionMode>();
        public DbSet<ServiceAvailability> ServiceAvailabilities => Set<ServiceAvailability>();
        public DbSet<AvailabilityDetail> AvailabilityDetails => Set<AvailabilityDetail>();
        public DbSet<Availability> Availabilities => Set<Availability>();

        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<SlotMaster> SlotMasters => Set<SlotMaster>();
        public DbSet<ServiceSlotSettings> ServiceSlotSettingss => Set<ServiceSlotSettings>();

        public DbSet<AreaLayout> AreaLayouts => Set<AreaLayout>();
        public DbSet<AreaLayoutDetail> AreaLayoutDetails => Set<AreaLayoutDetail>();
        public DbSet<TableSetup> TableSetups => Set<TableSetup>();
        public DbSet<Committee> Committees => Set<Committee>();
        public DbSet<CommitteeDetail> CommitteeDetails => Set<CommitteeDetail>();
        public DbSet<BoardMeetingMinuet> BoardMeetingMinuets => Set<BoardMeetingMinuet>();
        public DbSet<CommitteeCategory> CommitteeCategories => Set<CommitteeCategory>();

        public DbSet<MemberSpouseTemp> MemberSpouseTemps => Set<MemberSpouseTemp>();

        public DbSet<MemberTemp> MemberTemps => Set<MemberTemp>();
        public DbSet<ServiceSale> ServiceSales => Set<ServiceSale>();
        //public DbSet<ServiceSaleDetail> ServiceSaleDetails => Set<ServiceSaleDetail>();
        public DbSet<ServiceSaleDetail> ServiceSalesDetails => Set<ServiceSaleDetail>();
        public DbSet<ServiceSaleAvailability> ServiceSaleAvailabilities => Set<ServiceSaleAvailability>();
        public DbSet<VenueBooking> VenueBookings => Set<VenueBooking>();
        public DbSet<VenueBookingDetail> VenueBookingDetails => Set<VenueBookingDetail>();
        public DbSet<SaleEventTicket> SaleEventTickets => Set<SaleEventTicket>();
        public DbSet<SaleEventTicketDetail> SaleEventTicketDetails => Set<SaleEventTicketDetail>();
        public DbSet<AdminNotification> AdminNotifications => Set<AdminNotification>();
        
        #endregion
        #region ServicesType
        public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
        #endregion
        #region SubscriptionFees
        public DbSet<SubscriptionFees> SubscriptionFees => Set<SubscriptionFees>();
        #endregion

        #region misc
        public DbSet<MiscItem> MiscItems => Set<MiscItem>();
        public DbSet<Notice> Notices => Set<Notice>();
        public DbSet<LiquorPermit> LiquorPermits => Set<LiquorPermit>();
     
        public DbSet<MiscSale> MiscSales => Set<MiscSale>();
        public DbSet<MiscSaleDetail> MiscSaleDetails => Set<MiscSaleDetail>();
        #endregion

        #region Restaurant
        public DbSet<RawMeterialGroup> RawMeterialGroups => Set<RawMeterialGroup>();
        public DbSet<RawMeterial> RawMeterials => Set<RawMeterial>();
        public DbSet<Department> Departments => Set<Department>();
        #endregion



        #region Division
        public DbSet<Division> Divisions => Set<Division>();
        public DbSet<District> Districts => Set<District>();
        public DbSet<ZoneInfo> ZoneInfos => Set<ZoneInfo>();
        public DbSet<Thana> Thanas => Set<Thana>();
        public DbSet<Municipality> Municipalities => Set<Municipality>();
        public DbSet<UnionInfo> UnionInfos => Set<UnionInfo>();
        public DbSet<Ward> Wards => Set<Ward>();
        #endregion

        #region Restaurant Owner Member
        public DbSet<MemberRegistrationInfo> MemberRegistrationInfos => Set<MemberRegistrationInfo>();
        public DbSet<MultipleOwner> MultipleOwners => Set<MultipleOwner>();
        public DbSet<RoaMemberCategory> RoaMemberCategories => Set<RoaMemberCategory>();
        #endregion


        #region ROA Subscription
        public DbSet<ROASubscriptionFee> ROASubscriptionFees => Set<ROASubscriptionFee>();

        public DbSet<RoSubscriptionDueTemp> RoSubscriptionDueTemps => Set<RoSubscriptionDueTemp>();

        public DbSet<ROASubscriptionPayment> ROASubscriptionPayments => Set<ROASubscriptionPayment>();
        public DbSet<RoaMembershipFeePayment> ROAMembershipFeePayments => Set<RoaMembershipFeePayment>();

        public DbSet<ROASubscriptionPaymentDetail> ROASubscriptionPaymentDetail => Set<ROASubscriptionPaymentDetail>();
        public DbSet<RoMemberLedger> RoMemberLedgers => Set<RoMemberLedger>();

        #endregion

        #region ROA Committee
        public DbSet<RoCommittee> RoCommittees => Set<RoCommittee>();
        public DbSet<RoCommitteeDetail> RoCommitteeDetails => Set<RoCommitteeDetail>();
        
        public DbSet<RoCommitteeCategory> RoCommitteeCategories => Set<RoCommitteeCategory>();
        #endregion
        public IDbContextTransaction BeginTransaction()
        {
            return Database.BeginTransaction();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await Database.BeginTransactionAsync(cancellationToken);
        }
        public IDbConnection Connection => Database.GetDbConnection();
        public bool HasChanges => ChangeTracker.HasChanges();

        public DbSet<ServiceTicketType> ServiceTicketTypes => Set<ServiceTicketType>();

        public DbSet<SaleMaster> SaleMasters => Set<SaleMaster>();

        public DbSet<SaleTicketDetail> SaleTicketDetails => Set<SaleTicketDetail>();

        public DbSet<SaleLayoutDetail> SaleLayoutDetails => Set<SaleLayoutDetail>();


        public DbSet<SubscriptionPayment> SubscriptionPayments => Set<SubscriptionPayment>();

        public DbSet<SubscriptionPaymentDetail> SubscriptionPaymentDetails => Set<SubscriptionPaymentDetail>();
        public DbSet<SubscriptionPaymentTemp> SubscriptionPaymentTemps => Set<SubscriptionPaymentTemp>();
        public DbSet<SubscriptionDueTemp> SubscriptionDueTemps => Set<SubscriptionDueTemp>();




        public DbSet<TopUp> TopUps => Set<TopUp>();
        public DbSet<TopUpDetail> TopUpDetails => Set<TopUpDetail>();

        public DbSet<MemTransaction> MemTransactions => Set<MemTransaction>();
        public DbSet<MemFiles> MemFiles => Set<MemFiles>();
        public DbSet<FileInformation> FileInformations => Set<FileInformation>();
        

        public DbSet<AreaTableMatrix> AreaTableMatrixs => Set<AreaTableMatrix>();
        public DbSet<MemLedger> MemLedgers => Set<MemLedger>();
        public DbSet<SerTicketAreaLayout> SerTicketAreaLayouts => Set<SerTicketAreaLayout>();
        public DbSet<SerTicketAreaLayoutMatrix> SerTicketAreaLayoutMatrices => Set<SerTicketAreaLayoutMatrix>();
        public DbSet<UserLog> UserLogs => Set<UserLog>();
        public DbSet<UserConference> UserConferences => Set<UserConference>();
        public DbSet<TicketFiles> TicketFiless => Set<TicketFiles>();
        public DbSet<MemberEducation> MemberEducations => Set<MemberEducation>();
        public DbSet<EventToken> EventTokens => Set<EventToken>();
        public DbSet<ReleaseVersion> ReleaseVersions => Set<ReleaseVersion>();

        public DbSet<VenueBlockedSetup> VenueBlockedSetups => Set<VenueBlockedSetup>();
        public DbSet<NotificationToken> NotificationTokens => Set<NotificationToken>();
        public DbSet<NotificationEmail> NotificationEmails => Set<NotificationEmail>();

        //    public DbSet<TicketTableChairMatrix> TicketTableChairMatrixs => Set<TicketTableChairMatrix>();





        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>().ToList())
            {


                var originalValues = entry.OriginalValues;
                var currentValues = entry.CurrentValues;
                var logMessage = new List<string>();

                switch (entry.State)
                {

                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.Now;
                        entry.Entity.CreatedBy = Convert.ToInt32(_authenticatedUser.UserId);
                        entry.Entity.CreatedByName = _authenticatedUser.Username;

                        //foreach (var property in originalValues.Properties)
                        //{
                        //    var original = originalValues[property];
                        //    var current = currentValues[property];

                        //    if (!object.Equals(original, current))
                        //    {
                        //        logMessage.Add($"Entity {entry.Entity.GetType().Name}, Property {property.Name} has been modified. Old value: {original}, New value: {current}");
                        //    }
                        //}
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTime.Now;
                        entry.Entity.LastModifiedBy = Convert.ToInt32(_authenticatedUser.UserId);
                        entry.Entity.LastModifiedByName = _authenticatedUser.Username;

                        foreach (var property in originalValues.Properties)
                        {
                            var original = originalValues[property];
                            var current = currentValues[property];

                            if (!object.Equals(original, current))
                            {
                                logMessage.Add($"Entity {entry.Entity.GetType().Name}, Property {property.Name} has been modified. Old value: {original}, New value: {current}");
                            }
                        }

                        using (LogContext.PushProperty("UserId", _authenticatedUser.UserId))
                        {
                            Log.Information(string.Join(',', logMessage));
                        }

                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
        public async Task<int> SaveAsyncOnly()
        {          
            return await SaveChangesAsync();
        }
        public async Task<int> SaveAsync()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _authenticatedUser.UserId;
                        entry.Entity.CreatedByName = _authenticatedUser.Username;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = _authenticatedUser.UserId;
                        entry.Entity.LastModifiedByName = _authenticatedUser.Username;
                        break;
                }
            }
            return await SaveChangesAsync();
        }
        public int Save()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        break;
                }
            }
            return SaveChanges();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(MemDbContext).Assembly);

            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,3)");
            }
            base.OnModelCreating(builder);



        }

        //public async Task<List<T>> SelectByQueryAsync(string sql, params object[] parameters)
        //{
        //    //string sql,params object[] parameters

        //    //await _dbContext.Set<T>().AddAsync(entity);
        //    if (parameters.Length > 0)
        //        return MemDbContext.Set<T>().FromSqlRaw<T>(sql, parameters).ToList();
        //    else
        //        return _dbContext.Set<T>().FromSqlRaw<T>(sql).ToList();


        //}
    }
}