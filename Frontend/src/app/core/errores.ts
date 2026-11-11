// El backend responde { mensaje: '...' } en español. Se muestra tal cual.
export function mensajeDeError(err: any): string {
  return err?.error?.mensaje ?? 'No se pudo completar la acción. Intenta de nuevo.';
}
