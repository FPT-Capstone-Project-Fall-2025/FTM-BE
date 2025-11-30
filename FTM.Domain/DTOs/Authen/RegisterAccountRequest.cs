using FTM.Domain.Helpers;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.DTOs.Authen
{
    public class RegisterAccountRequest
    {
        [EmailAddress]
        [JsonConverter(typeof(EmailSensitive))]
        public string Email { get; set; }

        [Required]
        [Phone]
        [JsonConverter(typeof(PhoneSensitive))]
        public string PhoneNumber { get; set; }

        [Required]
        [JsonConverter(typeof(StringSensitive))]
        public string Name { get; set; }

        [Required]
        [RegularExpression("(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
            ErrorMessage = "Mật khẩu phải có ít nhất một ký tự in hoa, một ký tự in thường, một chữ số, một ký tự đặc biệt và số ký tự ít nhất là 8 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
