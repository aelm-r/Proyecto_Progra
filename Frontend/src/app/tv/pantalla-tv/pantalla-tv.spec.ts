import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PantallaTv } from './pantalla-tv';

describe('PantallaTv', () => {
  let component: PantallaTv;
  let fixture: ComponentFixture<PantallaTv>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PantallaTv],
    }).compileComponents();

    fixture = TestBed.createComponent(PantallaTv);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
