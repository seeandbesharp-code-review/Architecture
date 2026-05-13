

export interface AddCartItem {
  userId: number;
  giftId: number;
}

export interface UpdateCartItem {
  userId: number;
  giftId: number;
  quantity: number; // Angular לא מחייב Range, אבל אפשר לבדוק בטפסים
}

export interface CartItem {
  giftId: number;
  giftName: string; // שם המתנה
  quantity: number;
  ticketPrice: number;
  category: string;
  image: string;
}

export enum Status {
  Draft = 0,
  Purchased = 1
}

export interface CartModel {
  id: number;       // id של העגלה
  userId: number;   // id של המשתמש
  status: Status;
  items: CartItem[];
}

export interface CreateCart {
  userId: number;
}

export interface MergeCartItem {
  giftId: number;
  quantity: number;
}
