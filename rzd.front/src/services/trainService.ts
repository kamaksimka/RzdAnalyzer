import api from '@/api/api.ts';

export const TrainService = {
  async getTrainsByRouteIdAndDate(trackedRouteId: number, date: string) {
    return await api.post('/api/train/trains', { trackedRouteId, date });
  },
};
