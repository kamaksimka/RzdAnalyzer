using RZD.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Models
{
    public class SubscriptionModel
    {
        public string OriginStationName { get; set; }
        public string OriginRegion { get; set; }
        public string DestinationStationName { get; set; }
        public string DestinationRegion { get; set; }
        public long TrackedRouteId { get; set; }
        public DateTime StartArrivalTime { get; set; }

        public DateTime EndArrivalTime { get; set; }

        public DateTime? StartDepartureTime { get; set; }

        public DateTime? EndDepartureTime { get; set; }

        public List<string> CarTypes { get; set; }

        public bool IsUpperSeat { get; set; }
        public bool IsLowerSeat { get; set; }
        public bool IsAnySeat {get; set; }

        public int? TravelTimeInMinutes { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public long UserId { get; set; }
        public string UserEmail { get; set; }
    }
}
