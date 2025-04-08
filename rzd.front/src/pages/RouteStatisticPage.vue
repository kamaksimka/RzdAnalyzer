<template>
  <div class="route-statistic">
    <h1><font-awesome-icon icon="chart-line" /> Статистика маршрута</h1>
    <h2>
      <font-awesome-icon icon="train" />
      {{ routeStatistic?.originStationName }} ({{ routeStatistic?.originRegion }}) →
      {{ routeStatistic?.destinationStationName }} ({{ routeStatistic?.destinationRegion }})
    </h2>
    <div class="tabs">
      <button :class="{ active: activeTab === 'stat' }" @click="activeTab = 'stat'">Общая статистика</button>
      <button :class="{ active: activeTab === 'trains' }" @click="activeTab = 'trains'">Список поездов</button>
    </div>

    <!-- Общая статистика -->
    <div v-if="activeTab === 'stat' && routeStatistic" class="statistic-card">
      <div class="statistic-details">
        <div class="statistic-item">
          <font-awesome-icon icon="calendar-alt" class="icon" />
          <strong>Дата начала отслеживания:</strong> {{ formatDate(routeStatistic?.startTrackedDate) }}
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="route" class="icon" />
          <strong>Дистанция:</strong> {{ routeStatistic?.avarageTripDistance?.toFixed(1) }} км
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="subway" class="icon" />
          <strong>Количество поездов:</strong> {{ routeStatistic?.numberTrains }}
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="chair" class="icon" />
          <strong>Количество мест:</strong> {{ routeStatistic?.numberCarPlaces }}
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="arrow-up" class="icon" />
          <strong>Максимальная цена:</strong> {{routeStatistic? formatCurrency(routeStatistic.maxPrice):0 }}
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="arrow-down" class="icon" />
          <strong>Минимальная цена:</strong> {{ routeStatistic? formatCurrency(routeStatistic.minPrice):0 }}
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="bolt" class="icon" />
          <strong>Самый быстрый поезд:</strong> {{ routeStatistic? formatTime(routeStatistic.fastestTrain) }}
        </div>

        <div class="statistic-item">
          <font-awesome-icon icon="hourglass-end" class="icon" />
          <strong>Самый медленный поезд:</strong> {{ routeStatistic? formatTime(routeStatistic.slowestTrain) }}
        </div>
      </div>
    </div>

    <!-- Список поездов через ag-Grid -->
    <div v-if="activeTab === 'trains'" class="train-grid">
      <!-- Используем компонент TrainGrid.vue -->
      <TrainGrid :trackedRouteId="trackedRouteId" />
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import TrainGrid from '@/components/TrainGrid.vue';
  import { TrackedRouteService } from '@/services/trackedRouteService';
  import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
  import { library } from '@fortawesome/fontawesome-svg-core';
  import {
    faChartLine,
    faTrain,
    faCalendarAlt,
    faSubway,
    faChair,
    faArrowUp,
    faArrowDown,
    faBolt,
    faHourglassEnd,
    faRoute
  } from '@fortawesome/free-solid-svg-icons';

  library.add(
    faChartLine,
    faTrain,
    faCalendarAlt,
    faSubway,
    faChair,
    faArrowUp,
    faArrowDown,
    faBolt,
    faHourglassEnd,
    faRoute
  );

  interface RouteStatistic {
    startTrackedDate: string;
    originStationName: string;
    originRegion: string;
    destinationStationName: string;
    destinationRegion: string;
    numberTrains: number;
    numberCarPlaces: number;
    maxPrice: number;
    minPrice: number;
    fastestTrain: string;
    slowestTrain: string;
  }

  export default defineComponent({
    name: 'RouteStatisticPage',
    components: {
      TrainGrid,
      FontAwesomeIcon
    },
    setup() {
      const route = useRoute();
      const trackedRouteId = Number(route.params.id);
      const activeTab = ref<'stat' | 'trains'>('stat');
      const routeStatistic = ref<any>(null);

      const formatDate = (dateString: string): string => {
        return new Date(dateString).toLocaleDateString('ru-RU');
      };

      const formatCurrency = (value: number | null): string => {
        return value != null ? value.toLocaleString('ru-RU', { style: 'currency', currency: 'RUB' }) : 'Не указано';
      };

      const formatTime = (time: string | null): string => {
        if (!time) {
          return 'Не указано';
        }

        const timeParts = time.split(' '); // Разделяем по пробелу, если есть
        const [dayPart, timePart] = timeParts.length === 2 ? timeParts : [null, time];

        // Разбираем день, если он есть
        const days = dayPart ? parseInt(dayPart, 10) : 0;

        // Разбираем время (часы, минуты, секунды)
        const [hours, minutes, seconds] = timePart.split(':').map(Number);

        // Формируем финальный результат
        let result = '';

        if (days > 0) {
          result += `${days}д `;
        }

        result += `${hours}ч ${minutes}м`;

        return result;
      };

      onMounted(async () => {
        const response = await TrackedRouteService.getRouteStatistic(trackedRouteId);
        if (response?.data) {
          routeStatistic.value = response.data;
        }
      });

      return {
        activeTab,
        routeStatistic,
        formatDate,
        formatCurrency,
        formatTime,
        trackedRouteId,
      };
    },
  });
</script>

<style scoped>
  .route-statistic {
    margin: 20px;
    font-family: Arial, sans-serif;
  }

  h1 {
    text-align: center;
    color: #333;
    font-size: 2rem;
    margin-bottom: 20px;
  }

  h2 {
    text-align: center;
    color: #333;
    font-size: 1.6rem;
    margin-bottom: 20px;
  }

  .tabs {
    display: flex;
    justify-content: center;
    margin-bottom: 20px;
  }

    .tabs button {
      padding: 10px 20px;
      margin: 0 10px;
      background-color: #eee;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      font-weight: bold;
      transition: background-color 0.3s;
    }

      .tabs button.active {
        background-color: #4CAF50;
        color: white;
      }

  .statistic-card {
    background-color: #f9f9f9;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    max-width: 800px;
    margin: 0 auto;
  }

    .statistic-card h2 {
      text-align: center;
      color: #333;
      font-size: 1.5rem;
      margin-bottom: 20px;
    }

  .statistic-details {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 20px;
    font-size: 1rem;
  }

  .statistic-item {
    padding: 10px;
    background-color: #fff;
    border-radius: 6px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  }

    .statistic-item strong {
      color: #333;
      font-weight: bold;
    }


  .icon {
    margin-right: 8px;
    color: #555;
  }

  .train-table {
    max-width: 1000px;
    margin: 0 auto;
  }

  .tabs {
    display: flex;
    justify-content: center;
    margin-bottom: 20px;
  }

    .tabs button {
      padding: 10px 20px;
      margin: 0 10px;
      background-color: #eee;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      font-weight: bold;
      transition: background-color 0.3s;
    }

      .tabs button.active {
        background-color: #4caf50;
        color: white;
      }
</style>
