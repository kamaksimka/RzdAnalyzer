<template>
  <div>
    <h1>Список маршрутов</h1>

    <!-- Кнопка для создания маршрута (только для админов) -->
    <button v-if="isAdmin" @click="showCreateForm = !showCreateForm">Создать маршрут</button>

    <!-- Форма для создания маршрута (только если showCreateForm активна) -->
    <div v-if="showCreateForm">
      <h2>Создание нового маршрута</h2>
      <form @submit.prevent="createRoute">
        <div>
          <label for="originExpressCode">Откуда:</label>
          <input type="text"
                 v-model="originQuery"
                 @input="fetchOriginSuggestions"
                 @focus="showOriginSuggestions = true"
                 @blur="hideSuggestionsWithDelay('origin')" />
          <ul v-if="showOriginSuggestions && originSuggestions.length" class="suggestions-list">
            <li v-for="station in originSuggestions" :key="station.expressCode" @mousedown.prevent="selectOrigin(station)">
              {{ station.name }} ({{ station.region }})
            </li>
          </ul>
        </div>

        <div>
          <label for="destinationExpressCode">Куда:</label>
          <input type="text"
                 v-model="destinationQuery"
                 @input="fetchDestinationSuggestions"
                 @focus="showDestinationSuggestions = true"
                 @blur="hideSuggestionsWithDelay('destination')" />
          <ul v-if="showDestinationSuggestions && destinationSuggestions.length" class="suggestions-list">
            <li v-for="station in destinationSuggestions" :key="station.expressCode" @mousedown.prevent="selectDestination(station)">
              {{ station.name }} ({{ station.region }})
            </li>
          </ul>
        </div>

        <button type="submit">Создать</button>
      </form>
    </div>

    <table>
      <thead>
        <tr>
          <th>Откуда</th>
          <th>Регион (Откуда)</th>
          <th>Куда</th>
          <th>Регион (Куда)</th>
          <th>Дата создания</th>
          <th v-if="isAdmin">Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="route in routes" :key="route.id" @click="goToRouteStatistic(route.id)" class="route-row">
          <td>{{ route.originName }}</td>
          <td>{{ route.originRegion }}</td>
          <td>{{ route.destinationName }}</td>
          <td>{{ route.destinationRegion }}</td>
          <td>{{ formatDate(route.createdDate) }}</td>
          <td v-if="isAdmin">
            <button @click="deleteRoute(route.id)">Удалить</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, onMounted, computed } from 'vue';
  import { useAuthStore } from '@/stores/auth';
  import { TrackedRouteService } from '@/services/trackedRouteService';
  import { useRouter } from 'vue-router';

  export default defineComponent({
    name: 'TrackedRoutesPage',
    setup() {
      const authStore = useAuthStore();
      const router = useRouter();

      const routes = ref<Array<{
        id: number;
        originName: string;
        originRegion: string;
        destinationName: string;
        destinationRegion: string;
        createdDate: string;
      }>>([]);

      const showCreateForm = ref(false);
      const originQuery = ref('');
      const destinationQuery = ref('');
      const originSuggestions = ref<any[]>([]);
      const destinationSuggestions = ref<any[]>([]);
      const showOriginSuggestions = ref(false);
      const showDestinationSuggestions = ref(false);

      const newRoute = ref({
        originExpressCode: '',
        destinationExpressCode: ''
      });

      const isAdmin = computed(() => authStore.isAdmin);

      const formatDate = (dateString: string): string => {
        const date = new Date(dateString);
        return date.toLocaleDateString();
      };

      const loadRoutes = async () => {
        const response = await TrackedRouteService.getAllRoutes();
        routes.value = response.data;
      };

      const fetchOriginSuggestions = async () => {
        const res = await TrackedRouteService.getSuggestions(originQuery.value);
        originSuggestions.value = res.data;
      };

      const fetchDestinationSuggestions = async () => {
        const res = await TrackedRouteService.getSuggestions(destinationQuery.value);
        destinationSuggestions.value = res.data;
      };

      const selectOrigin = (station: any) => {
        originQuery.value = `${station.name} (${station.region})`;
        newRoute.value.originExpressCode = station.expressCode;
        originSuggestions.value = [];
        showOriginSuggestions.value = false;
      };

      const selectDestination = (station: any) => {
        destinationQuery.value = `${station.name} (${station.region})`;
        newRoute.value.destinationExpressCode = station.expressCode;
        destinationSuggestions.value = [];
        showDestinationSuggestions.value = false;
      };

      const hideSuggestionsWithDelay = (type: 'origin' | 'destination') => {
        setTimeout(() => {
          if (type === 'origin') showOriginSuggestions.value = false;
          else showDestinationSuggestions.value = false;
        }, 100);
      };

      const createRoute = async () => {
        const response = await TrackedRouteService.createRoute(newRoute.value);
        if (response) {
          routes.value.push(response.data);
          showCreateForm.value = false;
          newRoute.value = { originExpressCode: '', destinationExpressCode: '' };
          originQuery.value = '';
          destinationQuery.value = '';
          await loadRoutes();
        }
      };

      const deleteRoute = async (routeId: number) => {
        const success = await TrackedRouteService.deleteRoute(routeId);
        if (success) {
          routes.value = routes.value.filter(route => route.id !== routeId);
        }
      };

      const goToRouteStatistic = (routeId: number) => {
        router.push({ name: 'route-statistic', params: { id: routeId } });
      };

      onMounted(() => {
        loadRoutes();
      });

      return {
        routes,
        showCreateForm,
        originQuery,
        destinationQuery,
        originSuggestions,
        destinationSuggestions,
        showOriginSuggestions,
        showDestinationSuggestions,
        goToRouteStatistic,
        newRoute,
        isAdmin,
        formatDate,
        fetchOriginSuggestions,
        fetchDestinationSuggestions,
        selectOrigin,
        selectDestination,
        hideSuggestionsWithDelay,
        createRoute,
        deleteRoute
      };
    }
  });
