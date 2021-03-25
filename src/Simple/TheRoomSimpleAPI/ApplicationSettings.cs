using TheRoomSimpleAPI.Interfaces;

namespace TheRoomSimpleAPI.Models
{
    public class ApplicationSettings
    {
        public string Secret { get; set; }
        public string ServerKey { get; set; }
        public string ClientDomain { get; set; }
        public double AppTokenLifeSpan { get; set; } = 1;
        public string EncryptKey { get; set; }
        public int AbsoluteExpirationInHours { get; set; }
        public int SlidingExpirationInMinutes { get; set; }
    }
}
