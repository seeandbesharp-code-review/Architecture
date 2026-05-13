// export interface AddGift {
//   name: string;
//   description: string;
//   categoryId: number;
//   ticketPrice: number;
//   image: string;
//   donorId: number;
// }

// DTO להוספת מתנה חדשה
export interface AddGift {
  Name: string;
  Description: string;
  CategoryId: number;
  TicketPrice: number;
  Image: string;
  DonorId: number;
}

// DTO לעדכון מתנה קיימת
export interface UpdateGift extends AddGift {
  id: number;
}

// DTO להצגת מתנה
export interface GiftModel {
  id: number;
  name: string;
  category: string;
  description: string;
  ticketPrice: number;
  image: string;
  donor: string;
  winnerName?: string; // יכול להיות ריק
}

// DTO למידע על קונים של מתנה
export interface GiftBuyersData {
  gift: GiftModel;
  totalTickets: number;
  purchaseCount: number;
}

// DTO למנהל
export interface GiftToManager extends GiftModel {
  totalTickets: number;
  purchaseCount: number;
  totalSum: number;
}

// DTO של מתנה שהוגרלה
export interface GiftRaffled {
  id: number;
  winnerUserId?: number; // יכול להיות null
}
