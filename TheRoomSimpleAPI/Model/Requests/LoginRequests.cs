using System;
using System.ComponentModel.DataAnnotations;

namespace TheRoomSimpleAPI.Model.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
