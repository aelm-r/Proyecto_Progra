// Configuración local del panel del juez.
// Cuando F3 suba environments/environment.ts, cambiar solo el import en juez.service.ts.
export const JUEZ_CONFIG = {
  apiUrl: 'http://localhost:5000', // confirmar puerto con Backend
  useMocks: true,                  // false cuando la API real esté lista (día 6)
};