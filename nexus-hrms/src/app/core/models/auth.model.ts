export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  returnCode: number;
  returnMessage: string;
}

export interface ChangePasswordRequest {
  userId: number;
  currentPassword?: string;
  newPassword: string;
  ipAddress: string;
}

export interface UserSession {
  id: number;
  userName: string;
  employeeGuid: string;
  designationId: number;
  hierarchyLevel: number;
}