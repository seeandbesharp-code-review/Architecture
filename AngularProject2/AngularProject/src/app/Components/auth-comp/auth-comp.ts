import { ChangeDetectorRef, Component } from '@angular/core';
import { AuthorService } from '../../Services/AutorService';
import { LoginDto, RegisterDto } from '../../Models/Author';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { FloatLabelModule } from 'primeng/floatlabel';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';
import e from 'express';
import { NgIf } from '@angular/common';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { RippleModule } from 'primeng/ripple';

@Component({
  selector: 'app-auth-comp',
  imports: [CommonModule, FormsModule, DialogModule, ButtonModule, InputTextModule, PasswordModule, FloatLabelModule, ToastModule, RippleModule],
  templateUrl: './auth-comp.html',
  styleUrl: './auth-comp.scss',
  providers: [MessageService]
})
export class AuthComp {

  visibleLogin: boolean = false;
  visibleRegister: boolean = false;
  errorMessage: string = '';

  email: string = '';
  password: string = '';

  firstName: string = '';
  lastName: string = '';
  phone: string = '';
  confirmPassword: string = '';
  passwordStrength: number = 0;
  passwordStrengthClass: string = 'weak';


  constructor(private authorService: AuthorService, private router: Router, private cdr: ChangeDetectorRef, private messageService: MessageService) { }
  allowOnlyNumbers(event: KeyboardEvent) {
    const charCode = event.key;
    if (!/[\d]/.test(charCode)) {
      event.preventDefault();
    }
  }
  checkPasswordStrength() {
    const pwd = this.password;
    let strength = 0;

    if (pwd.length >= 6) strength += 30;
    if (/[A-Z]/.test(pwd)) strength += 20;
    if (/[a-z]/.test(pwd)) strength += 20;
    if (/\d/.test(pwd)) strength += 20;
    if (/[\W_]/.test(pwd)) strength += 10;
    alert(this.passwordStrength)

    this.passwordStrength = Math.min(strength, 100);
    if (strength < 50) this.passwordStrengthClass = 'weak';
    else if (strength < 80) this.passwordStrengthClass = 'medium';
    else this.passwordStrengthClass = 'strong';

  }
  showToast(summary: string, detail: string, severity: 'success' | 'info' | 'warn' | 'error') {
    this.messageService.add({ severity, summary, detail });
  }


