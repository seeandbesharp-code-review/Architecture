import { TypeOfWork } from './TestWork'; // אם WorkModel וה‑enum במודל נפרד

// מודל משרד הנהלה
export interface BossModel {
  id: number;
  officeCode: number;
  name: string;
  address: string;
  phone: string;
  nameOfBoss: string;     // סמנכ"ל
  typeOfWork: TypeOfWork.Management; // תמיד Management
}
