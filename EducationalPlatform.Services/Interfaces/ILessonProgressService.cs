namespace EducationalPlatform.Services.Interfaces
{
    public interface ILessonProgressService
    {
        Task<HashSet<int>> GetCompletedLessonIdsAsync(string studentId, int courseId);

        Task<int> GetProgressPercentageAsync(string studentId, int courseId);

        Task<(bool Success, string? Error, int ProgressPercentage, bool CertificateIssued)> ToggleLessonCompletionAsync(
            string studentId, int lessonId, bool isCompleted);
    }
}