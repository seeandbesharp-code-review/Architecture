
export interface GiftResume {
  giftId: number;
  giftName: string;
  giftPrice: number;
  totalTicketsPurchase: number; // כמה כרטיסים נקנו עבור המתנה
  totalIncome: number; // בסה"כ כמה הרוויחו מהמתנה
}

export interface BuyerInfo {
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  ticketsPurshased: number; // כמה כרטיסים רכש
}

export interface GiftsWithBuyers {
  giftId: number;
  giftName: string;
  giftPrice: number;
  buyers: BuyerInfo[]; // רשימת רוכשים
}
