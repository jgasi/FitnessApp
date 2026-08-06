export interface UserProfileReadDto {
  id: number;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  fitnessGoalId: number | null;
  fitnessGoalName: string | null;
  dateOfBirth: string | null;
  gender: string | null;
  heightCm: number | null;
  currentWeightKg: number | null;
}

export interface UserProfileUpdateDto {
  fitnessGoalId: number | null;
  dateOfBirth: string | null;
  gender: string | null;
  heightCm: number | null;
  currentWeightKg: number | null;
}