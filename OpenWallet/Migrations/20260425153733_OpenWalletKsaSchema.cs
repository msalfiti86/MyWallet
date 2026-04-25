using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWallet.Migrations
{
    /// <inheritdoc />
    public partial class OpenWalletKsaSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockReason",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirthGregorian",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateOfBirthHijri",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FatherNameAr",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FatherNameEn",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstNameAr",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstNameEn",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GrandFatherNameAr",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GrandFatherNameEn",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "IdExpiryDateGregorian",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdExpiryDateHijri",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdType",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMobileVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KycRejectionReason",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KycStatus",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "KycVerifiedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastNameAr",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastNameEn",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalAddressAdditionalNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalAddressBuildingNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalAddressCity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalAddressDistrict",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalAddressPostalCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalAddressStreet",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalAddressUnitNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdOrIqama",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComplaintMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComplaintId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceReviewItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResolutionNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceReviewItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Browser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatingSystem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmailOrMobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedEntityId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LegalNameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LegalNameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommercialRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommercialRegistrationIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommercialRegistrationExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnifiedNationalNumber700 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VatNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessActivity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficialEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficialPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerNationalIdOrIqama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerMobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalAddressBuildingNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalAddressStreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalAddressDistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalAddressCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalAddressPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalAddressAdditionalNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalAddressUnitNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankAccountHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KycStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComplianceStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DailyTransferLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyTransferLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SingleTransactionLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsMainOrganization = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    SuspensionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [Organizations] WHERE [Id] = '11111111-1111-1111-1111-111111111111')
                BEGIN
                    INSERT INTO [Organizations] (
                        [Id], [LegalNameEn], [LegalNameAr], [ShortName], [OrganizationType],
                        [CommercialRegistrationNumber], [CommercialRegistrationIssueDate], [CommercialRegistrationExpiryDate],
                        [UnifiedNationalNumber700], [VatNumber], [BusinessActivity], [Sector], [WebsiteUrl],
                        [OfficialEmail], [OfficialPhone], [ManagerFullName], [ManagerNationalIdOrIqama],
                        [ManagerMobile], [ManagerEmail], [NationalAddressBuildingNumber], [NationalAddressStreet],
                        [NationalAddressDistrict], [NationalAddressCity], [NationalAddressPostalCode],
                        [NationalAddressAdditionalNumber], [NationalAddressUnitNumber], [Iban], [BankName],
                        [BankAccountHolderName], [KycStatus], [ComplianceStatus], [RejectionReason],
                        [DailyTransferLimit], [MonthlyTransferLimit], [SingleTransactionLimit],
                        [IsMainOrganization], [IsActive], [IsSuspended], [SuspensionReason],
                        [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsDeleted], [DeletedAt]
                    )
                    VALUES (
                        '11111111-1111-1111-1111-111111111111',
                        N'OpenWallet Main Organization', N'OpenWallet Main Organization', N'OpenWallet', N'Company',
                        N'', NULL, NULL, N'', N'', N'Wallet services', N'Fintech', N'',
                        N'admin@openwallet.local', N'+966500000000', N'OpenWallet Admin', N'1000000000',
                        N'+966500000000', N'admin@openwallet.local', N'', N'', N'', N'Riyadh', N'11564',
                        N'', N'', N'', N'', N'', N'Verified', N'Normal', N'',
                        50000.00, 500000.00, 10000.00,
                        CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'',
                        SYSUTCDATETIME(), N'Migration', NULL, N'', CAST(0 AS bit), NULL
                    );
                END

                UPDATE [AspNetUsers]
                SET
                    [OrganizationId] = '11111111-1111-1111-1111-111111111111',
                    [IsActive] = CAST(1 AS bit),
                    [KycStatus] = CASE WHEN NULLIF([KycStatus], N'') IS NULL THEN N'Pending' ELSE [KycStatus] END,
                    [Nationality] = CASE WHEN NULLIF([Nationality], N'') IS NULL THEN N'Saudi Arabia' ELSE [Nationality] END,
                    [IdType] = CASE WHEN NULLIF([IdType], N'') IS NULL THEN N'SaudiNationalId' ELSE [IdType] END,
                    [CreatedAt] = CASE WHEN [CreatedAt] = '0001-01-01T00:00:00.0000000' THEN SYSUTCDATETIME() ELSE [CreatedAt] END,
                    [CreatedBy] = CASE WHEN NULLIF([CreatedBy], N'') IS NULL THEN N'Migration' ELSE [CreatedBy] END
                WHERE [OrganizationId] = '00000000-0000-0000-0000-000000000000';
                """);

            migrationBuilder.CreateTable(
                name: "OtpCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardBrand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Last4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardExpiryMonth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardExpiryYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IbanMasked = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefundRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionApprovals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionApprovals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLimitRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WalletType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SingleLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApprovalThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLimitRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvitationToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvitedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInvitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserKycDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKycDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookEventLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookEventLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BeneficiaryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullNameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalIdOrIqama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WalletNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WalletType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HoldBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SingleTransactionLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Wallets_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromWalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToWalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalProviderReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSuspicious = table.Column<bool>(type: "bit", nullable: false),
                    SuspiciousReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlaggedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlaggedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    OriginalTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_FromWalletId",
                        column: x => x.FromWalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_ToWalletId",
                        column: x => x.ToWalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_OrganizationId",
                table: "Beneficiaries",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_OwnerUserId",
                table: "Beneficiaries",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_GroupName_Key",
                table: "SystemSettings",
                columns: new[] { "GroupName", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_OrganizationId",
                table: "Wallets",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_WalletNumber",
                table: "Wallets",
                column: "WalletNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_FromWalletId",
                table: "WalletTransactions",
                column: "FromWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_OrganizationId",
                table: "WalletTransactions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ToWalletId",
                table: "WalletTransactions",
                column: "ToWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_TransactionNumber",
                table: "WalletTransactions",
                column: "TransactionNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Beneficiaries");

            migrationBuilder.DropTable(
                name: "ComplaintMessages");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "ComplianceReviewItems");

            migrationBuilder.DropTable(
                name: "DeviceSessions");

            migrationBuilder.DropTable(
                name: "LoginAudits");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "OrganizationDocuments");

            migrationBuilder.DropTable(
                name: "OtpCodes");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "RefundRequests");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TransactionApprovals");

            migrationBuilder.DropTable(
                name: "TransactionCategories");

            migrationBuilder.DropTable(
                name: "TransactionLimitRules");

            migrationBuilder.DropTable(
                name: "UserInvitations");

            migrationBuilder.DropTable(
                name: "UserKycDocuments");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "WebhookEventLogs");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BlockReason",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirthGregorian",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirthHijri",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FatherNameAr",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FatherNameEn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstNameAr",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstNameEn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GrandFatherNameAr",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GrandFatherNameEn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdExpiryDateGregorian",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdExpiryDateHijri",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsMobileVerified",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KycRejectionReason",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KycStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KycVerifiedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastNameAr",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastNameEn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalAddressAdditionalNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalAddressBuildingNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalAddressCity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalAddressDistrict",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalAddressPostalCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalAddressStreet",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalAddressUnitNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalIdOrIqama",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AspNetUsers");
        }
    }
}
