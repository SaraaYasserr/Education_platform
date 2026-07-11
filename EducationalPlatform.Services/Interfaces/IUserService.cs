using EducationalPlatform.Models.ViewModels.Account;
using EducationalPlatform.Models.ViewModels.Instructor;
using Microsoft.AspNetCore.Http;

namespace EducationalPlatform.Services.Interfaces
{
    public interface IUserService
    {
        Task<ProfileViewModel?> GetProfileAsync(string userId);
        Task<(bool Success, string? Error)> UpdateProfileAsync(ProfileViewModel model, IFormFile? profileImageFile, string webRootPath);

        Task<List<InstructorListItemViewModel>> GetAllInstructorsAsync();
        Task<InstructorDetailsViewModel?> GetInstructorDetailsAsync(string instructorId);

        Task<List<ProfileViewModel>> GetAllUsersAsync();
        Task<(bool Success, string? Error)> DeleteUserAsync(string userId);
    }
}
