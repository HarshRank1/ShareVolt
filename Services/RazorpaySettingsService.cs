using Microsoft.Extensions.Configuration;

namespace ShareVolt.Services
{
    public class RazorpaySettingsService
    {
        public string Key;
        public string Secret;

        public RazorpaySettingsService(IConfiguration config)
        {
            Key = config.GetValue<string>("Razorpay:Key");
            Secret = config.GetValue<string>("Razorpay:Secret");
        }

        public string GetRazorpayKey()
        {
            return Key;
        }
        public string GetRazorpaySecret()
        {
            return Secret;
        }
    }

}
