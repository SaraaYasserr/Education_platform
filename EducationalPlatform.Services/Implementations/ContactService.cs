using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Shared;
using EducationalPlatform.Repository.UnitOfWork;
using EducationalPlatform.Services.Interfaces;

namespace EducationalPlatform.Services.Implementations
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string? Error)> SubmitAsync(ContactViewModel model)
        {
            var message = new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                Subject = model.Subject,
                Message = model.Message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.ContactMessageRepository.AddAsync(message);
            await _unitOfWork.SaveAsync();

            return (true, null);
        }

        public async Task<List<ContactMessageListItemViewModel>> GetAllAsync()
        {
            var messages = await _unitOfWork.ContactMessageRepository.GetAllAsync();

            return messages
                .OrderByDescending(m => m.SentAt)
                .Select(m => new ContactMessageListItemViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Subject = m.Subject,
                    Message = m.Message,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                }).ToList();
        }

        public async Task MarkAsReadAsync(int id)
        {
            var message = await _unitOfWork.ContactMessageRepository.GetByIdAsync(id);
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                _unitOfWork.ContactMessageRepository.Update(message);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync()
        {
            var unread = await _unitOfWork.ContactMessageRepository.FindAsync(m => !m.IsRead);
            return unread.Count();
        }
    }
}
