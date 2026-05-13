import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllGiftsComp } from './all-gifts-comp';

describe('AllGiftsComp', () => {
  let component: AllGiftsComp;
  let fixture: ComponentFixture<AllGiftsComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllGiftsComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllGiftsComp);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
