<template>
  <div>
    <h2>Информация о поезде</h2>
    <div class="link-container">
      <!-- Ссылка на график с иконкой FontAwesome -->
      <router-link :to="{ name: 'FreePlacesPlot', params: { trainId: trainId }}" class="info-link">
        <font-awesome-icon icon="chair" class="icon" /> Посмотреть график свободных мест
      </router-link>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue';
  import { useRoute } from 'vue-router';
  import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
  import { library } from '@fortawesome/fontawesome-svg-core';
  import {
    faChair
  } from '@fortawesome/free-solid-svg-icons';

  library.add(
    faChair,
  );
  export default defineComponent({
    name: 'TrainDetails',
    components: {
      FontAwesomeIcon
    },
    setup() {
      const route = useRoute();
      const trainId = route.params.id; // Получаем trainId из маршрута

      return {
        trainId
      };
    }
  });
</script>

<style scoped>
  h2 {
    color: #4CAF50;
    font-family: Arial, sans-serif;
  }

  /* Стиль для блока с ссылкой */
  .link-container {
    margin-top: 20px;
    display: flex;
    justify-content: center;
  }

  /* Оформление самой ссылки */
  .info-link {
    display: inline-flex;
    align-items: center;
    padding: 10px 20px;
    background-color: #4CAF50;
    color: white;
    text-decoration: none;
    border-radius: 8px;
    font-size: 16px;
    transition: background-color 0.3s, transform 0.3s;
  }

    /* Иконка FontAwesome */
    .info-link i {
      margin-right: 8px; /* Отступ между иконкой и текстом */
    }
</style>
