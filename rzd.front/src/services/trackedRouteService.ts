// src/services/trackedRouteService.ts
import api from '@/api/api.ts';

export const TrackedRouteService = {
  async getRouteStatistic(trackedRouteId: number) {
    try {
      return await api.post('/api/trackedroute/statistic', { trackedRouteId });
    } catch (error) {
      console.error('Ошибка при получении подсказок:', error);
      return { data: [] };
    }
  },

  async getAllRoutes() {
    try {
      return await api.get('/api/trackedroute/all');
    } catch (error) {
      console.error('Ошибка при получении маршрутов:', error);
      return { data: [] };
    }
  },

  async createRoute(payload: { originExpressCode: string; destinationExpressCode: string }) {
    try {
      return await api.post('/api/trackedroute/create', payload);
    } catch (error) {
      console.error('Ошибка при создании маршрута:', error);
      return null;
    }
  },

  async deleteRoute(trackeRouteId: number) {
    try {
      await api.post('/api/trackedroute/delete', { TrackeRouteId: trackeRouteId });
      return true;
    } catch (error) {
      console.error('Ошибка при удалении маршрута:', error);
      return false;
    }
  },

  async getSuggestions(query: string) {
    try {
      return await api.post('/api/trackedroute/suggests', { query });
    } catch (error) {
      console.error('Ошибка при получении подсказок:', error);
      return { data: [] };
    }
  },
};
