import {NutrientTracking} from './UserDTO';

export interface LoginResponseDTO {
  userId: string;
  name: string;
  email: string;
  token: string;
  roles: string[];
  nutrientTrackings:NutrientTracking[];
}
