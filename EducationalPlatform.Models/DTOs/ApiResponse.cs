namespace EducationalPlatform.Models.DTOs
{
    /// <summary>
    /// Consistent envelope for API responses so clients always get the same
    /// shape back: { success, message, data }.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message) =>
            new() { Success = false, Message = message, Data = default };
    }
}
