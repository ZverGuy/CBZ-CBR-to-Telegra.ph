using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace CBZ_To_Telegraph.UserBot
{
    public class UserBotConfig
    {
        private readonly UserBotSettings _settings;

        public UserBotConfig(IConfiguration config)
        {
            _settings = config.GetRequiredSection("UserBotSettings")
                .Get<UserBotSettings>() 
                        ?? throw new InvalidOperationException("UserBotSettings Not Configured");
        }

        public string GetConfig(string propname)
        {
            switch (propname)
            {
                case "api_id": return _settings.ApiId;
                case "api_hash": return _settings.ApiHash;
                case "phone_number": return _settings.PhoneNumber;
                case "verification_code": Console.Write("Code: "); return Console.ReadLine() 
                                                                          ?? throw new InvalidOperationException("Code Must Be Not Empty!");
                case "first_name": return _settings.FirstName;      // if sign-up is required
                case "last_name": return _settings.LastName;        // if sign-up is required
                case "password": return _settings.Password;     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }

    }
}