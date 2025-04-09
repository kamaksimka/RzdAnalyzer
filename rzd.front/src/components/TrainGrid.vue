<template>
  <div class="train-grid">
    <!-- Добавление выбора даты в строчку -->
    <div class="date-picker-container">
      <div class="date-picker">
        <label for="dateFrom">Выберите дату начала:</label>
        <input type="date" id="dateFrom" v-model="selectedDateFrom"
               :min="minDate" :max="maxDate" @change="onDateChange" />
      </div>

      <div class="date-picker">
        <label for="dateTo">Выберите дату конца:</label>
        <input type="date" id="dateTo" v-model="selectedDateTo"
               :min="minDate" :max="maxDate" @change="onDateChange" />
      </div>
    </div>

    <div class="total-trains">
      <strong>Всего найдено поездов:</strong> {{ totalTrains }}
    </div>

    <ag-grid-vue class="ag-theme-alpine"
                 style="width: 100%; height: 600px;"
                 :columnDefs="columnDefs"
                 :rowData="rowData"
                 :defaultColDef="defaultColDef"
                 :pagination="true"
                 :paginationPageSize="10"
                 :groupIncludeFooter="true"
                 :groupIncludeTotalFooter="true"
                 :animateRows="true"
                 :frameworkComponents="frameworkComponents"
                 @grid-ready="onGridReady"></ag-grid-vue>
  </div>
</template>


<script lang="ts">
  import { defineComponent, ref, onMounted } from 'vue';
  import { AgGridVue } from 'ag-grid-vue3';
  import { TrainService } from '@/services/trainService';
  import ServiceIconsRenderer from './ServiceIconsRenderer.vue';

  export default defineComponent({
    name: 'TrainGrid',
    components: {
      AgGridVue,
    },
    props: {
      trackedRouteId: {
        type: Number,
        required: true
      }
    },
    setup(props) {
      const selectedDateFrom = ref<string>(new Date().toISOString().split('T')[0]); // Инициализация с текущей датой
      const selectedDateTo = ref<string>(new Date().toISOString().split('T')[0]); // Инициализация с текущей датой

      const minDate = ref<string>('');
      const maxDate = ref<string>('');

      const frameworkComponents = {
        serviceIconsRenderer: ServiceIconsRenderer
      };
      const totalTrains = ref<number>(0); // Общее количество поездов
      // Определяем столбцы. Например:
      const columnDefs = ref([
        { field: 'trainNumber', headerName: '№ Поезда', sortable: true, filter: true },
        {
          field: 'departureDateTime',
          headerName: 'Отправление',
          sortable: true,
          filter: true,
          valueFormatter: (params: any) => new Date(params.value).toLocaleString('ru-RU')
        },
        {
          field: 'arrivalDateTime',
          headerName: 'Прибытие',
          sortable: true,
          filter: true,
          valueFormatter: (params: any) => new Date(params.value).toLocaleString('ru-RU')
        },
        {
          headerName: 'Услуги',
          field: 'carServices',
          cellRenderer: 'serviceIconsRenderer'
        },
        {
          field: 'tripDuration',
          headerName: 'Длительность',
          sortable: true,
          filter: true,
          valueFormatter: (params: any) => convertMinutesToHours(params.value) // Преобразуем минуты в часы
        },
        {
          field: 'minPrice',
          headerName: 'Минимальная цена',
          sortable: true,
          filter: true,
          valueFormatter: (params: any) => params.value?.toLocaleString('ru-RU', { style: 'currency', currency: 'RUB' })
        },
        {
          field: 'maxPrice',
          headerName: 'Максимальная цена',
          sortable: true,
          filter: true,
          valueFormatter: (params: any) => params.value?.toLocaleString('ru-RU', { style: 'currency', currency: 'RUB' })
        },
        {
          field: 'createdDate',
          headerName: 'Дата начала отслеживания',
          sortable: true,
          filter: true,
          valueFormatter: (params: any) => new Date(params.value).toLocaleDateString('ru-RU')
        },
      ]);



      // Общие настройки столбцов
      const defaultColDef = ref({
        resizable: true,
        filter: true,
        sortable: true,
        flex: 1,
        minWidth: 120,
      });

      const rowData = ref<any[]>([]);

      const onGridReady = async (params: any) => {
        await fetchTrainData();
      };

      const fetchTrainData = async () => {
        try {
          const response = await TrainService.getTrainsByRouteIdAndDate(props.trackedRouteId, selectedDateFrom.value, selectedDateTo.value);
          const data = await response.data;
          rowData.value = data;
          totalTrains.value = data.length;
        } catch (error) {
          console.error('Ошибка при загрузке поездов:', error);
        }
      };

      const fetchDateRange = async () => {
        try {
          const response = await TrainService.getTrainGridInitModel(props.trackedRouteId);
          minDate.value = response.data.minDate.split('T')[0];
          maxDate.value = response.data.maxDate.split('T')[0];
        } catch (error) {
          console.error('Ошибка при получении диапазона дат:', error);
        }
      };


      const onDateChange = () => {
        fetchTrainData();
      };

      const convertMinutesToHours = (minutes: number) => {
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        return `${hours} ч ${mins} мин`;
      };


      const gridContext = { trackedRouteId: 0 };

      onMounted(() => {
        fetchDateRange();
      });

      return {
        columnDefs,
        defaultColDef,
        rowData,
        onGridReady,
        gridContext,
        frameworkComponents,
        selectedDateFrom,
        selectedDateTo,
        onDateChange,
        convertMinutesToHours,
        totalTrains,
        minDate,
        maxDate
      };
    },
  });
