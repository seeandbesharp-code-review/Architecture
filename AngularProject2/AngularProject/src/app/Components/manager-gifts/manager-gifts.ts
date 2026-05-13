import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GiftModel, AddGift } from '../../Models/Gift';
import { Router, NavigationEnd } from '@angular/router';
import { Observable, filter } from 'rxjs';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogModule } from 'primeng/dialog';
import { PaginatorModule } from 'primeng/paginator';
import { InputTextModule } from 'primeng/inputtext';
import { FileUploadModule } from 'primeng/fileupload';

import { GiftsService } from '../../Services/GiftsService';
import { CategoryService } from '../../Services/CategoryService';
import { DonorService } from '../../Services/DonorService';
import { AuthorService } from '../../Services/AutorService';
import { CartService } from '../../Services/CartService';
import { SalesService } from '../../Services/SaleService';
import { GiftsWithBuyers } from '../../Models/Sales';
import { AddCategory, Category } from '../../Models/Category';
import { DonorModel } from '../../Models/Donor';
import { GiftBuyersDetailComp } from '../gift-buyers-detail-comp/gift-buyers-detail-comp';
import { GiftDetailComp } from '../gift-detail-comp/gift-detail-comp';
import { jwtDecode } from 'jwt-decode';
import { ToastModule } from 'primeng/toast';
import { RippleModule } from 'primeng/ripple';
import { MessageService } from 'primeng/api';

type SortOption = {
  label: string; value:
  { field: keyof GiftModel; order: 1 | -1 }
};

@Component({
  selector: 'app-manager-gifts',
  imports: [CommonModule, GiftDetailComp, GiftBuyersDetailComp, FileUploadModule, FormsModule, DataViewModule, SelectButtonModule, TagModule, ButtonModule, ProgressSpinnerModule, DialogModule, PaginatorModule, InputTextModule, ToastModule, RippleModule],
  templateUrl: './manager-gifts.html',
  styleUrl: './manager-gifts.scss',
  providers: [MessageService]
})
export class ManagerGifts implements OnInit {


  get totalBuyers(): number {
    return this.giftsWithBuyers.reduce((sum, gift) => sum + gift.buyers.length, 0);
  }

  get totalRevenue(): number {
    return this.giftsWithBuyers.reduce((sum, gift) => {
      const tickets = gift.buyers.reduce((tSum, buyer) => tSum + buyer.ticketsPurshased, 0);
      return sum + tickets * gift.giftPrice;
    }, 0);
  }
  get totalTicketsSold(): number {
    return this.giftsWithBuyers.reduce((sum, gift) => {
      const tickets = gift.buyers.reduce((tSum, buyer) => tSum + buyer.ticketsPurshased, 0);
      return sum + tickets;
    }, 0);
  }
  gifts$!: Observable<GiftModel[]>;
  donors$!: Observable<DonorModel[]>;
  categories$!: Observable<Category[]>;
  giftsWithBuyers: GiftsWithBuyers[] = []; // דוח מכירות
  showDialog = false;
  selectedGiftId!: number;
  selectedGiftName: string = '';
  displayBuyersDialog: boolean = false;


  isAdmin: boolean = false; // משתנה שיקבע אם להציג את הכפתורים
  displayEditDialog: boolean = false;
  selectedGiftForEdit: any = {}; // יחזיק את המתנה שאנחנו עורכים כרגע
  errorMessage: string = '';
  allGifts: GiftModel[] = [];        // כאן נשמור את כל המתנות מהשרת
  allDonors: DonorModel[] = [];        // כאן נשמור את כל המתנות מהשרת
  allCategories: Category[] = [];
  pagedGifts: GiftModel[] = [];      // כאן נשמור רק את המתנות שיוצגו בעמוד הנוכחי
  // הגדרות פגינציה
  first: number = 0;
  rows: number = 15; // כמה מוצרים להציג בעמוד
  cartUserId: number | null = null;
  isLoading: boolean = false;

  displayAddDialog: boolean = false;//למנהל להוסיף מתנה
  newGift: AddGift = {
    Name: '',
    Description: '',
    CategoryId: 1,
    TicketPrice: 10,
    Image: '',
    DonorId: 1
  };

  displayAddCategoryDialog: boolean = false;//למנהל להוסיף קטגוריה
  newCategory: AddCategory = {
    name: '',
  };
  selectedCategoryId: number = 0;
  selectedCategory: Category | null = null;
  sortOptions: SortOption[] = [
    { label: 'שם (א-ת)', value: { field: 'name', order: 1 } },
    { label: 'שם (ת-א)', value: { field: 'name', order: -1 } },
    { label: 'מחיר (זול-יקר)', value: { field: 'ticketPrice', order: 1 } },
    { label: 'מחיר (יקר-זול)', value: { field: 'ticketPrice', order: -1 } }
  ];
  selectedSort: SortOption | null = this.sortOptions[0];
  searchText = '';
  searchTextDonor = '';

