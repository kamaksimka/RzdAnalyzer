<template>
  <div class="train-grid">
    <div class="filters-container">
      <!-- Выбор маршрута -->
      <div class="filter-row">
        <label>Маршрут:</label>
        <select v-model="trackedRouteId">
          <option disabled value="">Выберите маршрут</option>
          <option v-for="route in routes" :key="route.id" :value="route.id">
            {{ route.originName }} - {{ route.destinationName }}
          </option>
        </select>
      </div>

      <!-- Фильтры времени -->
      <div class="filter-row">
        <label>Прибытие с:</label>
        <input type="datetime-local" v-model="startArrivalTime" />
        <label>по:</label>
        <input type="datetime-local" v-model="endArrivalTime" />
      </div>

      <div class="filter-row">
        <label>Отправление с:</label>
        <input type="datetime-local" v-model="startDepartureTime" />
        <label>по:</label>
        <input type="datetime-local" v-model="endDepartureTime" />
      </div>

      <!-- Время в пути -->
      <div class="filter-row">
        <label>Время в пути (ДД:ЧЧ:ММ):</label>
        <input type="text" v-model="travelTimeText" placeholder="01:02:30" />
      </div>

      <!-- Типы вагонов -->
      <div class="filter-row">
        <label>Тип вагона:</label>
        <select v-model="carTypes" multiple>
          <option value="Compartment">Купе</option>
          <option value="Luxury">Люкс</option>
          <option value="ReservedSeat">Плацкарт</option>
          <option value="Sedentary">Сидячий</option>
          <option value="Soft">Места для инвалидов</option>
        </select>
      </div>

      <!-- Услуги -->
      <div class="filter-row">
        <label>Услуги:</label>
        <select v-model="carServices" multiple>
          <option value="Meal">Питание</option>
          <option value="RestaurantCarOrBuffet">Ресторан / Буфет</option>
          <option value="InfotainmentService">Инфоразвлечения</option>
          <option value="BedClothes">Постельные принадлежности</option>
        </select>
      </div>

      <!-- Цены -->
      <div class="filter-row">
        <label>Мин. цена:</label>
        <input type="number" v-model.number="minPrice" />
        <label>Макс. цена:</label>
        <input type="number" v-model.number="maxPrice" />
      </div>

      <!-- Места -->
      <div class="filter-row">
        <label><input type="checkbox" v-model="isUpperSeat" /> Верхние места</label>
        <label><input type="checkbox" v-model="isLowerSeat" /> Нижние места</label>
        <label><input type="checkbox" v-model="isAnySeat" /> Любое место</label>
      </div>

      <!-- Кнопка -->
      <div class="filter-row">
        <button @click="fetchTrains">Подобрать поезда</button>
      </div>

      <!-- Результат -->
      <div class="total-trains">
        <strong>Всего найдено поездов:</strong> {{ totalTrains }}
      </div>
    </div>

    <!-- Таблица или кнопка подписки -->
    <div class="table-container">
      <div v-if="totalTrains > 0">
        <ag-grid-vue class="ag-theme-alpine"
                     style="width: 100%; height: 600px"
                     :columnDefs="columnDefs"
                     :rowData="rowData"
                     :defaultColDef="defaultColDef"
                     :pagination="true"
                     :paginationPageSize="10"
                     :animateRows="true"
                     @grid-ready="onGridReady"
                     :components="{ ServiceIconsRenderer }" />
      </div>
      <div v-else class="no-trains">
        <p>Поездов не найдено по заданным фильтрам.</p>
        <button @click="subscribeToTrains" class="subscribe-button">
          Подписаться на уведомление
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, onMounted } from 'vue';
  import { AgGridVue } from 'ag-grid-vue3';
  import { TrainService } from '@/services/trainService';
  import { TrackedRouteService } from '@/services/trackedRouteService';
  import ServiceIconsRenderer from '@/components/ServiceIconsRenderer.vue';
  import axios from 'axios';

  export default defineComponent({
    name: 'TrainSearchPage',
    components: {
      AgGridVue,
      ServiceIconsRenderer,
    },
    setup() {
      const trackedRouteId = ref<number | null>(null);
      const startArrivalTime = ref('');
      const endArrivalTime = ref('');
      const startDepartureTime = ref('');
      const endDepartureTime = ref('');
      const travelTimeText = ref('');
      const travelTimeInMinutes = ref<number | null>(null);
      const carTypes = ref<string[]>([]);
      const carServices = ref<string[]>([]);
      const isUpperSeat = ref(false);
      const isLowerSeat = ref(false);
      const isAnySeat = ref(false);
      const minPrice = ref<number | null>(null);
      const maxPrice = ref<number | null>(null);
      const routes = ref<any[]>([]);
      const rowData = ref<any[]>([]);
      const totalTrains = ref<number>(0);

      const columnDefs = ref([
        { field: 'trainNumber', headerName: '№ Поезда' },
        {
          field: 'departureDateTime',
          headerName: 'Отправление',
          valueFormatter: (p: any) => new Date(p.value).toLocaleString('ru-RU'),
        },
        {
          field: 'arrivalDateTime',
          headerName: 'Прибытие',
          valueFormatter: (p: any) => new Date(p.value).toLocaleString('ru-RU'),
        },
        {
          field: 'tripDuration',
          headerName: 'Длительность',
          valueFormatter: (p: any) => convertMinutesToHours(p.value),
        },
        {
          field: 'minPrice',
          headerName: 'Мин. цена',
          valueFormatter: (p: any) =>
            p.value?.toLocaleString('ru-RU', {
              style: 'currency',
              currency: 'RUB',
            }),
        },
        {
          field: 'maxPrice',
          headerName: 'Макс. цена',
          valueFormatter: (p: any) =>
            p.value?.toLocaleString('ru-RU', {
              style: 'currency',
              currency: 'RUB',
            }),
        },
        {
          field: 'carServices',
          headerName: 'Услуги',
          cellRenderer: 'ServiceIconsRenderer',
        },
      ]);

      const defaultColDef = ref({
        resizable: true,
        sortable: true,
        filter: true,
        flex: 1,
      });

      const fetchRoutes = async () => {
        try {
          const data = await TrackedRouteService.getAllRoutes();
          routes.value = data.data;
        } catch (e) {
          console.error('Ошибка загрузки маршрутов:', e);
        }
      };

      const buildRequest = () => {
        const match = travelTimeText.value.trim().match(/^(\d{1,2}):(\d{1,2}):(\d{1,2})$/);
        if (match) {
          const [_, d, h, m] = match.map(Number);
          travelTimeInMinutes.value = d * 1440 + h * 60 + m;
        } else {
          travelTimeInMinutes.value = null;
        }

        return {
          trackedRouteId: trackedRouteId.value!,
          startArrivalTime: startArrivalTime.value ? new Date(startArrivalTime.value) : null,
          endArrivalTime: endArrivalTime.value ? new Date(endArrivalTime.value) : null,
          startDepartureTime: startDepartureTime.value ? new Date(startDepartureTime.value) : null,
          endDepartureTime: endDepartureTime.value ? new Date(endDepartureTime.value) : null,
          carTypes: carTypes.value,
          carServices: carServices.value,
          isUpperSeat: isUpperSeat.value,
          isLowerSeat: isLowerSeat.value,
          isAnySeat: isAnySeat.value,
          travelTimeInMinutes: travelTimeInMinutes.value,
          minPrice: minPrice.value,
          maxPrice: maxPrice.value,
        };
      };

      const fetchTrains = async () => {
        const request = buildRequest();
        try {
          const res = await TrainService.pickUpTrain(request);
          rowData.value = res.data;
          totalTrains.value = res.data.length;
        } catch (e) {
          console.error('Ошибка при подборе поездов:', e);
        }
      };

      const subscribeToTrains = async () => {
        const request = buildRequest();
        try {
          await TrainService.subscribeToTrains(request);
          alert('Вы успешно подписались на уведомление!');
        } catch (e) {
          console.error('Ошибка при подписке:', e);
          alert('Не удалось подписаться. Попробуйте позже.');
        }
      };

      const convertMinutesToHours = (minutes: number) => {
        const h = Math.floor(minutes / 60);
        const m = minutes % 60;
        return `${h} ч ${m} мин`;
      };

      const onGridReady = () => { };

      onMounted(fetchRoutes);

      return {
        trackedRouteId,
        startArrivalTime,
        endArrivalTime,
        startDepartureTime,
        endDepartureTime,
        travelTimeText,
        carTypes,
        carServices,
        isUpperSeat,
        isLowerSeat,
        isAnySeat,
        minPrice,
        maxPrice,
        routes,
        fetchTrains,
        columnDefs,
        defaultColDef,
        rowData,
        totalTrains,
        onGridReady,
        subscribeToTrains,
      };
    },
  });
