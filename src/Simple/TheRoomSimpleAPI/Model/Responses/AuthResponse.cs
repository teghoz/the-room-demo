using System;

namespace TheRoomSimpleAPI.Model.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
