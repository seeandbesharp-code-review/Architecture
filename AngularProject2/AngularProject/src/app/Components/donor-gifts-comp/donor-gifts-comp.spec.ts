import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonorGiftsComp } from './donor-gifts-comp';

describe('DonorGiftsComp', () => {
  let component: DonorGiftsComp;
  let fixture: ComponentFixture<DonorGiftsComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonorGiftsComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DonorGiftsComp);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
