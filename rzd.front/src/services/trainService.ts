import api from '@/api/api.ts';
import type { PickUpTrainRequest } from '@/types/PickUpTrainRequest'; 

export const TrainService = {
  async getTrainsByRouteIdAndDate(trackedRouteId: number, dateFrom: string, dateTo: string) {
    return await api.post('/api/train/trains', { trackedRouteId, dateFrom, dateTo });
  },

  async getTrainGridInitModel(trackedRouteId: number) {
    return await api.post('/api/train/trainGridInitModel', { trackedRouteId });
  },

  async getFreePlacesPlot(trainId: number) {
    return await api.post('/api/train/freePlacesPlot', { trainId });
  },

  async getFreePlacesPlotByCarType(trainId: number, carType: string) {
    return await api.post('/api/train/freePlacesByCarTypePlot', { trainId, carType });
  },

  async getMinPricePlacesPlot(trainId: number, carType: string) {
    return await api.post('/api/train/minPricePlacesPlot', {
      trainId,
      carType
    });
  },

  async getMaxPricePlacesPlot(trainId: number, carType: string) {
    return await api.post('/api/train/maxPricePlacesPlot', {
      trainId,
      carType
    });
  },

  async getTrainById(trainId: number) {
    return await api.post('/api/train/train', { trainId }); // предполагается, что API ожидает поле trainId
  },

  // Новый метод для подбора поезда
  async pickUpTrain(payload: any) {
    try {
      return await api.post('/api/train/pickup', payload);
    } catch (error) {
      console.error('Ошибка при подборе поездов:', error);
      return { data: [] };
    }
  },

  async subscribeToTrains(request: any) {
    return await api.post('api/subscription/create', request);
  },

  async subscriptions() {
    return await api.get('api/subscription/subscriptions');
  }


};
