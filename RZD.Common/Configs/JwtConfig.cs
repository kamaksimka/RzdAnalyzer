using RZD.Common.Configs.Base;

namespace RZD.Common.Configs
{
    public class JwtConfig : IBaseConfig
    {
        public static string Section => "Jwt";

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret {get; set;}

        public int AccessExpiredIn { get; set; }
        public int RefreshExpiredIn { get; set; }

    }
}
