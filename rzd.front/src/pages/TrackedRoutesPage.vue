<template>
  <div>
    <h1>Список маршрутов</h1>

    <!-- Кнопка для добавления маршрута (только для админов) -->
    <button v-if="isAdmin" @click="showCreateForm = !showCreateForm">Создать маршрут</button>

    <!-- Форма для создания маршрута (только если isCreateForm активна) -->
    <div v-if="showCreateForm">
      <h2>Создание нового маршрута</h2>
      <form @submit.prevent="createRoute">
        <div>
          <label for="originExpressCode">Откуда (Express код):</label>
          <input type="text" v-model="newRoute.originExpressCode" required />
        </div>
        <div>
          <label for="destinationExpressCode">Куда (Express код):</label>
          <input type="text" v-model="newRoute.destinationExpressCode" required />
        </div>
        <button type="submit">Создать</button>
      </form>
    </div>

    <!-- Таблица маршрутов -->
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
        <tr v-for="route in routes" :key="route.id">
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
  import api from '@/api/api.ts';
  import jwt_decode from 'jwt-decode';

  export default {
    name: 'TrackedRoutesPage',
    data() {
      return {
        routes: [] as Array<{
          id: number;
          originName: string;
          originRegion: string;
          destinationName: string;
          destinationRegion: string;
          createdDate: string;
        }>,
        showCreateForm: false,
        newRoute: {
          originExpressCode: '',
          destinationExpressCode: ''
        },
        isAdmin: false, // Для проверки роли пользователя
      };
    },
    async created() {
      try {
        const response = await api.get('/api/trackedroute/all');
        this.routes = response.data; // Присваиваем данные маршрутов
      } catch (error) {
        console.error('Ошибка при получении данных: ', error);
      }

      // Проверка роли пользователя
      this.checkUserRole();
    },
    methods: {
      checkUserRole() {
        const token = localStorage.getItem('accessToken');
        if (token) {
          try {
            const decodedToken: any = jwt_decode(token);
            console.log(decodedToken);

            this.isAdmin = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']?.includes('Admin');
          } catch (error) {
            console.error('Ошибка при декодировании токена:', error);
          }
        }
      },
      async createRoute() {
        try {
          const response = await api.post('/api/trackedroute/create', this.newRoute);
          this.routes.push(response.data); // Добавляем новый маршрут в список
          this.showCreateForm = false; // Закрываем форму
          this.newRoute = { originExpressCode: '', destinationExpressCode: '' }; // Сбрасываем поля
        } catch (error) {
          console.error('Ошибка при создании маршрута: ', error);
        }
      },
      async deleteRoute(routeId: number) {
        try {
          console.log(routeId)
          await api.post('/api/trackedroute/delete', { TrackeRouteId: routeId });
          this.routes = this.routes.filter(route => route.id !== routeId); // Удаляем маршрут из списка
        } catch (error) {
          console.error('Ошибка при удалении маршрута: ', error);
        }
      },
      formatDate(dateString: string): string {
        const date = new Date(dateString);
        return date.toLocaleString();
      }
    }
  };
</script>

<style scoped>
  h1 {
    color: #282c34;
    font-size: 2rem;
    margin-bottom: 20px;
    text-align: center;
  }

  /* Кнопка для создания маршрута */
  button {
    background-color: #4CAF50;
    color: white;
    border: none;
    padding: 10px 20px;
    font-size: 1rem;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease, transform 0.2s ease;
  }

    button:hover {
      background-color: #45a049;
      transform: scale(1.05);
    }

    /* Кнопка удаления маршрута */
    button.delete {
      background-color: #f44336;
      color: white;
      border: none;
      padding: 8px 16px;
      font-size: 1rem;
      border-radius: 5px;
      cursor: pointer;
      transition: background-color 0.3s ease, transform 0.2s ease;
    }

      button.delete:hover {
        background-color: #d32f2f;
        transform: scale(1.05);
      }

  /* Стили для таблицы */
  table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 30px;
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }

  th,
  td {
    padding: 12px 15px;
    text-align: left;
    font-size: 1rem;
  }

  th {
    background-color: #f4f4f9;
    font-weight: bold;
    color: #333;
    text-transform: uppercase;
  }

  td {
    background-color: #fff;
    border-bottom: 1px solid #ddd;
    color: #555;
  }

  tr:nth-child(even) td {
    background-color: #f9f9f9;
  }

  tr:hover td {
    background-color: #f1f1f1;
  }

  /* Форма для создания маршрута */
  .form-container {
    margin-top: 30px;
    background-color: #f9f9f9;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
    max-width: 500px;
    margin: 0 auto;
  }

  form div {
    margin-bottom: 15px;
  }

  label {
    font-size: 1rem;
    font-weight: 600;
    color: #333;
    display: block;
    margin-bottom: 5px;
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
      border-color: #4CAF50;
      outline: none;
    }

  button[type='submit'] {
    background-color: #4CAF50;
    color: white;
    padding: 12px 20px;
    font-size: 1rem;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    width: 100%;
    transition: background-color 0.3s ease;
  }

    button[type='submit']:hover {
      background-color: #45a049;
    }
</style>

