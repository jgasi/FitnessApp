export interface RegisterDto {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  password: string;
}

export interface LoginDto {
  userNameOrEmail: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  userName: string;
  email: string;
  roles: string[];
}

export interface AuthUser {
  userName: string;
  email: string;
  roles: string[];
}