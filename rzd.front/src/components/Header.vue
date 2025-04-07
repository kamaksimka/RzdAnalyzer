<template>
  <header>
    <nav class="nav-bar">
      <ul class="left">
        <li><router-link to="/">Главная</router-link></li>
        <li><router-link to="/trackedRoutes">Список маршрутов</router-link></li>
      </ul>
      <ul class="right">
        <!-- Неавторизованный пользователь -->
        <template v-if="!isAuthenticated">
          <li>
            <router-link to="/login">
              <font-awesome-icon :icon="['fas', 'sign-in-alt']" class="icon" />
              Войти
            </router-link>
          </li>
          <li>
            <router-link to="/register">
              <font-awesome-icon :icon="['fas', 'user-plus']" class="icon" />
              Зарегистрироваться
            </router-link>
          </li>
        </template>

        <!-- Авторизованный пользователь -->
        <template v-if="isAuthenticated">
          <li>
            <router-link to="/feedback">
              <font-awesome-icon :icon="['fas', 'comments']" class="icon" />
              Обратная связь
            </router-link>
          </li>
          <li>
            <a href="#" @click.prevent="logout">
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
  import { defineComponent, onMounted } from 'vue';
  import { useAuthStore } from '@/stores/auth';
  import { storeToRefs } from 'pinia';

  import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
  import { library } from '@fortawesome/fontawesome-svg-core';
  import { faSignInAlt, faUserPlus, faSignOutAlt, faComments } from '@fortawesome/free-solid-svg-icons';

  library.add(faSignInAlt, faUserPlus, faSignOutAlt, faComments);  // Добавляем иконку для обратной связи

  export default defineComponent({
    name: 'Header',
    components: { FontAwesomeIcon },
    setup() {
      const authStore = useAuthStore();
      const { isAuthenticated } = storeToRefs(authStore);

      const logout = () => {
        authStore.logout();
        window.location.href = '/login';
      };

      onMounted(() => {
        authStore.checkToken();
      });

      return {
        isAuthenticated,
        logout,
      };
    },
  });
</script>

<style scoped>
  header {
    background-color: #282c34;
    padding: 10px 20px;
  }

  .nav-bar {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  ul {
    list-style-type: none;
    display: flex;
    gap: 15px;
    padding: 0;
    margin: 0;
  }

  nav a {
    display: flex;
    align-items: center;
    gap: 5px;
    color: white;
    text-decoration: none;
    padding: 8px 12px;
    border-radius: 6px;
    transition: background-color 0.3s;
  }

    nav a:hover {
      background-color: #4caf50;
    }

  .icon {
    font-size: 16px;
  }
</style>
