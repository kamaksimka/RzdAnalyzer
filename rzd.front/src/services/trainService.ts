import api from '@/api/api.ts';

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
  }
};
