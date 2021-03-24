using System.Collections.Generic;

namespace ServiceListAPI.Model
{
    public class ServiceListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public virtual List<ServiceListItemPromo> PromoCodes { get; set; }
    }
}