import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GiftModel, AddGift } from '../../Models/Gift';
// import { GiftsService } from '../../Services/GiftsService';
import { Router, NavigationEnd } from '@angular/router';
import { Observable, filter, map, startWith, switchMap } from 'rxjs';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
// import { SelectModule } from 'primeng/select';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogModule } from 'primeng/dialog';
import { Category } from '../../Models/Category';
// import { CategoryService } from '../../Services/CategoryService';
import { AuthorService } from '../../Services/AutorService';
import { PaginatorModule } from 'primeng/paginator';
// import { DropdownModule } from 'primeng/DropdownModule'; // בגרסה 17 קוראים לזה Dropdown ולא Select
import { InputTextModule } from 'primeng/inputtext';
import { GiftsService } from '../../Services/GiftsService';
import { CategoryService } from '../../Services/CategoryService';
import { jwtDecode } from 'jwt-decode';
import { CartService } from '../../Services/CartService';
import { DonorModel } from '../../Models/Donor';
import { FileUploadModule } from 'primeng/fileupload';
import { DonorService } from '../../Services/DonorService';
// import { DropdownModule } from 'primeng/dropdown'; 
import { SalesService } from '../../Services/SaleService';
import { GiftsWithBuyers } from '../../Models/Sales';
import { GiftBuyersDetailComp } from '../gift-buyers-detail-comp/gift-buyers-detail-comp';
import { GiftDetailComp } from '../gift-detail-comp/gift-detail-comp';
import { DropdownModule } from 'primeng/dropdown';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { RippleModule } from 'primeng/ripple';


type SortOption = {
  label: string; value:
  { field: keyof GiftModel; order: 1 | -1 }
};

@Component({
  selector: 'app-all-gifts-comp',
  standalone: true,
  imports: [CommonModule, DropdownModule, GiftDetailComp, FileUploadModule, FormsModule, DataViewModule, SelectButtonModule, TagModule, ButtonModule, ProgressSpinnerModule, DialogModule, PaginatorModule, InputTextModule, GiftBuyersDetailComp, ToastModule, RippleModule],
  templateUrl: './all-gifts-comp.html',
  styleUrl: './all-gifts-comp.scss',
    providers: [MessageService]

})
export class AllGiftsComp implements OnInit {
  gifts$!: Observable<GiftModel[]>;
  donors$!: Observable<DonorModel[]>;
  categories$!: Observable<Category[]>;

  filteredPrizes: GiftModel[] = [];
  sortOptions: SortOption[] = [
    { label: 'שם (א-ת)', value: { field: 'name', order: 1 } },
    { label: 'שם (ת-א)', value: { field: 'name', order: -1 } },
    { label: 'מחיר (זול-יקר)', value: { field: 'ticketPrice', order: 1 } },
    { label: 'מחיר (יקר-זול)', value: { field: 'ticketPrice', order: -1 } }
  ];
  selectedSort: SortOption | null = this.sortOptions[0];

  giftsWithBuyers: GiftsWithBuyers[] = []; // דוח מכירות
  showDialog = false;
  selectedGiftId!: number;
  selectedGiftName: string = '';
  displayBuyersDialog: boolean = false;

  winnersByPrize: Record<number, number> = {};

  allGifts: GiftModel[] = [];        // כאן נשמור את כל המתנות מהשרת
  allDonors: DonorModel[] = [];        // כאן נשמור את כל המתנות מהשרת
  allCategories: Category[] = [];
  pagedGifts: GiftModel[] = [];      // כאן נשמור רק את המתנות שיוצגו בעמוד הנוכחי
  // הגדרות פגינציה
  first: number = 0;
  rows: number = 9; // כמה מוצרים להציג בעמוד
  cartUserId: number | null = null;
  isLoading: boolean = false;
  searchText = '';
  searchTextDonor = '';
  selectedCategoryId: number = 0;

  selectedCategory: Category | null = null;

