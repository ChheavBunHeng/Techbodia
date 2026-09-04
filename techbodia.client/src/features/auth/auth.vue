<script setup lang="ts">
import { computed, ref } from "vue";
import axios from "axios";
import { useRoute, useRouter } from "vue-router";
import { useAuthStore } from "./useAuthStore";

const authStore = useAuthStore();
const route = useRoute();
const router = useRouter();
const isRegister = computed(() => route.name === "register");
const userName = ref("");
const email = ref("");
const password = ref("");
const confirmPassword = ref("");
const isSubmitting = ref(false);
const errorMessage = ref("");
const successMessage = ref("");

const title = computed(() => (isRegister.value ? "Create your workspace" : "Welcome back"));
const submitLabel = computed(() => (isRegister.value ? "Create account" : "Sign in"));

function switchMode(register: boolean) {
  errorMessage.value = "";
  successMessage.value = "";
  router.push({ name: register ? "register" : "login" });
}

function getErrorMessage(error: unknown) {
  if (axios.isAxiosError(error)) {
    if (!error.response) {
      return "The server could not be reached. Make sure the API is running and try again.";
    }
    return error.response.data?.message ?? "We could not complete that request. Please try again.";
  }
  return "We could not complete that request. Please try again.";
}

async function submit() {
  errorMessage.value = "";
  successMessage.value = "";

  if (isRegister.value && password.value !== confirmPassword.value) {
    errorMessage.value = "Passwords do not match.";
    return;
  }

  isSubmitting.value = true;
  try {
    if (isRegister.value) {
      await authStore.register({
        userName: userName.value,
        email: email.value,
        userPassword: password.value,
      });
      await router.push({ name: "login", query: { registered: "1" } });
      successMessage.value = "Account created. Sign in to continue.";
    } else {
      await authStore.login({ userName: userName.value, userPassword: password.value });
      await router.push({ name: "notes" });
    }
  } catch (error) {
    errorMessage.value = getErrorMessage(error);
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<template>
  <main class="min-h-screen bg-[#f4f7f2] text-slate-900 lg:grid lg:grid-cols-[1.05fr_0.95fr]">
    <section
      class="relative hidden overflow-hidden bg-[#173f35] p-12 text-[#f4f7f2] lg:flex lg:flex-col lg:justify-between"
    >
      <div class="relative">
        <p class="text-sm font-semibold uppercase tracking-[0.28em] text-[#d4e36c]">Techbodia</p>
        <h1 class="mt-24 max-w-xl text-6xl font-black leading-[0.95] tracking-tight">
          A clearer place for your best thinking.
        </h1>
      </div>
      <p class="relative max-w-sm text-sm leading-6 text-[#c8d8cf]">
        Capture ideas, keep projects moving, and return to the work that matters.
      </p>
    </section>

    <section class="flex min-h-screen items-center justify-center px-6 py-12 sm:px-10">
      <div class="w-full max-w-md">
        <div class="mb-10 lg:hidden">
          <p class="text-sm font-semibold uppercase tracking-[0.28em] text-[#28614d]">Techbodia</p>
        </div>
        <p class="mb-3 text-sm font-semibold uppercase tracking-[0.2em] text-[#e36d48]">
          Your workspace
        </p>
        <h2 class="text-4xl font-black tracking-tight text-[#173f35]">{{ title }}</h2>
        <p class="mt-3 text-slate-500">
          {{
            isRegister
              ? "Set up your account in a moment."
              : "Sign in to pick up where you left off."
          }}
        </p>

        <form class="mt-8 space-y-5" @submit.prevent="submit">
          <div>
            <label for="userName" class="mb-2 block text-sm font-semibold text-slate-700"
              >Username</label
            >
            <input
              id="userName"
              v-model="userName"
              required
              autocomplete="username"
              class="w-full rounded-xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-[#28614d] focus:ring-4 focus:ring-[#28614d]/10"
            />
          </div>
          <div v-if="isRegister">
            <label for="email" class="mb-2 block text-sm font-semibold text-slate-700">Email</label>
            <input
              id="email"
              v-model="email"
              required
              type="email"
              autocomplete="email"
              class="w-full rounded-xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-[#28614d] focus:ring-4 focus:ring-[#28614d]/10"
            />
          </div>
          <div>
            <label for="password" class="mb-2 block text-sm font-semibold text-slate-700"
              >Password</label
            >
            <input
              id="password"
              v-model="password"
              required
              minlength="6"
              type="password"
              autocomplete="new-password"
              class="w-full rounded-xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-[#28614d] focus:ring-4 focus:ring-[#28614d]/10"
            />
          </div>
          <div v-if="isRegister">
            <label for="confirmPassword" class="mb-2 block text-sm font-semibold text-slate-700"
              >Confirm password</label
            >
            <input
              id="confirmPassword"
              v-model="confirmPassword"
              required
              type="password"
              autocomplete="new-password"
              class="w-full rounded-xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-[#28614d] focus:ring-4 focus:ring-[#28614d]/10"
            />
          </div>

          <p v-if="errorMessage" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">
            {{ errorMessage }}
          </p>
          <p
            v-if="successMessage"
            class="rounded-lg bg-emerald-50 px-3 py-2 text-sm text-emerald-700"
          >
            {{ successMessage }}
          </p>
          <button
            :disabled="isSubmitting"
            type="submit"
            class="w-full rounded-xl bg-[#e36d48] px-4 py-3.5 font-bold text-white transition hover:bg-[#c95838] disabled:cursor-not-allowed disabled:opacity-60"
          >
            {{ isSubmitting ? "Please wait..." : submitLabel }}
          </button>
        </form>

        <p class="mt-8 text-center text-sm text-slate-500">
          {{ isRegister ? "Already have an account?" : "New to Techbodia?" }}
          <button
            type="button"
            class="ml-1 font-bold text-[#28614d] hover:underline"
            @click="switchMode(!isRegister)"
          >
            {{ isRegister ? "Sign in" : "Create an account" }}
          </button>
        </p>
      </div>
    </section>
  </main>
</template>
