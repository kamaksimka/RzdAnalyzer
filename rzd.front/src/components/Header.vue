<template>
  <header>
    <nav class="nav-bar">
      <!-- Кнопка Назад -->
      <ul class="left">
        <li>
          <button v-if="showBackButton" @click="goBack" class="back-button">
            <font-awesome-icon :icon="['fas', 'arrow-left']" class="icon" />
            Назад
          </button>
        </li>
        <li><router-link to="/" class="nav-link">Главная</router-link></li>
        <li><router-link to="/trackedRoutes" class="nav-link">Список маршрутов</router-link></li>
      </ul>

      <ul class="right">
        <template v-if="!isAuthenticated">
          <li>
            <router-link to="/login" class="nav-link">
              <font-awesome-icon :icon="['fas', 'sign-in-alt']" class="icon" />
              Войти
            </router-link>
          </li>
          <li>
            <router-link to="/register" class="nav-link">
              <font-awesome-icon :icon="['fas', 'user-plus']" class="icon" />
              Зарегистрироваться
            </router-link>
          </li>
        </template>

        <template v-if="isAuthenticated">
          <!-- Ссылка на страницу подписок -->
          <li>
            <router-link to="/subscriptions" class="nav-link">
              <font-awesome-icon :icon="['fas', 'bell']" class="icon" />
              Мои подписки
            </router-link>
          </li>

          <li>
            <router-link to="/feedback" class="nav-link">
              <font-awesome-icon :icon="['fas', 'comments']" class="icon" />
              Обратная связь
            </router-link>
          </li>
          <li>
            <a href="#" @click.prevent="logout" class="nav-link">
              <font-awesome-icon :icon="['fas', 'sign-out-alt']" class="icon" />
              Выйти
            </a>
          </li>
        </template>
      </ul>
    </nav>
  </header>
</template>

<script lang="ts">
  import { defineComponent, onMounted, computed } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';
  import { storeToRefs } from 'pinia';

  import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
  import { library } from '@fortawesome/fontawesome-svg-core';
  import {
    faSignInAlt,
    faUserPlus,
    faSignOutAlt,
    faComments,
    faArrowLeft,
    faBell,
  } from '@fortawesome/free-solid-svg-icons';

  library.add(faSignInAlt, faUserPlus, faSignOutAlt, faComments, faArrowLeft, faBell);

  export default defineComponent({
    name: 'Header',
    components: { FontAwesomeIcon },
    setup() {
      const authStore = useAuthStore();
      const { isAuthenticated } = storeToRefs(authStore);
      const route = useRoute();
      const router = useRouter();

      const logout = () => {
        authStore.logout();
        window.location.href = '/login';
      };

      const goBack = () => {
        window.history.back(); // Используем window.history.back() для возврата
      };

      // Показывать кнопку "Назад", если не главная страница и можно вернуться назад
      const showBackButton = computed(() => {
        const hasHistory = window.history.length > 1; // Проверяем длину истории браузера
        return route.path !== '/' && hasHistory;
      });

      onMounted(() => {
        authStore.checkToken();
      });

      return {
        isAuthenticated,
        logout,
        goBack,
        showBackButton
      };
    },
  });
</script>

<style scoped>
  /* Общие стили для header */
  header {
    background-color: #e60012; /* Красный цвет в стиле РЖД */
    padding: 10px 20px;
  }

  .nav-bar {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  /* Стили для навигации */
  ul {
    list-style-type: none;
    display: flex;
    gap: 20px;
    padding: 0;
    margin: 0;
  }

  /* Ссылки навигации */
  .nav-link {
    display: flex;
    align-items: center;
    gap: 5px;
    color: white;
    text-decoration: none;
    padding: 8px 12px;
    border-radius: 6px;
    font-weight: 600;
    transition: background-color 0.3s ease;
  }

    .nav-link:hover {
      background-color: #b20000; /* Темный красный при наведении */
    }

  .icon {
    font-size: 18px;
  }

  /* Кнопка назад */
  .back-button {
    background-color: transparent;
    color: white;
    border: none;
    cursor: pointer;
    font-size: 16px;
    margin-right: 15px;
    display: flex;
    align-items: center;
    gap: 5px;
    padding: 8px 12px;
    border-radius: 6px;
    transition: background-color 0.3s ease;
  }

    .back-button:hover {
      background-color: #b20000; /* Темный красный при наведении */
    }
</style>
