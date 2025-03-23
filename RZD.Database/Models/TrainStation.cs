using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Database.Models
{
    public class TrainStation
    {
        public long Id { get; set; }
        public string ExpressCode { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string ForeignCode { get; set; }

        public long? CityId { get; set; }
        public virtual City? City { get; set; }
    }
}
