import { CombateResponse } from './juez.models';

const voltios = { id: 1, nombre: 'Los Voltios', colorHex: '#e63946' };
const pulsos = { id: 2, nombre: 'Los Pulsos', colorHex: '#2a9d8f' };
const maquina = { id: 3, nombre: 'Máquina Cero', colorHex: '#f4a261' };
const corto = { id: 4, nombre: 'Cortocircuito', colorHex: '#6a4c93' };

// Un combate por cada estado para probar la botonera
export const COMBATES_MOCK: CombateResponse[] = [
  { id: 1, numeroOrden: 1, fase: 'grupos', estado: 'finalizado', equipoA: voltios, equipoB: pulsos, marcador: '2 – 0', asaltoAbierto: null },
  { id: 2, numeroOrden: 2, fase: 'grupos', estado: 'en_curso', equipoA: maquina, equipoB: corto, marcador: '1 – 0', asaltoAbierto: { numero: 2, esMuerteSubita: false } },
  { id: 3, numeroOrden: 3, fase: 'grupos', estado: 'programado', equipoA: voltios, equipoB: maquina, marcador: '0 – 0', asaltoAbierto: null },
  { id: 4, numeroOrden: 4, fase: 'grupos', estado: 'en_pausa', equipoA: pulsos, equipoB: corto, marcador: '1 – 1', asaltoAbierto: null },
  { id: 13, numeroOrden: 13, fase: 'semifinal', estado: 'programado', equipoA: null, equipoB: null, marcador: '0 – 0', asaltoAbierto: null },
];