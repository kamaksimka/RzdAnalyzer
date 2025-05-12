<template>
  <div class="subscriptions-page">
    <h2>Ваши подписки</h2>

    <div v-if="subscriptions.length > 0" class="table-container">
      <table>
        <thead>
          <tr>
            <th>Маршрут</th>
            <th>Регионы</th>
            <th>Прибытие</th>
            <th>Отправление</th>
            <th>Типы вагонов</th>
            <th>Сервисы</th>
            <th>Верхние</th>
            <th>Нижние</th>
            <th>Любое место</th>
            <th>Время в пути</th>
            <th>Цена</th>
            <th>Email</th>
            <th>Завершена</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="sub in subscriptions" :key="sub.trackedRouteId">
            <td>{{ sub.originStationName }} - {{ sub.destinationStationName }}</td>
            <td>{{ sub.originRegion }} - {{ sub.destinationRegion }}</td>
            <td>
              {{ formatDate(sub.startArrivalTime) }} - {{ formatDate(sub.endArrivalTime) }}
            </td>
            <td>
              {{ formatDate(sub.startDepartureTime) }} - {{ formatDate(sub.endDepartureTime) }}
            </td>
            <td>{{ sub.carTypes?.join(', ') || '—' }}</td>
            <td>{{ sub.carServices?.join(', ') || '—' }}</td>
            <td>{{ sub.isUpperSeat ? 'Да' : 'Нет' }}</td>
            <td>{{ sub.isLowerSeat ? 'Да' : 'Нет' }}</td>
            <td>{{ sub.isAnySeat ? 'Да' : 'Нет' }}</td>
            <td>{{ formatTravelTime(sub.travelTimeInMinutes) }}</td>
            <td>
              {{ formatPrice(sub.minPrice) }} - {{ formatPrice(sub.maxPrice) }}
            </td>
            <td>{{ sub.userEmail }}</td>
            <td>{{ sub.isComplete ? 'Да' : 'Нет' }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="no-subscriptions">
      У вас пока нет активных подписок.
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent, onMounted, ref } from 'vue';
  import { TrainService } from '@/services/trainService';

  export default defineComponent({
    name: 'UserSubscriptionsPage',
    setup() {
      const subscriptions = ref<any[]>([]);

      const fetchSubscriptions = async () => {
        const res = await TrainService.subscriptions();
        subscriptions.value = res.data;
      };

      const formatDate = (date: string | null) => {
        if (!date) return '—';
        return new Date(date).toLocaleString('ru-RU');
      };

      const formatPrice = (price: number | null) => {
        if (price == null) return '—';
        return price.toLocaleString('ru-RU', {
          style: 'currency',
          currency: 'RUB',
        });
      };

      const formatTravelTime = (minutes: number | null) => {
        if (!minutes) return '—';
        const h = Math.floor(minutes / 60);
        const m = minutes % 60;
        return `${h} ч ${m} мин`;
      };

      onMounted(fetchSubscriptions);

      return {
        subscriptions,
        formatDate,
        formatPrice,
        formatTravelTime,
      };
    },
  });
</script>

<style scoped>
  .subscriptions-page {
    padding: 20px;
    max-width: 1200px;
    margin: auto;
  }

  h2 {
    margin-bottom: 20px;
  }

  .table-container {
    overflow-x: auto;
  }

  table {
    width: 100%;
    border-collapse: collapse;
    font-size: 14px;
  }

  th,
  td {
    padding: 8px 12px;
    border: 1px solid #ccc;
  }

  th {
    background-color: #f0f0f0;
    text-align: left;
  }

  .no-subscriptions {
    margin-top: 30px;
    font-size: 16px;
    text-align: center;
    color: #666;
  }
</style>
