export interface dto_register {
  userName: string;
  email: string;
  userPassword: string;
}

export interface dto_login {
  userName: string;
  userPassword: string;
}

export interface AuthResponse {
  userId: number;
  userName?: string;
}
