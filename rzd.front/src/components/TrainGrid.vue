<template>
  <div class="train-grid">
    <!-- Добавление выбора даты -->
    <div class="date-picker">
      <label for="date">Выберите дату:</label>
      <input type="date" id="date" v-model="selectedDate" @change="onDateChange" />
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
  import { defineComponent, ref, watch } from 'vue';
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
      const selectedDate = ref<string>(new Date().toISOString().split('T')[0]); // Инициализация с текущей датой

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

      // Данные строк
      const rowData = ref<any[]>([]);

      // Обработчик готовности сетки (Grid Ready)
      const onGridReady = async (params: any) => {
        // При первой загрузке данных для текущей даты
        await fetchTrainData();
      };

      // Функция для получения данных с сервера
      const fetchTrainData = async () => {
        try {
          const response = await TrainService.getTrainsByRouteIdAndDate(props.trackedRouteId, selectedDate.value);
          const data = await response.data;
          rowData.value = data;
           totalTrains.value = data.length; 
        } catch (error) {
          console.error('Ошибка при загрузке поездов:', error);
        }
      };

      // Обработчик изменения даты
      const onDateChange = () => {
        // При изменении даты обновляем данные
        fetchTrainData();
      };

      const convertMinutesToHours = (minutes: number) => {
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        return `${hours} ч ${mins} мин`;
      };

      // Для передачи trackedRouteId в контекст аг‑Grid (если необходимо)
      const gridContext = { trackedRouteId: 0 };

      return {
        columnDefs,
        defaultColDef,
        rowData,
        onGridReady,
        gridContext,
        frameworkComponents,
        selectedDate,
        onDateChange,
        convertMinutesToHours,
        totalTrains
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



</style>
