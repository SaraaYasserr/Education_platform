using EducationalPlatform.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationalPlatform.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            // Course 1 -> Many Reviews.
            builder.HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student 1 -> Many Reviews.
            // Restrict here (not Cascade) because SQL Server disallows
            // multiple cascade paths from ApplicationUser into Reviews
            // (one through Course->Instructor is not direct, but Enrollment
            // already cascades from Student; keeping this Restrict avoids
            // any future multi-path cascade errors during migration).
            builder.HasOne(r => r.Student)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // A student may only review a given course once.
            builder.HasIndex(r => new { r.StudentId, r.CourseId }).IsUnique();
        }
    }
}
