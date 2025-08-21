import { UserModel } from "./UserModel";



export interface AuthResponse {
  token: string;
  user: UserModel;
}
