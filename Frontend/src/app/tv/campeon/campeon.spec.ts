import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Campeon } from './campeon';

describe('Campeon', () => {
  let component: Campeon;
  let fixture: ComponentFixture<Campeon>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Campeon],
    }).compileComponents();

    fixture = TestBed.createComponent(Campeon);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
