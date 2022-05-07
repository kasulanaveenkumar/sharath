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
    [Migration("20210816044736_Alter_Table_AppStakeholders_Modify_UserGuid")]
    partial class Alter_Table_AppStakeholders_Modify_UserGuid
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core.API.Entities.AppActivities", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("AppActivities");
                });

            modelBuilder.Entity("Core.API.Entities.AppActivityLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AppActivityId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProcessedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ProcessedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AppActivityLogs");
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

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AppDocuments");
                });

            modelBuilder.Entity("Core.API.Entities.AppImageReasons", b =>
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

                    b.Property<short>("DocGroup")
                        .HasColumnType("smallint");

                    b.Property<string>("Extension")
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

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

                    b.Property<string>("OtherFlagReason")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("OtherRejectReason")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("SizeInKb")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("AppImages");
                });

            modelBuilder.Entity("Core.API.Entities.AppStakeholders", b =>
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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<short>("Role")
                        .HasColumnType("smallint");

                    b.Property<string>("SurName")
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

                    b.Property<string>("StateCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateSetGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("Core.API.Entities.LenderConfigurations", b =>
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

                    b.Property<bool>("IsAdditionalDataRequired")
                        .HasColumnType("bit");

                    b.Property<bool>("IsBSAllowed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsByPassAdditionalDataAllowed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsByPassSalesInvoiceAllowed")
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

                    b.Property<string>("ReportEmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReportImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LenderConfigurations");
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

                    b.Property<long>("NotificationId")
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

            modelBuilder.Entity("Core.API.Entities.ReasonMappings", b =>
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

            modelBuilder.Entity("Core.API.Entities.Reasons", b =>
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
#pragma warning restore 612, 618
        }
    }
}
