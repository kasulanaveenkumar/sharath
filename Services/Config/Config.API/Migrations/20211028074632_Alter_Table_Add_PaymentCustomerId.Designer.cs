﻿// <auto-generated />
using System;
using Config.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Config.API.Migrations
{
    [DbContext(typeof(ConfigContext))]
    [Migration("20211028074632_Alter_Table_Add_PaymentCustomerId")]
    partial class Alter_Table_Add_PaymentCustomerId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Config.API.Entities.BrokerLenderMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("BrokerCompanyId")
                        .HasColumnType("bigint");

                    b.Property<long>("LenderCompanyId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("BrokerLenderMappings");
                });

            modelBuilder.Entity("Config.API.Entities.BrokerTemplateMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CompanyId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("BrokerTemplateMappings");
                });

            modelBuilder.Entity("Config.API.Entities.Companies", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ABN")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("AllowOnlyInvitedUser")
                        .HasColumnType("bit");

                    b.Property<string>("City")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CompanyAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CompanyDescription")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("CompanyGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CompanyLogoURL")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<long>("CompanyTypeId")
                        .HasColumnType("bigint");

                    b.Property<string>("ContractLocation")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("ExcemptPayment")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("GoLiveDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsPayer")
                        .HasColumnType("bit");

                    b.Property<short>("LenderVisibility")
                        .HasColumnType("smallint");

                    b.Property<string>("PaymentCustomerId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("RegisteredName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("SignDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("State")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Website")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ZIPCode")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Config.API.Entities.CompanyContactTypes", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("CompanyContactTypes");
                });

            modelBuilder.Entity("Config.API.Entities.CompanyContacts", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("CompanyContactTypeId")
                        .HasColumnType("smallint");

                    b.Property<long>("CompanyId")
                        .HasColumnType("bigint");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Mobile")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SurName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("CompanyContacts");
                });

            modelBuilder.Entity("Config.API.Entities.CompanyTypes", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("CompanyTypes");
                });

            modelBuilder.Entity("Config.API.Entities.LenderTemplateDocImageMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSkippable")
                        .HasColumnType("bit");

                    b.Property<long>("LenderCompanyId")
                        .HasColumnType("bigint");

                    b.Property<bool>("NotRequired")
                        .HasColumnType("bit");

                    b.Property<long>("TemplateDocumentId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateImageId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateSetId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("LenderTemplateDocImageMappings");
                });

            modelBuilder.Entity("Config.API.Entities.LenderTemplateSetDocMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("LenderCompanyId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateDocumentId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateSetId")
                        .HasColumnType("bigint");

                    b.Property<string>("UploadOptions")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LenderTemplateSetDocMappings");
                });

            modelBuilder.Entity("Config.API.Entities.States", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("State")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("StateCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("States");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateDocCategories", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("TemplateDocCategories");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateDocImageMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsDefaultSelected")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("bit");

                    b.Property<short>("Position")
                        .HasColumnType("smallint");

                    b.Property<long>("TemplateDocumentId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateImageId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TemplateDocImageMappings");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateDocNoLenderPreferences", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CompanyGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Preference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TemplateSetId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateSetPlanId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserGuid")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("TemplateDocNoLenderPreferences");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateDocOptions", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("ActionId")
                        .HasColumnType("smallint");

                    b.Property<string>("OptionName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<short>("OptionType")
                        .HasColumnType("smallint");

                    b.Property<long>("TemplateDocumentId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TemplateDocOptions");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateDocuments", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AdditionalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("TemplateDocCategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TemplateDocuments");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateImages", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<short>("DocGroup")
                        .HasColumnType("smallint");

                    b.Property<short>("ImageType")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("TemplateDocCategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TemplateImages");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateSetDocMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsDefaultSelected")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("bit");

                    b.Property<short>("Position")
                        .HasColumnType("smallint");

                    b.Property<long>("TemplateDocumentId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateSetId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TemplateSetDocMappings");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateSetPlanDocMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("TemplateDocumentId")
                        .HasColumnType("bigint");

                    b.Property<long>("TemplateSetPlanId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TemplateSetPlanDocMappings");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateSetPlans", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<long>("LenderCompanyId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("LoanAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<short>("MaxDocument")
                        .HasColumnType("smallint");

                    b.Property<string>("PlanGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PlanName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("TemplateSetId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TemplateSetPlans");
                });

            modelBuilder.Entity("Config.API.Entities.TemplateSets", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TemplateSetGuid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("TemplateSets");
                });
#pragma warning restore 612, 618
        }
    }
}
