import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartService } from '../../Services/CartService';
import { CartModel, Status } from '../../Models/Cart';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { SelectButtonModule } from 'primeng/selectbutton'; // עבור ה-p-selectbutton שצעק
import { NgClass, NgFor, CurrencyPipe } from '@angular/common'; // תוסיפי את אלו
import { InputNumberModule } from 'primeng/inputnumber';
import { Router, RouterLink } from '@angular/router';
import { GiftsService } from '../../Services/GiftsService';
import { MessageModule } from 'primeng/message';
import { RatingModule } from 'primeng/rating';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { PaymentDataService } from '../../Services/PaymentDataService';

interface Column {
  field: string;
  header: string;
}



@Component({
  selector: 'app-cart-comp',
  standalone: true,
  imports: [CommonModule, FormsModule,RouterLink, ButtonModule, DataViewModule, TagModule, SelectButtonModule, NgClass, NgFor, CurrencyPipe, InputNumberModule, MessageModule, RatingModule, TableModule, DialogModule],
  templateUrl: './cart-comp.html',
  styleUrl: './cart-comp.scss',
})
export class CartComp {
  // שימוש ב-Signal כמו בדוגמה של PrimeNG
  cartData = signal<CartModel | null>(null);
  paidOrders = signal<CartModel[]>([]);
  showPaidOrders = signal(false);

  confirmDialogVisible: boolean = false   // האם חלון האישור פתוח של התשלום
  totalToPay = signal(0);                 // הסכום הכולל להצגה



  cart: CartModel | null = null;
  userId: number | null = null;
  giftId: number | null = null;

  messages = signal<any[]>([]);



  constructor(private cartService: CartService, private paymentDataService: PaymentDataService, private router: Router, private giftService: GiftsService) { }

