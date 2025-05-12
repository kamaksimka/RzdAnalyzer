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
    <div v-if="activeTab === 'stat' && routeStatistic" class="statistic-wrapper">
      <div class="statistic-card">
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
            <strong>Максимальная цена:</strong> {{ formatCurrency(routeStatistic.maxPrice) }}
          </div>
          <div class="statistic-item">
            <font-awesome-icon icon="arrow-down" class="icon" />
            <strong>Минимальная цена:</strong> {{ formatCurrency(routeStatistic.minPrice) }}
          </div>
          <div class="statistic-item">
            <font-awesome-icon icon="bolt" class="icon" />
            <strong>Самый быстрый поезд:</strong> {{ formatTime(routeStatistic.fastestTrain) }}
          </div>
          <div class="statistic-item">
            <font-awesome-icon icon="hourglass-end" class="icon" />
            <strong>Самый медленный поезд:</strong> {{ formatTime(routeStatistic.slowestTrain) }}
          </div>
        </div>
      </div>
    </div>

    <!-- Список поездов -->
    <div v-if="activeTab === 'trains'" class="train-grid">
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
        return value != null
          ? value.toLocaleString('ru-RU', { style: 'currency', currency: 'RUB' })
          : 'Не указано';
      };

      const formatTime = (time: string | null): string => {
        if (!time) return 'Не указано';
        const timeParts = time.split(' ');
        const [dayPart, timePart] = timeParts.length === 2 ? timeParts : [null, time];
        const days = dayPart ? parseInt(dayPart, 10) : 0;
        const [hours, minutes] = timePart.split(':').map(Number);
        return `${days > 0 ? `${days}д ` : ''}${hours}ч ${minutes}м`;
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
        trackedRouteId
      };
    }
  });
</script>

<style scoped>
  .route-statistic {
    margin: 40px auto;
    max-width: 1200px; /* увеличено с 1000 */
    font-family: Arial, sans-serif;
    color: #2D2D2D;
  }

  .statistic-card {
    background-color: #FFFFFF;
    padding: 30px;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    max-width: 1000px; /* было 800px */
    width: 100%;
    border: 1px solid #E0E0E0;
  }

  h1, h2 {
    text-align: center;
    margin-bottom: 20px;
  }

  .tabs {
    display: flex;
    justify-content: center;
    margin-bottom: 30px;
  }

    .tabs button {
      padding: 10px 20px;
      margin: 0 10px;
      background-color: #eeeeee;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      font-weight: bold;
      transition: background-color 0.3s, transform 0.2s;
    }

      .tabs button.active {
        background-color: #D52B1E;
        color: white;
      }

      .tabs button:hover {
        background-color: #BB1E16;
        color: white;
      }

  .statistic-wrapper {
    display: flex;
    justify-content: center;
    align-items: flex-start;
  }

  .statistic-details {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 20px;
  }

  .statistic-item {
    background-color: #F8F8F8;
    padding: 12px 16px;
    border-radius: 8px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    display: flex;
    align-items: center;
  }

    .statistic-item .icon {
      color: #D52B1E;
      margin-right: 10px;
      font-size: 18px;
    }

    .statistic-item strong {
      margin-right: 5px;
      color: #000;
    }
</style>
