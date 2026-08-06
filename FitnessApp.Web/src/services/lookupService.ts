import { apiClient } from "./apiClient";
import type { LookupDto } from "../types/lookups";

export const lookupService = {
  getExerciseCategories: async (): Promise<LookupDto[]> => {
    return await apiClient.get<LookupDto[]>("/api/ExerciseCategories");
  },

  getMuscleGroups: async (): Promise<LookupDto[]> => {
    return await apiClient.get<LookupDto[]>("/api/MuscleGroups");
  },

  getFitnessGoals: async (): Promise<LookupDto[]> => {
    return await apiClient.get<LookupDto[]>("/api/FitnessGoals");
  },
};