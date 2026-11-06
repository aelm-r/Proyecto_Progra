import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PanelJuez } from './panel-juez';

describe('PanelJuez', () => {
  let component: PanelJuez;
  let fixture: ComponentFixture<PanelJuez>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PanelJuez],
    }).compileComponents();

    fixture = TestBed.createComponent(PanelJuez);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
