using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Dashboard;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            var students = await _userManager.GetUsersInRoleAsync(Roles.Student);
            var instructors = await _userManager.GetUsersInRoleAsync(Roles.Instructor);
            var totalUsers = _userManager.Users.Count();

            var courses = await _unitOfWork.CourseRepository.Query()
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments)
                .ToListAsync();

            var enrollments = await _unitOfWork.EnrollmentRepository.Query()
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Status != EnrollmentStatus.Cancelled)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            var categoriesCount = (await _unitOfWork.CategoryRepository.GetAllAsync()).Count();

            var unreadMessages = (await _unitOfWork.ContactMessageRepository.FindAsync(m => !m.IsRead)).Count();

            var totalRevenue = enrollments.Sum(e => e.Course?.Price ?? 0);

            return new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalStudents = students.Count,
                TotalInstructors = instructors.Count,
                TotalCourses = courses.Count,
                TotalCategories = categoriesCount,
                TotalEnrollments = enrollments.Count,
                TotalRevenue = totalRevenue,
                UnreadContactMessages = unreadMessages,
                RecentEnrollments = enrollments.Take(10).Select(e => new RecentEnrollmentItem
                {
                    StudentName = e.Student?.FullName ?? string.Empty,
                    CourseTitle = e.Course?.Title ?? string.Empty,
                    EnrollmentDate = e.EnrollmentDate
                }).ToList(),
                TopCourses = courses
                    .OrderByDescending(c => c.Enrollments.Count)
                    .Take(5)
                    .Select(c => new TopCourseItem
                    {
                        CourseId = c.Id,
                        Title = c.Title,
                        EnrollmentCount = c.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),
                        AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0
                    }).ToList()
            };
        }

        public async Task<InstructorDashboardViewModel> GetInstructorDashboardAsync(string instructorId)
        {
            var courses = await _unitOfWork.CourseRepository.Query()
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments).ThenInclude(e => e.Student)
                .Include(c => c.Lessons)
                .Where(c => c.InstructorId == instructorId)
                .ToListAsync();

            var allReviews = courses.SelectMany(c => c.Reviews).ToList();

            var recentStudents = courses
                .SelectMany(c => c.Enrollments.Where(e => e.Status != EnrollmentStatus.Cancelled)
                    .Select(e => new { Enrollment = e, CourseTitle = c.Title }))
                .OrderByDescending(x => x.Enrollment.EnrollmentDate)
                .Take(10)
                .Select(x => new RecentEnrollmentItem
                {
                    StudentName = x.Enrollment.Student?.FullName ?? string.Empty,
                    CourseTitle = x.CourseTitle,
                    EnrollmentDate = x.Enrollment.EnrollmentDate
                }).ToList();

            return new InstructorDashboardViewModel
            {
                TotalCourses = courses.Count,
                TotalStudents = courses.SelectMany(c => c.Enrollments)
                    .Where(e => e.Status != EnrollmentStatus.Cancelled)
                    .Select(e => e.StudentId).Distinct().Count(),
                TotalLessons = courses.Sum(c => c.Lessons.Count),
                AverageRating = allReviews.Any() ? allReviews.Average(r => r.Rating) : 0,
                MyCourses = courses
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new TopCourseItem
                    {
                        CourseId = c.Id,
                        Title = c.Title,
                        EnrollmentCount = c.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),
                        AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0
                    }).ToList(),
                RecentStudents = recentStudents
            };
        }

        public async Task<StudentDashboardViewModel> GetStudentDashboardAsync(string studentId)
        {
            var enrollments = await _unitOfWork.EnrollmentRepository.Query()
                .Include(e => e.Course).ThenInclude(c => c.Instructor)
                .Where(e => e.StudentId == studentId && e.Status != EnrollmentStatus.Cancelled)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            return new StudentDashboardViewModel
            {
                EnrolledCourseCount = enrollments.Count,
                CompletedCourseCount = enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
                AverageProgress = enrollments.Any() ? enrollments.Average(e => e.ProgressPercentage) : 0,
                MyCourses = enrollments.Select(e => new Models.ViewModels.Enrollment.EnrollmentViewModel
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course?.Title ?? string.Empty,
                    CourseImageUrl = e.Course?.ImageUrl,
                    CoursePrice = e.Course?.Price ?? 0,
                    InstructorName = e.Course?.Instructor?.FullName ?? string.Empty,
                    StudentId = e.StudentId,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    ProgressPercentage = e.ProgressPercentage
                }).ToList()
            };
        }
    }
}
