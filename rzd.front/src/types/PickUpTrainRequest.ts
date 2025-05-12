export interface PickUpTrainRequest {
  trackedRouteId: number;
  startArrivalTime: string;
  endArrivalTime: string;
  startDepartureTime: string;
  endDepartureTime: string;
  carTypes: string[];
  isUpperSeat: boolean;
  travelTimeInMinutes: number;
  minPrice: number;
  maxPrice: number;
  carServices: string[];
}
