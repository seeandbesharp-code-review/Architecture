import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonorsComp } from './donors-comp';

describe('DonorsComp', () => {
  let component: DonorsComp;
  let fixture: ComponentFixture<DonorsComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonorsComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DonorsComp);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
