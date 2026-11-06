import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Proxcombate } from './proxcombate';

describe('Proxcombate', () => {
  let component: Proxcombate;
  let fixture: ComponentFixture<Proxcombate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Proxcombate],
    }).compileComponents();

    fixture = TestBed.createComponent(Proxcombate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
