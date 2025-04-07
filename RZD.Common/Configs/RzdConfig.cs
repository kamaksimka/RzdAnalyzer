using RZD.Common.Configs.Base;

namespace RZD.Common.Configs
{
    public class RzdConfig:IBaseConfig
    {
        public static string Section => "Rzd";

        public string BaseAddress { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;

        public bool ExecuteCitiesJob { get; set; }
        public string TrainsJobSchedule { get; set; } = null!;

        public int TimeBetweenRequests { get; set; }

        public int NumberOfDay { get; set; }

    }
}
