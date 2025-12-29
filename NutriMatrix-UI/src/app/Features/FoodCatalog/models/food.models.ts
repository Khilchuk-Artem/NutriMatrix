export interface FoodDTO {
  id: number;
  name: string;
  photo: string;
  barcode:string;
  foodNutrients: FoodNutrientIn100gDto[];
  measures: MeasureDto[];
}

export interface FoodNutrientIn100gDto {
  nutrientId: number;
  amount: number;
}

export interface MeasureDto {
  id: number;
  name: string;
  weightInGrams: number;
}

export interface FoodShortcutDTO {
  id: number;
  name: string;
  nutrients: FoodNutrientIn100gDto[];
}
