<template>
  <div class="login-container">
    <div class="form-card">
      <h2>Войти в систему</h2>
      <form @submit.prevent="login">
        <div class="form-group">
          <label for="email">Email</label>
          <input type="email" id="email" v-model="email" required />
        </div>
        <div class="form-group">
          <label for="password">Пароль</label>
          <input type="password" id="password" v-model="password" required />
        </div>
        <button type="submit">Войти</button>
      </form>
      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref } from 'vue';
  import api from '@/api/api.ts';
  import { useRouter } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';

  export default defineComponent({
    name: 'LoginPage',
    setup() {
      const email = ref('');
      const password = ref('');
      const errorMessage = ref('');
      const router = useRouter();
      const authStore = useAuthStore();

      const login = async () => {
        try {
          const response = await api.post('/api/auth/login', {
            email: email.value,
            password: password.value,
          });

          const accessToken = response.data.accessToken;
          const refreshToken = response.data.refreshToken;

          authStore.login(accessToken);
          localStorage.setItem('refreshToken', refreshToken);

          router.push('/');
        } catch (error) {
          errorMessage.value = 'Ошибка входа. Проверьте правильность данных.';
        }
      };

      return {
        email,
        password,
        errorMessage,
        login,
      };
    },
  });
</script>

<style scoped>
  /* Контейнер для всей страницы */
  .login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background: #f4f6f9;
    padding: 20px;
  }

  /* Карточка формы входа */
  .form-card {
    background-color: white;
    padding: 30px 40px;
    border-radius: 12px;
    box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
    max-width: 400px;
    width: 100%;
  }

  /* Заголовок формы */
  h2 {
    text-align: center;
    margin-bottom: 20px;
    color: #E60012; /* Красный цвет РЖД */
    font-size: 1.8rem;
    font-family: 'Arial', sans-serif;
  }

  /* Стили для каждого поля формы */
  .form-group {
    margin-bottom: 15px;
  }

  label {
    display: block;
    font-weight: 600;
    margin-bottom: 6px;
    color: #444;
  }

  input {
    width: 100%;
    padding: 12px;
    font-size: 1rem;
    border: 1px solid #ddd;
    border-radius: 6px;
    transition: border-color 0.3s ease;
    background-color: #fafafa;
  }

    input:focus {
      border-color: #E60012; /* Красный цвет РЖД */
      outline: none;
      background-color: #fff;
    }

  /* Кнопка входа */
  button {
    width: 100%;
    padding: 12px;
    background-color: #E60012; /* Красный цвет РЖД */
    color: white;
    font-size: 1rem;
    font-weight: 600;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    transition: background-color 0.3s ease, transform 0.2s ease;
  }

    button:hover {
      background-color: #B20000; /* Темно-красный при наведении */
      transform: scale(1.02);
    }

  /* Стили для сообщения об ошибке */
  .error {
    margin-top: 15px;
    color: #d32f2f; /* Красный для ошибок */
    text-align: center;
    font-weight: 500;
  }
</style>
