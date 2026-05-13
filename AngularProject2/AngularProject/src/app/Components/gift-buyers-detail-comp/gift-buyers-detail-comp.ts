import { Component, Input, OnInit } from '@angular/core';
import { BuyerInfo, GiftsWithBuyers } from '../../Models/Sales';
import { SalesService } from '../../Services/SaleService';
import { DialogModule } from 'primeng/dialog';
import { CommonModule } from '@angular/common';
import { Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-gift-buyers-detail-comp',
  standalone: true,
  imports: [DialogModule, CommonModule, TableModule],
  templateUrl: './gift-buyers-detail-comp.html',
  styleUrl: './gift-buyers-detail-comp.scss',
})
export class GiftBuyersDetailComp implements OnInit {
  @Input() giftId!: number;
  @Input() giftName!: string;
  @Input() displayDialog: boolean = false;
  @Output() displayDialogChange = new EventEmitter<boolean>();

  buyers: BuyerInfo[] = [];
  isLoading: boolean = false;

  constructor(private salesService: SalesService) { }

  ngOnInit(): void {
    if (this.giftId) {
      this.loadBuyers();
    }
  }


  ngOnChanges(changes: SimpleChanges) {
    if (changes['giftId'] && this.giftId) {
      this.loadBuyers();
    }

  }
  loadBuyers() {
    this.isLoading = true;
    this.buyers = []; // <--- מחיקה של הרשימה הקודמת
    this.salesService.getGiftWithBuyers().subscribe((data: GiftsWithBuyers[]) => {
      const gift = data.find(g => g.giftId === this.giftId);
      if (gift) {
        this.buyers = gift.buyers; // מגדיר את המערך מחדש
        this.giftName = gift.giftName;
      }
      this.isLoading = false;
    }, err => {
      console.error('Error loading buyers', err);
      this.isLoading = false;
    });
  }

  closeDialog() {
    this.displayDialog = false;
    this.displayDialogChange.emit(this.displayDialog);
  }
}
