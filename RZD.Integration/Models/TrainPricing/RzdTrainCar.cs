namespace RZD.Integration.Models.TrainPricing
{
    public class RzdTrainCar
    {
        public List<string> Carriers { get; set; }
        public List<string> CarrierDisplayNames { get; set; }
        public List<string> ServiceClasses { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string CarType { get; set; }
        public string CarTypeName { get; set; }
        public int PlaceQuantity { get; set; }
        public int LowerPlaceQuantity { get; set; }
        public int UpperPlaceQuantity { get; set; }
        public int LowerSidePlaceQuantity { get; set; }
        public int UpperSidePlaceQuantity { get; set; }
        public int PlacesWithConditionalRefundableTariffQuantity { get; set; }
        public int LowerPlacesWithConditionalRefundableTariffQuantity { get; set; }
        public int UpperPlacesWithConditionalRefundableTariffQuantity { get; set; }
        public int MalePlaceQuantity { get; set; }
        public int FemalePlaceQuantity { get; set; }
        public int EmptyCabinQuantity { get; set; }
        public int MixedCabinQuantity { get; set; }
        public bool IsSaleForbidden { get; set; }
        public string AvailabilityIndication { get; set; }
        public List<string> CarDescriptions { get; set; }
        public string ServiceClassNameRu { get; set; }
        public string ServiceClassNameEn { get; set; }
        public List<string> InternationalServiceClasses { get; set; }
        public List<decimal> ServiceCosts { get; set; }
        public bool IsBeddingSelectionPossible { get; set; }
        public List<string> BoardingSystemTypes { get; set; }
        public bool HasElectronicRegistration { get; set; }
        public bool HasGenderCabins { get; set; }
        public bool HasPlaceNumeration { get; set; }
        public bool HasPlacesNearPlayground { get; set; }
        public bool HasPlacesNearPets { get; set; }
        public bool HasPlacesForDisabledPersons { get; set; }
        public bool HasPlacesNearBabies { get; set; }
        public bool HasNonRefundableTariff { get; set; }
        public string InfoRequestSchema { get; set; }
        public int TotalPlaceQuantity { get; set; }
        public List<string> PlaceReservationTypes { get; set; }
        public bool IsThreeHoursReservationAvailable { get; set; }
        public bool IsMealOptionPossible { get; set; }
        public bool IsAdditionalMealOptionPossible { get; set; }
        public bool IsOnRequestMealOptionPossible { get; set; }
        public bool IsTransitDocumentRequired { get; set; }
        public bool IsInterstate { get; set; }
        public bool HasNonBrandedCars { get; set; }
        public int TripPointQuantity { get; set; }
        public bool HasPlacesForBusinessTravelBooking { get; set; }
        public bool IsCarTransportationCoaches { get; set; }
        public string ServiceClassName { get; set; }
        public bool HasFssBenefit { get; set; }
    }
}