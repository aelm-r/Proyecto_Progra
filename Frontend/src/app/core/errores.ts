export function mensajeDeError(err: any): string {
  if (err?.status === 0) {
    return 'No se pudo conectar con el servidor. Revisa que el backend esté corriendo.';
  }
  const base = err?.error?.mensaje ?? 'No se pudo completar la acción. Intenta de nuevo.';
  const errores = err?.error?.errores as Record<string, string[]> | undefined;
  const detalle = errores ? Object.values(errores).flat().join(' ') : '';
  return detalle ? `${base} ${detalle}` : base;
}
