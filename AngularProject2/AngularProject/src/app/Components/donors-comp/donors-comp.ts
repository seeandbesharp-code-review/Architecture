import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DonorService } from '../../Services/DonorService';
import { Donor } from '../../Services/DonorService';
import { Observable } from 'rxjs';
import { AddDonor, DonorModel, UpdateDonor } from '../../Models/Donor';
import { TableModule } from 'primeng/table';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button'; 
import { InputTextModule } from 'primeng/inputtext'; 
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';



@Component({
  selector: 'app-donors-comp',
  standalone: true,
  imports: [TableModule, CommonModule, RouterLink, FormsModule, DialogModule, ButtonModule, InputTextModule],
  templateUrl: './donors-comp.html',
  styleUrls: ['./donors-comp.scss'],
})
export class DonorsComp implements OnInit {
  donors: Donor[] = [];
  loading: boolean = true;
  addDialogVisible = false;//חלונית להוספת תורם
  editDialogVisible = false;//חלונית לעדכון תורם
searchTerm: string = '';

private searchSubject = new Subject<string>();


  newDonor: AddDonor = {
    firstName: '',
    lastName: '',
    emailAddress: '',
    phone: ''
  };
  editDonor: UpdateDonor = {
    donorId: 0,
    firstName: '',
    lastName: '',
    emailAddress: '',
    phone: ''
  };


  constructor(private donorService: DonorService, private cdr: ChangeDetectorRef) { }


  ngOnInit() {
    console.time('donors');
this.loadAllDonors();


  this.searchSubject
    .pipe(
      debounceTime(300),        
      distinctUntilChanged()    
    )
    .subscribe(term => {
      if (!term.trim()) {
        this.loadAllDonors();
        return;
      }

      this.donorService.searchDonorsByName(term).subscribe({
        next: (res) => {
          this.donors = res;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error(err);
          alert('שגיאה בחיפוש תורמים');
        }
      });
    });

    
  }


  
loadAllDonors() {
  this.loading = true;

  this.donorService.getAllDonors().subscribe({
    next: (res) => {
      this.donors = res;
      this.loading = false;
      this.cdr.detectChanges();
    },
    error: (err) => {
      console.error(err);
      this.loading = false;
      alert('שגיאה בטעינת תורמים');
    }
  });
}
  showAddDialog() {
    this.newDonor = {
      firstName: '',
      lastName: '',
      emailAddress: '',
      phone: ''
    };


    this.addDialogVisible = true;
  }

  addDonor() {
    if (!this.newDonor.firstName || !this.newDonor.lastName) {
      alert('נא למלא שם פרטי ושם משפחה');
      return;
    }

    this.donorService.createDonor(this.newDonor).subscribe({
      next: (donor) => {
        this.donors.push(donor);
    setTimeout(() => {
      this.addDialogVisible = false;
      this.cdr.detectChanges();
    });
      },
      error: (err) => {
        console.error(err);
        alert('אירעה שגיאה ביצירת התורם');
      }
    });
  }




showEditDialog(donor: DonorModel) {
    this.editDonor = { 
      donorId: donor.id, 
      firstName: donor.firstName, 
      lastName: donor.lastName, 
      emailAddress: donor.email, 
      phone: donor.phone 
    };
    this.editDialogVisible = true;
  }

  updateDonor() {
      console.log('Updating donor:', this.editDonor); // <-- בדיקה

    if (!this.editDonor) return;
    if (!this.editDonor.firstName || !this.editDonor.lastName) {
      alert('נא למלא שם פרטי ושם משפחה');
      return;
    }

    this.donorService.updateDonor(this.editDonor).subscribe({
      next: (updated) => {
        const index = this.donors.findIndex(d => d.id === updated.id);
        if (index !== -1) this.donors[index] = updated; 
        this.editDialogVisible = false;
        this.cdr.detectChanges(); 
      },
      error: (err) => {
        console.error(err);
        alert('אירעה שגיאה בעדכון התורם');
      },
    });
  }
onSearchChange(value: string) {
  this.searchSubject.next(value);
}




deleteDonor(donor: DonorModel) {
  if (!donor || donor.id === undefined) {
    alert('לא נבחר תורם למחיקה!');
    return;
  }

  if (donor.gifts && donor.gifts.length > 0) {
    alert('לא ניתן למחוק תורם שיש לו מתנות!');
    return;
  }

  if (!confirm('בטוחה שברצונך למחוק את התורם?')) {
    return;
  }

  this.donorService.deleteDonor(donor.id).subscribe({
    next: () => {
      this.donors = this.donors.filter(d => d.id !== donor.id);
      this.cdr.detectChanges(); 
    },
    error: (err) => {
      console.error(err);
      alert('אירעה שגיאה במחיקת התורם');
    }
  });
}

}
