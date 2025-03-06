using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RZD.Test.Models.AutoGenerateModels.TrainPricing;

namespace RZD.Test.API.Models.TrainPricing
{
    public class TrainPricingResponseModel
    {
        public string DestinationCode { get; set; }
        public string DestinationStationCode { get; set; }
        public string OriginCode { get; set; }
        public string OriginStationCode { get; set; }
        public List<Train> Trains { get; set; }
    }
}
