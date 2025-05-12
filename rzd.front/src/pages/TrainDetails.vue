<template>
  <div class="train-details">
    <h2>Информация о поезде</h2>
    <div v-if="train">
      <p><strong>Номер поезда:</strong> {{ train.trainNumber }}</p>
      <p><strong>Бренд:</strong> {{ train.trainBrandCode || 'Не указано' }}</p>
      <p><strong>Описание:</strong> {{ train.trainDescription || 'Нет описания' }}</p>
      <p><strong>Дата отправления:</strong> {{ formatDate(train.departureDateTime) }}</p>
      <p><strong>Дата прибытия:</strong> {{ formatDate(train.arrivalDateTime) }}</p>
      <p><strong>Дистанция:</strong> {{ train.tripDistance }} км</p>
      <p><strong>Длительность:</strong> {{ formatDuration(train.tripDuration) }}</p>

      <div class="services-block">
        <p><strong>Услуги:</strong></p>
        <ServiceIconsRenderer :params="{ value: train.carServices }" />
      </div>
    </div>
    <div v-else>
      <p>Загрузка информации о поезде...</p>
    </div>

    <div class="link-container">
      <router-link :to="{ name: 'FreePlacesPlot', params: { trainId: trainId }}" class="info-link">
        <font-awesome-icon icon="chart-bar" class="icon" /> Свободные места
      </router-link>
      <router-link :to="{ name: 'PricePlot', params: { trainId: trainId }}" class="info-link">
        <font-awesome-icon icon="chart-line" class="icon" /> График цен
      </router-link>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
  import { library } from '@fortawesome/fontawesome-svg-core';
  import { TrainService } from '@/services/trainService';
  import ServiceIconsRenderer from '@/components/ServiceIconsRenderer.vue';

  import { faChartBar, faChartLine } from '@fortawesome/free-solid-svg-icons';
  library.add(faChartBar, faChartLine);

  interface TrainModel {
    id: number;
    createdDate: string;
    arrivalDateTime: string;
    departureDateTime: string;
    carServices: string[];
    trainBrandCode: string | null;
    trainDescription: string | null;
    trainNumber: string;
    tripDistance: number;
    tripDuration: number;
  }

  export default defineComponent({
    name: 'TrainDetails',
    components: {
      FontAwesomeIcon,
      ServiceIconsRenderer
    },
    setup() {
      const route = useRoute();
      const trainId = Number(route.params.id);
      const train = ref<TrainModel | null>(null);

      const fetchTrainInfo = async () => {
        try {
          const response = await TrainService.getTrainById(trainId);
          train.value = response.data;
        } catch (error) {
          console.error('Ошибка при загрузке данных поезда:', error);
        }
      };

      const formatDate = (date: string) => {
        return new Date(date).toLocaleString('ru-RU');
      };

      const formatDuration = (minutes: number) => {
        const days = Math.floor(minutes / (60 * 24));
        const hours = Math.floor((minutes % (60 * 24)) / 60);
        const mins = minutes % 60;

        const parts = [];
        if (days > 0) parts.push(`${days}д`);
        if (hours > 0) parts.push(`${hours}ч`);
        if (mins > 0 || parts.length === 0) parts.push(`${mins}мин`);

        return parts.join(' ');
      };

      onMounted(() => {
        fetchTrainInfo();
      });

      return {
        trainId,
        train,
        formatDate,
        formatDuration
      };
    }
  });
</script>

<style scoped>
  .train-details {
    font-family: Arial, sans-serif;
    background-color: #FFFFFF;
    padding: 24px;
    margin: 20px auto;
    max-width: 900px;
    border: 1px solid #E0E0E0;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
    color: #2D2D2D;
  }

  h2 {
    color: #D52B1E;
    font-size: 24px;
    margin-bottom: 20px;
    border-bottom: 1px solid #E0E0E0;
    padding-bottom: 8px;
  }

  p {
    margin: 6px 0;
    font-size: 16px;
  }

  strong {
    color: #000000;
  }

  .services-block {
    display: flex;
    flex-direction: column;
    margin-top: 20px;
  }

  .service-icons {
    margin-top: 8px;
    display: flex;
    flex-wrap: wrap;
    gap: 12px;
  }

  .service-icon {
    color: #D52B1E;
    font-size: 20px;
    transition: transform 0.2s ease;
    cursor: help;
  }

    .service-icon:hover {
      transform: scale(1.2);
    }

  .link-container {
    margin-top: 30px;
    display: flex;
    justify-content: center;
    gap: 16px;
  }

  .info-link {
    display: inline-flex;
    align-items: center;
    padding: 12px 20px;
    background-color: #D52B1E;
    color: #FFFFFF;
    text-decoration: none;
    border-radius: 10px;
    font-size: 15px;
    font-weight: 500;
    box-shadow: 0 2px 4px rgba(213, 43, 30, 0.25);
    transition: background-color 0.3s ease, transform 0.3s ease;
  }

    .info-link:hover {
      background-color: #BB1E16;
      transform: scale(1.05);
    }

  .icon {
    margin-right: 8px;
    font-size: 16px;
  }
</style>
