using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TimelessTechnicians.UI.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allergies",
                columns: table => new
                {
                    AllergyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeletionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergies", x => x.AllergyId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfirmPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Suburb = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TermsOfServiceAccepted = table.Column<bool>(type: "bit", nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChangeEmailViewModel",
                columns: table => new
                {
                    CurrentEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmNewEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ChangePasswordViewModel",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewPassword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfirmPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Conditions",
                columns: table => new
                {
                    ConditionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeletionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditions", x => x.ConditionId);
                });

            migrationBuilder.CreateTable(
                name: "Consumables",
                columns: table => new
                {
                    ConsumableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumables", x => x.ConsumableId);
                });

            migrationBuilder.CreateTable(
                name: "HospitalInfos",
                columns: table => new
                {
                    HospitalInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HospitalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalInfos", x => x.HospitalInfoId);
                });

            migrationBuilder.CreateTable(
                name: "Login",
                columns: table => new
                {
                    UserNameorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RememberMe = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    MedicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Schedule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.MedicationId);
                });

            migrationBuilder.CreateTable(
                name: "UserListViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserListViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    WardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WardName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    WardStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.WardId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdmitPatients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdmitPatientStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NurseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DischargeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DischargeNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmitPatients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmitPatients_AspNetUsers_NurseId",
                        column: x => x.NurseId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmitPatients_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorAdviceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NurseId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RequestDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorAdviceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorAdviceRequests_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorAdviceRequests_AspNetUsers_NurseId",
                        column: x => x.NurseId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DoctorAdviceRequests_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NurseId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientFolders_AspNetUsers_NurseId",
                        column: x => x.NurseId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientTreatments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TreatmentDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DatePerformed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TreatmentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientTreatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientTreatments_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientVitals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BloodPressure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temperature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SugarLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientVitalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Active"),
                    AdministeredBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVitals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientVitals_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Medication = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateWritten = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prescriptions_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockLogs",
                columns: table => new
                {
                    StockLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    QuantityTaken = table.Column<int>(type: "int", nullable: false),
                    DateTaken = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLogs", x => x.StockLogId);
                    table.ForeignKey(
                        name: "FK_StockLogs_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockRequests",
                columns: table => new
                {
                    StockRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockRequests", x => x.StockRequestId);
                    table.ForeignKey(
                        name: "FK_StockRequests_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonScheduledMedications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdministeredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministeredBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonScheduledMedications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonScheduledMedications_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledMedications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdministeredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministeredBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ScheduledMedicationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMedications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledMedications_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Beds",
                columns: table => new
                {
                    BedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BedNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WardId = table.Column<int>(type: "int", nullable: false),
                    DeletionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beds", x => x.BedId);
                    table.ForeignKey(
                        name: "FK_Beds_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "WardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    DischargeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DischargeStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discharges_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicationPrescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    DatePrescribed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdministeredBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationPrescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicationPrescriptions_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationPrescriptions_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientAllergies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    AllergyId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientAllergies_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAllergies_Allergies_AllergyId",
                        column: x => x.AllergyId,
                        principalTable: "Allergies",
                        principalColumn: "AllergyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientAllergies_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientAppointments_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientAppointments_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConditionId = table.Column<int>(type: "int", nullable: false),
                    DateAdministered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientConditions_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientConditions_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientConditions_Conditions_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "Conditions",
                        principalColumn: "ConditionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientDischargeInstructions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    DischargeReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DischargeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministeredBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDischargeInstructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDischargeInstructions_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientInstructions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateIssued = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdministeredBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructionType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientInstructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientInstructions_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientMedications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    DateAdministered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMedications_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientMedications_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientMedications_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MovementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MovementStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMovements_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientVisitSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVisitSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientVisitSchedules_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReAdmissionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    ReAdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReAdmissionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReAdmissionHistories_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BedAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmitPatientId = table.Column<int>(type: "int", nullable: false),
                    BedId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BedAssignmentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BedAssignments_AdmitPatients_AdmitPatientId",
                        column: x => x.AdmitPatientId,
                        principalTable: "AdmitPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BedAssignments_Beds_BedId",
                        column: x => x.BedId,
                        principalTable: "Beds",
                        principalColumn: "BedId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Allergies",
                columns: new[] { "AllergyId", "DeletionStatus", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Active", "Allergy to peanuts and peanut products.", "Peanuts" },
                    { 2, "Active", "Allergy to shellfish including shrimp, crab, and lobster.", "Shellfish" },
                    { 3, "Active", "Allergy to penicillin and related antibiotics.", "Penicillin" },
                    { 4, "Active", "Allergy to eggs and products containing eggs.", "Eggs" },
                    { 5, "Active", "Allergy to cow's milk and dairy products.", "Milk" },
                    { 6, "Active", "Allergy to wheat or wheat-based products.", "Wheat" },
                    { 7, "Active", "Allergy to soybeans and soy-based products.", "Soy" },
                    { 8, "Active", "Allergy to natural rubber latex products.", "Latex" },
                    { 9, "Active", "Allergic reaction to insect stings, such as bees or wasps.", "Insect stings" },
                    { 10, "Active", "Allergy to pollen from trees, grass, or weeds.", "Pollen" },
                    { 11, "Active", "Allergy to mold spores found indoors and outdoors.", "Mold" },
                    { 12, "Active", "Allergy to dander from animals, particularly cats and dogs.", "Animal Dander" },
                    { 13, "Active", "Allergy to dust mites that live in household dust.", "Dust Mites" },
                    { 14, "Active", "Allergy or sensitivity to fragrances found in perfumes, soaps, or detergents.", "Fragrances" },
                    { 15, "Active", "Allergy to nickel found in jewelry, watches, or belt buckles.", "Nickel" },
                    { 16, "Active", "Allergy to citrus fruits such as oranges, lemons, or grapefruits.", "Citrus Fruits" },
                    { 17, "Active", "Allergy to gluten, a protein found in wheat, barley, and rye.", "Gluten" },
                    { 18, "Active", "Allergy to avocados and products containing avocado.", "Avocados" },
                    { 19, "Active", "Allergy to tree nuts such as almonds, walnuts, or cashews.", "Tree Nuts" },
                    { 20, "Active", "Allergy or sensitivity to chocolate and cocoa products.", "Chocolate" },
                    { 21, "Active", "Allergy to corn or corn-based products.", "Corn" },
                    { 22, "Active", "Allergy to bananas and products containing bananas.", "Bananas" },
                    { 23, "Active", "Allergy to garlic and garlic-based products.", "Garlic" },
                    { 24, "Active", "Allergy to tomatoes and products containing tomatoes.", "Tomatoes" },
                    { 25, "Active", "Allergy to apples and products containing apples.", "Apples" },
                    { 26, "Active", "Allergy or sensitivity to artificial sweeteners like aspartame.", "Aspartame" },
                    { 27, "Active", "Allergy to barley and barley-based products.", "Barley" },
                    { 28, "Active", "Allergy to sesame seeds and sesame oil.", "Sesame Seeds" },
                    { 29, "Active", "Allergy to sunflower seeds and sunflower oil.", "Sunflower Seeds" },
                    { 30, "Active", "Allergy to mustard seeds and mustard-based products.", "Mustard" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "City", "ConcurrencyStamp", "ConfirmPassword", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "Password", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Role", "SecurityStamp", "Status", "Suburb", "TermsOfServiceAccepted", "Title", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "06c4ff00-6ad1-4fa2-b0b8-38f888f40a93", 0, "12th Nelson Mandela Road , SummerStrand", "PortElizabeth", "15dd2a56-8667-4c59-98b3-ed61be2ad26d", "Patient@44", new DateTime(1990, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "patient22@gmail.com", false, "James", "Doe", false, null, null, null, "Patient@44", "AQAAAAIAAYagAAAAEKw9vm6RtO0evWNpJKLFFpIwtRy5nX3YYj/rlvOxaAG4fH2IrzRsK5U00QZKrswD5g==", null, false, "PATIENT", "d31c208e-f411-4923-8d0f-72fb71acb78b", "Inactive", "Summerstrand", true, "Mr", false, "Patient 2" },
                    { "19510ebd-c911-4c36-826a-20cf5e58ef30", 0, "321 Pharmacy Ln, SummerStrand", "PortElizabeth", "9df8a6af-c2d2-4694-92ea-f06e0de0a178", "consumable@123", new DateTime(1990, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "siba@gmail.com", false, "Anna", "Script", false, null, null, null, "consumable@123", "AQAAAAIAAYagAAAAEOABTuKistSO4IcLUIR08k9wS2WQ0YBcv0eiKa0TWyg969AxDtdvoxD5jWQCHFw0zQ==", null, false, "CONSUMABLESMANAGER", "07fcc089-8b3e-4380-b4fd-b3b2702d959c", "Active", "Summerstrand", true, "Mrs", false, "Consumable Manager Siba" },
                    { "1d04efe8-185c-49e8-a250-6ad3d88388a3", 0, "456 Nurse Ave, North End", "PortElizabeth", "98c06fab-d053-4987-a8ae-6d0831c18b10", "nurse@123", new DateTime(1985, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "nurse@gmail.com", false, "Princess", "Phiri", false, null, null, null, "nurse@123", "AQAAAAIAAYagAAAAEJd9HD65QoqE19KLcVYxfgwOPzXOsDQAsCl7ytcll8UUezUk8k/Bfkax/s4FlLICkw==", null, false, "NURSE", "9ab41f87-ab9e-41cc-97ca-86223e7175f9", "Active", "NorthEnd", true, "Mrs", false, "Nurse Princess" },
                    { "2e26990d-9722-4583-82aa-8976e7b95d18", 0, "321 Pharmacy Ln, SummerStrand", "PortElizabeth", "ddccd0cf-11a9-4180-95cb-68136038d10f", "script@123", new DateTime(1990, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "siba@pharmacy.com", false, "Sibabalwe", "Nzono", false, null, null, null, "script@123", "AQAAAAIAAYagAAAAEGDb3PxRXsJMx1eeWD0Iqkl2oz6KF6zng3PeIy1IY0ijrsxZTBXtwDkWa6H1mfmZCw==", null, false, "SCRIPTMANAGER", "9949d906-cbbd-47fe-b2fd-74a10ec78d87", "Active", "Summerstrand", true, "Mrs", false, "Script Manager Siba" },
                    { "37930bab-a31c-425c-b9a7-f61deb128c00", 0, "456 Nurse Ave, North End", "PortElizabeth", "26c4c432-092b-4fc7-a515-a9eafa6bcb8b", "123@Gomolemo", new DateTime(1985, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "gomolemo@gmail.com", false, "Gomolemo", "Mogale", false, null, null, null, "123@Gomolemo", "AQAAAAIAAYagAAAAEMuI8JMDS+1ijmYZH/SjBhdlHGbrk/+1oVOwJe/7qmbz/OP9EByqLwi4qtJ1WiSHMg==", null, false, "DOCTOR", "ed7ddac8-338b-423e-bdf0-919264c44fe8", "Active", "NorthEnd", true, "Mrs", false, "Gomolemo" },
                    { "50223db8-df0a-421d-803d-1d879d014b54", 0, "13th Nelson Mandela Road , SummerStrand", "PortElizabeth", "c6eb0784-88d4-4a97-9ab2-9f8a95b9a16e", "patientLuckpatient123", new DateTime(1990, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "patient33@gmail.com", false, "Lucky", "Doe", false, null, null, null, "Luckpatient@123", "AQAAAAIAAYagAAAAEMNjhbUTaYulcE38wLJrnS2nFxhSINIXzXlUSPrCHll8svgVxERmgx8iH0N9J9ALDQ==", null, false, "PATIENT", "4f2732c9-30b9-4155-a645-9c048f645ff1", "Inactive", "Summerstrand", true, "Mrs", false, "Patient 3" },
                    { "8ea6faa2-3ea7-489a-a41e-a2bce7ca86a5", 0, "4th avenue , SummerStrand", "PortElizabeth", "1eef44f8-3517-4aa5-9534-7f389d36a73d", "nursesister@123", new DateTime(1990, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "nursesister@gmail.com", false, "Princess", "Phiri", false, null, null, null, "nursesister@123", "AQAAAAIAAYagAAAAEHiGcSST7uacsInhG14uGfiFYNuzM+lY167aQGJoyQb3lA2z4ADqKcrFPgaf8Ah0tA==", null, false, "NURSINGSISTER", "4a7fcd43-3e3d-4c19-86f5-4784621bbbf2", "Active", "Summerstrand", true, "Mrs", false, "Nurse Sister Princess" },
                    { "a22502c6-b742-4592-91b3-c03f85d2b8e1", 0, "123 Admin St, Port Elizabeth", "PortElizabeth", "779ab212-2857-481d-8d07-c2f779516e60", "admin@123", new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "group4@gmail.com", false, "Group-4", "Group-4e", false, null, null, null, "admin@123", "AQAAAAIAAYagAAAAEKzs+4dMCwLgdmHaS5yWh7xTOUvY0OolqrcafM2ARK7GZhpZqgLnFyDTJTEP7F9jlA==", null, false, "ADMINISTRATOR", "c99c1477-229d-497e-8d67-b799758c6ef8", "Active", "Summerstrand", true, "Mr", false, "Group-4" },
                    { "aea403dc-8f45-4908-90fe-615ee5677578", 0, "2nd avenue , SummerStrand", "PortElizabeth", "076083de-5b76-45b0-8713-0aaa2316c9f4", "ward@123", new DateTime(1990, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "shafeeq@gmail.com", false, "Shafeeq", "Agnew", false, null, null, null, "ward@123", "AQAAAAIAAYagAAAAEDWjyzOkWiigcDRk8Iu0PCyY9W9OlhyD64tThB0gShan4dj9coShy4Y9yfri1Ax0MQ==", null, false, "WARDADMIN", "8d4bbdae-43e7-42bc-b41a-a16c17cfe1b5", "Active", "Summerstrand", true, "Mr", false, "Shafeeq" },
                    { "b3ebbcee-5ada-479f-a9c0-cad49fa55c19", 0, "12th Nelson Mandela Road , SummerStrand", "PortElizabeth", "a9bf2fb0-100b-4818-b07b-520c87418bf0", "patient@123", new DateTime(1990, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "patient@gmail.com", false, "John", "Doe", false, null, null, null, "patient@123", "AQAAAAIAAYagAAAAEGedef+sogAP+MkiIlNGedgniFqz5FbtGrYgTTWl04UiThe7Nq64OsvxlD6ZMFY9Ng==", null, false, "PATIENT", "e6bf8fe1-28ca-45d7-bac2-0be02aa15186", "Active", "Summerstrand", true, "Mr", false, "Patient" },
                    { "ed43645e-dea7-42dd-896b-ec820ecafc39", 0, "2nd Nelson Mandela Road , SummerStrand", "PortElizabeth", "d54acef3-ccb1-4f25-9b72-815e01542e4c", "Alfredpatient@123", new DateTime(1990, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "patient55@gmail.com", false, "Alfred", "Doe", false, null, null, null, "Alfredpatient@123", "AQAAAAIAAYagAAAAEF24qIf+a7zQ/TeArJRT2geJBky8u6eU1AHS7tEyLoytjjj1agasfCMsY35O7xQ8gQ==", null, false, "PATIENT", "f86bf184-9175-4c99-90ec-00fa6a62043e", "Inactive", "Summerstrand", true, "Mr", false, "Patient 5" }
                });

            migrationBuilder.InsertData(
                table: "Conditions",
                columns: new[] { "ConditionId", "DeletionStatus", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Active", "A chronic condition that affects how the body processes blood sugar (glucose).", "Diabetes" },
                    { 2, "Active", "A condition in which the blood pressure in the arteries is persistently elevated.", "Hypertension" },
                    { 3, "Active", "A condition in which a person's airways become inflamed, narrow, and swell.", "Asthma" },
                    { 4, "Active", "A long-term condition where the kidneys do not work effectively.", "Chronic Kidney Disease" },
                    { 5, "Active", "A range of conditions that affect the heart.", "Heart Disease" },
                    { 6, "Active", "A medical condition where poor blood flow to the brain results in cell death.", "Stroke" },
                    { 7, "Active", "A group of lung diseases that block airflow and make it difficult to breathe.", "Chronic Obstructive Pulmonary Disease (COPD)" },
                    { 8, "Active", "A progressive disease that destroys memory and other important mental functions.", "Alzheimer's Disease" },
                    { 9, "Active", "A disorder of the central nervous system that affects movement, often including tremors.", "Parkinson's Disease" },
                    { 10, "Active", "A neurological disorder marked by sudden recurrent episodes of sensory disturbance, loss of consciousness, or convulsions.", "Epilepsy" },
                    { 11, "Active", "A disease in which the immune system eats away at the protective covering of nerves.", "Multiple Sclerosis (MS)" },
                    { 12, "Active", "A virus that attacks the immune system, leading to life-threatening infections and cancers.", "HIV/AIDS" },
                    { 13, "Active", "Inflammation of one or more of your joints causing pain and stiffness.", "Arthritis" },
                    { 14, "Active", "A condition in which bones become weak and brittle.", "Osteoporosis" },
                    { 15, "Active", "A disease in which abnormal cells divide uncontrollably and destroy body tissue.", "Cancer" }
                });

            migrationBuilder.InsertData(
                table: "Consumables",
                columns: new[] { "ConsumableId", "CreatedDate", "DeletionStatus", "Description", "ExpiryDate", "LastUpdatedDate", "Name", "Quantity", "Type" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6705), "Active", "Sterile bandages for wound care", new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6706), "Bandages", 100, 1 },
                    { 2, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6709), "Active", "Disposable syringes for injections", new DateTime(2024, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6710), "Syringes", 150, 1 },
                    { 3, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6712), "Active", "Latex gloves for medical procedures", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6712), "Gloves", 200, 1 },
                    { 4, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6714), "Active", "Film used for X-ray imaging", new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6714), "X-Ray Film", 50, 2 },
                    { 5, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6716), "Active", "Topical ointment for infections", new DateTime(2024, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6716), "Antibiotic Ointment", 75, 0 },
                    { 6, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6718), "Active", "Masks for protection against infection", new DateTime(2024, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6718), "Surgical Masks", 500, 1 },
                    { 7, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6720), "Active", "Digital thermometers for temperature measurement", new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6720), "Thermometers", 30, 2 },
                    { 8, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6747), "Active", "Sterile saline solution for wound cleaning and IV use", new DateTime(2025, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6747), "Saline Solution", 100, 0 },
                    { 9, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6749), "Active", "Sterile IV sets for intravenous therapy", new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6750), "IV Drip Sets", 75, 1 },
                    { 10, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6751), "Active", "Sterile cotton swabs for wound cleaning or diagnostic purposes", new DateTime(2024, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6752), "Sterile Swabs", 200, 2 },
                    { 11, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6753), "Active", "Surgical sutures for wound closure", new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6754), "Sutures", 120, 1 },
                    { 12, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6755), "Active", "Tubes for blood sample collection", new DateTime(2024, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6756), "Blood Collection Tubes", 300, 2 },
                    { 13, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6757), "Active", "Sterile drapes for surgical procedures", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6758), "Sterile Drapes", 100, 1 },
                    { 14, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6759), "Active", "Disposable scalpel blades for surgical procedures", new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6760), "Scalpel Blades", 80, 1 },
                    { 15, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6761), "Active", "Sterile wipes for disinfection purposes", new DateTime(2025, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6762), "Alcohol Wipes", 250, 2 },
                    { 16, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6763), "Active", "Electrodes used in ECG monitoring", new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6764), "ECG Electrodes", 150, 2 },
                    { 17, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6765), "Active", "Sterile catheters for medical procedures", new DateTime(2025, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6766), "Catheters", 100, 1 },
                    { 18, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6767), "Active", "Cuffs used in blood pressure monitoring", new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6768), "Blood Pressure Cuffs", 60, 2 },
                    { 19, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6769), "Active", "Sterile cannulas for intravenous therapy", new DateTime(2024, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6770), "IV Cannulas", 150, 1 },
                    { 20, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6771), "Active", "Gel used to reduce friction during procedures", new DateTime(2025, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6772), "Lubricating Gel", 100, 1 },
                    { 21, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6773), "Active", "Sterile gauze pads for wound dressing", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6774), "Gauze Pads", 400, 1 },
                    { 22, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6775), "Active", "Pouches for sterilizing surgical instruments", new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6776), "Sterilization Pouches", 250, 1 },
                    { 23, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6783), "Active", "Masks for administering oxygen to patients", new DateTime(2025, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6783), "Oxygen Masks", 70, 1 },
                    { 24, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6785), "Active", "Strips used for non-invasive wound closure", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6785), "Wound Closure Strips", 150, 1 },
                    { 25, new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6787), "Active", "Inhalers for asthma and respiratory conditions", new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 21, 12, 22, 13, 763, DateTimeKind.Local).AddTicks(6787), "Inhalers", 60, 0 }
                });

            migrationBuilder.InsertData(
                table: "HospitalInfos",
                columns: new[] { "HospitalInfoId", "Address", "Email", "HospitalName", "PhoneNumber", "WebsiteUrl" },
                values: new object[] { 1, "123 Health St, Wellness City, HC 45678", "contact@timelesstechmed.com", "Timeless Technicians Medical Center", "+1-234-567-8901", "https://www.timelesstechmed.com" });

            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "MedicationId", "DeletionStatus", "Description", "ExpiryDate", "Name", "Quantity", "Schedule", "Type" },
                values: new object[,]
                {
                    { 1, "Active", "Pain reliever and anti-inflammatory", new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aspirin", 100, "Schedule1", "Prescription" },
                    { 2, "Active", "Anti-inflammatory and pain relief", new DateTime(2024, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ibuprofen", 200, "Schedule4", "OverTheCounter" },
                    { 3, "Active", "Supports bone health", new DateTime(2024, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vitamin D", 150, "Schedule3", "Supplement" },
                    { 4, "Active", "Used to manage type 2 diabetes", new DateTime(2025, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Metformin", 50, "Schedule2", "Prescription" },
                    { 5, "Active", "Antihistamine for allergy relief", new DateTime(2024, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cetirizine", 80, "Schedule7", "OverTheCounter" },
                    { 6, "Active", "Supports bone health", new DateTime(2025, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Calcium Supplement", 60, "Schedule5", "Supplement" },
                    { 7, "Active", "Antibiotic used to treat bacterial infections", new DateTime(2025, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amoxicillin", 100, "Schedule1", "Prescription" },
                    { 8, "Active", "Pain reliever and fever reducer", new DateTime(2024, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paracetamol", 300, "Schedule6", "OverTheCounter" },
                    { 9, "Active", "Antihistamine for allergy symptoms", new DateTime(2024, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Loratadine", 120, "Schedule7", "OverTheCounter" },
                    { 10, "Active", "Hormone used to control blood sugar in diabetes", new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Insulin", 50, "Schedule1", "Prescription" },
                    { 11, "Active", "Used to treat acid reflux and heartburn", new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Omeprazole", 80, "Schedule2", "Prescription" },
                    { 12, "Active", "Used for temporary relief of cough", new DateTime(2024, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cough Syrup", 60, "Schedule6", "OverTheCounter" },
                    { 13, "Active", "Supports cell production and prevents certain birth defects", new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Folic Acid", 90, "Schedule5", "Supplement" },
                    { 14, "Active", "Steroid used to treat inflammation", new DateTime(2025, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Prednisone", 40, "Schedule1", "Prescription" },
                    { 15, "Active", "Anticoagulant used to prevent blood clots", new DateTime(2025, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Warfarin", 70, "Schedule3", "Prescription" },
                    { 16, "Active", "Used to lower cholesterol", new DateTime(2025, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Simvastatin", 60, "Schedule2", "Prescription" },
                    { 17, "Active", "Used to prevent strokes and heart attacks", new DateTime(2025, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Clopidogrel", 50, "Schedule2", "Prescription" },
                    { 18, "Active", "Diuretic used to treat high blood pressure", new DateTime(2025, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hydrochlorothiazide", 80, "Schedule1", "Prescription" },
                    { 19, "Active", "Hormone replacement for thyroid conditions", new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Levothyroxine", 60, "Schedule2", "Prescription" },
                    { 20, "Active", "Calcium channel blocker used to treat high blood pressure", new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amlodipine", 70, "Schedule2", "Prescription" },
                    { 21, "Active", "Relieves indigestion and heartburn", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Antacids", 150, "Schedule6", "OverTheCounter" },
                    { 22, "Active", "Antibiotic used to treat infections", new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ciprofloxacin", 50, "Schedule1", "Prescription" },
                    { 23, "Active", "ACE inhibitor used to treat high blood pressure", new DateTime(2025, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lisinopril", 80, "Schedule1", "Prescription" },
                    { 24, "Active", "Non-narcotic cough suppressant", new DateTime(2025, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Benzonatate", 90, "Schedule4", "Prescription" },
                    { 25, "Active", "Supports general health and wellness", new DateTime(2024, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Multivitamins", 200, "Schedule5", "Supplement" }
                });

            migrationBuilder.InsertData(
                table: "Wards",
                columns: new[] { "WardId", "Capacity", "Description", "WardName", "WardStatus" },
                values: new object[,]
                {
                    { 1, 20, "For patients with a range of medical conditions not requiring specialized care.", "General Medical Ward", "Active" },
                    { 2, 15, "Ward for surgical patients.", "Surgical", "Active" },
                    { 3, 10, "Ward for pediatric patients.", "Pediatrics", "Active" },
                    { 4, 12, "Ward for orthopedic patients.", "Orthopedics", "Active" },
                    { 5, 8, "Ward for neurological conditions.", "Neurology", "Active" },
                    { 6, 10, "Ward for cancer patients.", "Oncology", "Active" },
                    { 7, 10, "For critically ill patients requiring constant monitoring and intensive care.", "Intensive Care Unit (ICU)", "Active" },
                    { 8, 10, "For acute cases requiring immediate attention.", "Emergency Ward", "Active" },
                    { 9, 12, "Ward for pregnant women and childbirth.", "Maternity", "Active" },
                    { 10, 15, "Ward specializing in heart-related conditions.", "Cardiology", "Active" },
                    { 11, 6, "Specialized unit for burn patients.", "Burn Unit", "Active" },
                    { 12, 8, "Ward focused on elderly patients.", "Geriatrics", "Active" },
                    { 13, 20, "Ward for mental health conditions.", "Psychiatric", "Active" },
                    { 14, 15, "For patients recovering from surgeries or trauma.", "Rehabilitation", "Active" },
                    { 15, 8, "Ward for premature or critically ill newborns.", "Neonatal Intensive Care Unit (NICU)", "Active" },
                    { 16, 10, "Ward for patients requiring kidney dialysis.", "Dialysis", "Active" }
                });

            migrationBuilder.InsertData(
                table: "Beds",
                columns: new[] { "BedId", "BedNumber", "DeletionStatus", "Status", "WardId" },
                values: new object[,]
                {
                    { 1, "101", "Active", "Available", 1 },
                    { 2, "102", "Active", "Available", 1 },
                    { 3, "103", "Active", "Available", 1 },
                    { 4, "104", "Active", "Available", 1 },
                    { 5, "105", "Active", "Available", 1 },
                    { 6, "106", "Active", "Available", 1 },
                    { 7, "201", "Active", "Available", 2 },
                    { 8, "202", "Active", "Available", 2 },
                    { 9, "203", "Active", "Available", 2 },
                    { 10, "204", "Active", "Available", 2 },
                    { 11, "205", "Active", "Available", 2 },
                    { 12, "206", "Active", "Available", 2 },
                    { 13, "301", "Active", "Available", 3 },
                    { 14, "302", "Active", "Available", 3 },
                    { 15, "303", "Active", "Available", 3 },
                    { 16, "304", "Active", "Available", 3 },
                    { 17, "305", "Active", "Available", 3 },
                    { 18, "306", "Active", "Available", 3 },
                    { 19, "401", "Active", "Available", 4 },
                    { 20, "402", "Active", "Available", 4 },
                    { 21, "403", "Active", "Available", 4 },
                    { 22, "404", "Active", "Available", 4 },
                    { 23, "405", "Active", "Available", 4 },
                    { 24, "406", "Active", "Available", 4 },
                    { 25, "501", "Active", "Available", 5 },
                    { 26, "502", "Active", "Available", 5 },
                    { 27, "503", "Active", "Available", 5 },
                    { 28, "504", "Active", "Available", 5 },
                    { 29, "505", "Active", "Available", 5 },
                    { 30, "506", "Active", "Available", 5 },
                    { 31, "601", "Active", "Available", 6 },
                    { 32, "602", "Active", "Available", 6 },
                    { 33, "603", "Active", "Available", 6 },
                    { 34, "604", "Active", "Available", 6 },
                    { 35, "605", "Active", "Available", 6 },
                    { 36, "606", "Active", "Available", 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmitPatients_NurseId",
                table: "AdmitPatients",
                column: "NurseId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmitPatients_PatientId",
                table: "AdmitPatients",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BedAssignments_AdmitPatientId",
                table: "BedAssignments",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_BedAssignments_BedId",
                table: "BedAssignments",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_Beds_WardId",
                table: "Beds",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Discharges_AdmitPatientId",
                table: "Discharges",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAdviceRequests_DoctorId",
                table: "DoctorAdviceRequests",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAdviceRequests_NurseId",
                table: "DoctorAdviceRequests",
                column: "NurseId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAdviceRequests_PatientId",
                table: "DoctorAdviceRequests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationPrescriptions_AdmitPatientId",
                table: "MedicationPrescriptions",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationPrescriptions_MedicationId",
                table: "MedicationPrescriptions",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_NonScheduledMedications_MedicationId",
                table: "NonScheduledMedications",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergies_AdmitPatientId",
                table: "PatientAllergies",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergies_AllergyId",
                table: "PatientAllergies",
                column: "AllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergies_PatientId",
                table: "PatientAllergies",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAppointments_AdmitPatientId",
                table: "PatientAppointments",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAppointments_DoctorId",
                table: "PatientAppointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientConditions_AdmitPatientId",
                table: "PatientConditions",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientConditions_ConditionId",
                table: "PatientConditions",
                column: "ConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientConditions_PatientId",
                table: "PatientConditions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischargeInstructions_AdmitPatientId",
                table: "PatientDischargeInstructions",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientFolders_NurseId",
                table: "PatientFolders",
                column: "NurseId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientInstructions_AdmitPatientId",
                table: "PatientInstructions",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_AdmitPatientId",
                table: "PatientMedications",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_MedicationId",
                table: "PatientMedications",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_PatientId",
                table: "PatientMedications",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMovements_AdmitPatientId",
                table: "PatientMovements",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTreatments_PatientId",
                table: "PatientTreatments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisitSchedules_AdmitPatientId",
                table: "PatientVisitSchedules",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVitals_PatientId",
                table: "PatientVitals",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_DoctorId",
                table: "Prescriptions",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_PatientId",
                table: "Prescriptions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ReAdmissionHistories_AdmitPatientId",
                table: "ReAdmissionHistories",
                column: "AdmitPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMedications_MedicationId",
                table: "ScheduledMedications",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLogs_ConsumableId",
                table: "StockLogs",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRequests_ConsumableId",
                table: "StockRequests",
                column: "ConsumableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BedAssignments");

            migrationBuilder.DropTable(
                name: "ChangeEmailViewModel");

            migrationBuilder.DropTable(
                name: "ChangePasswordViewModel");

            migrationBuilder.DropTable(
                name: "Discharges");

            migrationBuilder.DropTable(
                name: "DoctorAdviceRequests");

            migrationBuilder.DropTable(
                name: "HospitalInfos");

            migrationBuilder.DropTable(
                name: "Login");

            migrationBuilder.DropTable(
                name: "MedicationPrescriptions");

            migrationBuilder.DropTable(
                name: "NonScheduledMedications");

            migrationBuilder.DropTable(
                name: "PatientAllergies");

            migrationBuilder.DropTable(
                name: "PatientAppointments");

            migrationBuilder.DropTable(
                name: "PatientConditions");

            migrationBuilder.DropTable(
                name: "PatientDischargeInstructions");

            migrationBuilder.DropTable(
                name: "PatientFolders");

            migrationBuilder.DropTable(
                name: "PatientInstructions");

            migrationBuilder.DropTable(
                name: "PatientMedications");

            migrationBuilder.DropTable(
                name: "PatientMovements");

            migrationBuilder.DropTable(
                name: "PatientTreatments");

            migrationBuilder.DropTable(
                name: "PatientVisitSchedules");

            migrationBuilder.DropTable(
                name: "PatientVitals");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "ReAdmissionHistories");

            migrationBuilder.DropTable(
                name: "ScheduledMedications");

            migrationBuilder.DropTable(
                name: "StockLogs");

            migrationBuilder.DropTable(
                name: "StockRequests");

            migrationBuilder.DropTable(
                name: "UserListViewModel");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Beds");

            migrationBuilder.DropTable(
                name: "Allergies");

            migrationBuilder.DropTable(
                name: "Conditions");

            migrationBuilder.DropTable(
                name: "AdmitPatients");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "Consumables");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
