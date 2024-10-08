import { Photo } from './photo';

export interface User {
  id: number;
  email: string;
  userName: string;
  roles: string[];
  knownAs: string;
  age: number;
  gender: string;
  created: Date;
  lastActive: any;
  photoUrl: string;
  city: string;
  country: string;
  interests?: string;
  introduction?: string;
  lookingFor?: string;
  photos?: Photo[];
  token?: string;
}
