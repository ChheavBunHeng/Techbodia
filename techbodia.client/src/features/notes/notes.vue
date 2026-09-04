<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import api from "../../api/axios";
import { useAuthStore } from "../auth/useAuthStore";
import type { Note } from "./page.type";

const router = useRouter();
const authStore = useAuthStore();
const searchQuery = ref("");
const isModalOpen = ref(false);
const selectedNote = ref<Note | null>(null);
const noteTitle = ref("");
const noteContent = ref("");
const notes = ref<Note[]>([]);
const errorMessage = ref("");

const filteredNotes = computed(() => {
  if (!searchQuery.value.trim()) return notes.value;
  const q = searchQuery.value.toLowerCase();
  return notes.value.filter(
    (n) => n.title.toLowerCase().includes(q) || n.content.toLowerCase().includes(q),
  );
});

async function loadNotes() {
  if (!authStore.userId.value) return;

  try {
    const response = await api.get<Note[]>(`/Notes/${authStore.userId.value}`);
    notes.value = response.data;
  } catch {
    errorMessage.value = "Could not load notes.";
  }
}

const openCreateModal = () => {
  selectedNote.value = null;
  noteTitle.value = "";
  noteContent.value = "";
  isModalOpen.value = true;
};

const openEditModal = (note: Note) => {
  selectedNote.value = note;
  noteTitle.value = note.title;
  noteContent.value = note.content;
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};

const saveNote = async () => {
  if (!noteTitle.value.trim() || !authStore.userId.value) return;

  const note = {
    title: noteTitle.value.trim(),
    content: noteContent.value.trim(),
    userId: authStore.userId.value,
  };

  try {
    if (selectedNote.value) {
      await api.put(`/Notes/${selectedNote.value.id}`, note);
    } else {
      await api.post("/Notes", note);
    }
    closeModal();
    await loadNotes();
  } catch {
    errorMessage.value = "Could not save the note.";
  }
};

const deleteNote = async (id: number) => {
  if (!authStore.userId.value) return;

  try {
    await api.delete(`/Notes/${id}`, { params: { userId: authStore.userId.value } });
    await loadNotes();
  } catch {
    errorMessage.value = "Could not delete the note.";
  }
};

function logout() {
  authStore.logout();
  router.push({ name: "login" });
}

onMounted(loadNotes);
</script>

<template>
  <div class="min-h-screen bg-slate-50 text-slate-800 p-6 md:p-10">
    <div class="max-w-6xl mx-auto space-y-6">
      <!-- Header Section -->
      <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 class="text-3xl font-extrabold tracking-tight text-slate-900">Notes</h1>
          <p class="text-slate-500 text-sm mt-1">Manage and organize your personal workspace.</p>
        </div>

        <div class="flex gap-2">
          <button
            @click="openCreateModal"
            class="rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-medium text-white shadow-sm transition hover:bg-indigo-700"
          >
            Create Note
          </button>
          <button
            @click="logout"
            class="rounded-lg border border-slate-200 bg-white px-4 py-2.5 text-sm font-medium text-slate-600 hover:bg-slate-100"
          >
            Logout
          </button>
        </div>
      </div>

      <p v-if="errorMessage" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">
        {{ errorMessage }}
      </p>

      <!-- Toolbar Section -->
      <div
        class="flex flex-col gap-3 rounded-xl border border-slate-200 bg-white p-3 shadow-sm sm:flex-row sm:items-center sm:justify-between"
      >
        <div class="w-full sm:w-80">
          <input
            type="text"
            placeholder="Search notes..."
            v-model="searchQuery"
            class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-sm transition focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/20"
          />
        </div>

        <div class="text-xs text-slate-500 font-medium self-end sm:self-center">
          Showing <span class="font-semibold text-slate-700">{{ filteredNotes.length }}</span> notes
        </div>
      </div>

      <!-- Notes Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
        <div
          v-for="note in filteredNotes"
          :key="note.id"
          class="group bg-white border border-slate-200 hover:border-slate-300 rounded-xl p-5 shadow-sm hover:shadow-md transition-all flex flex-col justify-between"
        >
          <div>
            <div class="flex items-start justify-between gap-2 mb-2">
              <h3 class="font-semibold text-slate-900 group-hover:text-indigo-600 transition">
                {{ note.title }}
              </h3>
            </div>
            <p class="text-slate-600 text-sm leading-relaxed line-clamp-4 whitespace-pre-line mb-4">
              {{ note.content }}
            </p>
          </div>

          <div
            class="flex items-center justify-between pt-4 border-t border-slate-100 text-xs text-slate-400"
          >
            <span>{{ new Date(note.createdAt).toLocaleDateString() }}</span>

            <div class="flex items-center gap-2">
              <button
                @click="openEditModal(note)"
                class="text-xs font-medium text-indigo-600 transition hover:text-indigo-800"
              >
                Edit
              </button>
              <button
                @click="deleteNote(note.id)"
                class="text-xs font-medium text-red-600 transition hover:text-red-800"
              >
                Delete
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create / Edit Modal Dialog -->
    <div
      v-if="isModalOpen"
      class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm z-50 flex items-center justify-center p-4"
    >
      <div
        class="bg-white rounded-2xl max-w-lg w-full p-6 shadow-xl border border-slate-100 space-y-4"
      >
        <div class="flex justify-between items-center pb-2 border-b border-slate-100">
          <h2 class="text-lg font-bold text-slate-800">
            {{ selectedNote ? "Edit Note" : "New Note" }}
          </h2>
          <button @click="closeModal" class="text-sm text-slate-500 hover:text-slate-800">
            Close
          </button>
        </div>

        <div class="space-y-3">
          <div>
            <label class="block text-xs font-semibold uppercase text-slate-500 mb-1"> Title </label>
            <input
              type="text"
              v-model="noteTitle"
              placeholder="Enter note title..."
              class="w-full px-3.5 py-2 text-sm bg-slate-50 rounded-lg border border-slate-200 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500"
            />
          </div>

          <div>
            <label class="block text-xs font-semibold uppercase text-slate-500 mb-1">
              Content
            </label>
            <textarea
              :rows="5"
              v-model="noteContent"
              placeholder="Write your note here..."
              class="w-full px-3.5 py-2 text-sm bg-slate-50 rounded-lg border border-slate-200 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 resize-none"
            ></textarea>
          </div>
        </div>

        <div class="flex justify-end gap-2 pt-3">
          <button
            @click="closeModal"
            class="px-4 py-2 text-sm text-slate-600 hover:bg-slate-100 rounded-lg font-medium transition"
          >
            Cancel
          </button>
          <button
            @click="saveNote"
            class="px-4 py-2 text-sm bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg font-medium shadow-sm transition"
          >
            Save
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
