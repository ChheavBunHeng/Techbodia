import { createRouter, createWebHistory } from "vue-router";
import notes from "../features/notes/notes.vue";
import auth from "../features/auth/auth.vue";
import { useAuthStore } from "../features/auth/useAuthStore";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      name: "notes",
      component: notes,
      meta: { requiresAuth: true },
    },
    {
      path: "/login",
      name: "login",
      component: auth,
      meta: { guestOnly: true },
    },
    {
      path: "/register",
      name: "register",
      component: auth,
      meta: { guestOnly: true },
    },
    {
      path: "/:pathMatch(.*)*",
      redirect: "/",
    },
  ],
});

router.beforeEach((to) => {
  const authStore = useAuthStore();

  if (to.meta.requiresAuth && !authStore.isAuthenticated.value) {
    return { name: "login" };
  }

  if (to.meta.guestOnly && authStore.isAuthenticated.value) {
    return { name: "notes" };
  }

  return true;
});

export default router;
