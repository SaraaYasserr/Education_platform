using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Course;
using EducationalPlatform.Models.ViewModels.Lesson;
using EducationalPlatform.Models.ViewModels.Review;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILessonProgressService _lessonProgressService;
        private readonly ICertificateService _certificateService;

        public CourseService(
            IUnitOfWork unitOfWork,
            ILessonProgressService lessonProgressService,
            ICertificateService certificateService)
        {
            _unitOfWork = unitOfWork;
            _lessonProgressService = lessonProgressService;
            _certificateService = certificateService;
        }

        public async Task<CourseListViewModel> SearchAsync(CourseListViewModel filter)
        {
            // IQueryable lets us build up the filter/sort/paging in-database
            // instead of pulling every course into memory first.
            var query = _unitOfWork.CourseRepository.Query()
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments)
                .Include(c => c.Lessons)
                .Where(c => c.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                query = query.Where(c => c.Title.Contains(term) || c.Description.Contains(term));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == filter.CategoryId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(c => c.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= filter.MaxPrice.Value);
            }

            query = filter.SortBy switch
            {
                "price_asc" => query.OrderBy(c => c.Price),
                "price_desc" => query.OrderByDescending(c => c.Price),
                "rating" => query.OrderByDescending(c => c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0),
                _ => query.OrderByDescending(c => c.CreatedAt) // "newest" default
            };

            filter.TotalCount = await query.CountAsync();

            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? 6 : filter.PageSize;

            var courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            filter.Page = page;
            filter.PageSize = pageSize;
            filter.Courses = courses.Select(MapToListItem).ToList();

            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            filter.Categories = categories
                .OrderBy(c => c.Name)
                .Select(c => new Models.ViewModels.Category.CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToList();

            return filter;
        }

        public async Task<CourseDetailsViewModel?> GetDetailsAsync(int courseId, string? currentUserId)
        {
            var course = await _unitOfWork.CourseRepository.Query()
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Reviews).ThenInclude(r => r.Student)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return null;

            var relatedCourses = await _unitOfWork.CourseRepository.Query()
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments)
                .Where(c => c.CategoryId == course.CategoryId && c.Id != course.Id && c.IsPublished)
                .Take(3)
                .ToListAsync();

            var isEnrolled = currentUserId != null &&
                course.Enrollments.Any(e => e.StudentId == currentUserId && e.Status != EnrollmentStatus.Cancelled);

            var viewModel = new CourseDetailsViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                ImageUrl = course.ImageUrl,
                CreatedAt = course.CreatedAt,
                DurationMinutes = course.DurationMinutes,
                SeatsAvailable = course.SeatsAvailable,
                CategoryId = course.CategoryId,
                CategoryName = course.Category.Name,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor.FullName,
                InstructorImage = course.Instructor.ProfileImage,
                Lessons = course.Lessons
                    .OrderBy(l => l.LessonOrder)
                    .Select(l => new LessonViewModel
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Content = l.Content,
                        VideoUrl = l.VideoUrl,
                        LessonOrder = l.LessonOrder,
                        DurationMinutes = l.DurationMinutes,
                        CourseId = l.CourseId,
                        CourseTitle = course.Title
                    }).ToList(),
                Reviews = course.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ReviewViewModel
                    {
                        Id = r.Id,
                        StudentName = r.Student.FullName,
                        StudentImage = r.Student.ProfileImage,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    }).ToList(),
                NewReview = new ReviewCreateViewModel { CourseId = course.Id },
                AverageRating = course.Reviews.Any() ? course.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = course.Reviews.Count,
                EnrollmentCount = course.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),
                RelatedCourses = relatedCourses.Select(MapToListItem).ToList(),
                IsCurrentUserEnrolled = isEnrolled,
                CanEnroll = !course.SeatsAvailable.HasValue ||
                            course.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled) < course.SeatsAvailable.Value
            };

            // ===== Progress & Certificate (only meaningful for an enrolled student) =====
            if (!string.IsNullOrEmpty(currentUserId) && viewModel.IsCurrentUserEnrolled)
            {
                viewModel.CompletedLessonIds = await _lessonProgressService.GetCompletedLessonIdsAsync(currentUserId, courseId);
                viewModel.ProgressPercentage = await _lessonProgressService.GetProgressPercentageAsync(currentUserId, courseId);

                if (viewModel.ProgressPercentage >= 100)
                {
                    viewModel.HasCertificate = (await _certificateService.GetByCourseAsync(currentUserId, courseId)) != null;
                }
            }

            return viewModel;
        }

        public async Task<CourseEditViewModel?> GetForEditAsync(int courseId)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            if (course == null) return null;

            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            return new CourseEditViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                ExistingImageUrl = course.ImageUrl,
                CategoryId = course.CategoryId,
                DurationMinutes = course.DurationMinutes,
                SeatsAvailable = course.SeatsAvailable,
                IsPublished = course.IsPublished,
                AvailableCategories = categories
                    .OrderBy(c => c.Name)
                    .Select(c => new Models.ViewModels.Category.CategoryViewModel { Id = c.Id, Name = c.Name })
                    .ToList()
            };
        }

        public async Task<List<CourseListItemViewModel>> GetByInstructorAsync(string instructorId)
        {
            var courses = await _unitOfWork.CourseRepository.Query()
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments)
                .Include(c => c.Lessons)
                .Where(c => c.InstructorId == instructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return courses.Select(MapToListItem).ToList();
        }

        public async Task<(bool Success, string? Error, int CourseId)> CreateAsync(
            CourseCreateViewModel model, IFormFile? imageFile, string instructorId, string webRootPath)
        {
            var imageUrl = await SaveCourseImageAsync(imageFile, webRootPath);

            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = imageUrl ?? "/assets/img/course/1.png", // sensible default reusing template art
                CategoryId = model.CategoryId,
                InstructorId = instructorId,
                DurationMinutes = model.DurationMinutes,
                SeatsAvailable = model.SeatsAvailable,
                IsPublished = model.IsPublished,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CourseRepository.AddAsync(course);
            await _unitOfWork.SaveAsync();

            return (true, null, course.Id);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(
            CourseEditViewModel model, IFormFile? imageFile, string instructorId, bool isAdmin, string webRootPath)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(model.Id);
            if (course == null)
            {
                return (false, "Course not found.");
            }

            if (!isAdmin && course.InstructorId != instructorId)
            {
                return (false, "You are not allowed to edit this course.");
            }

            if (imageFile != null)
            {
                var newImageUrl = await SaveCourseImageAsync(imageFile, webRootPath);
                if (newImageUrl != null)
                {
                    course.ImageUrl = newImageUrl;
                }
            }

            course.Title = model.Title;
            course.Description = model.Description;
            course.Price = model.Price;
            course.CategoryId = model.CategoryId;
            course.DurationMinutes = model.DurationMinutes;
            course.SeatsAvailable = model.SeatsAvailable;
            course.IsPublished = model.IsPublished;

            _unitOfWork.CourseRepository.Update(course);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int courseId, string instructorId, bool isAdmin)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return (false, "Course not found.");
            }

            if (!isAdmin && course.InstructorId != instructorId)
            {
                return (false, "You are not allowed to delete this course.");
            }

            _unitOfWork.CourseRepository.Delete(course);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<bool> IsOwnerOrAdminAsync(int courseId, string userId, bool isAdmin)
        {
            if (isAdmin) return true;

            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            return course != null && course.InstructorId == userId;
        }

        public async Task<List<CourseListItemViewModel>> GetFeaturedAsync(int count = 6)
        {
            var courses = await _unitOfWork.CourseRepository.Query()
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments)
                .Include(c => c.Lessons)
                .Where(c => c.IsPublished)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync();

            return courses.Select(MapToListItem).ToList();
        }

        private static CourseListItemViewModel MapToListItem(Course c) => new()
        {
            Id = c.Id,
            Title = c.Title,
            ImageUrl = c.ImageUrl,
            Price = c.Price,
            CategoryName = c.Category?.Name ?? string.Empty,
            InstructorName = c.Instructor?.FullName ?? string.Empty,
            AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0,
            LessonCount = c.Lessons.Count,
            DurationMinutes = c.DurationMinutes,
            EnrollmentCount = c.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled)
        };

        /// <summary>
        /// Saves an uploaded course image under wwwroot/uploads/courses and
        /// returns the public-facing relative URL, or null if no file was
        /// provided (caller keeps the existing/default image in that case).
        /// </summary>
        private static async Task<string?> SaveCourseImageAsync(IFormFile? file, string webRootPath)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "courses");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/courses/{fileName}";
        }

        /// <summary>
        /// Lightweight lookup used by the Payment flow. Unlike GetDetailsAsync,
        /// this doesn't need Category/Instructor/Lessons/Reviews includes —
        /// PaymentController only reads Id, Title, and Price to build the checkout screen.
        /// </summary>
        public async Task<CourseDetailsViewModel?> GetCourseForPaymentAsync(int courseId)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
            if (course == null) return null;

            return new CourseDetailsViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Price = course.Price
            };
        }
    }
}