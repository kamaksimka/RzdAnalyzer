<template>
  <div>
    <h2>Добро пожаловать на главную страницу</h2>
    <p>Вы можете войти или зарегистрироваться через навигацию в хедере.</p>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue';

  export default defineComponent({
    name: 'HomePage',
  });
</script>

<style scoped>
  h2 {
    color: #4CAF50;
  }
</style>