  login() {

    this.errorMessage = ''; // איפוס שגיאה קודמת
    const token = localStorage.getItem('authToken');
    if (token) {
      console.log('User already logged in.');
      this.errorMessage = 'משתמש כבר מחובר';
      this.showToast('משתמש כבר מחובר', 'ברוך הבא!', 'info');

      return;
    }

    // בדיקת תקינות מקומית
    if (!this.email || !this.password || !this.phone) {
      this.errorMessage = 'יש להזין את כל הנתונים';
      return;
    }
    const dto: LoginDto = { email: this.email, password: this.password, phone: this.phone };

    this.authorService.login(dto).subscribe({
      next: (res: any) => {
        if (res && res.token) {
          localStorage.setItem('authToken', res.token);


          this.visibleLogin = false;
          console.log('Login successful, token saved in localStorage!');
          this.showToast('התחברות', 'התחברת בהצלחה', 'success');

          document.body.classList.remove('p-overflow-hidden');
          this.reset();
          this.router.navigate(['/all-gifts']);
        } else {
          console.error('Login succeeded but no token returned!');
          this.showToast('התחברות נכשלה', 'לא התקבל טוקן מהשרת', 'error');
          this.errorMessage = 'התחברות נכשלה: לא התקבל טוקן מהשרת';
        }
      },
      error: (err) => {
        console.error('Full Error Object:', err);

        // 1. בדיקה אם השרת החזיר הודעה ספציפית בתוך אובייקט error
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
        }
        // 2. בדיקה אם השרת החזיר שגיאות ולידציה של ASP.NET (מבנה של errors: { field: [messages] })
        else if (err.error && err.error.errors) {
          const errorDetails = err.error.errors;
          // לוקחים את השגיאה הראשונה שנמצאה ומציגים אותה
          const firstErrorKey = Object.keys(errorDetails)[0];
          this.errorMessage = errorDetails[firstErrorKey][0];
        }
        // 3. בדיקה אם ה-error עצמו הוא מחרוזת טקסט
        else if (typeof err.error === 'string') {
          this.errorMessage = err.error;
        }
        // 4. ברירת מחדל אם לא הצלחנו לחלץ הודעה ספציפית
        else {
          this.errorMessage = 'אירעה שגיאה בביצוע הפעולה, אנא נסו שנית';
        }
        this.cdr.detectChanges();
      }

    });
  }
  register() {
    this.errorMessage = '';
    const token = localStorage.getItem('authToken');
    if (token) {
      console.log('User already logged in.');
      this.errorMessage = 'משתמש כבר מחובר';
      return;
    }


    if (!this.firstName || !this.lastName || !this.email || !this.password || !this.phone) {
      this.errorMessage = 'יש למלא את כל השדות';
      return;
    }
    const dto: RegisterDto = {
      email: this.email,
      password: this.password,
      firstName: this.firstName,
      lastName: this.lastName,
      phone: this.phone
    };
    if (!this.firstName || !this.lastName || !this.email || !this.password) {
      this.errorMessage = 'אנא מלאי את כל השדות הנדרשים';
      return;
    }

    // בדיקת התאמת סיסמאות רק בלחיצה על הכפתור
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'הסיסמאות אינן תואמות';
      return;
    }


    this.authorService.register(dto).subscribe({
      next: (res: any) => {
        if (res && res.token) {
          localStorage.setItem('authToken', res.token);
          console.log('Registration successful, token saved in localStorage!');
          this.visibleRegister = false; // סגירת החלון
          document.body.classList.remove('p-overflow-hidden');
          this.reset();
          this.router.navigate(['/all-gifts']);
        }
      },
      error: (err) => {
        console.error('Full Error Object:', err);

        // 1. בדיקה אם השרת החזיר הודעה ספציפית בתוך אובייקט error
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
        }
        // 2. בדיקה אם השרת החזיר שגיאות ולידציה של ASP.NET (מבנה של errors: { field: [messages] })
        else if (err.error && err.error.errors) {
          const errorDetails = err.error.errors;
          // לוקחים את השגיאה הראשונה שנמצאה ומציגים אותה
          const firstErrorKey = Object.keys(errorDetails)[0];
          this.errorMessage = errorDetails[firstErrorKey][0];
        }
        // 3. בדיקה אם ה-error עצמו הוא מחרוזת טקסט
        else if (typeof err.error === 'string') {
          this.errorMessage = err.error;
        }
        // 4. ברירת מחדל אם לא הצלחנו לחלץ הודעה ספציפית
        else {
          this.errorMessage = 'אירעה שגיאה בביצוע הפעולה, אנא נסו שנית';
        }
        this.cdr.detectChanges();
      }
    });
  }

  resetForm() {
    this.firstName = '';
    this.lastName = '';
    this.email = '';
    this.phone = '';
    this.password = '';
    this.confirmPassword = '';
    this.errorMessage = '';
    this.passwordStrength = 0;
  }


  logout() {
    const token = localStorage.getItem('authToken');
    if (token) {
      localStorage.removeItem('authToken');


      console.log('User logged out, localStorage cleared!');
      this.messageService.add({ severity: 'success', summary: 'התנתקות', detail: 'התנתקת בהצלחה' });
      this.reset();
      window.location.reload();
    }
    else {
      this.showToast('אין משתמש מחובר', 'אין משתמש מחובר', 'warn');
      console.log('No user is currently logged in.');
    }
  }
  reset() {
    this.email = '';
    this.password = '';
    this.firstName = '';
    this.lastName = '';
    this.phone = '';
    console.log('Fields cleared!');
  }
  showLogin() {
    this.errorMessage = '';
    this.visibleLogin = true;
  }

  showRegister() {
    this.errorMessage = '';
    this.visibleRegister = true;
  }
}
