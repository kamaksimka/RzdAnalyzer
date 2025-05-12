<template>
  <div class="free-places-container">
    <h2>График свободных мест</h2>
    <p>Поезд №{{ trainId }}</p>

    <div class="checkbox-group">
      <label v-for="type in carTypes" :key="type">
        <input type="checkbox" :value="type" v-model="selectedTypes" @change="fetchFreePlaces" />
        <span>{{ getLabelByType(type) }}</span>
      </label>
    </div>

    <canvas id="freePlacesChart"></canvas>

    <div class="back-button-container">
      <button @click="goBack" class="back-button">← Назад</button>
    </div>
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

      const chart = shallowRef<ChartJS | null>(null);

      const fetchFreePlaces = async () => {
        if (!selectedTypes.value.length) return;

        const allDates = new Set<string>();
        const dataByType: Record<string, Record<string, number>> = {};

        // Загружаем и собираем все данные
        for (const carType of selectedTypes.value) {
          try {
            const response = await TrainService.getFreePlacesPlotByCarType(trainId, carType);
            const data: Record<string, number> = response.data;
            dataByType[carType] = data;
            Object.keys(data).forEach(date => allDates.add(date));
          } catch (error) {
            console.error(`Ошибка при получении данных для ${carType}`, error);
          }
        }

        const labels = Array.from(allDates).sort((a, b) => new Date(a).getTime() - new Date(b).getTime());

        const datasets = Object.entries(dataByType).map(([carType, data]) => ({
          label: getLabelByType(carType),
          data: labels.map(date => data[date] ?? null),
          borderColor: getColorByType(carType),
          fill: false
        }));

        const chartData = {
          labels,
          datasets
        };

        const options = {
          responsive: true,
          spanGaps: true,
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

      const getLabelByType = (type: string) => {
        const labels: Record<string, string> = {
          'Compartment': 'Купе',
          'Luxury': 'Люкс',
          'ReservedSeat': 'Плацкарт',
          'Sedentary': 'Сидячий',
          'Soft': 'Для инвалидов'
        };
        return labels[type] || type;
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

      const goBack = () => {
        window.history.back();
      };

      return {
        trainId,
        carTypes,
        selectedTypes,
        goBack,
        getLabelByType,
        fetchFreePlaces
      };
    }
  });
</script>

<style scoped>
  .free-places-container {
    padding: 20px;
    background-color: #fff;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
    max-width: 90%; 
    margin: auto;
    font-family: 'Arial', sans-serif;
  }

  h2 {
    color: #E30611;
    font-size: 24px;
    text-align: center;
    margin-bottom: 16px;
  }

  p {
    text-align: center;
    font-weight: bold;
    color: #333;
    margin-bottom: 20px;
  }

  .checkbox-group {
    text-align: center;
    margin-bottom: 30px;
  }

  label {
    display: inline-block;
    margin: 6px 10px;
    padding: 6px 16px;
    border: 1px solid #E30611;
    border-radius: 20px;
    cursor: pointer;
    font-weight: 500;
    color: #E30611;
    transition: all 0.3s;
  }

  input[type="checkbox"] {
    display: none;
  }

    label:has(input:checked),
    input[type="checkbox"]:checked + span {
      background-color: #E30611;
      color: white;
    }

  canvas {
    background-color: #fff;
    border: 1px solid #ccc;
    border-radius: 12px;
    padding: 10px;
    max-width: 100%;
  }

  .back-button-container {
    display: flex;
    justify-content: center;
    margin-top: 40px;
  }

  .back-button {
    display: inline-block;
    background-color: #E30611;
    color: white;
    padding: 10px 20px;
    border-radius: 8px;
    text-decoration: none;
    font-weight: bold;
    transition: background-color 0.3s;
    cursor: pointer;
  }

    .back-button:hover {
      background-color: #c1040e;
    }
</style>
