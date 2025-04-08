<template>
  <div>
    <vue-good-table :columns="columns"
                    :rows="rows"
                    :pagination="pagination"
                    :sort-options="sortOptions"
                    :searchable="true"
                    :global-search="true" />
  </div>
</template>

<script>
  import { VueGoodTable } from 'vue-good-table-next'; // Импортируем компонент
  import 'vue-good-table-next/dist/vue-good-table-next.css'; // Импортируем стили

  export default {
    components: {
      VueGoodTable
    },
    data() {
      return {
        columns: [
          {
            label: 'ID',
            field: 'id',
            sortable: true
          },
          {
            label: 'Имя',
            field: 'name',
            sortable: true
          },
          {
            label: 'Возраст',
            field: 'age',
            sortable: true
          },
          {
            label: 'Город',
            field: 'city',
            sortable: true
          }
        ],
        rows: [
          { id: 1, name: 'Иван', age: 28, city: 'Москва' },
          { id: 2, name: 'Анна', age: 34, city: 'Санкт-Петербург' },
          { id: 3, name: 'Петр', age: 25, city: 'Новосибирск' },
          { id: 4, name: 'Мария', age: 30, city: 'Екатеринбург' }
        ],
        pagination: {
          currentPage: 1,
          perPage: 5
        },
        sortOptions: {
          enabled: true,
          initialSort: {
            field: 'id',
            type: 'asc'
          }
        }
      };
    }
  };
</script>

<style scoped>
  /* Можете добавить свои стили */
</style>
