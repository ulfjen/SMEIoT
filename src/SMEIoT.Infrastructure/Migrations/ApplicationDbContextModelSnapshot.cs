// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SMEIoT.Infrastructure.Data;

namespace SMEIoT.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<long>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("concurrency_stamp")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnName("normalized_name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("role_name_index");

                    b.ToTable("roles");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            ConcurrencyStamp = "4d49acee-0341-4c00-9982-651ba49cf735",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnName("claim_type")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("claim_value")
                        .HasColumnType("text");

                    b.Property<long>("RoleId")
                        .HasColumnName("role_id")
                        .HasColumnType("bigint");

                    b.HasKey("Id")
                        .HasName("pk_role_claims");

                    b.HasIndex("RoleId")
                        .HasName("ix_role_claims_role_id");

                    b.ToTable("role_claims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnName("claim_type")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("claim_value")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("bigint");

                    b.HasKey("Id")
                        .HasName("pk_user_claims");

                    b.HasIndex("UserId")
                        .HasName("ix_user_claims_user_id");

                    b.ToTable("user_claims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnName("login_provider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnName("provider_key")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnName("provider_display_name")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("bigint");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_logins");

                    b.HasIndex("UserId")
                        .HasName("ix_user_logins_user_id");

                    b.ToTable("user_logins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("bigint");

                    b.Property<long>("RoleId")
                        .HasColumnName("role_id")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_user_roles");

                    b.HasIndex("RoleId")
                        .HasName("ix_user_roles_role_id");

                    b.ToTable("user_roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("bigint");

                    b.Property<string>("LoginProvider")
                        .HasColumnName("login_provider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnName("value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_tokens");

                    b.ToTable("user_tokens");
                });

            modelBuilder.Entity("SMEIoT.Core.Entities.Device", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AuthenticationType")
                        .IsRequired()
                        .HasColumnName("authentication_type")
                        .HasColumnType("text");

                    b.Property<bool>("Connected")
                        .HasColumnName("connected")
                        .HasColumnType("boolean");

                    b.Property<Instant?>("ConnectedAt")
                        .HasColumnName("connected_at")
                        .HasColumnType("timestamp");

                    b.Property<Instant>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp");

                    b.Property<Instant?>("LastMessageAt")
                        .HasColumnName("last_message_at")
                        .HasColumnType("timestamp");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnName("normalized_name")
                        .HasColumnType("text");

                    b.Property<string>("PreSharedKey")
                        .HasColumnName("pre_shared_key")
                        .HasColumnType("text");

                    b.Property<Instant>("UpdatedAt")
                        .HasColumnName("updated_at")
                        .HasColumnType("timestamp");

                    b.HasKey("Id")
                        .HasName("pk_devices");

                    b.HasIndex("NormalizedName");

                    b.ToTable("devices");
                });

            modelBuilder.Entity("SMEIoT.Core.Entities.Sensor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Connected")
                        .HasColumnName("connected")
                        .HasColumnType("boolean");

                    b.Property<Instant?>("ConnectedAt")
                        .HasColumnName("connected_at")
                        .HasColumnType("timestamp");

                    b.Property<Instant>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp");

                    b.Property<long>("DeviceId")
                        .HasColumnName("device_id")
                        .HasColumnType("bigint");

                    b.Property<Instant?>("LastMessageAt")
                        .HasColumnName("last_message_at")
                        .HasColumnType("timestamp");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnName("normalized_name")
                        .HasColumnType("text");

                    b.Property<Instant>("UpdatedAt")
                        .HasColumnName("updated_at")
                        .HasColumnType("timestamp");

                    b.HasKey("Id")
                        .HasName("pk_sensors");

                    b.HasIndex("DeviceId")
                        .HasName("ix_sensors_device_id");

                    b.HasIndex("NormalizedName", "DeviceId");

                    b.ToTable("sensors");
                });

            modelBuilder.Entity("SMEIoT.Core.Entities.SensorValue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Instant>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp");

                    b.Property<long>("SensorId")
                        .HasColumnName("sensor_id")
                        .HasColumnType("bigint");

                    b.Property<double>("Value")
                        .HasColumnName("value")
                        .HasColumnType("double precision");

                    b.HasKey("Id")
                        .HasName("pk_sensor_values");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("SensorId")
                        .HasName("ix_sensor_values_sensor_id");

                    b.ToTable("sensor_values");
                });

            modelBuilder.Entity("SMEIoT.Core.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AccessFailedCount")
                        .HasColumnName("access_failed_count")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("concurrency_stamp")
                        .HasColumnType("text");

                    b.Property<Instant>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp");

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnName("email_confirmed")
                        .HasColumnType("boolean");

                    b.Property<Instant>("LastSeenAt")
                        .HasColumnName("last_seen_at")
                        .HasColumnType("timestamp");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnName("lockout_enabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnName("lockout_end")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnName("normalized_email")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnName("normalized_user_name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnName("password_hash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnName("phone_number")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnName("phone_number_confirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnName("security_stamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnName("two_factor_enabled")
                        .HasColumnType("boolean");

                    b.Property<Instant>("UpdatedAt")
                        .HasColumnName("updated_at")
                        .HasColumnType("timestamp");

                    b.Property<string>("UserName")
                        .HasColumnName("user_name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("NormalizedEmail")
                        .HasName("email_index");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("user_name_index");

                    b.ToTable("users");
                });

            modelBuilder.Entity("SMEIoT.Core.Entities.UserSensor", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("bigint");

                    b.Property<long>("SensorId")
                        .HasColumnName("sensor_id")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "SensorId");

                    b.HasIndex("SensorId")
                        .HasName("ix_user_sensors_sensor_id");

                    b.ToTable("user_sensors");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<long>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_role_claims_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.HasOne("SMEIoT.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_claims__asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.HasOne("SMEIoT.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_logins__asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<long>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_user_roles_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SMEIoT.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_roles__asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.HasOne("SMEIoT.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_tokens__asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SMEIoT.Core.Entities.Sensor", b =>
                {
                    b.HasOne("SMEIoT.Core.Entities.Device", "Device")
                        .WithMany("Sensors")
                        .HasForeignKey("DeviceId")
                        .HasConstraintName("fk_sensors_devices_device_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SMEIoT.Core.Entities.SensorValue", b =>
                {
                    b.HasOne("SMEIoT.Core.Entities.Sensor", "Sensor")
                        .WithMany("SensorValues")
                        .HasForeignKey("SensorId")
                        .HasConstraintName("fk_sensor_values_sensors_sensor_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SMEIoT.Core.Entities.UserSensor", b =>
                {
                    b.HasOne("SMEIoT.Core.Entities.Sensor", "Sensor")
                        .WithMany("UserSensors")
                        .HasForeignKey("SensorId")
                        .HasConstraintName("fk_user_sensors_sensors_sensor_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SMEIoT.Core.Entities.User", "User")
                        .WithMany("UserSensors")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_sensors_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
