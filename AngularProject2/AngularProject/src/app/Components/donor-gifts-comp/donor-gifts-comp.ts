import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DonorService, Donor } from '../../Services/DonorService';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DonorModel } from '../../Models/Donor';

@Component({
  selector: 'app-donor-gifts-comp',
  imports: [CommonModule, TableModule, ButtonModule, RouterLink],
  templateUrl: './donor-gifts-comp.html',
  styleUrl: './donor-gifts-comp.scss',
})
export class DonorGiftsComp implements OnInit {
  donor: DonorModel | null = null;
  loading: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private donorService: DonorService
  ) { }

  ngOnInit(): void {
    const idStr = this.route.snapshot.paramMap.get('id');

    // 2. המרה למספר (Number)
    if (idStr) {
      const donorId = Number(idStr); // המרה מפורשת למספר

      // 3. בדיקה שההמרה הצליחה (שה-ID הוא אכן מספר תקין)
      if (!isNaN(donorId)) {
        this.donorService.getDonorById(donorId).subscribe({
          next: (donor) => {
            console.log('Fetched donor:', donor);
            
            this.donor = donor;
            this.loading = false;
            this.cdr.detectChanges();

          },
          error: (err) => {
            console.error('Error fetching donor:', err);
            this.loading = false;
          }
        });
      } else {
        console.error('The ID provided in the URL is not a valid number');
        this.loading = false;
      }
    }
  }
}