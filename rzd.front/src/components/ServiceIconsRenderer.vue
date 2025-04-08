<template>
  <div class="service-icons">
    <font-awesome-icon v-for="service in services"
                       :key="service"
                       :icon="getServiceIcon(service)"
                       class="service-icon"
                       :title="service"
                       style=" margin-right: 7px; font-size: 1.1rem;"/>
  </div>
</template>

<script lang="ts">
  import { defineComponent, computed } from 'vue';
  import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
  import { library } from '@fortawesome/fontawesome-svg-core';
  import {
    faUtensils,
    faConciergeBell,
    faTv,
    faBed,
    faQuestionCircle
  } from '@fortawesome/free-solid-svg-icons';


  library.add(
    faUtensils,
    faConciergeBell,
    faTv,
    faBed,
    faQuestionCircle
  );

  export default defineComponent({
    name: 'ServiceIconsRenderer',
    components: { FontAwesomeIcon },
    props: {
      params: {
        type: Object,
        required: true
      }
    },
    setup(props) {
      const services = computed(() => Array.isArray(props.params?.value) ? props.params.value : []);

      const getServiceIcon = (service: string) => {
        switch (service.toLowerCase()) {
          case 'meal':
            return 'utensils';
          case 'restaurantcarorbuffet':
            return 'concierge-bell';
          case 'infotainmentservice':
            return 'tv';
          case 'bedclothes':
            return 'bed';
          default:
            return 'question-circle';
        }
      };

      return {
        services,
        getServiceIcon
      };
    }
  });
</script>