  constructor(
    private giftService: GiftsService,
    private donorService: DonorService,
    private authorService: AuthorService,
    private categoryService: CategoryService,
    private salesService: SalesService,
    private cartService: CartService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.checkAdminStatus();
    this.loadInitialData();
    this.sortByBuyers();
    this.loadGiftsWithBuyers();
    this.loadGifts();
    this.loadCategories();
    this.loadDonors();
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


    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.loadGifts();
      this.checkAdminStatus();
    });


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

  checkAdminStatus() {
    this.isAdmin = this.authorService.isManager();
  }
  loadGiftsWithBuyers() {
    this.salesService.getGiftsByQuantityDesc().subscribe(data => {
      this.giftsWithBuyers = data;
    });
    this.checkAdminStatus();



  }


  // === Load All Data ===
  loadInitialData() {
    this.loadGifts();
    this.loadCategories();
    this.loadDonors();
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
      error: err => console.error('Error loading categories', err),
    });
  }

  private loadDonors() {
    this.donors$ = this.donorService.getDonors();
    this.donors$.subscribe({
      next: data => {
        this.allDonors = data;
        this.updatePage();
        this.cdr.detectChanges();
      },
      error: err => console.error('Error loading donors', err),
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

  // === Dialogs ===
  openAddGiftDialog() {
    // איפוס האובייקט לפני פתיחה
    this.errorMessage = '';
    this.newGift = { Name: '', Description: '', CategoryId: 1, TicketPrice: 10, Image: '', DonorId: 1 };
    this.displayAddDialog = true;
  }
  openAddCategoryDialog() {
    // איפוס האובייקט לפני פתיחה
    this.errorMessage = '';
    this.newCategory = { name: '' };
    this.displayAddCategoryDialog = true;
  }

  saveNewCategory() {
    if (!this.newCategory.name.trim()) {
      this.errorMessage = 'נא למלא את שם הקטגוריה';
      return;
    }
    this.errorMessage = '';

    this.categoryService.addCategory(this.newCategory).subscribe({
      next: (res) => {
        this.displayAddCategoryDialog = false;
        this.loadCategories();
        this.showToast('הוספת קטגוריה', 'הקטגוריה נוספה בהצלחה', 'success');

      },
      error: (err) => {
        this.errorMessage = 'שגיאה בהוספת קטגוריה';
      }


    });
  }



  openEditGiftDialog(gift: GiftModel) {
    this.selectedGiftForEdit = JSON.parse(JSON.stringify(gift));
    const categoryObj = this.allCategories.find(c => c.name === gift.category);
    this.selectedGiftForEdit.CategoryId = categoryObj ? categoryObj.id : null;
    const donorObj = this.allDonors.find(d => (d.firstName + ' ' + d.lastName) === gift.donor);
    this.selectedGiftForEdit.DonorId = donorObj ? donorObj.id : null;
    this.selectedGiftForEdit.Image = gift.image;
    this.selectedGiftForEdit.TicketPrice = gift.ticketPrice;
    this.selectedGiftForEdit.Description = gift.description;
    this.selectedGiftForEdit.Name = gift.name;
    this.selectedGiftForEdit.CategoryId = categoryObj ? categoryObj.id : null;
    this.selectedGiftForEdit.DonorId = donorObj ? donorObj.id : null;
    this.selectedGiftForEdit.Image = gift.image;
    this.displayEditDialog = true;
    this.cdr.detectChanges();
  }
  onUpload(event: any, isEdit: boolean = false) {
    const file = event.files?.[0];
    if (!file) return;

    if (isEdit) {
      this.selectedGiftForEdit.Image = file.name;
    } else {
      this.newGift.Image = file.name;
    }
  }
  saveNewGift() {
    this.errorMessage = '';
    if (!this.newGift.Name || !this.newGift.Description || !this.newGift.TicketPrice || !this.newGift.DonorId || !this.newGift.Image) {
      this.errorMessage = "נא למלא את כל שדות החובה לפני השמירה";
      alert(this.newGift.Image);
      return;
    }

    this.isLoading = true;  // מצב טעינה

    this.giftService.addGift(this.newGift).subscribe({
      next: (res) => {
        console.log('Gift added');
        this.displayAddDialog = false;
        this.isLoading = false;
        this.showToast('הוספת מתנה', 'המתנה התווספה בהצלחה', 'success'); 

        setTimeout(() => {
          this.loadGifts();
          this.cdr.detectChanges();
        }, 0);
      },
      error: (err) => {
        this.errorMessage = 'שגיאה בהוספת המתנה ייתכן וכבר קיימת כזו מתנה';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  updateGift() {
    const g = this.selectedGiftForEdit;

    if (!g.Name || !g.Description || !g.TicketPrice || !g.DonorId || !g.CategoryId || !g.Image) {
      this.errorMessage = "נא למלא את כל שדות החובה לפני העדכון";
      return;
    }

    this.errorMessage = '';

    this.giftService.updateGift(g.id, g).subscribe({
      next: (res) => {
        if (res === null) {
          alert("לא ניתן לעדכן את המתנה (המתנה לא נמצאה , נמחקה או כבר נרכשה)");
          this.displayEditDialog = false;
          this.cdr.detectChanges();
          return;
        }

        this.displayEditDialog = false;
        this.loadGifts();
        this.showToast('עדכון מתנה', 'המתנה עודכנה בהצלחה', 'success');

      },
      error: (err) => {
        if (err.status === 404) {
          this.errorMessage = 'לא ניתן לעדכן את המתנה (המתנה לא נמצאה או נמחקה)';
        } else {
          this.errorMessage = 'אירעה שגיאה בשמירת המתנה.';
        }
        this.cdr.detectChanges();
      }
    });
  }
  validateGift(g: any): boolean {
    if (!g.Name || !g.Description || !g.TicketPrice || !g.DonorId || !g.CategoryId || !g.Image) {
      this.errorMessage = 'נא למלא את כל שדות החובה לפני השמירה';
      return false;
    }
    this.errorMessage = '';
    return true;
  }

  // === Delete Gift ===
  deleteGift(id: number) {
    if (confirm('האם את בטוחה שברצונך למחוק את המתנה הזו?')) {
      this.giftService.deleteGift(id).subscribe({
        next: () => {
          console.log('המתנה נמחקה בהצלחה');
          this.showToast('מחיקת מתנה', 'המתנה נמחקה בהצלחה', 'success');
          this.ngOnInit()
        },
        error: (err) => {
          console.error('שגיאה במחיקה:', err);
          alert('לא ניתן למחוק את המתנה (ייתכן שהיא כבר נרכשה)');
        }
      });
    }
  }
  raffleGift(giftId: number) {
    this.isLoading = true;
    this.giftService.ruffleGift(giftId).subscribe({
      next: (data) => {
        setTimeout(() => {
          this.loadGifts();
          this.isLoading = false;
          this.showToast('הגרלת מתנה', 'המתנה הוגרלה בהצלחה', 'success');
        });
      },
      error: (err) => {
        this.isLoading = false;
        console.log('שגיאה כללית', err.message);
        console.log('קוד סטטוס', err.status);

        // כאן ניגשים ל-details
        if (err.error && err.error.details) {
          alert('פרטי השגיאה:' + err.error.details);
          return;
        }
      }
    });
  }
  sortByName(event: Event) {
    const value = (event.target as HTMLInputElement).value.trim();
    if (!value) return this.loadGifts();

    this.giftService.sortByName(value).subscribe(data => {
      this.allGifts = data;
      this.first = 0;
      this.updatePage();
      this.cdr.detectChanges();
    });
  }

  sortByDonorName(event: Event) {
    const value = (event.target as HTMLInputElement).value.trim();
    if (!value) return this.loadGifts();

    this.giftService.sortByDonorName(value).subscribe(data => {
      this.allGifts = data;
      this.first = 0;
      this.updatePage();
      this.cdr.detectChanges();
    });
  }
  orderByCategory(categoryId: number) {
    if (!categoryId || categoryId === 0) return this.loadGifts();

    this.giftService.orderByCategory(categoryId).subscribe(data => {
      this.allGifts = data;
      this.first = 0;
      this.updatePage();
      this.cdr.detectChanges();
    });
  }
  orderByPrice() {
    this.giftService.orderByPrice().subscribe(data => {
      this.allGifts = data;
      this.first = 0;
      this.updatePage();
      this.cdr.detectChanges();
    });
  }

  sortByBuyers() {
    this.salesService.getGiftsByQuantityDesc().subscribe(data => {
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

  details(id: number) {
    this.router.navigate(['/gift', id]);
  }
  showToast(summary: string, detail: string, severity: 'success' | 'info' | 'warn' | 'error' = 'success') {
    this.messageService.add({ severity, summary, detail, life: 3000 });
  }

}
