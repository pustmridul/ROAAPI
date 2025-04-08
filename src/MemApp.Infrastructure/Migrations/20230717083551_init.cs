using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "com_RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_RefreshToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "com_RolePermissionMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionNo = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_RolePermissionMap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "com_UserPermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PermissionNo = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_UserPermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_BloodGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_BloodGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_categoryPattern",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_categoryPattern", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_College",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_College", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_MemberActiveStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_MemberActiveStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_MemberAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_MemberAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_MemberProfession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_MemberProfession", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_MemberServiceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_MemberServiceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_memberShipFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ammount = table.Column<double>(type: "float", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_memberShipFees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_MemberStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_MemberStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_Menu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false),
                    NavIcon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_SubscriptionFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscribedYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscribedQuater = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionFee = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    LateFee = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    AbroadFee = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_SubscriptionFees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_SubscriptionMode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_SubscriptionMode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginFailedAttemptCount = table.Column<int>(type: "int", nullable: false),
                    LastLoginFailedAttemptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mem_MemberType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    isSubscribed = table.Column<bool>(type: "bit", nullable: false),
                    CategoryPatternId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_MemberType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_MemberType_mem_categoryPattern_CategoryPatternId",
                        column: x => x.CategoryPatternId,
                        principalTable: "mem_categoryPattern",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_MemService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_MemService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_MemService_mem_MemberServiceType_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "mem_MemberServiceType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_SubMenu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false),
                    NavIcon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_SubMenu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_SubMenu_mem_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "mem_Menu",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "com_UserRoleMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_UserRoleMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_com_UserRoleMap_mem_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "mem_Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_com_UserRoleMap_mem_User_UserId",
                        column: x => x.UserId,
                        principalTable: "mem_User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrvCusID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CusCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CusProfession = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscAllowed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscPrcnt = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CreditDays = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Mrcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Opening = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MembershipNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsMasterMember = table.Column<bool>(type: "bit", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CadetNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MemberFullId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PaidTill = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Organaization = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialaization = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OfficeAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberStatusId = table.Column<int>(type: "int", nullable: true),
                    MemberActiveStatusId = table.Column<int>(type: "int", nullable: false),
                    MemberTypeId = table.Column<int>(type: "int", nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: false),
                    MemberAddressID = table.Column<int>(type: "int", nullable: false),
                    MemberProfessionID = table.Column<int>(type: "int", nullable: false),
                    BloodGroupId = table.Column<int>(type: "int", nullable: false),
                    HscYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Spouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpouseOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Anniversary = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_mem_BloodGroup_BloodGroupId",
                        column: x => x.BloodGroupId,
                        principalTable: "mem_BloodGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customer_mem_College_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "mem_College",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customer_mem_MemberActiveStatus_MemberActiveStatusId",
                        column: x => x.MemberActiveStatusId,
                        principalTable: "mem_MemberActiveStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customer_mem_MemberAddress_MemberAddressID",
                        column: x => x.MemberAddressID,
                        principalTable: "mem_MemberAddress",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customer_mem_MemberProfession_MemberProfessionID",
                        column: x => x.MemberProfessionID,
                        principalTable: "mem_MemberProfession",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customer_mem_MemberStatus_MemberStatusId",
                        column: x => x.MemberStatusId,
                        principalTable: "mem_MemberStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customer_mem_MemberType_MemberTypeId",
                        column: x => x.MemberTypeId,
                        principalTable: "mem_MemberType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_SaleMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseAmmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ServiceChargePercent = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    VatChargePercent = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    MemServiceId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_SaleMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_SaleMaster_mem_MemService_MemServiceId",
                        column: x => x.MemServiceId,
                        principalTable: "mem_MemService",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mem_SaleMaster_mem_MemberServiceType_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "mem_MemberServiceType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_ServiceTicket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasAvailability = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromoCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceChargePercent = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ServiceChargeAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    VatChargePercent = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    VatChargeAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MemServiceId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_ServiceTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_ServiceTicket_mem_MemService_MemServiceId",
                        column: x => x.MemServiceId,
                        principalTable: "mem_MemService",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_ServiceTicketType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    MemServiceId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_ServiceTicketType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_ServiceTicketType_mem_MemService_MemServiceId",
                        column: x => x.MemServiceId,
                        principalTable: "mem_MemService",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mem_ServiceTicketType_mem_MemberServiceType_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "mem_MemberServiceType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "com_UserMenuMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    SubMenuId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_UserMenuMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_com_UserMenuMap_mem_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "mem_Menu",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_com_UserMenuMap_mem_SubMenu_SubMenuId",
                        column: x => x.SubMenuId,
                        principalTable: "mem_SubMenu",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_com_UserMenuMap_mem_User_UserId",
                        column: x => x.UserId,
                        principalTable: "mem_User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_AccountTopUp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterMemberId = table.Column<int>(type: "int", nullable: false),
                    MembershipNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopUpAmmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerifiedBy = table.Column<int>(type: "int", nullable: false),
                    VerifierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_AccountTopUp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_AccountTopUp_Customer_RegisterMemberId",
                        column: x => x.RegisterMemberId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_memberChildren",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CadetNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterMemberID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_memberChildren", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_memberChildren_Customer_RegisterMemberID",
                        column: x => x.RegisterMemberID,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_MemberFeesMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterMemberId = table.Column<int>(type: "int", nullable: false),
                    MemberShipFeeId = table.Column<int>(type: "int", nullable: false),
                    MemberFeesTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ammount = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_MemberFeesMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_MemberFeesMap_Customer_RegisterMemberId",
                        column: x => x.RegisterMemberId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mem_MemberFeesMap_mem_memberShipFees_MemberShipFeeId",
                        column: x => x.MemberShipFeeId,
                        principalTable: "mem_memberShipFees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_SubscriptionPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterMemberId = table.Column<int>(type: "int", nullable: false),
                    MemberPayment = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_SubscriptionPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_SubscriptionPayment_Customer_RegisterMemberId",
                        column: x => x.RegisterMemberId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_ServiceAvailability",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTicketId = table.Column<int>(type: "int", nullable: false),
                    AvailabiltyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Morning = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Afternoon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Evening = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WholeDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_ServiceAvailability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_ServiceAvailability_mem_ServiceTicket_ServiceTicketId",
                        column: x => x.ServiceTicketId,
                        principalTable: "mem_ServiceTicket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_ServiceTicketDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTicketId = table.Column<int>(type: "int", nullable: false),
                    ServiceTicketTypeId = table.Column<int>(type: "int", nullable: true),
                    TicketType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MaxQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_ServiceTicketDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_ServiceTicketDetail_mem_ServiceTicketType_ServiceTicketTypeId",
                        column: x => x.ServiceTicketTypeId,
                        principalTable: "mem_ServiceTicketType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mem_ServiceTicketDetail_mem_ServiceTicket_ServiceTicketId",
                        column: x => x.ServiceTicketId,
                        principalTable: "mem_ServiceTicket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_SubscriptionPaymentDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerDue = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SubscriptionFeesId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPaymentId = table.Column<int>(type: "int", nullable: false),
                    RegisterMemberID = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_SubscriptionPaymentDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_SubscriptionPaymentDetail_Customer_RegisterMemberID",
                        column: x => x.RegisterMemberID,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mem_SubscriptionPaymentDetail_mem_SubscriptionFees_SubscriptionFeesId",
                        column: x => x.SubscriptionFeesId,
                        principalTable: "mem_SubscriptionFees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mem_SubscriptionPaymentDetail_mem_SubscriptionPayment_SubscriptionPaymentId",
                        column: x => x.SubscriptionPaymentId,
                        principalTable: "mem_SubscriptionPayment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mem_SaleDetailsTicket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleMasterId = table.Column<int>(type: "int", nullable: false),
                    ServiceTicketTypeId = table.Column<int>(type: "int", nullable: false),
                    ServiceTicketDetailId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mem_SaleDetailsTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mem_SaleDetailsTicket_mem_SaleMaster_SaleMasterId",
                        column: x => x.SaleMasterId,
                        principalTable: "mem_SaleMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mem_SaleDetailsTicket_mem_ServiceTicketDetail_ServiceTicketDetailId",
                        column: x => x.ServiceTicketDetailId,
                        principalTable: "mem_ServiceTicketDetail",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mem_SaleDetailsTicket_mem_ServiceTicketType_ServiceTicketTypeId",
                        column: x => x.ServiceTicketTypeId,
                        principalTable: "mem_ServiceTicketType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_com_UserMenuMap_MenuId",
                table: "com_UserMenuMap",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_com_UserMenuMap_SubMenuId",
                table: "com_UserMenuMap",
                column: "SubMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_com_UserMenuMap_UserId",
                table: "com_UserMenuMap",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_com_UserRoleMap_RoleId",
                table: "com_UserRoleMap",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_com_UserRoleMap_UserId",
                table: "com_UserRoleMap",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_BloodGroupId",
                table: "Customer",
                column: "BloodGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CollegeId",
                table: "Customer",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_MemberActiveStatusId",
                table: "Customer",
                column: "MemberActiveStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_MemberAddressID",
                table: "Customer",
                column: "MemberAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_MemberProfessionID",
                table: "Customer",
                column: "MemberProfessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_MemberStatusId",
                table: "Customer",
                column: "MemberStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_MemberTypeId",
                table: "Customer",
                column: "MemberTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_AccountTopUp_RegisterMemberId",
                table: "mem_AccountTopUp",
                column: "RegisterMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_memberChildren_RegisterMemberID",
                table: "mem_memberChildren",
                column: "RegisterMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_mem_MemberFeesMap_MemberShipFeeId",
                table: "mem_MemberFeesMap",
                column: "MemberShipFeeId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_MemberFeesMap_RegisterMemberId",
                table: "mem_MemberFeesMap",
                column: "RegisterMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_MemberType_CategoryPatternId",
                table: "mem_MemberType",
                column: "CategoryPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_MemService_ServiceTypeId",
                table: "mem_MemService",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SaleDetailsTicket_SaleMasterId",
                table: "mem_SaleDetailsTicket",
                column: "SaleMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SaleDetailsTicket_ServiceTicketDetailId",
                table: "mem_SaleDetailsTicket",
                column: "ServiceTicketDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SaleDetailsTicket_ServiceTicketTypeId",
                table: "mem_SaleDetailsTicket",
                column: "ServiceTicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SaleMaster_MemServiceId",
                table: "mem_SaleMaster",
                column: "MemServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SaleMaster_ServiceTypeId",
                table: "mem_SaleMaster",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_ServiceAvailability_ServiceTicketId",
                table: "mem_ServiceAvailability",
                column: "ServiceTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_ServiceTicket_MemServiceId",
                table: "mem_ServiceTicket",
                column: "MemServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_ServiceTicketDetail_ServiceTicketId",
                table: "mem_ServiceTicketDetail",
                column: "ServiceTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_ServiceTicketDetail_ServiceTicketTypeId",
                table: "mem_ServiceTicketDetail",
                column: "ServiceTicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_ServiceTicketType_MemServiceId",
                table: "mem_ServiceTicketType",
                column: "MemServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_ServiceTicketType_ServiceTypeId",
                table: "mem_ServiceTicketType",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SubMenu_MenuId",
                table: "mem_SubMenu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SubscriptionPayment_RegisterMemberId",
                table: "mem_SubscriptionPayment",
                column: "RegisterMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SubscriptionPaymentDetail_RegisterMemberID",
                table: "mem_SubscriptionPaymentDetail",
                column: "RegisterMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SubscriptionPaymentDetail_SubscriptionFeesId",
                table: "mem_SubscriptionPaymentDetail",
                column: "SubscriptionFeesId");

            migrationBuilder.CreateIndex(
                name: "IX_mem_SubscriptionPaymentDetail_SubscriptionPaymentId",
                table: "mem_SubscriptionPaymentDetail",
                column: "SubscriptionPaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "com_RefreshToken");

            migrationBuilder.DropTable(
                name: "com_RolePermissionMap");

            migrationBuilder.DropTable(
                name: "com_UserMenuMap");

            migrationBuilder.DropTable(
                name: "com_UserPermission");

            migrationBuilder.DropTable(
                name: "com_UserRoleMap");

            migrationBuilder.DropTable(
                name: "mem_AccountTopUp");

            migrationBuilder.DropTable(
                name: "mem_memberChildren");

            migrationBuilder.DropTable(
                name: "mem_MemberFeesMap");

            migrationBuilder.DropTable(
                name: "mem_SaleDetailsTicket");

            migrationBuilder.DropTable(
                name: "mem_ServiceAvailability");

            migrationBuilder.DropTable(
                name: "mem_SubscriptionMode");

            migrationBuilder.DropTable(
                name: "mem_SubscriptionPaymentDetail");

            migrationBuilder.DropTable(
                name: "mem_SubMenu");

            migrationBuilder.DropTable(
                name: "mem_Role");

            migrationBuilder.DropTable(
                name: "mem_User");

            migrationBuilder.DropTable(
                name: "mem_memberShipFees");

            migrationBuilder.DropTable(
                name: "mem_SaleMaster");

            migrationBuilder.DropTable(
                name: "mem_ServiceTicketDetail");

            migrationBuilder.DropTable(
                name: "mem_SubscriptionFees");

            migrationBuilder.DropTable(
                name: "mem_SubscriptionPayment");

            migrationBuilder.DropTable(
                name: "mem_Menu");

            migrationBuilder.DropTable(
                name: "mem_ServiceTicketType");

            migrationBuilder.DropTable(
                name: "mem_ServiceTicket");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "mem_MemService");

            migrationBuilder.DropTable(
                name: "mem_BloodGroup");

            migrationBuilder.DropTable(
                name: "mem_College");

            migrationBuilder.DropTable(
                name: "mem_MemberActiveStatus");

            migrationBuilder.DropTable(
                name: "mem_MemberAddress");

            migrationBuilder.DropTable(
                name: "mem_MemberProfession");

            migrationBuilder.DropTable(
                name: "mem_MemberStatus");

            migrationBuilder.DropTable(
                name: "mem_MemberType");

            migrationBuilder.DropTable(
                name: "mem_MemberServiceType");

            migrationBuilder.DropTable(
                name: "mem_categoryPattern");
        }
    }
}
