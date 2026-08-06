export interface ExerciseReadDto {
  id: number;
  name: string;
  description: string;
  youtubeUrl: string | null;
  exerciseCategoryId: number;
  exerciseCategoryName: string;
  muscleGroupId: number;
  muscleGroupName: string;
}