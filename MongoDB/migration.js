// ============================================================
// MongoDB Migration Script — Gift Lottery Platform
// ============================================================
// אופן הרצה: MongoDB Compass → פתח Shell → הדבק ורוץ
// ============================================================

// ============================================================
// ניתוח לוגי: רלציוני → לא רלציוני
// ============================================================
//
// טבלה          | החלטת עיצוב ב-MongoDB           | סיבה
// --------------|----------------------------------|----------------------------------
// users         | אוסף עצמאי                       | מקורות שאילתות עצמאיים
// donors        | אוסף עצמאי                       | לתורם מתנות רבות — שמור כ-reference
// categories    | אוסף עצמאי + שם מוטמע ב-gifts   | שם הקטגוריה דנורמלי במתנה (קריאה מהירה)
// gifts         | אוסף עצמאי                       | ישות מרכזית, מרובה שאילתות
// carts         | מוטמעים פריטי עגלה (items:[])    | CartItem תמיד נשלף עם העגלה — embed
// cartItems     | נעלם — מוטמע בתוך cart           | אין קיום עצמאי
// raffleResults | אוסף עצמאי + שמות מוטמעים        | כבד קריאה, לעתים נדירות מתעדכן
// purchases     | אוסף עצמאי                       | היסטוריה של רכישות
//
// ============================================================

use("GiftLotteryDB");

// ─── 1. Users ────────────────────────────────────────────────
db.users.drop();
db.users.insertMany([
  {
    _id: 1,
    firstName: "Sari",
    lastName: "Cohen",
    email: "sari@test.com",
    phone: "0501234567",
    role: "customer"
  },
  {
    _id: 2,
    firstName: "Dana",
    lastName: "Levi",
    email: "dana@test.com",
    phone: "0502345678",
    role: "customer"
  },
  {
    _id: 3,
    firstName: "Noam",
    lastName: "Peretz",
    email: "noam@test.com",
    phone: "0503456789",
    role: "customer"
  },
  {
    _id: 4,
    firstName: "Admin",
    lastName: "Manager",
    email: "admin@test.com",
    phone: "0504567890",
    role: "manager"
  }
]);

// ─── 2. Donors ───────────────────────────────────────────────
db.donors.drop();
db.donors.insertMany([
  {
    _id: 1,
    firstName: "Moshe",
    lastName: "Israeli",
    email: "moshe@donor.com",
    phone: "0521111111"
  },
  {
    _id: 2,
    firstName: "Rachel",
    lastName: "Mizrahi",
    email: "rachel@donor.com",
    phone: "0522222222"
  },
  {
    _id: 3,
    firstName: "Yosef",
    lastName: "Katz",
    email: "yosef@donor.com",
    phone: "0523333333"
  }
]);

// ─── 3. Categories ───────────────────────────────────────────
db.categories.drop();
db.categories.insertMany([
  { _id: 1, name: "Electronics" },
  { _id: 2, name: "Jewelry" },
  { _id: 3, name: "Home & Living" },
  { _id: 4, name: "Sport & Fitness" }
]);

