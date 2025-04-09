import api from '@/api/api.ts';

export const TrainService = {
  async getTrainsByRouteIdAndDate(trackedRouteId: number, dateFrom: string, dateTo: string) {
    return await api.post('/api/train/trains', { trackedRouteId, dateFrom, dateTo });
  },

  async getTrainGridInitModel(trackedRouteId: number) {
    return await api.post('/api/train/trainGridInitModel', { trackedRouteId });
  },
};
