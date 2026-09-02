import type { NumberLiteralType } from "typescript";

export interface Note {
  id: number;
  title: string;
  content: string;
  createdAt: string;
}

export interface CreateNoteDto {
  title: string;
  content: string;
}

export interface UpdateNoteDto {
  title: string;
  content: string;
}
