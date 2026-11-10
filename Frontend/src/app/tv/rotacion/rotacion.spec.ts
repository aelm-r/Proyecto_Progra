import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Rotacion } from './rotacion';

describe('Rotacion', () => {
  let component: Rotacion;
  let fixture: ComponentFixture<Rotacion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Rotacion],
    }).compileComponents();

    fixture = TestBed.createComponent(Rotacion);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
