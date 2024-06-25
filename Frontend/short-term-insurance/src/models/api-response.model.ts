export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data: T | any; // any in case of error
}
