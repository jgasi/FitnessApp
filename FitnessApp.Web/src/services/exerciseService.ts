import { apiClient } from "./apiClient";
import type { ExerciseReadDto } from "../types/exercise";

interface ExerciseQueryParams {
  search?: string;
  exerciseCategoryId?: number | null;
  muscleGroupId?: number | null;
}

function buildQueryString(params: ExerciseQueryParams): string {
  const query = new URLSearchParams();

  if (params.search?.trim()) {
    query.set("search", params.search.trim());
  }

  if (params.exerciseCategoryId) {
    query.set("exerciseCategoryId", String(params.exerciseCategoryId));
  }

  if (params.muscleGroupId) {
    query.set("muscleGroupId", String(params.muscleGroupId));
  }

  const queryString = query.toString();
  return queryString ? `?${queryString}` : "";
}

export const exerciseService = {
  getAll: async (params: ExerciseQueryParams = {}): Promise<ExerciseReadDto[]> => {
    const queryString = buildQueryString(params);
    return await apiClient.get<ExerciseReadDto[]>(`/api/Exercise${queryString}`);
  },
};