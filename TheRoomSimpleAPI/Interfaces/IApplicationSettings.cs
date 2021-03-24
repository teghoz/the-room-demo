namespace TheRoomSimpleAPI.Interfaces
{
    public interface IApplicationSettings
    {
        string APIDomain { get; set; }
        public int AbsolueExpirationInHours { get; set; }
        public int SlidingExpirationInMinutes { get; set; }
        public int ResponseCacheDurationInSecond { get; set; }
        public bool ResponseCacheNoStore { get; set; }
    }
}
