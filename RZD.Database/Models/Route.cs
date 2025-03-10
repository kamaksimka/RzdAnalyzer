using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Database.Models
{
    public class Route
    {
        public int Id { get; set; }

        #region Key
        public DateTime DepartureDate { get; set; }
        public string TrainNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        #endregion

        public string Name { get; set; }
        public string OriginName { get; set; }
        public string DestinationName { get; set; }

        public virtual List<RouteStop> RouteStops { get; set; }


    }
}
