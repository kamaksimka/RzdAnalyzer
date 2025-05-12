using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Models
{
    public class TrainModel
    {
        public long Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }


        public DateTimeOffset ArrivalDateTime { get; set; }
        public List<string> CarServices { get; set; }
        public DateTimeOffset DepartureDateTime { get; set; }
        public string? TrainBrandCode { get; set; }
        public string? TrainDescription { get; set; }
        public string TrainNumber { get; set; }
        public int TripDistance { get; set; }
        public decimal TripDuration { get; set; }

        public string ArrivalStation {get; set;}
        public string DepartureStation { get; set;}

    }
}
