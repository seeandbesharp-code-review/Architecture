
// מנצח בהגרלה
export interface Winner {
  winnerId?: number; // יכול להיות null
  winnerName: string;
  winnerEmail: string;
}

// כניסה להגרלה
export interface RaffleEntry {
  userId: number;
  quantity: number;
}

// תוצאה סופית של הגרלה
export interface LotteryResult {
  giftId: number;
  giftName: string;
  winnerUserId: number;
  winnerName: string;
  winnerEmail: string;
  donorName: string;
  totalTicketsSold: number;
  totalParticipants: number;
  totalIncome: number; // decimal ב-TS
}
