using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Models
{
    public class CarPlaceModel
    {
        public long Id { get; set; }
        public string CarNumber { get; set; }
        public string CarPlaceNumber { get; set; }
        public string CarPlaceType { get; set; }
        public string? CarSubType { get; set; }
        public string? CarType { get; set; }
        public bool IsFree { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string? PassengerSpecifyingRules { get; set; }
        public string? PlaceReservationType { get; set; }
        public string? ServiceClass { get; set; }
        public decimal ServiceCost { get; set; }
        public List<string> Services { get; set; }
        public string? TripDirection { get; set; }

        public Dictionary<string, List<EntityHistoryModel>> HistoryOfChanges { get; set; }
    }
}
