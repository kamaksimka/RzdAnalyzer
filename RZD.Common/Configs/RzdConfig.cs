namespace RZD.Common.Configs
{
    public class RzdConfig
    {
        public const string Section = "Rzd";

        public string BaseAddress { get; set; }
        public string ConnectionString { get; set; }

        public string TrainsJobSchedule { get; set; }

    }
}
