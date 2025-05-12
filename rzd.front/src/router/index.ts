import { createRouter, createWebHistory } from 'vue-router';
import HomePage from '@/pages/HomePage.vue';
import LoginPage from '@/pages/LoginPage.vue';
import RegisterPage from '@/pages/RegisterPage.vue';
import TrackedRoutesPage from '@/pages/TrackedRoutesPage.vue';
import FeedbackPage from '@/pages/FeedbackPage.vue';
import RouteStatisticPage from '@/pages/RouteStatisticPage.vue'
import TrainDetails from '@/pages/TrainDetails.vue'
import FreePlacesPlot from '@/pages/FreePlacesPlot.vue'
import PricePlot from '@/pages/PricePlot.vue'
import PickUpTrainPage from '@/pages/PickUpTrainPage.vue'
import UserSubscriptionsPage from '@/pages/UserSubscriptionsPage.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomePage,
  },
  {
    path: '/login',
    name: 'login',
    component: LoginPage,
  },
  {
    path: '/register',
    name: 'register',
    component: RegisterPage,
  },
  {
    path: '/trackedRoutes',
    name: 'trackedRoutes',
    component: TrackedRoutesPage, 
  },
  {
    path: '/feedback',
    name: 'feedback',
    component: FeedbackPage,
  },
  {
    path: '/route-statistic/:id',
    name: 'route-statistic',
    component: RouteStatisticPage,
  },
  {
    path: '/train-details/:id',
    name: 'train-details',
    component: TrainDetails,
  },
  {
    path: '/train/:trainId/free-places',
    name: 'FreePlacesPlot',
    component: FreePlacesPlot
  },
  {
    path: '/train/:trainId/price',
    name: 'PricePlot',
    component: PricePlot
  },
  {
    path: '/pickup',
    name: 'PickUpTrain',
    component: PickUpTrainPage
  },
  {
    path: '/subscriptions',
    name: 'Subscriptions',
    component: UserSubscriptionsPage
  }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.VITE_BASE_URL),
  routes,
});

export default router;
