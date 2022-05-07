﻿// <auto-generated />
using System;
using Core.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.API.Migrations
{
    [DbContext(typeof(CoreContext))]
    [Migration("20220214082529_Alter_Table_ADCompanies_Add_Column_IsUseCompanyLogoForWebApp")]
    partial class Alter_Table_ADCompanies_Add_Column_IsUseCompanyLogoForWebApp
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Common.Models.Core.Entities.ADCompanies", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CompanyGuid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("CompanyTypeId")
                        .HasColumnType("bigint");

                    b.Property<bool>("ExcemptPayment")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPayer")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUseCompanyLogoForWebApp")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("ADCompanies");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.ADTemplateSets", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TemplateName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateSetGUID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TemplateSetId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("ADTemplateSets");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.AppActivities", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsEnabledForNotifications")
                        .HasColumnType("bit");

                    b.Property<string>("NotificationDescription")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("NotificationGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("AppActivities");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.AppActivityLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AppActivityId")
                        .HasColumnType("bigint");

                    b.Property<long>("ApplicationId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsNotified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWebAppUser")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ProcessedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AppActivityLogs");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.AppImageReasons", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AppImageId")
                        .HasColumnType("bigint");

                    b.Property<long>("ApplicationId")
                        .HasColumnType("bigint");

                    b.Property<long>("ReasonId")
                        .HasColumnType("bigint");

                    b.Property<long>("ReasonType")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("AppImageReasons");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.AppStakeholders", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ApplicationId")
                        .HasColumnType("bigint");

                    b.Property<long>("IsOwner")
                        .HasColumnType("bigint");

                    b.Property<string>("UserGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("AppStakeholders");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.CoreConfigs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comments")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CoreConfigs");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.LenderConfigurations", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalTnC")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("AllowViewBS")
                        .HasColumnType("bit");

                    b.Property<bool>("BypassHourMeter")
                        .HasColumnType("bit");

                    b.Property<string>("CorrectInfoStatement")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAPIIntegrationEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAdditionalDataRequired")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowAwaitedRef")
                        .HasColumnType("bit");

                    b.Property<bool>("IsBSAllowed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsByPassAdditionalDataAllowed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsByPassSalesInvoiceAllowed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsForceLenderRefFormat")
                        .HasColumnType("bit");

                    b.Property<bool>("IsIllionIntegrationEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNonOwnerAllowed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsReportRequired")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRoadworthyAllowed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSalesInvoiceRequired")
                        .HasColumnType("bit");

                    b.Property<string>("LenderCompanyGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LenderRefPrefix")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ReportEmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReportImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LenderConfigurations");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.ReasonMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("ImageType")
                        .HasColumnType("smallint");

                    b.Property<long>("ReasonId")
                        .HasColumnType("bigint");

                    b.Property<short>("ReasonType")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("ReasonMappings");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.Reasons", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Reasons");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.TemplateImageHelpImages", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("HelpImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("ImageType")
                        .HasColumnType("smallint");

                    b.Property<long>("TemplateSetId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TemplateImageHelpImages");
                });

            modelBuilder.Entity("Common.Models.Core.Entities.TemplateImageOverlayImages", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("ImageType")
                        .HasColumnType("smallint");

                    b.Property<string>("OverlayImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TemplateSetId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TemplateImageOverlayImages");
                });

            modelBuilder.Entity("Core.API.Entities.ADStates", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("StateCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("StateID")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("ADStates");
                });

            modelBuilder.Entity("Core.API.Entities.ADTemplateSetPlans", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsDefaultActivated")
                        .HasColumnType("bit");

                    b.Property<string>("PlanGuid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("TemplateGuid")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ADTemplateSetPlans");
                });

            modelBuilder.Entity("Core.API.Entities.ADUserTypes", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("UserTypeId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("ADUserTypes");
                });

            modelBuilder.Entity("Core.API.Entities.ADUsers", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CompanyGuid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("bit");

                    b.Property<string>("Mobile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SurName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserTypeId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("ADUsers");
                });

            modelBuilder.Entity("Core.API.Entities.AppDocuments", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ApplicationId")
                        .HasColumnType("bigint");

                    b.Property<string>("BankStatementUrl")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<short>("DocStatus")
                        .HasColumnType("smallint");

                    b.Property<bool>("IsAdditionalDataMandatory")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAdditionalDataRequired")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AppDocuments");
                });

            modelBuilder.Entity("Core.API.Entities.AppImages", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AppDocumentId")
                        .HasColumnType("bigint");

                    b.Property<long>("ApplicationId")
                        .HasColumnType("bigint");

                    b.Property<string>("BypassReason")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<short>("DocGroup")
                        .HasColumnType("smallint");

                    b.Property<string>("Extension")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("FileName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FilePath")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ImageData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("ImageInternalStatus")
                        .HasColumnType("smallint");

                    b.Property<string>("ImageName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<short>("ImageStatus")
                        .HasColumnType("smallint");

                    b.Property<short>("ImageType")
                        .HasColumnType("smallint");

                    b.Property<bool>("IsBypassRequested")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSkipped")
                        .HasColumnType("bit");

                    b.Property<string>("OtherFlagReason")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("OtherRejectReason")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("SizeInKb")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<long>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("datetime2");

                    b.Property<short>("UserType")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("AppImages");
                });

            modelBuilder.Entity("Core.API.Entities.AppUsers", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ApplicationId")
                        .HasColumnType("bigint");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("LoginOTP")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("OTPGeneratedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<short>("Role")
                        .HasColumnType("smallint");

                    b.Property<string>("SurName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("Core.API.Entities.Applications", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("ApplicationStatus")
                        .HasColumnType("smallint");

                    b.Property<string>("BrokerCompanyGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalRefNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("InspectionGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsBypassRequested")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSuspended")
                        .HasColumnType("bit");

                    b.Property<string>("LenderCompanyGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("ProcessedBy")
                        .HasColumnType("int");

                    b.Property<short>("PurgeStatus")
                        .HasColumnType("smallint");

                    b.Property<string>("RefNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("RejectionCount")
                        .HasColumnType("int");

                    b.Property<string>("StateCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateSetGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TemplateSetPlanGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("WebAppShortLink")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("Core.API.Entities.ErrorLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CompleteException")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ErrorTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FilePath")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Member")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StackTrace")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("ErrorLogs");
                });

            modelBuilder.Entity("Core.API.Entities.NotificationEvents", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EventName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NotificationEvents");
                });

            modelBuilder.Entity("Core.API.Entities.NotificationUserMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AppActivityId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("NotificationUserMappings");
                });

            modelBuilder.Entity("Core.API.Entities.Notifications", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("EnvetType")
                        .HasColumnType("smallint");

                    b.Property<string>("EventDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventFor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("NotificationEventId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NotificationGuid")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Core.API.Entities.PaymentLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("ApplicationId")
                        .HasColumnType("bigint");

                    b.Property<string>("FailedReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InspectionType")
                        .HasColumnType("int");

                    b.Property<short>("Payer")
                        .HasColumnType("smallint");

                    b.Property<int>("PaymentStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("PaymentTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PaymentLogs");
                });

            modelBuilder.Entity("Core.API.Entities.Remainders", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("RemainderGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("RemainderTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Remainders");
                });

            modelBuilder.Entity("Core.API.Entities.SP.Usp_GetInspections", b =>
                {
                    b.Property<int>("ApplicationStatus")
                        .HasColumnType("int");

                    b.Property<string>("AssetType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrokerCompany")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BuyerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BypassStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("InspectionId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsBypassRequested")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPurged")
                        .HasColumnType("bit");

                    b.Property<bool>("IsShowFlag")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSuspended")
                        .HasColumnType("bit");

                    b.Property<string>("Lender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LenderRef")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReverseTimer")
                        .HasColumnType("int");

                    b.Property<string>("SellerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalRecordsCount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedTime")
                        .HasColumnType("datetime2");

                    b.ToTable("InspectionsList", t => t.ExcludeFromMigrations());
                });

            modelBuilder.Entity("Core.API.Entities.VedaDetails", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ApplicationId")
                        .HasColumnType("bigint");

                    b.Property<string>("ErrorMessage")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Result")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("VedaGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("VedaDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
