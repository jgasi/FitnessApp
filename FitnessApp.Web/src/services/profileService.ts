import { apiClient } from "./apiClient";
import type { UserProfileReadDto, UserProfileUpdateDto } from "../types/profile";

export const profileService = {
  getMyProfile: async (): Promise<UserProfileReadDto> => {
    return await apiClient.get<UserProfileReadDto>("/api/Profiles/me");
  },

  updateMyProfile: async (dto: UserProfileUpdateDto): Promise<void> => {
    await apiClient.put<void>("/api/Profiles/me", dto);
  },
};