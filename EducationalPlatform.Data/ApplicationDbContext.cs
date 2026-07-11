using EducationalPlatform.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Data
{
    /// <summary>
    /// The EF Core database context. Inherits from IdentityDbContext so the
    /// standard Identity tables (AspNetUsers, AspNetRoles, AspNetUserRoles,
    /// etc.) are created alongside our own domain tables in one database.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
        public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
        public DbSet<Certificate> Certificates => Set<Certificate>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // IMPORTANT: must be called first - it builds the Identity schema
            // (AspNetUsers, AspNetRoles, etc.) that everything else depends on.
            base.OnModelCreating(builder);

            // Apply every IEntityTypeConfiguration<T> found in this assembly
            // (Configurations/*.cs) instead of registering them one by one.
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
