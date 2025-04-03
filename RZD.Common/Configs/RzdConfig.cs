namespace RZD.Common.Configs
{
    public class RzdConfig
    {
        public const string Section = "Rzd";

        public string BaseAddress { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;

        public bool ExecuteCitiesJob { get; set; }
        public string TrainsJobSchedule { get; set; } = null!;

        public int TimeBetweenRequests { get; set; }

        public int NumberOfDay { get; set; }

    }
}
