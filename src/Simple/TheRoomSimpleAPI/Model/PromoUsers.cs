namespace TheRoomSimpleAPI.Model
{
    public class PromoUsers
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public virtual ServiceListItemPromo Promo { get; set; }
    }
}