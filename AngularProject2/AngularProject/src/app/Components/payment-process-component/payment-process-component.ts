import { Component, OnInit, inject, signal, computed, Input } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CartModel } from '../../Models/Cart';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { DialogModule } from 'primeng/dialog';

import { CartService } from '../../Services/CartService';
import { PaymentDataService } from '../../Services/PaymentDataService';
import { TableModule } from 'primeng/table';
import { json } from 'stream/consumers';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-payment-process',
  templateUrl: './payment-process-component.html',
  styleUrls: ['./payment-process-component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CurrencyPipe,
    ButtonModule,
    InputNumberModule,
    DialogModule,
    ReactiveFormsModule,
    TableModule,
    RouterLink,
    ToastModule
  ],
  providers: [MessageService]
})
export class PaymentProcessComponent implements OnInit {


  private messageService = inject(MessageService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  cartData!: CartModel | null;
  paymentForm!: FormGroup;
  detailsForm!: FormGroup;
  private paymentDataService = inject(PaymentDataService);
  cartItems: string = '';
  private cartService = inject(CartService);

  Name: string = '';
  paymentSuccess = signal(false);


  // שלבים
  steps = [
    { label: 'פרטים' },
    { label: 'תשלום' },
    { label: 'אישור' }
  ];

  currentStep = signal(0); // 0=פרטים, 1=תשלום, 2=אישור


  // כאן נוכל לדמות פרטי הזמנה

  subtotal = computed(() => this.cartData?.items?.reduce((sum, item) => sum + item.ticketPrice * item.quantity, 0) ?? 0);


  // חישוב סכומים
  // subtotal = computed(() => this.orderSummary.quantity * this.orderSummary.unitPrice);
  shipping = 19.9;
  discount = 20;
  total = computed(() => this.subtotal() + this.shipping - this.discount);

  processing = signal(false);

  ngOnInit(): void {
    this.paymentDataService.cartData$.subscribe(data => {
      // if (!data) {
      //   alert("לא נמצאו נתוני סל – חזור לדף הסל");
      //   this.router.navigate(['/cart']);
      //   return;
      // }
      this.cartData = data;
      this.subtotal = computed(() => this.cartData?.items.reduce((sum, item) => sum + item.ticketPrice * item.quantity, 0) || 0);
      this.total = computed(() => this.subtotal() + this.shipping - this.discount);
    });

    // const nav = this.router.getCurrentNavigation();
    // if (nav?.extras.state && nav.extras.state['cartData']) {
    //   this.cartData = nav.extras.state['cartData'];
    // }

    this.detailsForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      fullName: ['', Validators.required],
      phone: ['', Validators.required],
      acceptPrivacy: [false, Validators.requiredTrue],
      // כתובת
      country: ['Israel', Validators.required],
      city: ['', Validators.required],
      address: ['', Validators.required],
      zip: ['', Validators.required],
    });

