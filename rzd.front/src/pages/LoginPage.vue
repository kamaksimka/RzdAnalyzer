<template>
  <div>
    <h2>Войти в систему</h2>
    <form @submit.prevent="login">
      <div>
        <label for="email">Email:</label>
        <input type="email" id="email" v-model="email" required />
      </div>
      <div>
        <label for="password">Пароль:</label>
        <input type="password" id="password" v-model="password" required />
      </div>
      <button type="submit">Войти</button>
    </form>
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref } from 'vue';
  import api from '@/api/api.ts';
  import { useRouter } from 'vue-router';

  export default defineComponent({
    name: 'LoginPage',
    setup() {
      const email = ref('');
      const password = ref('');
      const errorMessage = ref('');
      const router = useRouter();

      const login = async () => {
        try {
          const response = await api.post('/api/auth/login', {
            email: email.value,
            password: password.value,
          });
          localStorage.setItem('accessToken', response.data.accessToken);
          localStorage.setItem('refreshToken', response.data.refreshToken);
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
  .error {
    color: red;
  }

  form {
    display: flex;
    flex-direction: column;
  }

  button {
    margin-top: 10px;
    background-color: #4CAF50;
    color: white;
    padding: 10px;
    border: none;
    cursor: pointer;
  }

    button:hover {
      background-color: #45a049;
    }
</style>
