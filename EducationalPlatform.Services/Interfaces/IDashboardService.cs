using EducationalPlatform.Models.ViewModels.Dashboard;

namespace EducationalPlatform.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardViewModel> GetAdminDashboardAsync();
        Task<InstructorDashboardViewModel> GetInstructorDashboardAsync(string instructorId);
        Task<StudentDashboardViewModel> GetStudentDashboardAsync(string studentId);
    }
}
