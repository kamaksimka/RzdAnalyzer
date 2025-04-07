// stores/auth.ts
import { defineStore } from 'pinia';
import jwt_decode from 'jwt-decode';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    accessToken: localStorage.getItem('accessToken') || '',
  }),
  getters: {
    isAuthenticated: (state) => !!state.accessToken,
    isAdmin: (state) => {
      if (!state.accessToken)
        return false

      const decodedToken: any = jwt_decode(state.accessToken);
      return decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']?.includes('Admin');
    }
  },
  actions: {
    login(token: string) {
      this.accessToken = token;
      localStorage.setItem('accessToken', token);
    },
    logout() {
      this.accessToken = '';
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
    },
    checkToken() {
      this.accessToken = localStorage.getItem('accessToken') || '';
    }
  },
});
