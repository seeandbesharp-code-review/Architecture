import { ChangeDetectorRef, Component } from '@angular/core';
import { GiftModel } from '../../Models/Gift';
import { ActivatedRoute } from '@angular/router';
import { GiftsService } from '../../Services/GiftsService';
import { CommonModule } from '@angular/common';
import { Observable, switchMap } from 'rxjs';

@Component({
  selector: 'app-gift-detail-comp',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './gift-detail-comp.html',
  styleUrl: './gift-detail-comp.scss',
})
export class GiftDetailComp {
  id!: number;
gift$!: Observable<GiftModel>;
  constructor(private route: ActivatedRoute, private giftService: GiftsService, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
this.gift$ = this.route.paramMap.pipe(
      switchMap(params => {
        const id = Number(params.get('id'));
        return this.giftService.getGiftById(id);
      })
    );  
  };

}
