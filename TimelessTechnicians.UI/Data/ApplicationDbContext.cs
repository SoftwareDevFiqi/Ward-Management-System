using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using TimelessTechnicians.UI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using TimelessTechnicians.UI.ViewModel;

namespace TimelessTechnicians.UI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ApplicationUser> Register { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Consumable> Consumables { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<HospitalInfo> HospitalInfos { get; set; }
        public DbSet<BedAssignment> BedAssignments { get; set; }
        public DbSet<Discharge> Discharges { get; set; }
        public DbSet<PatientMovement> PatientMovements { get; set; }
        public DbSet<PatientVital> PatientVitals { get; set; }
        public DbSet<PatientTreatment> PatientTreatments { get; set; }
        public DbSet<DoctorAdviceRequest> DoctorAdviceRequests { get; set; }
        public DbSet<PatientInstruction> PatientInstructions { get; set; }
        public DbSet<PatientVisitSchedule> PatientVisitSchedules { get; set; }
        public DbSet<PatientAppointment> PatientAppointments { get; set; }
        public DbSet<PatientDischarge> PatientDischargeInstructions { get; set; }
        public DbSet<NonScheduledMedication> NonScheduledMedications { get; set; }
        public DbSet<ReAdmissionHistory> ReAdmissionHistories { get; set; }
        public DbSet<ScheduledMedication> ScheduledMedications { get; set; }
        public DbSet<AdmitPatient> AdmitPatients { get; set; }
        public DbSet<PatientAllergy> PatientAllergies { get; set; }
        public DbSet<PatientMedication> PatientMedications { get; set; }
        public DbSet<PatientCondition> PatientConditions { get; set; }
        public DbSet<PatientFolder> PatientFolders { get; set; }
        public DbSet<MedicationPrescription> MedicationPrescriptions { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        public DbSet<StockLog> StockLogs { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }












        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure uniqueness of UserName and Email
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.UserName).IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email).IsUnique();











            // Configure Login entity as keyless if it's not supposed to have a key
            modelBuilder.Entity<Login>().HasNoKey();
            modelBuilder.Entity<ChangeEmailViewModel>().HasNoKey();
            modelBuilder.Entity<ChangePasswordViewModel>().HasNoKey();

            // Conversion of enum properties to string

            modelBuilder.Entity<AdmitPatient>()
       .Property(p => p.AdmitPatientStatus)
       .HasConversion<string>();

