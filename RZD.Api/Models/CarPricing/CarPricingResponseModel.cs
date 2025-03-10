using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.API.Models.CarPricing
{
    public class CarPricingResponseModel
    {
        public sbyte DestinationCode { get; set; }
        public string OriginCode { get; set; }

        public List<Car> Cars { get; set; }
    }
}
