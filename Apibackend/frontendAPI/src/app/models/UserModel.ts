export interface UserModel {
  fullName: string;
  email: string;
  password: string;
  role?: string;             
  createdAt?: Date;      
  updatedAt?: Date;          
}