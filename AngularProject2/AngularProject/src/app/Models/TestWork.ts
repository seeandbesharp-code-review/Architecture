// סוג משרד (enum)
export enum TypeOfWork {
  Field =0,
  Management = 1
}

// מודל משרד שטח
export interface WorkModel {
  id: number;
  officeCode: number;
  name: string;
  address: string;
  phone: string;
  typeOfWork: TypeOfWork.Field; // כברירת מחדל ניתן להגדיר Field בצד קליינט
}
