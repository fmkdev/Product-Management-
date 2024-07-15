using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class Auth
    {
    }
    public class AuthTokenDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }

    }
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class ResponseDto
    {
        public ResponseDto()
        {
        }



        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("data")]
        public virtual object Data { get; set; }

        public static ResponseDto Success(object data, string message = "")
        {
            return new ResponseDto()
            {
                Data = data,
                Message = message,
                Status = "success"
            };
        }

        public static ResponseDto Failed(object data, string message = "")
        {
            return new ResponseDto()
            {
                Data = data,
                Message = message,
                Status = "failed"
            };
        }

        public static class ResponseStatus
        {
            public static string Success = "success", Fail = "fail", Pending = "pending";
        }
    }
}
