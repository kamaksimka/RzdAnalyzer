<template>
  <div class="price-plot-container">
    <h2>График цен</h2>
    <p>Поезд №{{ trainId }}</p>

    <div class="checkbox-group">
      <label v-for="type in carTypes" :key="type">
        <input type="checkbox" :value="type" v-model="selectedTypes" />
        <span>{{ getLabelByType(type) }}</span>
      </label>
    </div>

    <canvas id="priceChart"></canvas>

    <div class="back-button-container">
      <button @click="goBack" class="back-button">← Назад</button>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, watch, onMounted, shallowRef } from 'vue';
  import { useRoute } from 'vue-router';
  import { Chart as ChartJS, Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement, LineController } from 'chart.js';
  import { TrainService } from '@/services/trainService';

  ChartJS.register(Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement, LineController);

  export default defineComponent({
    name: 'PricePlot',
    setup() {
      const route = useRoute();
      const trainId = Number(route.params.trainId);

      const carTypes = ['Compartment', 'Luxury', 'ReservedSeat', 'Sedentary', 'Soft'];
      const selectedTypes = ref<string[]>(['Compartment']);
      const chart = shallowRef<ChartJS | null>(null);

      const fetchPriceData = async () => {
        if (!selectedTypes.value.length) return;

        const allLabelSet = new Set<string>();

        // Сначала собираем все даты
        const responses = await Promise.all(
          selectedTypes.value.map(async (carType) => {
            try {
              const [minResponse, maxResponse] = await Promise.all([
                TrainService.getMinPricePlacesPlot(trainId, carType),
                TrainService.getMaxPricePlacesPlot(trainId, carType)
              ]);

              const minData: Record<string, number> = minResponse.data;
              const maxData: Record<string, number> = maxResponse.data;

              // Сохраняем все ключи (даты) в общий набор
              Object.keys(minData).forEach(date => allLabelSet.add(date));
              Object.keys(maxData).forEach(date => allLabelSet.add(date));

              return { carType, minData, maxData };
            } catch (error) {
              console.error(`Ошибка загрузки цен для ${carType}`, error);
              return null;
            }
          })
        );

        const labels = Array.from(allLabelSet).sort((a, b) => new Date(a).getTime() - new Date(b).getTime());

        const datasets: any[] = [];

        for (const res of responses) {
          if (!res) continue;

          const { carType, minData, maxData } = res;

          const fillValues = (data: Record<string, number>) =>
            labels.map(date => data[date] ?? null);

          datasets.push(
            {
              label: `${getLabelByType(carType)} (min)`,
              data: fillValues(minData),
              borderColor: getColorByType(carType, true),
              fill: false
            },
            {
              label: `${getLabelByType(carType)} (max)`,
              data: fillValues(maxData),
              borderColor: getColorByType(carType, false),
              fill: false,
              borderDash: [5, 5]
            }
          );
        }

        if (!chart.value) {
          renderChart(labels, datasets);
        } else {
          updateChart(labels, datasets);
        }
      };

      const renderChart = (labels: string[], datasets: any[]) => {
        const ctx = document.getElementById('priceChart') as HTMLCanvasElement;
        chart.value = new ChartJS(ctx, {
          type: 'line',
          data: {
            labels,
            datasets
          },
          options: {
            spanGaps: true,
            responsive: true,
            plugins: {
              title: {
                display: true,
                text: 'Минимальная и максимальная цена по типу вагона'
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
                  text: 'Цена (₽)'
                },
                min: 0
              }
            }
          }
        });
      };

      const updateChart = (labels: string[], datasets: any[]) => {
        if (chart.value) {
          chart.value.data.labels = labels;
          chart.value.data.datasets = datasets;
          chart.value.update();
        }
      };

      const getColorByType = (type: string, isMin: boolean) => {
        const baseColors: Record<string, string> = {
          'Compartment': '#4CAF50',
          'Luxury': '#FF9800',
          'ReservedSeat': '#2196F3',
          'Sedentary': '#9C27B0',
          'Soft': '#E91E63'
        };
        const base = baseColors[type] || '#000';
        return isMin ? base : base + 'CC'; // slightly transparent color for max
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

      const goBack = () => {
        window.history.back();
      };

      watch(selectedTypes, fetchPriceData, { deep: true });

      onMounted(() => {
        fetchPriceData();
      });

      return {
        trainId,
        carTypes,
        selectedTypes,
        getLabelByType,
        goBack
      };
    }
  });
</script>

<style scoped>
  .price-plot-container {
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
