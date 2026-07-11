using EducationalPlatform.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationalPlatform.Data.Configurations
{
    public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            // One progress row per student/lesson pair.
            builder.HasIndex(p => new { p.StudentId, p.LessonId }).IsUnique();

            builder.HasOne(p => p.Student)
                .WithMany(u => u.LessonProgresses)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // NOTE: Restrict here (not Cascade) is deliberate. LessonProgress
            // already has a cascade path to Course through Lesson (Course ->
            // Lesson -> LessonProgress). Also cascading the direct CourseId
            // FK would create a second cascade path to the same table, which
            // SQL Server rejects at migration time ("may cause cycles or
            // multiple cascade paths") - the same issue documented for
            // Review.Course/Review.Student in this project's README.
            builder.HasOne(p => p.Course)
                .WithMany()
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Lesson)
                .WithMany(l => l.ProgressRecords)
                .HasForeignKey(p => p.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
