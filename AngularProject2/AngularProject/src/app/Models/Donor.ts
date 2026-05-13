import { GiftModel } from './Gift'; // נניח שיש לך את ה-GiftModel מ-GiftDto

// DTO ליצירת תורם חדש
export interface AddDonor {
  firstName: string;
  lastName: string;
  emailAddress: string;
  phone: string;
}

// DTO לעדכון תורם קיים
export interface UpdateDonor extends AddDonor {
  donorId: number;
}

// DTO להצגת תורם כולל מתנות
export interface DonorModel {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  gifts: GiftModel[]; // כל המתנות של התורם
}
