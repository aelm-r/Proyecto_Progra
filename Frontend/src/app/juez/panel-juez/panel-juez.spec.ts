import { provideHttpClient } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PanelJuez } from './panel-juez';

describe('PanelJuez', () => {
  let component: PanelJuez;
  let fixture: ComponentFixture<PanelJuez>;

  beforeEach(async () => {
    sessionStorage.clear();
    await TestBed.configureTestingModule({
      imports: [PanelJuez],
      providers: [provideHttpClient()],
    }).compileComponents();

    fixture = TestBed.createComponent(PanelJuez);
    component = fixture.componentInstance;
  });

  it('se crea', () => {
    expect(component).toBeTruthy();
  });

  it('sin combate cargado no habilita ninguna acción', () => {
    expect(Object.values(component.puede()).every((v) => v === false)).toBe(true);
  });

  it('sin sesión muestra el login', () => {
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.login')).toBeTruthy();
  });
});