    this.paymentForm = this.fb.group({
      cardName: ['', Validators.required],
      cardNumber: ['', [Validators.required, Validators.pattern(/^\d{16}$/)]],  // 16 ספרות
      expiry: ['', [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)]],  // MM/YY
      cvv: ['', [Validators.required, Validators.pattern(/^\d{3}$/)]],
      saveCard: [false]
    });
  }

  // nextStep() {
  //   if (this.currentStep() === 0 && this.detailsForm.invalid) {
  //     this.detailsForm.markAllAsTouched();
  //     return;
  //   }
  //   if (this.currentStep() === 1 && this.paymentForm.invalid) {
  //     this.paymentForm.markAllAsTouched();
  //     return;
  //   }
  //   this.currentStep.update(v => v + 1);
  // }
  nextStep() {
    alert(this.currentStep());
    if (this.currentStep() === 0 && this.detailsForm.invalid ) {
      alert("invalid details form");
      this.detailsForm.markAllAsTouched();
      this.currentStep.update(v => v + 1); // ✅ מעבר מיד לשלב הבא

      return;
    }
    if (this.currentStep() === 1 && this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      this.currentStep.update(v => v + 1); // ✅ מעבר מיד לשלב הבא

      return;
    }

    // אם אנחנו בשלב תשלום, נבדוק אם צריך לבצע תשלום
    if (this.currentStep() === 1) {
      // לא מבצעים פה קריאה לשרת — רק מעבירים את השלב ל־2
      // confirmPayment() תקרא רק כשיש כפתור "שלם עכשיו"
    }

    this.currentStep.update(v => v + 1); // ✅ מעבר מיד לשלב הבא
  }


  previousStep() {
    this.currentStep.update(v => Math.max(0, v - 1));
  }

  // confirmPayment() {
  //   if (this.paymentForm.invalid) {
  //     this.paymentForm.markAllAsTouched();
  //     return;
  //   }
  //   this.processing.set(true);

  //   // סימולציה של תשלום - נניח 2 שניות לעיבוד
  //   setTimeout(() => {
  //     this.processing.set(false);
  //     this.paymentSuccess.set(true);
  //     this.currentStep.set(2);
  //   }, 2000);
  // }
  // confirmPayment() {
  //   if (!this.cartData || this.cartData.items.length === 0) return;

  //   if (this.paymentForm.invalid) {
  //     this.paymentForm.markAllAsTouched();
  //     return;
  //   }

  //   this.processing.set(true);

  //   // קריאה לשרת כדי לרכוש את הסל
  //   // this.cartService.PurchaseCart(this.cartData.userId).subscribe({
  //   //   next: (purchasedCart) => {
  //   //     console.log('purchasedCart:', purchasedCart);
  //   //     this.processing.set(false);
  //   //     this.paymentSuccess.set(true);
  //   //     this.currentStep.set(2);

  //   //     // אופציונלי: לאחר התשלום נוכל לרוקן את הסל או להציג הודעה
  //   //     console.log('Purchase successful', purchasedCart);

  //   //     // עדכון cartData להצגה של סיכום
  //   //     this.cartData = purchasedCart;
  //   //   },
  //   //   error: (error) => {
  //   //     this.processing.set(false);
  //   //     if (error.status === 500) {
  //   //       alert("לא ניתן לבצע את ההזמנה ייתכן ואחת או יותר מהמתנות כבר הוגרלו");
  //   //     } else {
  //   //       console.error('Unexpected error', error);
  //   //     }
  //   //   }
  //   // });
  //   this.cartService.PurchaseCart(this.cartData.userId).subscribe({
  //     next: (purchasedCart) => {
  //       this.processing.set(false);
  //       this.paymentSuccess.set(true);
  //       this.currentStep.set(2);

  //       // ודא שיש items להצגה
  //       if (purchasedCart.items && purchasedCart.items.length > 0) {
  //         this.cartData = {
  //           id: purchasedCart.id ?? this.cartData.id,          // אם undefined קח את הקיים
  //           userId: purchasedCart.userId ?? this.cartData.userId,
  //           status: purchasedCart.status,
  //           items: purchasedCart.items ?? []
  //         };
  //       } else {
  //         this.cartData.items = [];
  //       }

  //       console.log('Updated cartData:', this.cartData);
  //     },
  //     error: (error) => {
  //       this.processing.set(false);
  //       if (error.status === 500) {
  //         alert("לא ניתן לבצע את ההזמנה ייתכן ואחת או יותר מהמתנות כבר הוגרלו");
  //       } else {
  //         console.error('Unexpected error', error);
  //       }
  //     }
  //   });

  // }
  confirmPayment() {
    if (!this.cartData || this.cartData.items.length === 0) return;

    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    this.processing.set(true);

    this.cartService.PurchaseCart(this.cartData.userId).subscribe({
      next: (purchasedCart) => {
        console.log("Purchase successful:", this.cartData);
        this.processing.set(false);
        this.paymentSuccess.set(true);
        this.currentStep.set(2);

        if (!purchasedCart) {
          alert("התרחשה שגיאה בתהליך התשלום");
          return;
        }

        // עדכון cartData להצגת סיכום
        // this.cartData = purchasedCart;
      },
      error: (error) => {
        this.processing.set(false);
        if (error.status === 500) {
          alert("לא ניתן לבצע את ההזמנה – ייתכן ואחת או יותר מהמתנות כבר הוגרלו");
        } else {
          console.error('Unexpected error', error);
        }
      }
    });
  }




}
