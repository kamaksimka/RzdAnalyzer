using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.API.Models.TrainRoute
{
    public class RzdRoute
    {
        public string TrainNumber { get; set; }
        public string Name { get; set; }    
        public string OriginName { get; set; }
        public string DestinationName { get; set; }
        public List<RzdRouteStop> RouteStops { get; set; }
    }
}
