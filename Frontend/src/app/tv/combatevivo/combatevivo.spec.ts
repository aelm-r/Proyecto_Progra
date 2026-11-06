import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Combatevivo } from './combatevivo';

describe('Combatevivo', () => {
  let component: Combatevivo;
  let fixture: ComponentFixture<Combatevivo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Combatevivo],
    }).compileComponents();

    fixture = TestBed.createComponent(Combatevivo);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
