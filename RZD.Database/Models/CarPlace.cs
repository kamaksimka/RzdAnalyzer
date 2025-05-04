﻿namespace RZD.Database.Models
{
    public class CarPlace
    {
        public long Id { get; set; }
        public string CarNumber { get; set; }
        public string CarPlaceNumber { get; set; }
        public bool IsFree { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public bool ArePlacesForBusinessTravelBooking { get; set; }
        public DateTimeOffset ArrivalDateTime { get; set; }
        public string? AvailabilityIndication { get; set; }
        public string CarPlaceType { get; set; }
        public string? CarSubType { get; set; }
        public string? CarType { get; set; }
        public string DestinationStationCode { get; set; }
        
        public bool HasDynamicPricing { get; set; }
        public bool HasFssBenefit { get; set; }
        public bool HasGenderCabins { get; set; }
        public bool HasNonRefundableTariff { get; set; }
        public bool HasPlacesNearBabies { get; set; }
        public bool HasPlacesNearPets { get; set; }
        public bool IsAdditionalMealOptionPossible { get; set; }
        public bool IsAdditionalPassengerAllowed { get; set; }
        public bool IsBranded { get; set; }
        public bool IsBuffet { get; set; }
        public bool IsCarTransportationCoach { get; set; }
        public bool IsChildTariffTypeAllowed { get; set; }
        public bool IsForDisabledPersons { get; set; }
        public bool IsMealOptionPossible { get; set; }
        public bool IsOnRequestMealOptionPossible { get; set; }
        public bool IsTwoStorey { get; set; }
        public DateTimeOffset MealSalesOpenedTill { get; set; }
        public bool OnlyNonRefundableTariff { get; set; }
        public string? PassengerSpecifyingRules { get; set; }
        public string? PlaceReservationType { get; set; }
        public string? ServiceClass { get; set; }
        public decimal ServiceCost { get; set; }
        public List<string> Services { get; set; }
        public string? TripDirection { get; set; }
        public long TrainId { get; set; }
        public virtual Train Train { get; set; }

    }
}