  ngOnInit() {
    this.addMessages();
    const token = localStorage.getItem('authToken');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        console.log('Full Token Payload:', decodedToken); // תבדוק כאן איך קוראים לשדה של ה-ID
        // שליפת ה-ID לפי המפתח הארוך שמופיע בתמונה שלך
        const key = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
        this.userId = decodedToken[key];
        // this.userId = decodedToken.id;
        if (this.userId) {
          this.loadCart();
        }
      }
      catch (error) {
        console.error('Invalid token:', error);
      }
    }
    else {
      console.log('No token found');
      // this.loadGuestCart(); // מציג סל אורח אם אין משתמש מחובר
    }
  }
  details(id: number) {
    this.router.navigate(['/gift', id]);
  }
  // טוען את הסל שלא שולם של המשתמש המחובר
  loadCart() {
    if (!this.userId) {
      this.loadGuestCart();
      return;
    }
    else {
      console.log("!!!!!!!!!!!!!!!!!");
    }
    this.cartService.GetUnpaidCartByUserId(this.userId).subscribe({
      next: (data) => {
        this.cartData.set(data);
        // this.cart = data;
        console.log(this.cartData());
      },
      error: (err) => console.error(err)
    });
  }

  // טוען את הסל של אורח מה-LS
  loadGuestCart() {
    // 1. קבל את המוצרים מה-LS
    const items = JSON.parse(localStorage.getItem('guestCart') || '[]');

    if (items.length === 0) {
      // אין מוצרים, הסל ריק
      this.cartData.set({
        id: 0,
        userId: 0,
        status: Status.Draft,
        items: []
      });
      return;
    }

    // 2. קבל את כל המתנות מה-API
    this.giftService.getGifts().subscribe(allGifts => {
      // 3. מיפוי המתנות מה-LS עם הפרטים המלאים
      const cartItems = items.map((i: any) => {
        const gift = allGifts.find(g => g.id === i.giftId);
        return {
          giftId: i.giftId,
          giftName: gift ? gift.name : 'Unknown Gift',
          quantity: i.quantity,
          ticketPrice: gift ? gift.ticketPrice : 0
        };
      });
      // 4. עדכון ה-Signal
      this.cartData.set({
        id: 0,
        userId: 0,
        status: Status.Draft,
        items: cartItems
      });
      console.log('Guest cart loaded:', this.cartData());
    }, err => {
      console.error('Error loading gifts for guest cart', err);
    });
  }

  // הוספת מתנה לסל
  addGift() {
    if (!this.giftId || !this.userId) {
      console.log("GIFTID");
      return;
    }
    this.cartService.AddGiftToCart({ userId: this.userId, giftId: this.giftId }).subscribe({
      next: (data) => {
        this.cart = data;
        console.log(this.cart);
        this.loadCart();
      },
      error: (err) => console.error(err)
    })
  }
  // פונקציה לקבלת הסגנון של התגית בהתאם למחיר
  getSeverity(item: any) {
    if (item.price > 100) return 'success';
    return 'info';
  }

  plusOne(item: any) {
    if (!this.userId) {
      const guestCart = JSON.parse(localStorage.getItem('guestCart') || '[]');
      const index = guestCart.findIndex((i: any) => i.giftId === item.giftId);
      if (index !== -1) {
        guestCart[index].quantity += 1;
      }
      localStorage.setItem('guestCart', JSON.stringify(guestCart));
      this.loadGuestCart(); // מעדכן את ה-Signal
      return;
    }
    this.cartService.PlusOne({ userId: this.userId, giftId: item.giftId }).subscribe({
      next: (data) => {
        this.cartData.set(data);
        this.loadCart();

      },
      error: (err) => console.error(err)
    });
  }

  minusOne(item: any) {
    if (item.quantity <= 1) return;

    if (!this.userId) {
      const guestCart = JSON.parse(localStorage.getItem('guestCart') || '[]');
      const index = guestCart.findIndex((i: any) => i.giftId === item.giftId);
      if (index !== -1) {
        guestCart[index].quantity -= 1;
        if (guestCart[index].quantity <= 0) {
          guestCart.splice(index, 1); 
        }
      }
      localStorage.setItem('guestCart', JSON.stringify(guestCart));
      this.loadGuestCart(); 
      return;
    }
    this.cartService.MinusOne({ userId: this.userId, giftId: item.giftId }).subscribe({
      next: (data) => {
        this.cartData.set(data);
        this.loadCart();
      },
      error: (err) => console.error(err)
    });
  }

  deleteItem(item: any) {
    if (!this.userId) {
      const guestCart = JSON.parse(localStorage.getItem('guestCart') || '[]');
      const index = guestCart.findIndex((i: any) => i.giftId === item.giftId);
      if (index !== -1) {
        guestCart.splice(index, 1); // מוחק את הפריט מהסל של אורח
      }
      localStorage.setItem('guestCart', JSON.stringify(guestCart));
      this.loadGuestCart(); // מעדכן את ה-Signal
      return;
    }
    this.cartService.DeleteGiftFromCart(this.userId, item.giftId).subscribe({
      next: (data) => {
        this.cartData.set(data);
        this.loadCart();
      },
      error: (err) => console.error(err)
    });
  }
  goToAllGifts() {
    this.router.navigate(['/all-gifts']);
  }
  clearMessages() {
    this.messages.set([]);
  }

  addMessages() {
    this.messages.set([
      {
        severity: 'success',
        content: 'הוספת מתנה חדשה',
        action: 'newGift'
      },
      {
        severity: 'secondary',
        content: 'היסטורית הזמנות',
        action: 'oldOrders'
      },
      {
        severity: 'warning',
        content: 'הזמנה בתהליך',
        action: 'toPay'
      }
    ]);
  }
  onMessageClick(action: string) {
    if (action === 'newGift') this.goToAllGifts();
    if (action === 'oldOrders') this.oldOrders();
    if (action === 'toPay') this.showCurrentCart();
  }
  // הצגת הסל הנוכחי
  showCurrentCart() {
    this.showPaidOrders.set(false);
  }
  // הצגת ההזמנות הישנות ששולמו
  oldOrders() {
    console.log("Old Orders");
    if (!this.userId) {
      return;
    }
    this.showPaidOrders.set(true); 

    this.cartService.GetPaidCartByUserId(this.userId).subscribe({
      next: (data) => {
        if (data) {
          const ordersArray = Array.isArray(data) ? data : [data];
          this.paidOrders.set(ordersArray); 
          this.showPaidOrders.set(true); 
        } else {
          this.paidOrders.set([]);
          this.showPaidOrders.set(true);

        }
      }
    });
  }
  // חישוב הסכום הכולל של הזמנה
  getOrderTotal(order: CartModel) {
    if (!order || !order.items) return 0;
    return order.items.reduce((sum, item) => sum + (item.ticketPrice * item.quantity), 0);
  }
  toPay() {
    if (!this.cartData()?.items || this.cartData()?.items.length === 0) return;

    // ודא שהמשתמש מחובר
    if (!this.userId) {
      alert("Please log in to complete the purchase.");
      return;
    }
    const data = this.cartData();
    if (!data) {
      alert("אין פריטים בסל, לא ניתן לעבור לתשלום");
      return;
    }

    // ניווט לדף התשלום עם מצב (cartData)
    this.paymentDataService.setCartData(data);
    this.router.navigate(['/payment']);
  }

  // ביטול התשלום
  cancelPayment() {
    this.confirmDialogVisible = false;
  }



}