</script>

<style scoped>
  /* Подключение темы ag‑Grid. Здесь используется класс alpine */
  .ag-theme-alpine {
    height: 600px; /* или нужная высота */
    width: 100%;
  }

  /* Стиль для блока выбора даты */
  .date-picker {
    margin-bottom: 20px;
  }

    .date-picker label {
      margin-right: 10px;
    }

    .date-picker input {
      padding: 5px;
      font-size: 16px;
    }

  .train-grid {
    margin: 20px;
    font-family: Arial, sans-serif;
  }

  /* Стили для блока выбора даты */
  .date-picker {
    display: flex;
    align-items: center;
    margin-bottom: 20px;
  }

    .date-picker label {
      font-size: 1rem;
      font-weight: bold;
      margin-right: 10px;
    }

    .date-picker input {
      padding: 8px;
      font-size: 1rem;
      border-radius: 5px;
      border: 1px solid #ccc;
      width: 200px;
      transition: border-color 0.3s;
    }

      .date-picker input:focus {
        border-color: #4CAF50;
        outline: none;
      }

  /* Стили для отображения количества найденных поездов */
  .total-trains {
    margin-bottom: 20px;
    font-size: 1.1rem;
    font-weight: bold;
    color: #333;
  }

  /* Стили для таблицы ag-Grid */
  .ag-theme-alpine {
    width: 100%;
    height: 600px;
    border-radius: 10px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }

  /* Стили для контейнера выбора даты */
  .date-picker-container {
    display: flex;
    gap: 20px; /* Отступ между полями */
    margin-bottom: 20px;
    align-items: center; /* Выравнивание по вертикали */
  }

  .date-picker label {
    margin-right: 10px; /* Отступ справа от метки */
  }

  .date-picker input {
    padding: 8px;
    font-size: 1rem;
    border-radius: 5px;
    border: 1px solid #ccc;
    width: 200px;
    transition: border-color 0.3s;
  }

    .date-picker input:focus {
      border-color: #4CAF50;
      outline: none;
    }

  .train-grid {
    margin: 20px;
    font-family: Arial, sans-serif;
  }
</style>
