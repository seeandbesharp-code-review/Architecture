import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GiftDetailComp } from './gift-detail-comp';

describe('GiftDetailComp', () => {
  let component: GiftDetailComp;
  let fixture: ComponentFixture<GiftDetailComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GiftDetailComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GiftDetailComp);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
