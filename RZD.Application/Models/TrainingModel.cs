using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Models
{
    public class TrainingFreePlacesModel
    {
        public long TrainId { get; set; }
        public string CarType { get; set; }
        public Dictionary<DateTime,int> FreePlaces { get; set; }
        public Dictionary<DateTime, decimal> MaxPrice { get; set; }
        public Dictionary<DateTime, decimal> MinPrice { get; set; }
        public int CountPlace { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public decimal StartMinPrice { get; set; }
        public decimal StartMaxPrice { get; set; }

    }
}
