using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Account;
using EducationalPlatform.Models.ViewModels.Instructor;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Role = roles.FirstOrDefault() ?? string.Empty,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<(bool Success, string? Error)> UpdateProfileAsync(ProfileViewModel model, IFormFile? profileImageFile, string webRootPath)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return (false, "User not found.");
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            if (profileImageFile != null && profileImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(webRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profileImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImageFile.CopyToAsync(stream);
                }

                user.ProfileImage = $"/uploads/profiles/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? (true, null)
                : (false, string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        public async Task<List<InstructorListItemViewModel>> GetAllInstructorsAsync()
        {
            var instructors = await _userManager.GetUsersInRoleAsync(Roles.Instructor);

            var result = new List<InstructorListItemViewModel>();

            foreach (var instructor in instructors)
            {
                var courses = await _unitOfWork.CourseRepository.Query()
                    .Include(c => c.Enrollments)
                    .Where(c => c.InstructorId == instructor.Id)
                    .ToListAsync();

                result.Add(new InstructorListItemViewModel
                {
                    Id = instructor.Id,
                    FullName = instructor.FullName,
                    ProfileImage = instructor.ProfileImage,
                    CourseCount = courses.Count,
                    StudentCount = courses.SelectMany(c => c.Enrollments)
                        .Select(e => e.StudentId)
                        .Distinct()
                        .Count()
                });
            }

            return result.OrderBy(i => i.FullName).ToList();
        }

        public async Task<InstructorDetailsViewModel?> GetInstructorDetailsAsync(string instructorId)
        {
            var instructor = await _userManager.FindByIdAsync(instructorId);
            if (instructor == null) return null;

            var courses = await _unitOfWork.CourseRepository.Query()
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments)
                .Include(c => c.Lessons)
                .Where(c => c.InstructorId == instructorId && c.IsPublished)
                .ToListAsync();

            return new InstructorDetailsViewModel
            {
                Id = instructor.Id,
                FullName = instructor.FullName,
                ProfileImage = instructor.ProfileImage,
                Email = instructor.Email,
                PhoneNumber = instructor.PhoneNumber,
                CourseCount = courses.Count,
                StudentCount = courses.SelectMany(c => c.Enrollments).Select(e => e.StudentId).Distinct().Count(),
                Courses = courses.Select(c => new Models.ViewModels.Course.CourseListItemViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    ImageUrl = c.ImageUrl,
                    Price = c.Price,
                    CategoryName = c.Category.Name,
                    InstructorName = instructor.FullName,
                    AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0,
                    LessonCount = c.Lessons.Count,
                    DurationMinutes = c.DurationMinutes,
                    EnrollmentCount = c.Enrollments.Count
                }).ToList()
            };
        }

        public async Task<List<ProfileViewModel>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<ProfileViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new ProfileViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    ProfileImage = user.ProfileImage,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    CreatedAt = user.CreatedAt
                });
            }

            return result.OrderBy(u => u.FullName).ToList();
        }

        public async Task<(bool Success, string? Error)> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded
                ? (true, null)
                : (false, string.Join(" ", result.Errors.Select(e => e.Description)));
        }
    }
}
