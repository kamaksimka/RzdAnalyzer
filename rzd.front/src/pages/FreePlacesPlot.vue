<template>
  <div>
    <h2>График свободных мест</h2>
    <p>График свободных мест для поезда: {{ trainId }}</p>
    <!-- Канвас для рендеринга графика -->
    <canvas id="freePlacesChart"></canvas>
  </div>
</template>

<script lang="ts">
  import { defineComponent, onMounted, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import { TrainService } from '@/services/trainService';
  import { Chart as ChartJS, Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement, LineController } from 'chart.js';
  ChartJS.register(Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement, LineController);


  export default defineComponent({
    name: 'FreePlacesPlot',
    setup() {
      const route = useRoute();
      const trainId = Number(route.params.trainId);
      const chartData = ref<{ [key: string]: number } | null>(null); // Данные для графика

      // Запрос для получения данных о свободных местах
      const fetchFreePlaces = async () => {
        try {
          const response = await TrainService.getFreePlacesPlot(trainId);
          chartData.value = response.data;
          renderChart();
        } catch (error) {
          console.error('Ошибка при загрузке данных для графика', error);
        }
      };

      // Рендеринг графика
      const renderChart = () => {
        if (chartData.value) {
          const labels = Object.keys(chartData.value);
          const data = Object.values(chartData.value);

          new ChartJS('freePlacesChart', {
            type: 'line',
            data: {
              labels: labels,
              datasets: [{
                label: 'Свободные места',
                data: data,
                borderColor: '#4CAF50',
                fill: false,
              }]
            },
            options: {
              responsive: true,
              plugins: {
                title: {
                  display: true,
                  text: 'График свободных мест'
                }
              },
              scales: {
                x: {
                  title: {
                    display: true,
                    text: 'Дата'
                  }
                },
                y: {
                  title: {
                    display: true,
                    text: 'Количество мест'
                  },
                  min: 0
                }
              }
            }
          });
        }
      };

      // Загрузка данных после монтирования компонента
      onMounted(() => {
        fetchFreePlaces();
      });

      return {
        trainId
      };
    }
  });
</script>

<style scoped>
  h2 {
    color: #4CAF50;
  }
</style>
