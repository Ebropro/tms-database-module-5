using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Entities;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EnrolledAt)
            .IsRequired();

        builder.Property(e => e.Grade)
            .HasPrecision(5, 2);

        // ============================
        // RELATIONSHIP: Student → Enrollments
        // ============================
        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
            // Prevent deleting a student if enrollments exist

        // ============================
        // RELATIONSHIP: Course → Enrollments
        // ============================
        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
            // Prevent deleting a course if students are enrolled
    }
}