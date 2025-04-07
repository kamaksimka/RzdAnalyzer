<template>
  <div class="route-statistic" v-if="routeStatistic">
    <h1><font-awesome-icon icon="chart-line" /> Статистика маршрута</h1>

    <div class="statistic-card">
      <h2>
        <font-awesome-icon icon="train" />
        {{ routeStatistic.originStationName }} ({{ routeStatistic.originRegion }}) →
        {{ routeStatistic.destinationStationName }} ({{ routeStatistic.destinationRegion }})
      </h2>

      <div class="statistic-details">
        <div class="statistic-item">
          <font-awesome-icon icon="calendar-alt" class="icon" />
          <strong>Дата начала отслеживания:</strong> {{ formatDate(routeStatistic.startTrackedDate) }}
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="subway" class="icon" />
          <strong>Количество поездов:</strong> {{ routeStatistic.numberTrains }}
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="chair" class="icon" />
          <strong>Количество мест:</strong> {{ routeStatistic.numberCarPlaces }}
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
          <strong>Самый быстрый поезд:</strong> {{ routeStatistic.fastestTrain }}
        </div>
        <div class="statistic-item">
          <font-awesome-icon icon="hourglass-end" class="icon" />
          <strong>Самый медленный поезд:</strong> {{ routeStatistic.slowestTrain }}
        </div>
      </div>
    </div>
  </div>
</template>




<script lang="ts">
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
    faHourglassEnd
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
    faHourglassEnd
  );

  export default {
    name: 'RouteStatisticPage',
    components: {
      FontAwesomeIcon
    },
    data() {
      return {
        routeStatistic: null as null | {
          startTrackedDate: string,
          originStationName: string,
          originRegion: string,
          destinationStationName: string,
          destinationRegion: string,
          numberTrains: number,
          numberCarPlaces: number,
          maxPrice: number,
          minPrice: number,
          fastestTrain: string,
          slowestTrain: string
        }
      };
    },
    async created() {
      const trackedRouteId = Number(this.$route.params.id);
      const response = await TrackedRouteService.getRouteStatistic(trackedRouteId);
      if (response?.data) {
        this.routeStatistic = response.data;
      }
    },
    methods: {
      formatDate(dateString: string): string {
        if (!dateString) return "";
        const date = new Date(dateString);
        return date.toLocaleDateString('ru-RU');
      },
      formatCurrency(value: number): string {
        if (!value) return "";
        return value.toLocaleString('ru-RU', {
          style: 'currency',
          currency: 'RUB'
        });
      }
    }
  };
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

    .statistic-item:last-child {
      grid-column: span 2;
    }

  .icon {
    margin-right: 8px;
    color: #555;
  }
</style>
