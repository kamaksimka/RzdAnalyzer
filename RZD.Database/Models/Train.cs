using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Database.Models
{
    public class Train
    {
        public long Id { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int ArrivalStopTime { get; set; }
        public List<string> CarServices { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public int DepartureStopTime { get; set; }
        public string DisplayTrainNumber { get; set; }
        public bool HasCarTransportationCoaches { get; set; }
        public bool HasDynamicPricingCars { get; set; }
        public bool HasTwoStoreyCars { get; set; }
        public string InitialTrainStationCode { get; set; }
        public bool IsBranded { get; set; }
        public bool IsFromSchedule { get; set; }
        public bool IsPlaceRangeAllowed { get; set; }
        public bool IsSaleForbidden { get; set; }
        public bool IsSuburban { get; set; }
        public bool IsTicketPrintRequiredForBoarding { get; set; }
        public bool IsTourPackagePossible { get; set; }
        public bool IsTrainRouteAllowed { get; set; }
        public bool IsWaitListAvailable { get; set; }
        public DateTime LocalArrivalDateTime { get; set; }
        public DateTime LocalDepartureDateTime { get; set; }
        public string TrainBrandCode { get; set; }
        public string TrainDescription { get; set; }
        public string TrainNumber { get; set; }
        public string TrainNumberToGetRoute { get; set; }
        public int TripDistance { get; set; }
        public int TripDuration { get; set; }

        public virtual List<Car> Cars { get; set; }
        public virtual Route Route { get; set; }

        public long OriginStationId { get; set; }
        public virtual TrainStation OriginStation { get; set; }

        public long DestinationStationId { get; set; }
        public virtual TrainStation DestinationStation { get; set; }

        public long FinalTrainStationId { get; set; }
        public virtual TrainStation FinalTrainStation { get; set; }
    }
}