  isAdmin: boolean = true; // כדי לראות את הכפתור למנהל

  constructor(private giftService: GiftsService, private donorService: DonorService, private authorService: AuthorService, private categoryService: CategoryService, private router: Router, private cdr: ChangeDetectorRef, private salesService: SalesService, private cartService: CartService, private messageService: MessageService) {
  }


  ngOnInit() {
    this.checkAdminStatus();

    if (typeof window !== 'undefined') {  // <--- לבדוק אם window קיים
      const token = window.localStorage.getItem('authToken');
      if (token) {
        try {
          const decodedToken: any = jwtDecode(token);
          const key = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
          this.cartUserId = decodedToken[key];
        } catch (error) {
          console.error('Invalid token:', error);
        }
      }
    }

    this.loadGifts();
    this.loadCategories();
    this.loadDonors();
  }
  logCategory(event: any) {
    alert(event.value);
  }

  onSearch(event: any): void {
    this.filteredAndSortedPrizes;
  }
  get filteredAndSortedPrizes(): GiftModel[] {
    const q = this.searchText.trim().toLowerCase();
    const d = this.searchTextDonor.trim().toLowerCase();

    let items = this.allGifts;

    if (q) {
      items = items.filter((p) => {
        const name = (p.name ?? '').toLowerCase();
        const desc = (p.description ?? '').toLowerCase();
        const cat = this.categoryLabel(p).toLowerCase();
        return name.includes(q) || desc.includes(q) || cat.includes(q);
      });
    }
    if (d) {
      items = items.filter((p) => {
        const donor = (p.donor ?? '').toLowerCase();
        return donor.includes(d);

      });
    }
    if (this.selectedCategoryId && this.selectedCategoryId !== 0) {
      items = items.filter(p => Number(p.category) === this.selectedCategoryId);
    }



    const opt = this.selectedSort?.value;
    if (opt) {
      const { field, order } = opt;
      items = [...items].sort((a: any, b: any) => {
        const av = a?.[field];
        const bv = b?.[field];
        if (av == null && bv == null) return 0;
        if (av == null) return 1;
        if (bv == null) return -1;

        if (typeof av === 'string' && typeof bv === 'string') return av.localeCompare(bv) * order;
        return (Number(av) - Number(bv)) * order;
      });
    }

    return items;
  }
  categoryLabel(p: GiftModel): string {
    return `קטגוריה #${p.category}`;
  }

  onSortChange(event: any) {
    const value = event.value;
  }
  loadGifts() {
    this.gifts$ = this.giftService.getGifts();
    this.gifts$.subscribe({
      next: (data) => {
        this.allGifts = data; // כאן הנתונים נשמרים במערך
        this.updatePage();    // כאן אנחנו חותכים את ה-6 הראשונים
        this.cdr.detectChanges(); // עדכון התצוגה
      },
      error: (err) => console.error("Error loading gifts", err)
    });
  }
  loadCategories() {
    this.categories$ = this.categoryService.getCategories();
    this.categories$.subscribe({
      next: (data) => {
        this.allCategories = [{ id: 0, name: 'כל הקטגוריות' }, ...data];

        this.updatePage();
        this.cdr.detectChanges();
      },
      error: (err) => console.error("Error loading categories", err)
    });
  }
  loadDonors() {
    this.donors$ = this.donorService.getDonors();
    this.donors$.subscribe({
      next: (data) => {
        this.allDonors = data;
        this.updatePage();
        this.cdr.detectChanges();
      },
      error: (err) => console.error("Error loading donors", err)
    });
  }

  checkAdminStatus() {
    this.isAdmin = this.authorService.isManager();
  }

