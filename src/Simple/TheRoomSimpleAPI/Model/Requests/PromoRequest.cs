using System;
using System.ComponentModel.DataAnnotations;

namespace TheRoomSimpleAPI.Model.Requests
{
    public class PromoRequests
    {
        [Required]
        public string Promocode { get; set; }
        public decimal Discount { get; set; }
        public double DiscountPercentage { get; set; }
        public bool UsePercentage { get; set; }
    }
}
