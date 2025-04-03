using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.API.Models.TrainRoute
{
    public class RzdRouteStop
    {
        public string ActualMovement { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string ArrivalTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DepartureTime { get; set; }
        public DateTime LocalArrivalDateTime { get; set; }
        public string LocalArrivalTime { get; set; }
        public DateTime LocalDepartureDateTime { get; set; }
        public string LocalDepartureTime { get; set; }
        public string StationCode { get; set; }
        public int StopDuration { get; set; }
        public int TimeZoneDifference { get; set; }
    }
}
