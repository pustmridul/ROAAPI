using Dapper;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Attendances.Model;
using MemApp.Application.Models;
using MemApp.Application.Models.Requests;
using MemApp.Application.Services;
using MemApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MemApp.Application.Com.Queries.GetRolePermission
{
    public class GetRolePermissionQuery : IRequest<RolePermissionRes>
    {
        public int RoleId { get; set; }
    }

    public class GetRolePermissionQueryHandler : IRequestHandler<GetRolePermissionQuery, RolePermissionRes>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;

        public GetRolePermissionQueryHandler(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<RolePermissionRes> Handle(GetRolePermissionQuery request, CancellationToken cancellationToken)
        {
            var result = new RolePermissionRes();

            var role = await _context.Roles.FirstOrDefaultAsync(q => q.Id == request.RoleId, cancellationToken);
            if (role == null)
            {
                result.HasError = true;
                result.Messages.Add("Role Id NotFound");
            }

           // var permissions = await _context.Permissions.Where(x=>x.IsActive).ToListAsync(cancellationToken);
            var rolePermission = await _context.RolePermissionMaps.Where(q => q.RoleId == role!.Id && q.IsActive).Select(s => s.PermissionNo).ToListAsync(cancellationToken);

            var permissions = new List<PermissionDetailVm>();

            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("select * from VW_DailyAttendance ");

                    string sql = @"
                            SELECT 
                                p.ModuleName Name,
                                p.OperationName Operation,
                                p.PermissionNo,
                                CASE WHEN rpm.PermissionNo IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsChecked
                            FROM Permissions p
                            LEFT JOIN com_RolePermissionMap rpm 
                                ON rpm.PermissionNo = p.PermissionNo 
                                AND rpm.RoleId = @RoleId 
                                AND rpm.IsActive = 1
                            WHERE p.IsActive = 1
                            ORDER BY p.ModuleName, p.PermissionNo;";

                    var existPermission = await connection
                        .QueryAsync<PermissionDetailVm>(sql, new { RoleId = request.RoleId });
                    permissions = existPermission.ToList();

                }

                var permissionList = permissions
                                    .GroupBy(p => p.Name)
                                    .Select(group => new PermissionVm
                                    {
                                        Title = group.Key,
                                        PermissionNo = 0, // or assign a group-level PermissionNo if needed
                                        IsChecked = group.Any(x => x.IsChecked),
                                        PermissionDetailVms = group.Select(x => new PermissionDetailVm
                                        {
                                            Name = x.Name,
                                            Operation = x.Operation,
                                            PermissionNo = x.PermissionNo,
                                            IsChecked = x.IsChecked
                                        }).ToList()
                                    }).ToList();

                result.PermissionList = permissionList;

                result.RoleId = role!.Id;
                result.Name = role.Name;
            }
            catch (Exception ex)
            {

            }

            return result;




            var user = new PermissionVm()
            {
                Title = "User",
                PermissionNo = 10,
                IsChecked = rolePermission.Where(q => q == 10).Any()

            };
            user.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1001, IsChecked = rolePermission.Where(q => q == 1001).Any() });
            user.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1002, IsChecked = rolePermission.Where(q => q == 1002).Any() });
            user.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1003, IsChecked = rolePermission.Where(q => q == 1003).Any() });
            user.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1004, IsChecked = rolePermission.Where(q => q == 1004).Any() });

            result.PermissionList.Add(user);

            var memService = new PermissionVm()
            {
                Title = "Member Service",
                PermissionNo = 11,
                IsChecked = rolePermission.Where(q => q == 11).Any()
            };
            memService.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1101, IsChecked = rolePermission.Where(q => q == 1101).Any() });
            memService.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1102, IsChecked = rolePermission.Where(q => q == 1102).Any() });
            memService.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1103, IsChecked = rolePermission.Where(q => q == 1103).Any() });
            memService.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1104, IsChecked = rolePermission.Where(q => q == 1104).Any() });

            result.PermissionList.Add(memService);

            //var collegeSetup = new PermissionVm()
            //{
            //    Title = "College Setup",
            //    PermissionNo = 12,
            //    IsChecked = rolePermission.Where(q => q == 12).Any()
            //};
            //collegeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1201, IsChecked = rolePermission.Where(q => q == 1201).Any() });
            //collegeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1202, IsChecked = rolePermission.Where(q => q == 1202).Any() });
            //collegeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1203, IsChecked = rolePermission.Where(q => q == 1203).Any() });
            //collegeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1204, IsChecked = rolePermission.Where(q => q == 1204).Any() });
            //result.PermissionList.Add(collegeSetup);


            //var memberTypeSetup = new PermissionVm()
            //{
            //    Title = "Member TypeSetup",
            //    PermissionNo = 13,
            //    IsChecked = rolePermission.Where(q => q == 13).Any()
            //};
            //memberTypeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1301, IsChecked = rolePermission.Where(q => q == 1301).Any() });
            //memberTypeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1302, IsChecked = rolePermission.Where(q => q == 1302).Any() });
            //memberTypeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1303, IsChecked = rolePermission.Where(q => q == 1303).Any() });
            //memberTypeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1304, IsChecked = rolePermission.Where(q => q == 1304).Any() });
            //result.PermissionList.Add(memberTypeSetup);

            //var memberStatusSetup = new PermissionVm()
            //{
            //    Title = "Member Status Setup",
            //    PermissionNo = 14,
            //    IsChecked = rolePermission.Where(q => q == 14).Any()
            //};
            //memberStatusSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1401, IsChecked = rolePermission.Where(q => q == 1401).Any() });
            //memberStatusSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1402, IsChecked = rolePermission.Where(q => q == 1402).Any() });
            //memberStatusSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1403, IsChecked = rolePermission.Where(q => q == 1403).Any() });
            //memberStatusSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1404, IsChecked = rolePermission.Where(q => q == 1404).Any() });
            //result.PermissionList.Add(memberStatusSetup);


            //var memberProfessionSetup = new PermissionVm()
            //{
            //    Title = "Member Profession Setup",
            //    PermissionNo = 15,
            //    IsChecked = rolePermission.Where(q => q == 15).Any()
            //};
            //memberProfessionSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1501, IsChecked = rolePermission.Where(q => q == 1501).Any() });
            //memberProfessionSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1502, IsChecked = rolePermission.Where(q => q == 1502).Any() });
            //memberProfessionSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1503, IsChecked = rolePermission.Where(q => q == 1503).Any() });
            //memberProfessionSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1504, IsChecked = rolePermission.Where(q => q == 1504).Any() });
            //result.PermissionList.Add(memberProfessionSetup);


            //var memberShipFeesSetup = new PermissionVm()
            //{
            //    Title = "MemberShip Fees Setup",
            //    PermissionNo = 16,
            //    IsChecked = rolePermission.Where(q => q == 16).Any()
            //};
            //memberShipFeesSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1601, IsChecked = rolePermission.Where(q => q == 1601).Any() });
            //memberShipFeesSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1602, IsChecked = rolePermission.Where(q => q == 1602).Any() });
            //memberShipFeesSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1603, IsChecked = rolePermission.Where(q => q == 1603).Any() });
            //memberShipFeesSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1604, IsChecked = rolePermission.Where(q => q == 1604).Any() });
            //result.PermissionList.Add(memberShipFeesSetup);


            //var memberActiveStatusSetup = new PermissionVm()
            //{
            //    Title = "Member Active Status Setup",
            //    PermissionNo = 17,
            //    IsChecked = rolePermission.Where(q => q == 17).Any()
            //};
            //memberActiveStatusSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1701, IsChecked = rolePermission.Where(q => q == 1701).Any() });
            //memberActiveStatusSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1702, IsChecked = rolePermission.Where(q => q == 1702).Any() });
            //memberActiveStatusSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1703, IsChecked = rolePermission.Where(q => q == 1703).Any() });
            //memberActiveStatusSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1704, IsChecked = rolePermission.Where(q => q == 1704).Any() });
            //result.PermissionList.Add(memberActiveStatusSetup);

            /////
            ///User Setup


            var roleSetup = new PermissionVm()
            {
                Title = "Role Setup",
                PermissionNo = 18,
                IsChecked = rolePermission.Where(q => q == 18).Any()
            };
            roleSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1801, IsChecked = rolePermission.Where(q => q == 1801).Any() });
            roleSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1802, IsChecked = rolePermission.Where(q => q == 1802).Any() });
            roleSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1803, IsChecked = rolePermission.Where(q => q == 1803).Any() });
            roleSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1804, IsChecked = rolePermission.Where(q => q == 1804).Any() });
            result.PermissionList.Add(roleSetup);



            //var serviceTicketTypeSetup = new PermissionVm()
            //{
            //    Title = "Service Ticket Type Setup",
            //    PermissionNo = 19,
            //    IsChecked = rolePermission.Where(q => q == 19).Any()
            //};
            //serviceTicketTypeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 1901, IsChecked = rolePermission.Where(q => q == 1901).Any() });
            //serviceTicketTypeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 1902, IsChecked = rolePermission.Where(q => q == 1902).Any() });
            //serviceTicketTypeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 1903, IsChecked = rolePermission.Where(q => q == 1903).Any() });
            //serviceTicketTypeSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 1904, IsChecked = rolePermission.Where(q => q == 1904).Any() });
            //result.PermissionList.Add(serviceTicketTypeSetup);


            //var availabilitySetup = new PermissionVm()
            //{
            //    Title = "Availability Setup",
            //    PermissionNo = 20,
            //    IsChecked = rolePermission.Where(q => q == 20).Any()
            //};
            //availabilitySetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2001, IsChecked = rolePermission.Where(q => q == 2001).Any() });
            //availabilitySetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2002, IsChecked = rolePermission.Where(q => q == 2002).Any() });
            //availabilitySetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2003, IsChecked = rolePermission.Where(q => q == 2003).Any() });
            //availabilitySetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2004, IsChecked = rolePermission.Where(q => q == 2004).Any() });
            //result.PermissionList.Add(availabilitySetup);




            //var areaLayoutSetup = new PermissionVm()
            //{
            //    Title = "Area Layout Setup",
            //    PermissionNo = 21,
            //    IsChecked = rolePermission.Where(q => q == 21).Any()
            //};
            //areaLayoutSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2101, IsChecked = rolePermission.Where(q => q == 2101).Any() });
            //areaLayoutSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2102, IsChecked = rolePermission.Where(q => q == 2102).Any() });
            //areaLayoutSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2103, IsChecked = rolePermission.Where(q => q == 2103).Any() });
            //areaLayoutSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2104, IsChecked = rolePermission.Where(q => q == 2104).Any() });
            //result.PermissionList.Add(areaLayoutSetup);


            //var tableSetup = new PermissionVm()
            //{
            //    Title = "Table Setup",
            //    PermissionNo = 22,
            //    IsChecked = rolePermission.Where(q => q == 22).Any()
            //};
            //tableSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2201, IsChecked = rolePermission.Where(q => q == 2201).Any() });
            //tableSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2202, IsChecked = rolePermission.Where(q => q == 2202).Any() });
            //tableSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2203, IsChecked = rolePermission.Where(q => q == 2203).Any() });
            //tableSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2204, IsChecked = rolePermission.Where(q => q == 2204).Any() });
            //result.PermissionList.Add(tableSetup);

            ///Member

            var memberList = new PermissionVm()
            {
                Title = "Member List",
                PermissionNo = 23,
                IsChecked = rolePermission.Where(q => q == 23).Any()
            };
            memberList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2301, IsChecked = rolePermission.Where(q => q == 2301).Any() });
            memberList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2302, IsChecked = rolePermission.Where(q => q == 2302).Any() });
            memberList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2303, IsChecked = rolePermission.Where(q => q == 2303).Any() });
            memberList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2304, IsChecked = rolePermission.Where(q => q == 2304).Any() });
            result.PermissionList.Add(memberList);


            var memberRegistration = new PermissionVm()
            {
                Title = "Member Registration",
                PermissionNo = 24,
                IsChecked = rolePermission.Where(q => q == 24).Any()
            };
            memberRegistration.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2401, IsChecked = rolePermission.Where(q => q == 2401).Any() });
            memberRegistration.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2402, IsChecked = rolePermission.Where(q => q == 2402).Any() });
            memberRegistration.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2403, IsChecked = rolePermission.Where(q => q == 2403).Any() });
            memberRegistration.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2404, IsChecked = rolePermission.Where(q => q == 2404).Any() });
            result.PermissionList.Add(memberRegistration);


            //var topupList = new PermissionVm()
            //{
            //    Title = "Topup List",
            //    PermissionNo = 25,
            //    IsChecked = rolePermission.Where(q => q == 25).Any()
            //};
            //topupList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2501, IsChecked = rolePermission.Where(q => q == 2501).Any() });
            //topupList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2502, IsChecked = rolePermission.Where(q => q == 2502).Any() });
            //topupList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2503, IsChecked = rolePermission.Where(q => q == 2503).Any() });
            //topupList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2504, IsChecked = rolePermission.Where(q => q == 2504).Any() });
            //result.PermissionList.Add(topupList);


            //var memberTemp = new PermissionVm()
            //{
            //    Title = "Member Temp",
            //    PermissionNo = 26,
            //    IsChecked = rolePermission.Where(q => q == 26).Any()
            //};
            //memberTemp.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2601, IsChecked = rolePermission.Where(q => q == 2601).Any() });
            //memberTemp.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2602, IsChecked = rolePermission.Where(q => q == 2602).Any() });
            //memberTemp.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2603, IsChecked = rolePermission.Where(q => q == 2603).Any() });
            //memberTemp.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2604, IsChecked = rolePermission.Where(q => q == 2604).Any() });
            //result.PermissionList.Add(memberTemp);

            ////
            /// Subscription 


            //var subscriptionCharge = new PermissionVm()
            //{
            //    Title = "Subscription Charge",
            //    PermissionNo = 27,
            //    IsChecked = rolePermission.Where(q => q == 27).Any()
            //};
            //subscriptionCharge.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2701, IsChecked = rolePermission.Where(q => q == 2701).Any() });
            //subscriptionCharge.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2702, IsChecked = rolePermission.Where(q => q == 2702).Any() });
            //subscriptionCharge.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2703, IsChecked = rolePermission.Where(q => q == 2703).Any() });
            //subscriptionCharge.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2704, IsChecked = rolePermission.Where(q => q == 2704).Any() });
            //result.PermissionList.Add(subscriptionCharge);

            //////
            //// Service


            //var serviceSetup = new PermissionVm()
            //{
            //    Title = "Service Setup",
            //    PermissionNo = 28,
            //    IsChecked = rolePermission.Where(q => q == 28).Any()
            //};
            //serviceSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2801, IsChecked = rolePermission.Where(q => q == 2801).Any() });
            //serviceSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2802, IsChecked = rolePermission.Where(q => q == 2802).Any() });
            //serviceSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2803, IsChecked = rolePermission.Where(q => q == 2803).Any() });
            //serviceSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2804, IsChecked = rolePermission.Where(q => q == 2804).Any() });
            //result.PermissionList.Add(serviceSetup);


            //var serviceTicket = new PermissionVm()
            //{
            //    Title = "Service Ticket",
            //    PermissionNo = 29,
            //    IsChecked = rolePermission.Where(q => q == 29).Any()
            //};
            //serviceTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 2901, IsChecked = rolePermission.Where(q => q == 2901).Any() });
            //serviceTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 2902, IsChecked = rolePermission.Where(q => q == 2902).Any() });
            //serviceTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 2903, IsChecked = rolePermission.Where(q => q == 2903).Any() });
            //serviceTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 2904, IsChecked = rolePermission.Where(q => q == 2904).Any() });
            //serviceTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Status", PermissionNo = 2905, IsChecked = rolePermission.Where(q => q == 2905).Any() });

            //result.PermissionList.Add(serviceTicket);



            //var manageServiceSetup = new PermissionVm()
            //{
            //    Title = "Manage Service Setup",
            //    PermissionNo = 30,
            //    IsChecked = rolePermission.Where(q => q == 30).Any()
            //};
            //manageServiceSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3001, IsChecked = rolePermission.Where(q => q == 3001).Any() });
            //manageServiceSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3002, IsChecked = rolePermission.Where(q => q == 3002).Any() });
            //manageServiceSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3003, IsChecked = rolePermission.Where(q => q == 3003).Any() });
            //manageServiceSetup.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3004, IsChecked = rolePermission.Where(q => q == 3004).Any() });
            //result.PermissionList.Add(manageServiceSetup);


            //////
            ///// Sale

            //var saleList = new PermissionVm()
            //{
            //    Title = "Sale List",
            //    PermissionNo = 31,
            //    IsChecked = rolePermission.Where(q => q == 31).Any()
            //};
            //saleList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3101, IsChecked = rolePermission.Where(q => q == 3101).Any() });
            //saleList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3102, IsChecked = rolePermission.Where(q => q == 3102).Any() });
            //saleList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3103, IsChecked = rolePermission.Where(q => q == 3103).Any() });
            //saleList.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3104, IsChecked = rolePermission.Where(q => q == 3104).Any() });
            //result.PermissionList.Add(saleList);

            //////
            ///// Committee

            //var executiveCommittee = new PermissionVm()
            //{
            //    Title = "Executive Committee",
            //    PermissionNo = 32,
            //    IsChecked = rolePermission.Where(q => q == 32).Any()
            //};
            //executiveCommittee.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3201, IsChecked = rolePermission.Where(q => q == 3201).Any() });
            //executiveCommittee.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3202, IsChecked = rolePermission.Where(q => q == 3202).Any() });
            //executiveCommittee.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3203, IsChecked = rolePermission.Where(q => q == 3203).Any() });
            //executiveCommittee.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3204, IsChecked = rolePermission.Where(q => q == 3204).Any() });
            //result.PermissionList.Add(executiveCommittee);


            //var subCommittee = new PermissionVm()
            //{
            //    Title = "Sub Committee",
            //    PermissionNo = 33,
            //    IsChecked = rolePermission.Where(q => q == 33).Any()
            //};
            //subCommittee.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3301, IsChecked = rolePermission.Where(q => q == 3301).Any() });
            //subCommittee.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3302, IsChecked = rolePermission.Where(q => q == 3302).Any() });
            //subCommittee.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3303, IsChecked = rolePermission.Where(q => q == 3303).Any() });
            //subCommittee.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3304, IsChecked = rolePermission.Where(q => q == 3304).Any() });
            //result.PermissionList.Add(subCommittee);



            //var committeeCategory = new PermissionVm()
            //{
            //    Title = "Committee Category",
            //    PermissionNo = 34,
            //    IsChecked = rolePermission.Where(q => q == 34).Any()
            //};
            //committeeCategory.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3401, IsChecked = rolePermission.Where(q => q == 3401).Any() });
            //committeeCategory.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3402, IsChecked = rolePermission.Where(q => q == 3402).Any() });
            //committeeCategory.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3403, IsChecked = rolePermission.Where(q => q == 3403).Any() });
            //committeeCategory.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3404, IsChecked = rolePermission.Where(q => q == 3404).Any() });
            //result.PermissionList.Add(committeeCategory);

            ////
            ///Board

            //var boardMeeting = new PermissionVm()
            //{
            //    Title = "Board Meeting",
            //    PermissionNo = 35,
            //    IsChecked = rolePermission.Where(q => q == 35).Any()
            //};
            //boardMeeting.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3501, IsChecked = rolePermission.Where(q => q == 3501).Any() });
            //boardMeeting.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3502, IsChecked = rolePermission.Where(q => q == 3502).Any() });
            //boardMeeting.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3503, IsChecked = rolePermission.Where(q => q == 3503).Any() });
            //boardMeeting.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3504, IsChecked = rolePermission.Where(q => q == 3504).Any() });
            //result.PermissionList.Add(boardMeeting);



            //var venueBooking = new PermissionVm()
            //{
            //    Title = "Venue Booking",
            //    PermissionNo = 36,
            //    IsChecked = rolePermission.Where(q => q == 36).Any()
            //};
            //venueBooking.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3601, IsChecked = rolePermission.Where(q => q == 3601).Any() });
            //venueBooking.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3602, IsChecked = rolePermission.Where(q => q == 3602).Any() });
            //venueBooking.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3603, IsChecked = rolePermission.Where(q => q == 3603).Any() });
            //venueBooking.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3604, IsChecked = rolePermission.Where(q => q == 3604).Any() });
            //result.PermissionList.Add(venueBooking);


            //var serviceSale = new PermissionVm()
            //{
            //    Title = "Service Sale",
            //    PermissionNo = 37,
            //    IsChecked = rolePermission.Where(q => q == 37).Any()
            //};

            //serviceSale.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3701, IsChecked = rolePermission.Where(q => q == 3701).Any() });
            //serviceSale.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3702, IsChecked = rolePermission.Where(q => q == 3702).Any() });
            //serviceSale.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3703, IsChecked = rolePermission.Where(q => q == 3703).Any() });
            //serviceSale.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3704, IsChecked = rolePermission.Where(q => q == 3704).Any() });
            //result.PermissionList.Add(serviceSale);



            //var saleEventTicket = new PermissionVm()
            //{
            //    Title = "Sale Event Ticket",
            //    PermissionNo = 38,
            //    IsChecked = rolePermission.Where(q => q == 38).Any()
            //};
            //saleEventTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3801, IsChecked = rolePermission.Where(q => q == 3801).Any() });
            //saleEventTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3802, IsChecked = rolePermission.Where(q => q == 3802).Any() });
            //saleEventTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3803, IsChecked = rolePermission.Where(q => q == 3803).Any() });
            //saleEventTicket.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3804, IsChecked = rolePermission.Where(q => q == 3804).Any() });
            //result.PermissionList.Add(saleEventTicket);


            //var addOnsItem = new PermissionVm()
            //{
            //    Title = "Add Ons Item",
            //    PermissionNo = 39,
            //    IsChecked = rolePermission.Where(q => q == 39).Any()
            //};
            //addOnsItem.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 3901, IsChecked = rolePermission.Where(q => q == 3901).Any() });
            //addOnsItem.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 3902, IsChecked = rolePermission.Where(q => q == 3902).Any() });
            //addOnsItem.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 3903, IsChecked = rolePermission.Where(q => q == 3903).Any() });
            //addOnsItem.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 3904, IsChecked = rolePermission.Where(q => q == 3904).Any() });
            //result.PermissionList.Add(addOnsItem);

            var divisionItem = new PermissionVm()
            {
                Title = "Division Item",
                PermissionNo = 40,
                IsChecked = rolePermission.Where(q => q == 40).Any()
            };
            divisionItem.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Add", PermissionNo = 4001, IsChecked = rolePermission.Where(q => q == 4001).Any() });
            divisionItem.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Edit", PermissionNo = 4002, IsChecked = rolePermission.Where(q => q == 4002).Any() });
            divisionItem.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "View", PermissionNo = 4003, IsChecked = rolePermission.Where(q => q == 4003).Any() });
            divisionItem.PermissionDetailVms.Add(new PermissionDetailVm() { Name = "Remove", PermissionNo = 4004, IsChecked = rolePermission.Where(q => q == 4004).Any() });
            result.PermissionList.Add(divisionItem);



            result.RoleId = role!.Id;
            result.Name = role.Name;

            return result;
        }


    }




}