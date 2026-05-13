import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagerGifts } from './manager-gifts';

describe('ManagerGifts', () => {
  let component: ManagerGifts;
  let fixture: ComponentFixture<ManagerGifts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManagerGifts]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManagerGifts);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