            modelBuilder.Entity<Discharge>()
                .Property(d => d.DischargeStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Allergy>()
                .Property(a => a.DeletionStatus)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Title)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.City)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.Suburb)
                .HasConversion<string>();

            modelBuilder.Entity<Bed>()
                .Property(b => b.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Bed>()
                .Property(b => b.DeletionStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Condition>()
                .Property(c => c.DeletionStatus)
                .HasConversion<string>();

            modelBuilder.Entity<BedAssignment>()
                .Property(b => b.BedAssignmentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Consumable>()
                .Property(c => c.DeletionStatus)
                .HasConversion<string>();

            modelBuilder.Entity<DoctorAdviceRequest>()
                .Property(d => d.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Medication>()
                .Property(m => m.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Medication>()
                .Property(m => m.DeletionStatus)
                .HasConversion<string>();

            modelBuilder.Entity<MedicationPrescription>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Medication>()
                .Property(m => m.Schedule)
                .HasConversion<string>();

            modelBuilder.Entity<NonScheduledMedication>()
                .Property(n => n.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PatientAllergy>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PatientAppointment>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PatientCondition>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PatientDischarge>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PatientFolder>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PatientInstruction>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PatientInstruction>()
                .Property(p => p.InstructionType)
                .HasConversion<string>();

            modelBuilder.Entity<PatientMedication>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PatientMovement>()
                .Property(p => p.MovementType)
                .HasConversion<string>();

            modelBuilder.Entity<PatientMovement>()
                .Property(p => p.MovementStatus)
                .HasConversion<string>();

            modelBuilder.Entity<PatientTreatment>()
                .Property(p => p.TreatmentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<PatientVisitSchedule>()
                .Property(p => p.Status)
                .HasConversion<string>();


            modelBuilder.Entity<PatientVital>()
                .Property(p => p.PatientVitalStatus)
                .HasConversion<string>();



            modelBuilder.Entity<Prescription>()
                .Property(p => p.Status)
                .HasConversion<string>();




            modelBuilder.Entity<ScheduledMedication>()
                .Property(p => p.ScheduledMedicationStatus)
                .HasConversion<string>();




            modelBuilder.Entity<StockRequest>()
                .Property(p => p.RequestStatus)
                .HasConversion<string>();




            modelBuilder.Entity<Ward>()
                .Property(p => p.WardStatus)
                .HasConversion<string>();




















            modelBuilder.Entity<AdmitPatient>()
    .HasMany(a => a.PatientAllergies)
    .WithOne(pa => pa.AdmitPatient)
    .HasForeignKey(pa => pa.AdmitPatientId)
    .OnDelete(DeleteBehavior.Restrict);






            modelBuilder.Entity<PatientAllergy>()
     .HasOne(pa => pa.Patient)
     .WithMany()
     .HasForeignKey(pa => pa.PatientId)
     .IsRequired() // Ensure the relationship is not optional
     .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Medication>()
        .HasQueryFilter(m => m.DeletionStatus == MedicationStatus.Active);

            modelBuilder.Entity<PatientMedication>()
                .HasOne(pm => pm.AdmitPatient)
                .WithMany()
                .HasForeignKey(pm => pm.AdmitPatientId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<PatientMedication>()
                .HasOne(pm => pm.Medication)
                .WithMany()
                .HasForeignKey(pm => pm.MedicationId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<PatientMedication>()
                .HasOne(pm => pm.Patient)
                .WithMany()
                .HasForeignKey(pm => pm.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdmitPatient>()
           .HasMany(a => a.PatientAllergies)
           .WithOne(pa => pa.AdmitPatient)
           .HasForeignKey(pa => pa.AdmitPatientId);

            modelBuilder.Entity<AdmitPatient>()
                .HasMany(a => a.PatientMedications)
                .WithOne(pm => pm.AdmitPatient)
                .HasForeignKey(pm => pm.AdmitPatientId);

            modelBuilder.Entity<PatientMedication>()
                .HasOne(pm => pm.Medication)
                .WithMany()
                .HasForeignKey(pm => pm.MedicationId);

            modelBuilder.Entity<PatientAllergy>()
                .HasOne(pa => pa.Allergy)
                .WithMany()
                .HasForeignKey(pa => pa.AllergyId);


            modelBuilder.Entity<PatientCondition>()
            .HasOne(pc => pc.Patient) // Navigation property in PatientCondition
            .WithMany() // No collection navigation property in ApplicationUser
            .HasForeignKey(pc => pc.PatientId)
            .OnDelete(DeleteBehavior.Restrict); // Or another delete behavior as needed

            // Configure Condition and PatientCondition relationships
            modelBuilder.Entity<PatientCondition>()
                .HasOne(pc => pc.Condition) // Navigation property in PatientCondition
                .WithMany() // No collection navigation property in Condition
                .HasForeignKey(pc => pc.ConditionId)
                .OnDelete(DeleteBehavior.Restrict); // Or another delete behavior as needed

            // Configure AdmitPatient and PatientCondition relationships
            modelBuilder.Entity<PatientCondition>()
                .HasOne(pc => pc.AdmitPatient) // Navigation property in PatientCondition
                .WithMany(ap => ap.PatientConditions) // Collection navigation property in AdmitPatient
                .HasForeignKey(pc => pc.AdmitPatientId)
                .OnDelete(DeleteBehavior.Cascade); // Or another delete behavior as needed




            modelBuilder.Entity<Condition>()
         .HasQueryFilter(c => c.DeletionStatus != ConditionStatus.Deleted);



            modelBuilder.Entity<Condition>()
        .HasQueryFilter(c => c.DeletionStatus != ConditionStatus.Deleted);

            // Apply matching filter for PatientCondition
            modelBuilder.Entity<PatientCondition>()
                .HasQueryFilter(pc => pc.Condition.DeletionStatus != ConditionStatus.Deleted);



            modelBuilder.Entity<PatientVital>()
           .HasOne(pv => pv.Patient)
           .WithMany() // Adjust if ApplicationUser has a collection of PatientVitals
           .HasForeignKey(pv => pv.PatientId)
           .OnDelete(DeleteBehavior.Cascade); // Adjust as needed


            modelBuilder.Entity<BedAssignment>()
                .HasOne(b => b.Bed)
                .WithMany()
                .HasForeignKey(b => b.BedId)
                .OnDelete(DeleteBehavior.Restrict); // Change to Restrict or SetNull


            modelBuilder.Entity<PatientVital>()
           .Property(pv => pv.PatientVitalStatus)
           .HasDefaultValue(PatientVitalStatus.Active);

            // Change to Restrict to avoid cascade

            modelBuilder.Entity<ScheduledMedication>()
                .HasOne(sm => sm.Medication)
                .WithMany() // or .WithMany(m => m.ScheduledMedications) if you have a navigation property in Medication
                .HasForeignKey(sm => sm.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientVital>()
        .HasOne(pv => pv.Patient)
        .WithMany() // Assuming there's no inverse navigation property
        .HasForeignKey(pv => pv.PatientId)
        .OnDelete(DeleteBehavior.Restrict); // No cascade delete





            modelBuilder.Entity<DoctorAdviceRequest>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Define the relationship between DoctorAdviceRequest and ApplicationUser (Nurse)


                // Define the relationship between DoctorAdviceRequest and ApplicationUser (Doctor)
                entity.HasOne(e => e.Doctor)
                      .WithMany() // No navigation property in ApplicationUser for Doctor
                      .HasForeignKey(e => e.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascading deletes

                // Define the relationship between DoctorAdviceRequest and ApplicationUser (Patient)
                entity.HasOne(e => e.Patient)
                      .WithMany() // No navigation property in ApplicationUser for Patient
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascading deletes
            });



            modelBuilder.Entity<DoctorAdviceRequest>()
        .HasOne(d => d.Nurse)
        .WithMany() // Assuming no inverse navigation property
        .HasForeignKey(d => d.NurseId)
        .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PatientAppointment>()
       .HasOne(pa => pa.Doctor)
       .WithMany() // Adjust if you have a collection of appointments in the Doctor entity
       .HasForeignKey(pa => pa.DoctorId)
       .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            modelBuilder.Entity<PatientAppointment>()
                .HasOne(pa => pa.AdmitPatient)
                .WithMany() // Adjust as necessary
                .HasForeignKey(pa => pa.AdmitPatientId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<AdmitPatient>()
                    .HasOne(a => a.Patient) // Navigation property to ApplicationUser (Patient)
                    .WithMany() // A Patient can have many AdmitPatients
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict); // Use Restrict instead of Cascade

            modelBuilder.Entity<AdmitPatient>()
                .HasOne<ApplicationUser>(a => a.Nurse) // Navigation property to ApplicationUser (Nurse)
                .WithMany() // A Nurse can have many AdmitPatients
                .HasForeignKey(a => a.NurseId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict instead of Cascade

            // Configure other entities as needed (PatientConditions, PatientAllergies, PatientMedications)
            modelBuilder.Entity<PatientCondition>()
                .HasOne(p => p.AdmitPatient) // Navigation property to AdmitPatient
                .WithMany(a => a.PatientConditions)
                .HasForeignKey(p => p.AdmitPatientId);

            modelBuilder.Entity<PatientAllergy>()
                .HasOne(p => p.AdmitPatient) // Navigation property to AdmitPatient
                .WithMany(a => a.PatientAllergies)
                .HasForeignKey(p => p.AdmitPatientId);

            modelBuilder.Entity<PatientMedication>()
                .HasOne(p => p.AdmitPatient) // Navigation property to AdmitPatient
                .WithMany(a => a.PatientMedications)
                .HasForeignKey(p => p.AdmitPatientId);


            modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Patient)
            .WithMany()
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Restrict); // Change this line

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);





































            SeedUsers(modelBuilder);









            modelBuilder.Entity<Allergy>().HasData(
                    new Allergy
                    {
                    AllergyId = 1,
                    Name = "Peanuts",
                    Description = "Allergy to peanuts and peanut products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 2,
                    Name = "Shellfish",
                    Description = "Allergy to shellfish including shrimp, crab, and lobster.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 3,
                    Name = "Penicillin",
                    Description = "Allergy to penicillin and related antibiotics.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 4,
                    Name = "Eggs",
                    Description = "Allergy to eggs and products containing eggs.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 5,
                    Name = "Milk",
                    Description = "Allergy to cow's milk and dairy products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 6,
                    Name = "Wheat",
                    Description = "Allergy to wheat or wheat-based products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 7,
                    Name = "Soy",
                    Description = "Allergy to soybeans and soy-based products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 8,
                    Name = "Latex",
                    Description = "Allergy to natural rubber latex products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 9,
                    Name = "Insect stings",
                    Description = "Allergic reaction to insect stings, such as bees or wasps.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 10,
                    Name = "Pollen",
                    Description = "Allergy to pollen from trees, grass, or weeds.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 11,
                    Name = "Mold",
                    Description = "Allergy to mold spores found indoors and outdoors.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 12,
                    Name = "Animal Dander",
                    Description = "Allergy to dander from animals, particularly cats and dogs.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 13,
                    Name = "Dust Mites",
                    Description = "Allergy to dust mites that live in household dust.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 14,
                    Name = "Fragrances",
                    Description = "Allergy or sensitivity to fragrances found in perfumes, soaps, or detergents.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 15,
                    Name = "Nickel",
                    Description = "Allergy to nickel found in jewelry, watches, or belt buckles.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 16,
                    Name = "Citrus Fruits",
                    Description = "Allergy to citrus fruits such as oranges, lemons, or grapefruits.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 17,
                    Name = "Gluten",
                    Description = "Allergy to gluten, a protein found in wheat, barley, and rye.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 18,
                    Name = "Avocados",
                    Description = "Allergy to avocados and products containing avocado.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 19,
                    Name = "Tree Nuts",
                    Description = "Allergy to tree nuts such as almonds, walnuts, or cashews.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 20,
                    Name = "Chocolate",
                    Description = "Allergy or sensitivity to chocolate and cocoa products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 21,
                    Name = "Corn",
                    Description = "Allergy to corn or corn-based products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 22,
                    Name = "Bananas",
                    Description = "Allergy to bananas and products containing bananas.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 23,
                    Name = "Garlic",
                    Description = "Allergy to garlic and garlic-based products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 24,
                    Name = "Tomatoes",
                    Description = "Allergy to tomatoes and products containing tomatoes.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 25,
                    Name = "Apples",
                    Description = "Allergy to apples and products containing apples.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 26,
                    Name = "Aspartame",
                    Description = "Allergy or sensitivity to artificial sweeteners like aspartame.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 27,
                    Name = "Barley",
                    Description = "Allergy to barley and barley-based products.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 28,
                    Name = "Sesame Seeds",
                    Description = "Allergy to sesame seeds and sesame oil.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 29,
                    Name = "Sunflower Seeds",
                    Description = "Allergy to sunflower seeds and sunflower oil.",
                    DeletionStatus = AllergyStatus.Active
                    },
                    new Allergy
                    {
                    AllergyId = 30,
                    Name = "Mustard",
                    Description = "Allergy to mustard seeds and mustard-based products.",
                    DeletionStatus = AllergyStatus.Active
                    }
                    );







            // Seed data for Wards
            modelBuilder.Entity<Ward>().HasData(
                new Ward { WardId = 1, WardName = "General Medical Ward", Description = "For patients with a range of medical conditions not requiring specialized care.", WardStatus = WardStatus.Active, Capacity = 20 },
                new Ward { WardId = 2, WardName = "Surgical", Description = "Ward for surgical patients.", WardStatus = WardStatus.Active, Capacity = 15 },
                new Ward { WardId = 3, WardName = "Pediatrics", Description = "Ward for pediatric patients.", WardStatus = WardStatus.Active, Capacity = 10 },
                new Ward { WardId = 4, WardName = "Orthopedics", Description = "Ward for orthopedic patients.", WardStatus = WardStatus.Active, Capacity = 12 },
                new Ward { WardId = 5, WardName = "Neurology", Description = "Ward for neurological conditions.", WardStatus = WardStatus.Active, Capacity = 8 },
                new Ward { WardId = 6, WardName = "Oncology", Description = "Ward for cancer patients.", WardStatus = WardStatus.Active, Capacity = 10 },
                new Ward { WardId = 7, WardName = "Intensive Care Unit (ICU)", Description = "For critically ill patients requiring constant monitoring and intensive care.", WardStatus = WardStatus.Active, Capacity = 10 },
                new Ward { WardId = 8, WardName = "Emergency Ward", Description = "For acute cases requiring immediate attention.", WardStatus = WardStatus.Active, Capacity = 10 },
                new Ward { WardId = 9, WardName = "Maternity", Description = "Ward for pregnant women and childbirth.", WardStatus = WardStatus.Active, Capacity = 12 },
                new Ward { WardId = 10, WardName = "Cardiology", Description = "Ward specializing in heart-related conditions.", WardStatus = WardStatus.Active, Capacity = 15 },
                new Ward { WardId = 11, WardName = "Burn Unit", Description = "Specialized unit for burn patients.", WardStatus = WardStatus.Active, Capacity = 6 },
                new Ward { WardId = 12, WardName = "Geriatrics", Description = "Ward focused on elderly patients.", WardStatus = WardStatus.Active, Capacity = 8 },
                new Ward { WardId = 13, WardName = "Psychiatric", Description = "Ward for mental health conditions.", WardStatus = WardStatus.Active, Capacity = 20 },
                new Ward { WardId = 14, WardName = "Rehabilitation", Description = "For patients recovering from surgeries or trauma.", WardStatus = WardStatus.Active, Capacity = 15 },
                new Ward { WardId = 15, WardName = "Neonatal Intensive Care Unit (NICU)", Description = "Ward for premature or critically ill newborns.", WardStatus = WardStatus.Active, Capacity = 8 },
                new Ward { WardId = 16, WardName = "Dialysis", Description = "Ward for patients requiring kidney dialysis.", WardStatus = WardStatus.Active, Capacity = 10 }
            );





            // Seed data for Beds
            modelBuilder.Entity<Bed>().HasData(
                // Beds for General Medicine
                new Bed { BedId = 1, BedNumber = "101", Status = BedStatus.Available, WardId = 1, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 2, BedNumber = "102", Status = BedStatus.Available, WardId = 1, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 3, BedNumber = "103", Status = BedStatus.Available, WardId = 1, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 4, BedNumber = "104", Status = BedStatus.Available, WardId = 1, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 5, BedNumber = "105", Status = BedStatus.Available, WardId = 1, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 6, BedNumber = "106", Status = BedStatus.Available, WardId = 1, DeletionStatus = CondtionStatus.Active },

                // Beds for Surgical
                new Bed { BedId = 7, BedNumber = "201", Status = BedStatus.Available, WardId = 2, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 8, BedNumber = "202", Status = BedStatus.Available, WardId = 2, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 9, BedNumber = "203", Status = BedStatus.Available, WardId = 2, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 10, BedNumber = "204", Status = BedStatus.Available, WardId = 2, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 11, BedNumber = "205", Status = BedStatus.Available, WardId = 2, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 12, BedNumber = "206", Status = BedStatus.Available, WardId = 2, DeletionStatus = CondtionStatus.Active },

                // Beds for Pediatrics
                new Bed { BedId = 13, BedNumber = "301", Status = BedStatus.Available, WardId = 3, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 14, BedNumber = "302", Status = BedStatus.Available, WardId = 3, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 15, BedNumber = "303", Status = BedStatus.Available, WardId = 3, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 16, BedNumber = "304", Status = BedStatus.Available, WardId = 3, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 17, BedNumber = "305", Status = BedStatus.Available, WardId = 3, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 18, BedNumber = "306", Status = BedStatus.Available, WardId = 3, DeletionStatus = CondtionStatus.Active },

                // Beds for Orthopedics
                new Bed { BedId = 19, BedNumber = "401", Status = BedStatus.Available, WardId = 4, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 20, BedNumber = "402", Status = BedStatus.Available, WardId = 4, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 21, BedNumber = "403", Status = BedStatus.Available, WardId = 4, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 22, BedNumber = "404", Status = BedStatus.Available, WardId = 4, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 23, BedNumber = "405", Status = BedStatus.Available, WardId = 4, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 24, BedNumber = "406", Status = BedStatus.Available, WardId = 4, DeletionStatus = CondtionStatus.Active },

                // Beds for Neurology
                new Bed { BedId = 25, BedNumber = "501", Status = BedStatus.Available, WardId = 5, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 26, BedNumber = "502", Status = BedStatus.Available, WardId = 5, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 27, BedNumber = "503", Status = BedStatus.Available, WardId = 5, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 28, BedNumber = "504", Status = BedStatus.Available, WardId = 5, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 29, BedNumber = "505", Status = BedStatus.Available, WardId = 5, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 30, BedNumber = "506", Status = BedStatus.Available, WardId = 5, DeletionStatus = CondtionStatus.Active },

                // Beds for Oncology
                new Bed { BedId = 31, BedNumber = "601", Status = BedStatus.Available, WardId = 6, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 32, BedNumber = "602", Status = BedStatus.Available, WardId = 6, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 33, BedNumber = "603", Status = BedStatus.Available, WardId = 6, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 34, BedNumber = "604", Status = BedStatus.Available, WardId = 6, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 35, BedNumber = "605", Status = BedStatus.Available, WardId = 6, DeletionStatus = CondtionStatus.Active },
                new Bed { BedId = 36, BedNumber = "606", Status = BedStatus.Available, WardId = 6, DeletionStatus = CondtionStatus.Active }
            );



            // Seed data for Conditions
            modelBuilder.Entity<Condition>().HasData(
                  new Condition
                  {
                      ConditionId = 1,
                      Name = "Diabetes",
                      Description = "A chronic condition that affects how the body processes blood sugar (glucose).",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 2,
                      Name = "Hypertension",
                      Description = "A condition in which the blood pressure in the arteries is persistently elevated.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 3,
                      Name = "Asthma",
                      Description = "A condition in which a person's airways become inflamed, narrow, and swell.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 4,
                      Name = "Chronic Kidney Disease",
                      Description = "A long-term condition where the kidneys do not work effectively.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 5,
                      Name = "Heart Disease",
                      Description = "A range of conditions that affect the heart.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 6,
                      Name = "Stroke",
                      Description = "A medical condition where poor blood flow to the brain results in cell death.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 7,
                      Name = "Chronic Obstructive Pulmonary Disease (COPD)",
                      Description = "A group of lung diseases that block airflow and make it difficult to breathe.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 8,
                      Name = "Alzheimer's Disease",
                      Description = "A progressive disease that destroys memory and other important mental functions.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 9,
                      Name = "Parkinson's Disease",
                      Description = "A disorder of the central nervous system that affects movement, often including tremors.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 10,
                      Name = "Epilepsy",
                      Description = "A neurological disorder marked by sudden recurrent episodes of sensory disturbance, loss of consciousness, or convulsions.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 11,
                      Name = "Multiple Sclerosis (MS)",
                      Description = "A disease in which the immune system eats away at the protective covering of nerves.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 12,
                      Name = "HIV/AIDS",
                      Description = "A virus that attacks the immune system, leading to life-threatening infections and cancers.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 13,
                      Name = "Arthritis",
                      Description = "Inflammation of one or more of your joints causing pain and stiffness.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 14,
                      Name = "Osteoporosis",
                      Description = "A condition in which bones become weak and brittle.",
                      DeletionStatus = ConditionStatus.Active
                  },
                  new Condition
                  {
                      ConditionId = 15,
                      Name = "Cancer",
                      Description = "A disease in which abnormal cells divide uncontrollably and destroy body tissue.",
                      DeletionStatus = ConditionStatus.Active
                  }
                );

























            modelBuilder.Entity<Consumable>().HasData(
     new Consumable
     {
         ConsumableId = 1,
         Name = "Bandages",
         Type = ConsumableType.Surgical,
         Quantity = 100,
         ExpiryDate = new DateTime(2025, 12, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile bandages for wound care",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 2,
         Name = "Syringes",
         Type = ConsumableType.Surgical,
         Quantity = 150,
         ExpiryDate = new DateTime(2024, 06, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Disposable syringes for injections",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 3,
         Name = "Gloves",
         Type = ConsumableType.Surgical,
         Quantity = 200,
         ExpiryDate = new DateTime(2024, 12, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Latex gloves for medical procedures",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 4,
         Name = "X-Ray Film",
         Type = ConsumableType.Diagnostic,
         Quantity = 50,
         ExpiryDate = new DateTime(2025, 03, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Film used for X-ray imaging",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 5,
         Name = "Antibiotic Ointment",
         Type = ConsumableType.Medication,
         Quantity = 75,
         ExpiryDate = new DateTime(2024, 09, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Topical ointment for infections",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 6,
         Name = "Surgical Masks",
         Type = ConsumableType.Surgical,
         Quantity = 500,
         ExpiryDate = new DateTime(2024, 08, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Masks for protection against infection",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 7,
         Name = "Thermometers",
         Type = ConsumableType.Diagnostic,
         Quantity = 30,
         ExpiryDate = new DateTime(2026, 01, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Digital thermometers for temperature measurement",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 8,
         Name = "Saline Solution",
         Type = ConsumableType.Medication,
         Quantity = 100,
         ExpiryDate = new DateTime(2025, 07, 15),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile saline solution for wound cleaning and IV use",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 9,
         Name = "IV Drip Sets",
         Type = ConsumableType.Surgical,
         Quantity = 75,
         ExpiryDate = new DateTime(2025, 04, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile IV sets for intravenous therapy",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 10,
         Name = "Sterile Swabs",
         Type = ConsumableType.Diagnostic,
         Quantity = 200,
         ExpiryDate = new DateTime(2024, 11, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile cotton swabs for wound cleaning or diagnostic purposes",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 11,
         Name = "Sutures",
         Type = ConsumableType.Surgical,
         Quantity = 120,
         ExpiryDate = new DateTime(2026, 05, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Surgical sutures for wound closure",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 12,
         Name = "Blood Collection Tubes",
         Type = ConsumableType.Diagnostic,
         Quantity = 300,
         ExpiryDate = new DateTime(2024, 09, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Tubes for blood sample collection",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 13,
         Name = "Sterile Drapes",
         Type = ConsumableType.Surgical,
         Quantity = 100,
         ExpiryDate = new DateTime(2025, 11, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile drapes for surgical procedures",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 14,
         Name = "Scalpel Blades",
         Type = ConsumableType.Surgical,
         Quantity = 80,
         ExpiryDate = new DateTime(2025, 04, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Disposable scalpel blades for surgical procedures",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 15,
         Name = "Alcohol Wipes",
         Type = ConsumableType.Diagnostic,
         Quantity = 250,
         ExpiryDate = new DateTime(2025, 08, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile wipes for disinfection purposes",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 16,
         Name = "ECG Electrodes",
         Type = ConsumableType.Diagnostic,
         Quantity = 150,
         ExpiryDate = new DateTime(2025, 06, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Electrodes used in ECG monitoring",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 17,
         Name = "Catheters",
         Type = ConsumableType.Surgical,
         Quantity = 100,
         ExpiryDate = new DateTime(2025, 07, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile catheters for medical procedures",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 18,
         Name = "Blood Pressure Cuffs",
         Type = ConsumableType.Diagnostic,
         Quantity = 60,
         ExpiryDate = new DateTime(2026, 01, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Cuffs used in blood pressure monitoring",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 19,
         Name = "IV Cannulas",
         Type = ConsumableType.Surgical,
         Quantity = 150,
         ExpiryDate = new DateTime(2024, 10, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile cannulas for intravenous therapy",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 20,
         Name = "Lubricating Gel",
         Type = ConsumableType.Surgical,
         Quantity = 100,
         ExpiryDate = new DateTime(2025, 09, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Gel used to reduce friction during procedures",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 21,
         Name = "Gauze Pads",
         Type = ConsumableType.Surgical,
         Quantity = 400,
         ExpiryDate = new DateTime(2025, 11, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Sterile gauze pads for wound dressing",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 22,
         Name = "Sterilization Pouches",
         Type = ConsumableType.Surgical,
         Quantity = 250,
         ExpiryDate = new DateTime(2025, 04, 30),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Pouches for sterilizing surgical instruments",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 23,
         Name = "Oxygen Masks",
         Type = ConsumableType.Surgical,
         Quantity = 70,
         ExpiryDate = new DateTime(2025, 05, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Masks for administering oxygen to patients",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 24,
         Name = "Wound Closure Strips",
         Type = ConsumableType.Surgical,
         Quantity = 150,
         ExpiryDate = new DateTime(2024, 12, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Strips used for non-invasive wound closure",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     },
     new Consumable
     {
         ConsumableId = 25,
         Name = "Inhalers",
         Type = ConsumableType.Medication,
         Quantity = 60,
         ExpiryDate = new DateTime(2025, 03, 31),
         DeletionStatus = ConsumableStatus.Active,
         Description = "Inhalers for asthma and respiratory conditions",
         CreatedDate = DateTime.Now,
         LastUpdatedDate = DateTime.Now
     }
 );






            // Seed data for HospitalInfo
            modelBuilder.Entity<HospitalInfo>().HasData(
                new HospitalInfo
                {
                    HospitalInfoId = 1,
                    HospitalName = "Timeless Technicians Medical Center",
                    Address = "123 Health St, Wellness City, HC 45678",
                    PhoneNumber = "+1-234-567-8901",
                    Email = "contact@timelesstechmed.com",
                    WebsiteUrl = "https://www.timelesstechmed.com"
                }
            );

            // Seed data for Medications
            modelBuilder.Entity<Medication>().HasData(
     new Medication
     {
         MedicationId = 1,
         Name = "Aspirin",
         Type = MedicationType.Prescription,
         Quantity = 100,
         ExpiryDate = new DateTime(2025, 12, 31),
         Description = "Pain reliever and anti-inflammatory",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule1
     },
     new Medication
     {
         MedicationId = 2,
         Name = "Ibuprofen",
         Type = MedicationType.OverTheCounter,
         Quantity = 200,
         ExpiryDate = new DateTime(2024, 11, 30),
         Description = "Anti-inflammatory and pain relief",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule4
     },
     new Medication
     {
         MedicationId = 3,
         Name = "Vitamin D",
         Type = MedicationType.Supplement,
         Quantity = 150,
         ExpiryDate = new DateTime(2024, 06, 30),
         Description = "Supports bone health",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule3
     },
     new Medication
     {
         MedicationId = 4,
         Name = "Metformin",
         Type = MedicationType.Prescription,
         Quantity = 50,
         ExpiryDate = new DateTime(2025, 01, 31),
         Description = "Used to manage type 2 diabetes",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule2
     },
     new Medication
     {
         MedicationId = 5,
         Name = "Cetirizine",
         Type = MedicationType.OverTheCounter,
         Quantity = 80,
         ExpiryDate = new DateTime(2024, 08, 15),
         Description = "Antihistamine for allergy relief",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule7
     },
     new Medication
     {
         MedicationId = 6,
         Name = "Calcium Supplement",
         Type = MedicationType.Supplement,
         Quantity = 60,
         ExpiryDate = new DateTime(2025, 09, 30),
         Description = "Supports bone health",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule5
     },
     new Medication
     {
         MedicationId = 7,
         Name = "Amoxicillin",
         Type = MedicationType.Prescription,
         Quantity = 100,
         ExpiryDate = new DateTime(2025, 07, 31),
         Description = "Antibiotic used to treat bacterial infections",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule1
     },
     new Medication
     {
         MedicationId = 8,
         Name = "Paracetamol",
         Type = MedicationType.OverTheCounter,
         Quantity = 300,
         ExpiryDate = new DateTime(2024, 10, 30),
         Description = "Pain reliever and fever reducer",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule6
     },
     new Medication
     {
         MedicationId = 9,
         Name = "Loratadine",
         Type = MedicationType.OverTheCounter,
         Quantity = 120,
         ExpiryDate = new DateTime(2024, 12, 15),
         Description = "Antihistamine for allergy symptoms",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule7
     },
     new Medication
     {
         MedicationId = 10,
         Name = "Insulin",
         Type = MedicationType.Prescription,
         Quantity = 50,
         ExpiryDate = new DateTime(2025, 03, 31),
         Description = "Hormone used to control blood sugar in diabetes",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule1
     },

     new Medication
     {
         MedicationId = 11,
         Name = "Omeprazole",
         Type = MedicationType.Prescription,
         Quantity = 80,
         ExpiryDate = new DateTime(2025, 06, 30),
         Description = "Used to treat acid reflux and heartburn",
         DeletionStatus = MedicationStatus.Active,
         Schedule = MedicationSchedule.Schedule2
     },
    new Medication
    {
        MedicationId = 12,
        Name = "Cough Syrup",
        Type = MedicationType.OverTheCounter,
        Quantity = 60,
        ExpiryDate = new DateTime(2024, 09, 15),
        Description = "Used for temporary relief of cough",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule6
    },
    new Medication
    {
        MedicationId = 13,
        Name = "Folic Acid",
        Type = MedicationType.Supplement,
        Quantity = 90,
        ExpiryDate = new DateTime(2025, 04, 30),
        Description = "Supports cell production and prevents certain birth defects",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule5
    },
    new Medication
    {
        MedicationId = 14,
        Name = "Prednisone",
        Type = MedicationType.Prescription,
        Quantity = 40,
        ExpiryDate = new DateTime(2025, 02, 28),
        Description = "Steroid used to treat inflammation",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule1
    },
    new Medication
    {
        MedicationId = 15,
        Name = "Warfarin",
        Type = MedicationType.Prescription,
        Quantity = 70,
        ExpiryDate = new DateTime(2025, 05, 31),
        Description = "Anticoagulant used to prevent blood clots",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule3
    },
    new Medication
    {
        MedicationId = 16,
        Name = "Simvastatin",
        Type = MedicationType.Prescription,
        Quantity = 60,
        ExpiryDate = new DateTime(2025, 12, 30),
        Description = "Used to lower cholesterol",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule2
    },
    new Medication
    {
        MedicationId = 17,
        Name = "Clopidogrel",
        Type = MedicationType.Prescription,
        Quantity = 50,
        ExpiryDate = new DateTime(2025, 08, 31),
        Description = "Used to prevent strokes and heart attacks",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule2
    },
    new Medication
    {
        MedicationId = 18,
        Name = "Hydrochlorothiazide",
        Type = MedicationType.Prescription,
        Quantity = 80,
        ExpiryDate = new DateTime(2025, 07, 31),
        Description = "Diuretic used to treat high blood pressure",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule1
    },
    new Medication
    {
        MedicationId = 19,
        Name = "Levothyroxine",
        Type = MedicationType.Prescription,
        Quantity = 60,
        ExpiryDate = new DateTime(2025, 10, 31),
        Description = "Hormone replacement for thyroid conditions",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule2
    },
    new Medication
    {
        MedicationId = 20,
        Name = "Amlodipine",
        Type = MedicationType.Prescription,
        Quantity = 70,
        ExpiryDate = new DateTime(2025, 06, 30),
        Description = "Calcium channel blocker used to treat high blood pressure",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule2
    },
    new Medication
    {
        MedicationId = 21,
        Name = "Antacids",
        Type = MedicationType.OverTheCounter,
        Quantity = 150,
        ExpiryDate = new DateTime(2024, 12, 31),
        Description = "Relieves indigestion and heartburn",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule6
    },
    new Medication
    {
        MedicationId = 22,
        Name = "Ciprofloxacin",
        Type = MedicationType.Prescription,
        Quantity = 50,
        ExpiryDate = new DateTime(2025, 03, 31),
        Description = "Antibiotic used to treat infections",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule1
    },
    new Medication
    {
        MedicationId = 23,
        Name = "Lisinopril",
        Type = MedicationType.Prescription,
        Quantity = 80,
        ExpiryDate = new DateTime(2025, 05, 31),
        Description = "ACE inhibitor used to treat high blood pressure",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule1
    },
    new Medication
    {
        MedicationId = 24,
        Name = "Benzonatate",
        Type = MedicationType.Prescription,
        Quantity = 90,
        ExpiryDate = new DateTime(2025, 02, 28),
        Description = "Non-narcotic cough suppressant",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule4
    },
    new Medication
    {
        MedicationId = 25,
        Name = "Multivitamins",
        Type = MedicationType.Supplement,
        Quantity = 200,
        ExpiryDate = new DateTime(2024, 10, 31),
        Description = "Supports general health and wellness",
        DeletionStatus = MedicationStatus.Active,
        Schedule = MedicationSchedule.Schedule5
    }
 );






























        }




















































        private void SeedUsers(ModelBuilder modelBuilder)
        {
            // Create a password hasher
            var passwordHasher = new PasswordHasher<ApplicationUser>();

            // Define an array of users with different roles
            var users = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            UserName = "Main Admin",
            Email = "main.admin@timelesstechnicians.co.za",
            FirstName = "Main",
            LastName = "Admin",
            Title = ApplicationUser.Titles.Mr,
            DateOfBirth = new DateTime(1980, 1, 1),
            Password = "admin@123",
            ConfirmPassword = "admin@123",
            Role = ApplicationUser.UserRole.ADMINISTRATOR,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "123 Admin St, Port Elizabeth",
            Status = ApplicationUser.UserStatus.Active,
            PasswordHash = passwordHasher.HashPassword(null, "admin@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
        new ApplicationUser
        {
            UserName = "Main Nurse",
            Email = "main.nurse@timelesstechnicians.co.za",
            FirstName = "Main",
            LastName = "Nurse",
            Title = ApplicationUser.Titles.Mrs,
            DateOfBirth = new DateTime(1985, 6, 15),
            Password = "nurse@123",
            ConfirmPassword = "nurse@123",
            Role = ApplicationUser.UserRole.NURSE,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.NorthEnd,
            Address = "456 Nurse Ave, North End",
            Status = ApplicationUser.UserStatus.Active,
            PasswordHash = passwordHasher.HashPassword(null, "nurse@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
       new ApplicationUser
        {
            UserName = "Main Doctor",
            Email = "main.doctor@timelesstechnicians.co.za",
            FirstName = "Main",
            LastName = "Doctor",
            Title = ApplicationUser.Titles.Mrs,
            DateOfBirth = new DateTime(1985, 6, 15),
            Password = "doctor@123",
            ConfirmPassword = "doctor@123",
            Role = ApplicationUser.UserRole.DOCTOR,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.NorthEnd,
            Address = "456 Nurse Ave, North End",
            Status = ApplicationUser.UserStatus.Active,
            PasswordHash = passwordHasher.HashPassword(null, "doctor@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
        new ApplicationUser
        {
            UserName = "Main Script Manager",
            Email = "main.sm@pharmacy.com",
            FirstName = "Main",
            LastName = "Script Manager",
            Title = ApplicationUser.Titles.Mrs,
            DateOfBirth = new DateTime(1990, 3, 10),
            Password = "script@123",
            ConfirmPassword = "script@123",
            Role = ApplicationUser.UserRole.SCRIPTMANAGER,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "321 Pharmacy Ln, SummerStrand",
            Status = ApplicationUser.UserStatus.Active,
            PasswordHash = passwordHasher.HashPassword(null, "script@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
        new ApplicationUser
        {
            UserName = "Main Consumable Manager",
            Email = "main.cm@timelesstechnicians.co.za",
            FirstName = "Main",
            LastName = "Consumables Manager",
            Title = ApplicationUser.Titles.Mrs,
            DateOfBirth = new DateTime(1990, 3, 10),
            Password = "consumable@123",
            ConfirmPassword = "consumable@123",
            Role = ApplicationUser.UserRole.CONSUMABLESMANAGER,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "321 Pharmacy Ln, SummerStrand",
            Status = ApplicationUser.UserStatus.Active,
            PasswordHash = passwordHasher.HashPassword(null, "consumable@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
        new ApplicationUser
        {
            UserName = "Main Ward Manager",
            Email = "main.wm@timelesstechnicians.co.za",
            FirstName = "Main",
            LastName = "Ward Manager",
            Title = ApplicationUser.Titles.Mr,
            DateOfBirth = new DateTime(1990, 3, 10),
            Password = "ward@123",
            ConfirmPassword = "ward@123",
            Role = ApplicationUser.UserRole.WARDADMIN,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "2nd avenue , SummerStrand",
            Status = ApplicationUser.UserStatus.Active,
            PasswordHash = passwordHasher.HashPassword(null, "ward@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
        new ApplicationUser
        {
            UserName = "Main Nurse Sister",
            Email = "main.ns@timelesstechnicians.co.za",
            FirstName = "Main",
            LastName = "Nurse Sister",
            Title = ApplicationUser.Titles.Mrs,
            DateOfBirth = new DateTime(1990, 3, 10),
            Password = "nursesister@123",
            ConfirmPassword = "nursesister@123",
            Role = ApplicationUser.UserRole.NURSINGSISTER,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "4th avenue , SummerStrand",
            Status = ApplicationUser.UserStatus.Active,
            PasswordHash = passwordHasher.HashPassword(null, "nursesister@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
        new ApplicationUser
        {
            UserName = "Patient",
            Email = "patient@gmail.com",
            FirstName = "John",
            LastName = "Doe",
            Title = ApplicationUser.Titles.Mr,
            DateOfBirth = new DateTime(1990, 3, 10),
            Password = "patient@123",
            ConfirmPassword = "patient@123",
            Role = ApplicationUser.UserRole.PATIENT,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "12th Nelson Mandela Road , SummerStrand",
            Status = ApplicationUser.UserStatus.Active,
            PasswordHash = passwordHasher.HashPassword(null, "patient@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },




        //this are our other patients for testing
          new ApplicationUser
        {
            UserName = "Patient 2",
            Email = "patient22@gmail.com",
            FirstName = "James",
            LastName = "Doe",
            Title = ApplicationUser.Titles.Mr,
            DateOfBirth = new DateTime(1990, 3, 10),
            Password = "Patient@44",
            ConfirmPassword = "Patient@44",
            Role = ApplicationUser.UserRole.PATIENT,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "12th Nelson Mandela Road , SummerStrand",
            Status = ApplicationUser.UserStatus.Inactive,
            PasswordHash = passwordHasher.HashPassword(null, "Patient@44"),
            TermsOfServiceAccepted = true // Set terms accepted
        },  new ApplicationUser
        {
            UserName = "Patient 3",
            Email = "patient33@gmail.com",
            FirstName = "Lucky",
            LastName = "Doe",
            Title = ApplicationUser.Titles.Mrs,
            DateOfBirth = new DateTime(1990, 3, 10),
            Password = "Luckpatient@123",
            ConfirmPassword = "patientLuckpatient123",
            Role = ApplicationUser.UserRole.PATIENT,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "13th Nelson Mandela Road , SummerStrand",
            Status = ApplicationUser.UserStatus.Inactive,
            PasswordHash = passwordHasher.HashPassword(null, "Luckpatient@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
            new ApplicationUser
        {
            UserName = "Patient 5",
            Email = "patient55@gmail.com",
            FirstName = "Alfred",
            LastName = "Doe",
            Title = ApplicationUser.Titles.Mr,
            DateOfBirth = new DateTime(1990, 3, 10),
            Password = "Alfredpatient@123",
            ConfirmPassword = "Alfredpatient@123",
            Role = ApplicationUser.UserRole.PATIENT,
            City = ApplicationUser.Cities.PortElizabeth,
            Suburb = ApplicationUser.Suburbs.Summerstrand,
            Address = "2nd Nelson Mandela Road , SummerStrand",
            Status = ApplicationUser.UserStatus.Inactive,
            PasswordHash = passwordHasher.HashPassword(null, "Alfredpatient@123"),
            TermsOfServiceAccepted = true // Set terms accepted
        },
    };

            // Seed the user data
            modelBuilder.Entity<ApplicationUser>().HasData(users);
        }








        public DbSet<Login> Login { get; set; } = default!;
        public DbSet<ChangeEmailViewModel> ChangeEmailViewModel { get; set; } = default!;
        public DbSet<UserListViewModel> UserListViewModel { get; set; } = default!;
    }
}
