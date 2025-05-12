using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Database.Models
{
    public class Subscription
    {
        public long Id { get; set; }
        public long TrackedRouteId { get; set; }
        public DateTime StartArrivalTime { get; set; }

        public DateTime EndArrivalTime { get; set; }

        public DateTime? StartDepartureTime { get; set; }

        public DateTime? EndDepartureTime { get; set; }

        public List<string> CarTypes { get; set; }

        public bool IsUpperSeat { get; set; }
        public bool IsLowerSeat { get; set; }
        public bool IsAnySeat { get; set; }

        public int? TravelTimeInMinutes { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string> CarServices { get; set; }

        public bool IsComplete { get; set; }
        public bool IsDelete { get; set; }


        public long UserId { get; set; }
        public User User { get; set; }
    }
}