</script>

<style scoped>
  /* Основные стили */
  h1 {
    color: #E60012; /* Красный цвет РЖД */
    font-size: 2.2rem;
    margin-bottom: 20px;
    text-align: center;
    font-family: 'Arial', sans-serif;
  }

  button {
    background-color: #E60012; /* Красный цвет для кнопок */
    color: white;
    border: none;
    padding: 10px 20px;
    font-size: 1rem;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease, transform 0.2s ease;
  }

    button:hover {
      background-color: #B20000; /* Темно-красный при наведении */
      transform: scale(1.05);
    }

    button.delete {
      background-color: #D32F2F;
      color: white;
    }

      button.delete:hover {
        background-color: #B71C1C;
      }

  table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 30px;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    font-family: 'Arial', sans-serif;
  }

  th, td {
    padding: 12px 15px;
    text-align: left;
    font-size: 1rem;
    color: #333;
  }

  th {
    background-color: #f1f1f1;
    font-weight: bold;
    text-transform: uppercase;
  }

  td {
    background-color: #fff;
    border-bottom: 1px solid #ddd;
  }

  tr:nth-child(even) td {
    background-color: #f9f9f9;
  }

  tr:hover td {
    background-color: #f1f1f1;
  }

  /* Формы */
  .form-container {
    margin-top: 30px;
    background-color: #f9f9f9;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
    max-width: 500px;
    margin: 0 auto;
  }

  input {
    width: 100%;
    padding: 10px;
    font-size: 1rem;
    border: 1px solid #ddd;
    border-radius: 5px;
    background-color: #fff;
    transition: border-color 0.3s ease;
  }

    input:focus {
      border-color: #E60012;
      outline: none;
    }

  .suggestions-list {
    position: absolute;
    background-color: white;
    border: 1px solid #ddd;
    border-radius: 5px;
    width: 100%;
    max-height: 150px;
    overflow-y: auto;
    z-index: 10;
    list-style: none;
    padding: 0;
    margin-top: 2px;
  }

    .suggestions-list li {
      padding: 10px;
      cursor: pointer;
    }

      .suggestions-list li:hover {
        background-color: #f1f1f1;
      }

  .route-row {
    cursor: pointer;
  }
</style>
