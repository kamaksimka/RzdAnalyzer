using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Test.API.Models.TrainRoute
{
    public class Route
    {
        public string TrainNumber { get; set; }
        public List<RouteStop> RouteStops { get; set; }
    }
}
