using System;
using System.Collections.Generic;

namespace TheRoomSimpleAPI.Model.Responses
{
    public class ServiceListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public virtual List<PromoResponse> Promos { get; set; }
    }
}
