<template>
  <div>
    <h2>График цен</h2>
    <p>Поезд №{{ trainId }}</p>

    <div>
      <label v-for="type in carTypes" :key="type">
        <input type="checkbox" :value="type" v-model="selectedTypes" />
        {{ type }}
      </label>
    </div>

    <canvas id="priceChart"></canvas>
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
              label: `${carType} (min)`,
              data: fillValues(minData),
              borderColor: getColorByType(carType, true),
              fill: false
            },
            {
              label: `${carType} (max)`,
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
        return isMin ? base : base + 'CC'; // слегка прозрачный цвет для max
      };

      watch(selectedTypes, fetchPriceData, { deep: true });

      onMounted(() => {
        fetchPriceData();
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
