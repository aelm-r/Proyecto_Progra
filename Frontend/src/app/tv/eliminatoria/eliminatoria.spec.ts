import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Eliminatoria } from './eliminatoria';

describe('Eliminatoria', () => {
  let component: Eliminatoria;
  let fixture: ComponentFixture<Eliminatoria>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Eliminatoria],
    }).compileComponents();

    fixture = TestBed.createComponent(Eliminatoria);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
