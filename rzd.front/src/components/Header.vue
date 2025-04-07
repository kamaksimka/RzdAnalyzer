<template>
  <header>
    <nav class="nav-bar">
      <ul class="left">
        <li><router-link to="/">Главная</router-link></li>
        <li><router-link to="/trackedRoutes">Список маршрутов</router-link></li>
      </ul>
      <ul class="right">
        <!-- Если пользователь не авторизован, показываем ссылки для входа и регистрации -->
        <li v-if="!isAuthenticated">
          <router-link to="/login" @click="setAuthenticated(true)">
            <font-awesome-icon :icon="['fas', 'sign-in-alt']" class="icon" />
            Войти
          </router-link>
        </li>
        <li v-if="!isAuthenticated">
          <router-link to="/register">
            <font-awesome-icon :icon="['fas', 'user-plus']" class="icon" />
            Зарегистрироваться
          </router-link>
        </li>

        <!-- Если пользователь авторизован, показываем кнопку выхода -->
        <li v-if="isAuthenticated">
          <a href="#" @click.prevent="logout">
            <font-awesome-icon :icon="['fas', 'sign-out-alt']" class="icon" />
            Выйти
          </a>
        </li>
      </ul>
    </nav>
  </header>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'
  import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
  import { library } from '@fortawesome/fontawesome-svg-core'
  import { faSignInAlt, faUserPlus, faSignOutAlt } from '@fortawesome/free-solid-svg-icons'

  // Регистрируем иконки
  library.add(faSignInAlt, faUserPlus, faSignOutAlt)

  export default defineComponent({
    name: 'Header',
    components: {
      FontAwesomeIcon,
    },
    data() {
      return {
        isAuthenticated: localStorage.getItem('accessToken') ? true : false, // Проверяем, есть ли токен при загрузке компонента
      }
    },
    methods: {
      // Метод для выхода
      logout() {
        localStorage.removeItem('accessToken'); // Удаляем токен
        this.isAuthenticated = false; // Обновляем состояние
        this.$router.push('/login'); // Перенаправляем на страницу входа
      },

      // Метод для установки авторизации (при входе)
      setAuthenticated(status: boolean) {
        this.isAuthenticated = status; // Обновляем состояние авторизации
      }
    }
  })
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
