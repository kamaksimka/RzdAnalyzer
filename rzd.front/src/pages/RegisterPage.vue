<template>
  <div>
    <h2>Регистрация</h2>
    <form @submit.prevent="register">
      <div>
        <label for="email">Email:</label>
        <input type="email" id="email" v-model="email" required />
      </div>
      <div>
        <label for="password">Пароль:</label>
        <input type="password" id="password" v-model="password" required />
      </div>
      <button type="submit">Зарегистрироваться</button>
    </form>
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref } from 'vue';
  import api from '@/api/api.ts';
  import { useRouter } from 'vue-router';

  export default defineComponent({
    name: 'RegisterPage',
    setup() {
      const email = ref('');
      const password = ref('');
      const errorMessage = ref('');
      const router = useRouter();

     const register = async () => {
        try {
          await api.post('/api/auth/register', {
            email: email.value,
            password: password.value,
          });
          router.push('/login');
        } catch (error) {
          errorMessage.value = 'Ошибка регистрации. Попробуйте снова.';
        }
      };

      return {
        email,
        password,
        errorMessage,
        register,
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
