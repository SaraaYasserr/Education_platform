using EducationalPlatform.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationalPlatform.Data.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");

            builder.HasKey(e => e.Id);

            // Student Many <-> Many Courses, modeled explicitly through this
            // join entity (rather than EF's implicit many-to-many) so we can
            // store EnrollmentDate/Status/Progress on the relationship itself.
            builder.HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // avoid multiple cascade paths into Enrollments

            // A student can only enroll once in the same course.
            builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}
