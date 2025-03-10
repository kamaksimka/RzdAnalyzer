using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.API.Models.Suggests
{
    public class SuggestsResponseModel
    {
        public List<City> City { get; set; }
        public List<TrainStation> Train { get; set; }
    }
}
