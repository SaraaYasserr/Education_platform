using EducationalPlatform.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationalPlatform.Data.Configurations
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            // One certificate per student/course pair, and codes must be globally unique.
            builder.HasIndex(c => new { c.StudentId, c.CourseId }).IsUnique();
            builder.HasIndex(c => c.CertificateCode).IsUnique();

            // Restrict on Student mirrors the existing Review.Student pattern
            // in this project, avoiding multiple-cascade-path errors since
            // ApplicationUser is also reachable via Course.Instructor.
            builder.HasOne(c => c.Student)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Course)
                .WithMany(course => course.Certificates)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
