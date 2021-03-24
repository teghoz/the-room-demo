using Microsoft.Extensions.Hosting;

namespace TheRoomSimpleAPI
{
    public class GlobalSettings
    {
        public GlobalSettings()
        {
        }
        public string APIBaseUrl { get; set; }

        public void SetEnvironmentVariables(IHostEnvironment env)
        {
            var apiSettings = ConfigurationManager.AppSetting.GetSection("API");
            if (env.IsProduction())
            {
                APIBaseUrl = apiSettings["Server"];
            }
            else
            {
                APIBaseUrl = apiSettings["Local"];
            }
        }
    }
}
