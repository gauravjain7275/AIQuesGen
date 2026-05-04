export interface User {
  userId: number;
  userName: string;
  email: string;
  userType: 'Individual' | 'Coaching';
  token: string;
}