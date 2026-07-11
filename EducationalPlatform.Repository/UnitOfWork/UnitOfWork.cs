using EducationalPlatform.Data;
using EducationalPlatform.Models.Entities;
using EducationalPlatform.Repository.Generic;

namespace EducationalPlatform.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // Lazily created so we only pay for a repository the first time it's
        // actually used in a given request.
        private IGenericRepository<Category>? _categoryRepository;
        private IGenericRepository<Course>? _courseRepository;
        private IGenericRepository<Lesson>? _lessonRepository;
        private IGenericRepository<Enrollment>? _enrollmentRepository;
        private IGenericRepository<Review>? _reviewRepository;
        private IGenericRepository<SubscriptionPlan>? _subscriptionPlanRepository;
        private IGenericRepository<UserSubscription>? _userSubscriptionRepository;
        private IGenericRepository<ContactMessage>? _contactMessageRepository;
        private IGenericRepository<LessonProgress>? _lessonProgressRepository;
        private IGenericRepository<Certificate>? _certificateRepository;

        private bool _disposed;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Category> CategoryRepository =>
            _categoryRepository ??= new GenericRepository<Category>(_context);

        public IGenericRepository<Course> CourseRepository =>
            _courseRepository ??= new GenericRepository<Course>(_context);

        public IGenericRepository<Lesson> LessonRepository =>
            _lessonRepository ??= new GenericRepository<Lesson>(_context);

        public IGenericRepository<Enrollment> EnrollmentRepository =>
            _enrollmentRepository ??= new GenericRepository<Enrollment>(_context);

        public IGenericRepository<Review> ReviewRepository =>
            _reviewRepository ??= new GenericRepository<Review>(_context);

        public IGenericRepository<SubscriptionPlan> SubscriptionPlanRepository =>
            _subscriptionPlanRepository ??= new GenericRepository<SubscriptionPlan>(_context);

        public IGenericRepository<UserSubscription> UserSubscriptionRepository =>
            _userSubscriptionRepository ??= new GenericRepository<UserSubscription>(_context);

        public IGenericRepository<ContactMessage> ContactMessageRepository =>
            _contactMessageRepository ??= new GenericRepository<ContactMessage>(_context);
        public IGenericRepository<LessonProgress> LessonProgressRepository =>
           _lessonProgressRepository ??= new GenericRepository<LessonProgress>(_context);

        public IGenericRepository<Certificate> CertificateRepository =>
            _certificateRepository ??= new GenericRepository<Certificate>(_context);

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
