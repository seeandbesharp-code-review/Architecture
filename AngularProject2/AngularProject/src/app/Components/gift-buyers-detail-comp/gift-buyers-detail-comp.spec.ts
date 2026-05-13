import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GiftBuyersDetailComp } from './gift-buyers-detail-comp';

describe('GiftBuyersDetailComp', () => {
  let component: GiftBuyersDetailComp;
  let fixture: ComponentFixture<GiftBuyersDetailComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GiftBuyersDetailComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GiftBuyersDetailComp);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
