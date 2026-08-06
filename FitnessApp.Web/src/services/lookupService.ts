import { apiClient } from "./apiClient";
import type { LookupDto } from "../types/lookups";

export const lookupService = {
  getFitnessGoals: async (): Promise<LookupDto[]> => {
    return await apiClient.get<LookupDto[]>("/api/FitnessGoals");
  },
};