// ─── 4. Gifts ────────────────────────────────────────────────
// categoryName מוטמע (denormalize) — נמנעים מ-join בכל שאילתה
// donorId נשמר כ-reference — לתורם יש נתונים עצמאיים
db.gifts.drop();
db.gifts.insertMany([
  {
    _id: 1,
    name: "iPhone 15 Pro",
    description: "סמארטפון Apple דגם 15 Pro",
    ticketPrice: 50,
    image: "iphone15.jpg",
    donorId: 1,
    categoryId: 1,
    categoryName: "Electronics",
    isRaffleDone: false,
    winnerId: null
  },
  {
    _id: 2,
    name: "Gold Necklace",
    description: "שרשרת זהב 18 קראט",
    ticketPrice: 30,
    image: "necklace.jpg",
    donorId: 2,
    categoryId: 2,
    categoryName: "Jewelry",
    isRaffleDone: true,
    winnerId: 1
  },
  {
    _id: 3,
    name: "DeLonghi Coffee Machine",
    description: "מכונת אספרסו DeLonghi",
    ticketPrice: 20,
    image: "coffee.jpg",
    donorId: 1,
    categoryId: 3,
    categoryName: "Home & Living",
    isRaffleDone: false,
    winnerId: null
  },
  {
    _id: 4,
    name: "AirPods Pro",
    description: "אוזניות אלחוטיות Apple",
    ticketPrice: 25,
    image: "airpods.jpg",
    donorId: 3,
    categoryId: 1,
    categoryName: "Electronics",
    isRaffleDone: true,
    winnerId: 2
  },
  {
    _id: 5,
    name: "Diamond Ring",
    description: "טבעת יהלום 0.5 קראט",
    ticketPrice: 100,
    image: "ring.jpg",
    donorId: 2,
    categoryId: 2,
    categoryName: "Jewelry",
    isRaffleDone: false,
    winnerId: null
  },
  {
    _id: 6,
    name: "Philips Air Fryer",
    description: "מטגן אוויר Philips XL",
    ticketPrice: 15,
    image: "airfryer.jpg",
    donorId: 3,
    categoryId: 3,
    categoryName: "Home & Living",
    isRaffleDone: false,
    winnerId: null
  },
  {
    _id: 7,
    name: "Fitbit Charge 6",
    description: "צמיד כושר חכם",
    ticketPrice: 18,
    image: "fitbit.jpg",
    donorId: 1,
    categoryId: 4,
    categoryName: "Sport & Fitness",
    isRaffleDone: true,
    winnerId: 3
  },
  {
    _id: 8,
    name: "Sony WH-1000XM5",
    description: "אוזניות ANC פרמיום",
    ticketPrice: 40,
    image: "sony.jpg",
    donorId: 2,
    categoryId: 1,
    categoryName: "Electronics",
    isRaffleDone: false,
    winnerId: null
  }
]);

// ─── 5. Carts ────────────────────────────────────────────────
// CartItem מוטמע בתוך עגלה — תמיד נשלף יחד
// giftName מוטמע (denormalize) — מונע join לכל הצגת עגלה
db.carts.drop();
db.carts.insertMany([
  {
    _id: 1,
    userId: 1,
    status: "Draft",
    items: [
      { giftId: 1, giftName: "iPhone 15 Pro",       ticketPrice: 50, quantity: 3 },
      { giftId: 3, giftName: "DeLonghi Coffee Machine", ticketPrice: 20, quantity: 2 }
    ]
  },
  {
    _id: 2,
    userId: 2,
    status: "Purchase",
    items: [
      { giftId: 4, giftName: "AirPods Pro",   ticketPrice: 25, quantity: 5 },
      { giftId: 2, giftName: "Gold Necklace", ticketPrice: 30, quantity: 1 }
    ]
  },
  {
    _id: 3,
    userId: 1,
    status: "Purchase",
    items: [
      { giftId: 5, giftName: "Diamond Ring", ticketPrice: 100, quantity: 2 }
    ]
  },
  {
    _id: 4,
    userId: 3,
    status: "Draft",
    items: [
      { giftId: 6, giftName: "Philips Air Fryer",  ticketPrice: 15, quantity: 1 },
      { giftId: 8, giftName: "Sony WH-1000XM5",    ticketPrice: 40, quantity: 2 }
    ]
  }
]);

// ─── 6. Raffle Results ───────────────────────────────────────
// שמות מוטמעים — כבד קריאה, לעתים נדירות מתעדכן
db.raffleResults.drop();
db.raffleResults.insertMany([
  {
    _id: 1,
    giftId: 2,
    giftName: "Gold Necklace",
    winnerId: 1,
    winnerName: "Sari Cohen"
  },
  {
    _id: 2,
    giftId: 4,
    giftName: "AirPods Pro",
    winnerId: 2,
    winnerName: "Dana Levi"
  },
  {
    _id: 3,
    giftId: 7,
    giftName: "Fitbit Charge 6",
    winnerId: 3,
    winnerName: "Noam Peretz"
  }
]);

// ─── 7. Purchases ────────────────────────────────────────────
db.purchases.drop();
db.purchases.insertMany([
  { _id: 1, userId: 2, giftId: 4, giftName: "AirPods Pro",   quantity: 5, totalPrice: 125 },
  { _id: 2, userId: 2, giftId: 2, giftName: "Gold Necklace", quantity: 1, totalPrice: 30  },
  { _id: 3, userId: 1, giftId: 5, giftName: "Diamond Ring",  quantity: 2, totalPrice: 200 }
]);

print("✅ Migration complete!");
print("Collections: users, donors, categories, gifts, carts, raffleResults, purchases");
