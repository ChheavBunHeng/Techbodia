import { computed, ref } from "vue";
import api from "../../api/axios";
import { AuthResponse, type dto_login, type dto_register } from "./auth.type";

const userName = ref<string | null>(localStorage.getItem("userName"));
const userId = ref<number | null>(Number(localStorage.getItem("userId")) || null);

export function useAuthStore() {
  const isAuthenticated = computed(() => Boolean(userName.value));

  async function login(dto: dto_login) {
    const { data } = await api.post<AuthResponse>("/Auth/Login", dto);
    userId.value = data.userId;
    userName.value = data.userName ?? dto.userName;
    localStorage.setItem("userId", String(data.userId));
    localStorage.setItem("userName", userName.value);
  }

  async function register(dto: dto_register) {
    await api.post("/Auth/RegisterUser", dto);
  }
  function logout() {
    userName.value = null;
    userId.value = null;
    localStorage.removeItem("userId");
    localStorage.removeItem("userName");
  }

  return {
    userName,
    userId,
    isAuthenticated,
    login,
    register,
    logout,
  };
}
