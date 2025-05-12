<template>
  <div class="feedback-page">
    <h1>Обратная связь</h1>

    <!-- Форма обратной связи для обычных пользователей -->
    <div v-if="!isAdmin">
      <h2>Оставьте свой отзыв</h2>

      <!-- Показываем форму, если отзыв не отправлен -->
      <div v-if="!feedbackSent">
        <form @submit.prevent="submitFeedback">
          <div>
            <label for="feedback">Ваш отзыв:</label>
            <textarea id="feedback"
                      v-model="feedbackBody"
                      placeholder="Напишите ваш отзыв..."
                      rows="4"
                      required></textarea>
          </div>
          <button type="submit" class="submit-btn">Отправить</button>
        </form>
      </div>

      <!-- Сообщение об успешной отправке -->
      <div v-if="feedbackSent" class="success-message">
        Отзыв успешно отправлен!
      </div>

      <!-- Кнопка для отправки нового отзыва -->
      <div v-if="feedbackSent">
        <button @click="resetForm" class="reset-btn">Отправить еще</button>
      </div>
    </div>

    <!-- Список отзывов для администраторов -->
    <div v-if="isAdmin">
      <h2>Список отзывов</h2>
      <table class="feedback-table">
        <thead>
          <tr>
            <th>Пользователь</th>
            <th>Отзыв</th>
            <th>Дата создания</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(feedback, index) in feedbacks" :key="index">
            <td>{{ feedback.userEmail }}</td>
            <td>
              <textarea class="admin-feedback-textarea"
                        :value="feedback.body"
                        readonly
                        rows="4"></textarea>
            </td>
            <td>{{ formatDate(feedback.createdDate) }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, onMounted } from "vue";
  import { useAuthStore } from "@/stores/auth";
  import { storeToRefs } from "pinia";
  import api from "@/api/api";

  export default defineComponent({
    name: "FeedbackPage",
    setup() {
      const authStore = useAuthStore();
      const { isAuthenticated, isAdmin } = storeToRefs(authStore);
      const feedbackBody = ref("");
      const feedbacks = ref<Array<{ userEmail: string; body: string; createdDate: string }>>([]);
      const feedbackSent = ref(false); // Новый реф для отображения успешного сообщения

      // Загружаем все отзывы для администраторов
      const loadFeedbacks = async () => {
        if (isAdmin.value) {
          try {
            const response = await api.get("/api/feedback/all");
            feedbacks.value = response.data;
          } catch (error) {
            console.error("Ошибка при загрузке отзывов", error);
          }
        }
      };

      // Форматирование даты
      const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        return date.toLocaleDateString();
      };

      // Отправка отзыва
      const submitFeedback = async () => {
        if (feedbackBody.value.trim() === "") return;

        try {
          await api.post("/api/feedback/create", { body: feedbackBody.value });
          feedbackBody.value = ""; // Очищаем поле ввода после отправки
          feedbackSent.value = true; // Устанавливаем, что отзыв отправлен
        } catch (error) {
          console.error("Ошибка при отправке отзыва", error);
        }
      };

      // Сброс формы для отправки нового отзыва
      const resetForm = () => {
        feedbackSent.value = false; // Сбрасываем статус отправки
        feedbackBody.value = ""; // Очищаем текстовое поле
      };

      onMounted(() => {
        if (isAdmin.value) {
          loadFeedbacks();
        }
      });

      return {
        feedbackBody,
        feedbacks,
        isAdmin,
        formatDate,
        submitFeedback,
        feedbackSent, // Возвращаем переменную для успешного сообщения
        resetForm, // Функция для сброса формы
      };
    },
  });
</script>

<style scoped>
  .feedback-page {
    padding: 20px;
    max-width: 800px;
    margin: 0 auto;
  }

  h1 {
    text-align: center;
    font-size: 2rem;
    margin-bottom: 20px;
    color: #E60012; /* Красный цвет РЖД */
  }

  h2 {
    font-size: 1.5rem;
    margin-bottom: 10px;
    color: #E60012; /* Красный цвет РЖД */
  }

  form {
    display: flex;
    flex-direction: column;
    gap: 15px;
  }

  textarea {
    padding: 10px;
    font-size: 1rem;
    border: 1px solid #ddd;
    border-radius: 5px;
    width: 100%;
    resize: vertical;
  }

  button.submit-btn {
    background-color: #E60012; /* Красный цвет РЖД */
    color: white;
    padding: 10px 20px;
    font-size: 1rem;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s;
  }

    button.submit-btn:hover {
      background-color: #B20000; /* Темно-красный при наведении */
    }

  /* Кнопка для сброса формы */
  button.reset-btn {
    background-color: #E60012; /* Красный цвет РЖД */
    color: white;
    padding: 10px 20px;
    font-size: 1rem;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s;
  }

    button.reset-btn:hover {
      background-color: #B20000; /* Темно-красный при наведении */
    }

  .feedback-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
  }

    .feedback-table th,
    .feedback-table td {
      padding: 12px;
      text-align: left;
      border-bottom: 1px solid #ddd;
    }

    .feedback-table th {
      background-color: #f4f4f9;
      color: #333;
    }

    .feedback-table tr:nth-child(even) {
      background-color: #f9f9f9;
    }

    .feedback-table tr:hover {
      background-color: #f1f1f1;
    }

  /* Стиль для сообщения об успешной отправке */
  .success-message {
    margin-top: 15px;
    padding: 10px;
    background-color: #E60012; /* Красный цвет РЖД */
    color: white;
    border-radius: 5px;
    text-align: center;
  }

  /* Стиль для textarea в таблице для администраторов */
  .admin-feedback-textarea {
    width: 100%;
    resize: both; /* Разрешаем ресайз как по горизонтали, так и по вертикали */
    border: 1px solid #ddd;
    padding: 10px;
    font-size: 1rem;
    background-color: #f9f9f9;
    padding: 0 0 0 10px;
    color: #333;
    border-radius: 5px;
    max-width: 100%; /* Ограничиваем максимальную ширину */
    overflow: auto;
  }
</style>
