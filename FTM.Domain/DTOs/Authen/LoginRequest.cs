using FTM.Domain.Helpers;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.DTOs.Authen
{
    public class LoginRequest
    {
        /// <summary>
        /// Email confirmed or phone number
        /// </summary>
        [Required]
        [JsonConverter(typeof(UsernameSensitive))]
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
