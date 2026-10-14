using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace EndpointsPage.DTOs.Response
{
    public class GeneralResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static GeneralResponseDto<T> SuccesResult(T data, string message = "successful operation")
        {
            return new GeneralResponseDto<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static GeneralResponseDto<T> FailureResult(string message = "erroneous transaction", string? errors = null)
        {
            return new GeneralResponseDto<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors
            };
        }
    }
}
