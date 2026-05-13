

// export interface AuthorLogin {
//   email: string;
//   password: string;
// }

// export interface AuthorRegister {
//   firstName: string;
//   lastName: string;
//   email: string;
//   phone: string;
//   password: string;
// }

// export interface User {
//   id: number;
//   firstName: string;
//   lastName: string;
//   email: string;
//   phone: string;
//   role: string;
//   token: string;
// }
export interface LoginDto {
  email: string;
  password: string;
  phone: string;
}

export interface RegisterDto {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  password: string;
}

export interface UserModelDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  role: string;
  token: string; // JWT Token
}