</script>

<style scoped>
  .train-grid {
    font-family: Arial, sans-serif;
    background-color: #FFFFFF;
    padding: 24px;
    margin: 20px auto;
    max-width: 90%;
    border: 1px solid #E0E0E0;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
    color: #2D2D2D;
    display: flex;
    gap: 32px;
  }

  .filters-container {
    flex-basis: 40%;
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  .filter-row {
    display: flex;
    flex-wrap: wrap;
    gap: 12px;
    align-items: center;
  }

    .filter-row label {
      font-weight: bold;
      font-size: 15px;
      color: #2D2D2D;
    }

    .filter-row input,
    .filter-row select {
      padding: 8px 12px;
      font-size: 14px;
      border: 1px solid #CCC;
      border-radius: 8px;
      background-color: #F9F9F9;
      transition: border-color 0.2s;
    }

      .filter-row input:focus,
      .filter-row select:focus {
        border-color: #D52B1E;
        outline: none;
      }

  button {
    padding: 12px 20px;
    background-color: #D52B1E;
    color: white;
    font-weight: bold;
    font-size: 15px;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    transition: background-color 0.3s ease, transform 0.2s ease;
  }

    button:hover {
      background-color: #BB1E16;
      transform: scale(1.02);
    }

  .total-trains {
    font-size: 16px;
    font-weight: bold;
    color: #000;
    margin-top: 8px;
  }

  .table-container {
    flex-basis: 60%;
  }

  .no-trains {
    padding: 40px;
    text-align: center;
    font-size: 1.1rem;
    background-color: #fdfdfd;
    border: 1px dashed #ccc;
    border-radius: 12px;
    color: #333;
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);
  }

  .subscribe-button {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    margin-top: 20px;
    padding: 12px 20px;
    font-size: 15px;
    font-weight: 500;
    background-color: #D52B1E;
    color: #FFFFFF;
    border: none;
    border-radius: 10px;
    text-decoration: none;
    box-shadow: 0 2px 4px rgba(213, 43, 30, 0.25);
    cursor: pointer;
    transition: background-color 0.3s ease, transform 0.3s ease;
  }

    .subscribe-button:hover {
      background-color: #BB1E16;
      transform: scale(1.05);
    }
</style>
