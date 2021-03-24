using System;
using System.Collections.Generic;

namespace ServiceListAPI.Model
{
    public class ServiceListItemPromo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool UsePercentage { get; set; }
        public decimal Discount { get; set; }
        public double DiscountPercentage { get; set; }
        public virtual ServiceListItem ServiceListItem { get; set; }
    }
}