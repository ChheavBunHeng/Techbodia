<script setup lang="ts">
import { ref, computed } from "vue";
import type { Note } from "./page.type";

// Reactive State
const searchQuery = ref("");
const isModalOpen = ref(false);
const selectedNote = ref<Note | null>(null);

// Form Fields for Modal
const noteTitle = ref("");
const noteContent = ref("");

// Mock Notes Data
const mockNotes = ref<Note[]>([
  {
    id: 1,
    title: "Project Architecture Notes",
    content:
      "Feature-based pattern separating page.vue, page.type.ts, page.api.ts, and page.action.ts works smoothly.",
    createdAt: new Date().toISOString(),
  },
  {
    id: 2,
    title: "Meeting Checklist",
    content:
      "Review ASP.NET Core controllers, check JWT expiration rules, and verify database migrations.",
    createdAt: new Date().toISOString(),
  },
]);

// Filtered Notes based on search
const filteredNotes = computed(() => {
  if (!searchQuery.value.trim()) return mockNotes.value;
  const q = searchQuery.value.toLowerCase();
  return mockNotes.value.filter(
    (n) => n.title.toLowerCase().includes(q) || n.content.toLowerCase().includes(q),
  );
});

// Handlers
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

const saveNote = () => {
  // Logic to save/update note can be placed here
  closeModal();
};

const deleteNote = (id: number) => {
  mockNotes.value = mockNotes.value.filter((n) => n.id !== id);
};
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

        <button
          @click="openCreateModal"
          class="inline-flex items-center justify-center gap-2 bg-indigo-600 hover:bg-indigo-700 text-white font-medium px-4 py-2.5 rounded-lg shadow-sm transition"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 4v16m8-8H4"
            />
          </svg>
          Create Note
        </button>
      </div>

      <!-- Toolbar Section -->
      <div
        class="flex flex-col sm:flex-row gap-3 items-center justify-between bg-white p-3 rounded-xl border border-slate-200 shadow-sm"
      >
        <div class="relative w-full sm:w-80">
          <svg
            class="w-5 h-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
            />
          </svg>
          <input
            type="text"
            placeholder="Search notes..."
            v-model="searchQuery"
            class="w-full pl-10 pr-4 py-2 text-sm bg-slate-50 rounded-lg border border-slate-200 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition"
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
                class="p-1 hover:text-indigo-600 transition"
                title="Edit Note"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                  />
                </svg>
              </button>
              <button
                @click="deleteNote(note.id)"
                class="p-1 hover:text-red-600 transition"
                title="Delete Note"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                  />
                </svg>
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
          <button @click="closeModal" class="text-slate-400 hover:text-slate-600 p-1">✕</button>
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
