using RZD.Integration.Models.TrainPricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Integration.Models.TrainPricing
{
    public class RzdTrain
    {
        public DateTimeOffset ArrivalDateTime { get; set; }
        public int ArrivalStopTime { get; set; }
        public List<string> CarServices { get; set; }
        public DateTimeOffset DepartureDateTime { get; set; }
        public int DepartureStopTime { get; set; }
        public string DestinationStationCode { get; set; }
        public string DisplayTrainNumber { get; set; }
        public string FinalTrainStationCode { get; set; }
        public bool HasCarTransportationCoaches { get; set; }
        public bool HasDynamicPricingCars { get; set; }
        public bool HasTwoStoreyCars { get; set; }
        public string InitialTrainStationCode { get; set; }
        public bool IsBranded { get; set; }
        public bool IsFromSchedule { get; set; }
        public bool IsPlaceRangeAllowed {get; set; }
        public bool IsSaleForbidden { get; set; }
        public bool IsSuburban { get; set; }
        public bool IsTicketPrintRequiredForBoarding { get; set; }
        public bool IsTourPackagePossible { get; set; }
        public bool IsTrainRouteAllowed { get; set; }
        public bool IsWaitListAvailable { get; set; }
        public DateTimeOffset LocalArrivalDateTime { get; set; }
        public DateTimeOffset LocalDepartureDateTime { get; set; }
        public string OriginStationCode { get; set; }
        public string TrainBrandCode { get; set; }
        public string TrainDescription { get; set; }
        public string TrainNumber { get; set; }
        public string TrainNumberToGetRoute { get; set; }
        public int TripDistance { get; set; }
        public decimal TripDuration { get; set; }

        public List<RzdTrainCar> CarGroups { get; set; }
    }
}
