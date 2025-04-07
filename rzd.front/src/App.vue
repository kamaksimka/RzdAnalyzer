<template>
  <div id="app">
    <Header />
    <main>
      <router-view></router-view>
    </main>
  </div>
</template>

  <script lang="ts">
    import { defineComponent } from 'vue';
    import Header from './components/Header.vue';

    export default defineComponent({
      name: 'App',
      components: {
        Header,
      },
    });
  </script>

  <style scoped>
    /* Общие стили для приложения */
    #app {
      font-family: 'Arial', sans-serif;
      display: flex;
      flex-direction: column;
      height: 100vh;
    }

    header {
      background-color: #282c34;
      padding: 10px;
    }

    nav ul {
      list-style-type: none;
      display: flex;
      justify-content: space-around;
    }

      nav ul li {
        margin: 0;
      }

    nav a {
      color: white;
      text-decoration: none;
      padding: 10px;
      border-radius: 5px;
    }

      nav a:hover {
        background-color: #4CAF50;
      }

    main {
      flex-grow: 1;
      padding: 20px;
    }
  </style>
