using System;
using System.ComponentModel.DataAnnotations;

namespace TheRoomSimpleAPI.Model.Requests
{
    public class ServiceListRequests
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
