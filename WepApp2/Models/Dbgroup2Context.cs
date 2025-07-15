using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WepApp2.Models;

public partial class Dbgroup2Context : DbContext
{
    public Dbgroup2Context()
    {
    }

    public Dbgroup2Context(DbContextOptions<Dbgroup2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BookingDevice> BookingDevices { get; set; }

    public virtual DbSet<Consultation> Consultations { get; set; }

    public virtual DbSet<ConsultationMajor> ConsultationMajors { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceLoan> DeviceLoans { get; set; }

    public virtual DbSet<LabVisit> LabVisits { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Technology> Technologies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VisitsDetail> VisitsDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-UBN31PE8;Database=DBGroup2;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Arabic_CI_AI");

        modelBuilder.Entity<BookingDevice>(entity =>
        {
            entity.HasKey(e => e.BookingDeviceId).HasName("PK__BookingD__6263A24949EBF196");

            entity.Property(e => e.BookingDeviceId).HasColumnName("BookingDeviceID");
            entity.Property(e => e.Department).HasMaxLength(255);
            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.Faculty).HasMaxLength(255);
            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Device).WithMany(p => p.BookingDevices)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK_BookingDevices_Devices");

            entity.HasOne(d => d.Request).WithMany(p => p.BookingDevices)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_BookingDevices_Request");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingDevices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_BookingDevices_Services");
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasKey(e => e.ConsultationId).HasName("PK__Consulta__5D014A785DB937CC");

            entity.Property(e => e.ConsultationId).HasColumnName("ConsultationID");
            entity.Property(e => e.ConsultationMajorId).HasColumnName("ConsultationMajorID");
            entity.Property(e => e.PreferredContactMethod).HasMaxLength(255);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.ConsultationMajor).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.ConsultationMajorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consultations_ConsultationMajor");

            entity.HasOne(d => d.Request).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_Consultations_Request");

            entity.HasOne(d => d.Service).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Consultations_Services");
        });

        modelBuilder.Entity<ConsultationMajor>(entity =>
        {
            entity.HasKey(e => e.ConsultationMajorId).HasName("PK__Consulta__C757E54F7E8D3679");

            entity.Property(e => e.ConsultationMajorId).HasColumnName("ConsultationMajorID");
            entity.Property(e => e.Major).HasMaxLength(255);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187C09ABD48");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseField).HasMaxLength(255);
            entity.Property(e => e.CourseName).HasMaxLength(255);
            entity.Property(e => e.PresenterName).HasMaxLength(255);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Request).WithMany(p => p.Courses)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_Courses_Request");

            entity.HasOne(d => d.Service).WithMany(p => p.Courses)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Courses_Services");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PK__Devices__49E12331FA20AC9E");

            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.BrandName).HasMaxLength(255);
            entity.Property(e => e.DeviceLocation).HasMaxLength(255);
            entity.Property(e => e.DeviceModel).HasMaxLength(255);
            entity.Property(e => e.DeviceName).HasMaxLength(255);
            entity.Property(e => e.DeviceStatus).HasMaxLength(255);
            entity.Property(e => e.DeviceType).HasMaxLength(255);
            entity.Property(e => e.LastMaintenance).HasColumnType("datetime");
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.SerialNumber).HasMaxLength(255);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.TechnologyId).HasColumnName("TechnologyID");

            entity.HasOne(d => d.Service).WithMany(p => p.Devices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Devices_Services");

            entity.HasOne(d => d.Technology).WithMany(p => p.Devices)
                .HasForeignKey(d => d.TechnologyId)
                .HasConstraintName("FK_Devices_Technologies");
        });

        modelBuilder.Entity<DeviceLoan>(entity =>
        {
            entity.HasKey(e => e.DeviceLoanId).HasName("PK__DeviceLo__385CB1795F83D7B4");

            entity.Property(e => e.DeviceLoanId).HasColumnName("DeviceLoanID");
            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.PreferredContactMethod).HasMaxLength(255);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceLoans)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK_DeviceLoans_Devices");

            entity.HasOne(d => d.Request).WithMany(p => p.DeviceLoans)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_DeviceLoans_Request");

            entity.HasOne(d => d.Service).WithMany(p => p.DeviceLoans)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_DeviceLoans_Services");
        });

        modelBuilder.Entity<LabVisit>(entity =>
        {
            entity.HasKey(e => e.LabVisitId).HasName("PK__LabVisit__DDA12342C1B18E03");

            entity.Property(e => e.LabVisitId).HasColumnName("LabVisitID");
            entity.Property(e => e.NumberOfVisitors).HasDefaultValue(1);
            entity.Property(e => e.PreferredContactMethod).HasMaxLength(255);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.VisitDetailsId).HasColumnName("VisitDetailsID");

            entity.HasOne(d => d.Request).WithMany(p => p.LabVisits)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_LabVisits_Request");

            entity.HasOne(d => d.Service).WithMany(p => p.LabVisits)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_LabVisits_Services");

            entity.HasOne(d => d.VisitDetails).WithMany(p => p.LabVisits)
                .HasForeignKey(d => d.VisitDetailsId)
                .HasConstraintName("FK_LabVisits_VisitsDetails");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Requests__33A8519A677781CF");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.AdminStatus).HasMaxLength(255);
            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.RequestType).HasMaxLength(255);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SupervisorStatus).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Device).WithMany(p => p.Requests)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK_Request_Devices");

            entity.HasOne(d => d.Service).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Request_Services");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Request_Users");

            entity.HasMany(d => d.CoursesNavigation).WithMany(p => p.Requests)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursesRequest",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursesRe__Cours__5AEE82B9"),
                    l => l.HasOne<Request>().WithMany()
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursesRe__Reque__59FA5E80"),
                    j =>
                    {
                        j.HasKey("RequestId", "CourseId").HasName("PK__CoursesR__5F3A86825A1B9ED2");
                        j.ToTable("CoursesRequests");
                        j.IndexerProperty<int>("RequestId").HasColumnName("RequestID");
                        j.IndexerProperty<int>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EAB48627A7");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.ServiceName).HasMaxLength(255);
        });

        modelBuilder.Entity<Technology>(entity =>
        {
            entity.HasKey(e => e.TechnologyId).HasName("PK__Technolo__70570178B6095524");

            entity.Property(e => e.TechnologyId).HasColumnName("TechnologyID");
            entity.Property(e => e.TechnologyName).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACFFACBBB8");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Department).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Faculty).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogIn).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.Property(e => e.UserPassWord).HasMaxLength(255);
            entity.Property(e => e.UserRole).HasMaxLength(255);
        });

        modelBuilder.Entity<VisitsDetail>(entity =>
        {
            entity.HasKey(e => e.VisitDetailsId).HasName("PK__VisitsDe__BCA58B1371E794BB");

            entity.Property(e => e.VisitDetailsId).HasColumnName("VisitDetailsID");
            entity.Property(e => e.VisitType)
                .HasMaxLength(255)
                .HasColumnName("visitType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
