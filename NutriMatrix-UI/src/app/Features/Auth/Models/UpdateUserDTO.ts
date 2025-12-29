import {NutrientTracking} from './UserDTO';

export interface UpdateUserDTO {
  name: string;
  updateNutrients: NutrientTracking[];
}
