using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Models
{
    public class CreateTrackedRouteRequest
    {
        public string OriginExpressCode { get; set; }
        public string DestinationExpressCode { get; set; }
    }
}
