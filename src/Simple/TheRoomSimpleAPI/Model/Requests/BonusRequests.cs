using System;
using System.ComponentModel.DataAnnotations;

namespace TheRoomSimpleAPI.Model.Requests
{
    public class BonusRequests
    {
        [Required]
        public string Promocode { get; set; }
    }
}
