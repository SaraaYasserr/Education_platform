using EducationalPlatform.Models.Entities;
using EducationalPlatform.Repository.Generic;

namespace EducationalPlatform.Repository.UnitOfWork
{
    /// <summary>
    /// Groups all the repositories the application needs behind one object,
    /// and exposes a single SaveAsync() so multiple repository operations can
    /// be committed to the database together as one transaction-like unit.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Course> CourseRepository { get; }
        IGenericRepository<Lesson> LessonRepository { get; }
        IGenericRepository<Enrollment> EnrollmentRepository { get; }
        IGenericRepository<Review> ReviewRepository { get; }
        IGenericRepository<SubscriptionPlan> SubscriptionPlanRepository { get; }
        IGenericRepository<UserSubscription> UserSubscriptionRepository { get; }
        IGenericRepository<ContactMessage> ContactMessageRepository { get; }
        IGenericRepository<LessonProgress> LessonProgressRepository { get; }

        /// <summary>Certificates issued to students who complete a course.</summary>
        IGenericRepository<Certificate> CertificateRepository { get; }

        /// <summary>Persists every tracked change across all repositories in one call.</summary>
        Task<int> SaveAsync();
    }
}
