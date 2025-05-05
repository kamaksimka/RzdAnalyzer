<template>
  <div>
    <h2>График свободных мест</h2>
    <p>Поезд №{{ trainId }}</p>

    <div>
      <label v-for="type in carTypes" :key="type">
        <input type="checkbox" :value="type" v-model="selectedTypes" @change="fetchFreePlaces"/>
        {{ type }}
      </label>
    </div>

    <canvas id="freePlacesChart"></canvas>
  </div>
</template>

<script lang="ts">
  import { defineComponent, onMounted, shallowRef, ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { TrainService } from '@/services/trainService';
  import {
    Chart as ChartJS,
    Title,
    Tooltip,
    Legend,
    LineElement,
    CategoryScale,
    LinearScale,
    PointElement,
    LineController
  } from 'chart.js';

  ChartJS.register(Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement, LineController);

  export default defineComponent({
    name: 'FreePlacesPlot',
    setup() {
      const route = useRoute();
      const trainId = Number(route.params.trainId);
      const carTypes = ['Compartment', 'Luxury', 'ReservedSeat', 'Sedentary', 'Soft'];
      const selectedTypes = ref<string[]>([]);

      // shallowRef, чтобы избежать излишней реактивности
      const chart = shallowRef<ChartJS | null>(null);

      const fetchFreePlaces = async () => {
        if (!selectedTypes.value.length) return;

        const datasets: any[] = [];
        let labels: string[] = [];

        for (const carType of selectedTypes.value) {
          try {
            const response = await TrainService.getFreePlacesPlotByCarType(trainId, carType);
            const data: Record<string, number> = response.data;

            if (labels.length === 0) {
              labels = Object.keys(data);
            }

            datasets.push({
              label: carType,
              data: Object.values(data),
              borderColor: getColorByType(carType),
              fill: false
            });

          } catch (error) {
            console.error(`Ошибка при получении данных для ${carType}`, error);
          }
        }

        const chartData = {
          labels,
          datasets
        };

        const options = {
          responsive: true,
          plugins: {
            title: {
              display: true,
              text: 'График свободных мест по типу вагона'
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
        };

        if (!chart.value) {
          createChart(chartData, options);
        } else {
          updateChart(chartData);
        }
      };

      const createChart = (data: any, options: any) => {
        const ctx = document.getElementById('freePlacesChart') as HTMLCanvasElement;
        if (ctx) {
          chart.value = new ChartJS(ctx, {
            type: 'line',
            data,
            options
          });
        }
      };

      const updateChart = (newData: any) => {
        if (chart.value) {
          chart.value.data = newData;
          chart.value.update();
        }
      };

      const getColorByType = (type: string) => {
        const colors: Record<string, string> = {
          'Compartment': '#4CAF50',
          'Luxury': '#FF9800',
          'ReservedSeat': '#2196F3',
          'Sedentary': '#9C27B0',
          'Soft': '#E91E63'
        };
        return colors[type] || '#000';
      };

      watch(selectedTypes, (newValue, oldValue) => {
        if (newValue !== oldValue) {
          fetchFreePlaces();
        }
      });

      onMounted(() => {
        selectedTypes.value = [...carTypes];
        fetchFreePlaces();
      });

      return {
        trainId,
        carTypes,
        selectedTypes
      };
    }
  });
</script>

<style scoped>
  h2 {
    color: #4CAF50;
  }

  label {
    margin-right: 10px;
  }
</style>
