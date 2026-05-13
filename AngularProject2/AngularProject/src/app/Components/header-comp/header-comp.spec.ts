import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HeaderComp } from './header-comp';

describe('HeaderComp', () => {
  let component: HeaderComp;
  let fixture: ComponentFixture<HeaderComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HeaderComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HeaderComp);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
