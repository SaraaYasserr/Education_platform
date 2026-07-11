using EducationalPlatform.Models.ViewModels.Shared;

namespace EducationalPlatform.Services.Interfaces
{
    public interface IContactService
    {
        Task<(bool Success, string? Error)> SubmitAsync(ContactViewModel model);
        Task<List<ContactMessageListItemViewModel>> GetAllAsync();
        Task MarkAsReadAsync(int id);
        Task<int> GetUnreadCountAsync();
    }
}