  raffleGift(giftId: number) {
    this.isLoading = true;
    this.giftService.ruffleGift(giftId).subscribe({
      next: (data) => {
        setTimeout(() => {
          this.loadGifts();
          this.isLoading = false;
                  this.messageService.add({ severity: 'success', summary: 'הגרלה', detail: 'הגרלת המתנה בוצעה בהצלחה!' });

        });
      },
      error: (err) => {
        this.isLoading = false;
        console.log('שגיאה כללית', err.message);
        console.log('קוד סטטוס', err.status);

        // כאן ניגשים ל-details
        if (err.error && err.error.details) {
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: err.error.details });
          return;
        }
      }
    });
  }


  onPageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    this.updatePage();
  }

  updatePage() {
    const list = this.filteredAndSortedPrizes;
    this.pagedGifts = list.slice(this.first, this.first + this.rows);
  }

  GetById(id: number) {
    this.router.navigate(['/gift', id]);
  }

  addToCart(giftId: number) {
    if (!this.cartUserId) {
    this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'לא ניתן להוסיף לסל כשאינך מחובר' });
      return;
    }

    this.cartService.AddGiftToCart({ userId: this.cartUserId, giftId }).subscribe({
      next: (data) => {
        console.log(`Gift ${giftId} added to cart`, data);
      this.messageService.add({ severity: 'success', summary: 'הוספה לסל', detail: 'המתנה נוספה בהצלחה לסל!' });
      },
      error: (err) => {
        console.error('Error adding gift to cart', err);
      this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'לא ניתן להוסיף את המתנה לסל (ייתכן שכבר נרכשה)' });
      }
    });
  }

  details(id: number) {
    this.router.navigate(['/gift', id]);
  }

  // חיפוש לפי שם
  sortByName(event: Event) {
    const value = (event.target as HTMLInputElement).value.trim();

    if (!value) {
      this.loadGifts(); // טוען את כל המתנות מחדש
      return;
    }

    this.giftService.sortByName(value).subscribe(data => {
      this.allGifts = data; // העדכון צריך להיות במערך הראשי
      this.first = 0;        // מתחילים מהעמוד הראשון
      this.updatePage();     // חותכים ל-pagedGifts
      this.cdr.detectChanges();
    });
  }

  // חיפוש לפי שם תורם
  sortByDonorName(event: Event) {
    const value = (event.target as HTMLInputElement).value.trim();

    if (!value) {
      this.loadGifts();
      return;
    }

    this.giftService.sortByDonorName(value).subscribe(data => {
      this.allGifts = data;
      this.first = 0;
      this.updatePage();
      this.cdr.detectChanges();
    });
  }
  orderByCategory(categoryId: number) {
    if (categoryId === 0) {
      // טוען את כל המתנות
      this.loadGifts();
      return;
    }
    if (!categoryId) {
      // אם לא נבחרה קטגוריה – טוענים את כל המתנות
      this.loadGifts();
      return;
    }
    this.giftService.orderByCategory(categoryId).subscribe(data => {
      this.allGifts = data;
      this.first = 0;       // מתחילים מהעמוד הראשון
      this.updatePage();    // מעדכנים את pagedGifts
      this.cdr.detectChanges();
    });
  }
  onCategoryChange(categoryId: number) {
    this.first = 0; // להתחלה של הפגינציה
    this.updatePage(); // חותך את המערך
  }

  // סינון לפי מחיר
  orderByPrice() {
    this.giftService.orderByPrice().subscribe(data => {
      this.allGifts = data;   // עדכון כל המתנות
      this.first = 0;         // מתחילים מהעמוד הראשון
      this.updatePage();      // מעדכנים את pagedGifts
      this.cdr.detectChanges();
    });
  }


  sortByBuyers() {
    this.salesService.getGiftsByQuantityDesc().subscribe((data: GiftsWithBuyers[]) => {
      this.giftsWithBuyers = data;
    });
  }
  showBuyersDialog(giftId: number) {
    const gift = this.allGifts.find(g => g.id === giftId);
    if (!gift) return;

    this.selectedGiftId = giftId;
    this.selectedGiftName = gift.name;
    this.displayBuyersDialog = true;
  }

}