export interface UserDto{
  name: string;
  roles: string[];
  userId: string;
  nutrientsToTrack: NutrientTracking[];
}
export interface NutrientTracking {
  id:string,
  userId: string;
  nutrientId: number;
  targetAmount: number;
  isActive: boolean;
}
