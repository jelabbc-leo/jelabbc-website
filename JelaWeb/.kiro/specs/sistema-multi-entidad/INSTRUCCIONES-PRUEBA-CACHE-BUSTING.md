# Instrucciones de Prueba - Cache Busting

**Fecha:** 21 de Enero de 2026  

---

## ✅ Problema Resuelto

El error `cboEntidad is not defined` era causado por el **caché del navegador** cargando versiones antiguas de los archivos JavaScript.

**Solución aplicada:** Se agregó un parámetro de versión (`?v=20260121`) a todos los scripts para forzar al navegador a descargar las nuevas versiones.

---

## 🧪 Cómo Probar

### Opción 1: Recarga Simple (Recomendada)

1. **Cerrar todas las pestañas** del navegador que tengan la aplicación abierta
2. **Abrir una nueva pestaña**
3. **Navegar a la aplicación** (login si es necesario)
4. **Ir a la página de Reservaciones**
5. **Hacer clic en el botón "Nuevo"**
6. **Verificar que el popup se abre correctamente**

### Opción 2: Verificación en DevTools

1. **Abrir DevTools** (presionar F12)
2. **Ir a la pestaña "Network"**
3. **Recargar la página** (F5)
4. **Buscar "reservaciones.js"** en la lista
5. **Verificar que la URL incluye** `?v=20260121`
6. **Verificar que el Status es 200** (no 304)

---

## 📋 Páginas a Probar

Todas estas páginas ahora tienen cache busting implementado:

1. ✅ **Reservaciones** - `/Views/Operacion/Condominios/Reservaciones.aspx`
2. ✅ **Unidades** - `/Views/Catalogos/Unidades.aspx`
3. ✅ **Residentes** - `/Views/Catalogos/Residentes.aspx`
4. ✅ **Visitantes** - `/Views/Operacion/Condominios/Visitantes.aspx`
5. ✅ **Conceptos de Cuota** - `/Views/Catalogos/ConceptosCuota.aspx`
6. ✅ **Áreas Comunes** - `/Views/Catalogos/AreasComunes.aspx`
7. ✅ **Comunicados** - `/Views/Operacion/Condominios/Comunicados.aspx`
8. ✅ **Cuotas** - `/Views/Operacion/Condominios/Cuotas.aspx`

---

## ✅ Qué Verificar en Cada Página

1. **Botón "Nuevo"** - Debe abrir el popup correctamente
2. **Botón "Editar"** - Debe abrir el popup con datos del registro seleccionado
3. **Doble clic en registro** - Debe abrir el popup con datos
4. **Botón "Eliminar"** - Debe pedir confirmación
5. **Guardar registro** - Debe guardar correctamente sin errores

---

## ❌ Errores que NO Deben Aparecer

En la consola del navegador (F12 → Console) NO deben aparecer estos errores:

- ❌ `cboEntidad is not defined`
- ❌ `cmbEntidad is not defined`
- ❌ `cboCuotaEntidad is not defined`
- ❌ `cboGenEntidad is not defined`

---

## 🎯 Resultado Esperado

Después de esta actualización:

- ✅ Todos los botones de toolbar funcionan correctamente
- ✅ Los popups se abren sin errores
- ✅ Los registros se pueden crear, editar y eliminar
- ✅ No hay errores en la consola del navegador
- ✅ No se requiere limpiar caché manualmente

---

## 🆘 Si Aún Hay Problemas

Si después de cerrar y abrir el navegador aún hay errores:

1. **Limpiar caché del navegador:**
   - Presionar `Ctrl + Shift + Delete`
   - Seleccionar "Imágenes y archivos en caché"
   - Hacer clic en "Borrar datos"

2. **Probar en modo incógnito:**
   - Presionar `Ctrl + Shift + N` (Chrome) o `Ctrl + Shift + P` (Firefox)
   - Navegar a la aplicación
   - Probar los botones

3. **Verificar que el servidor está actualizado:**
   - Asegurarse de que el proyecto se compiló correctamente
   - Verificar que IIS/IIS Express se reinició

---

## 📞 Reportar Resultados

Por favor reportar:

1. ✅ **Qué páginas funcionan correctamente**
2. ❌ **Qué páginas aún tienen errores** (si las hay)
3. 📸 **Capturas de pantalla de errores** en la consola (si los hay)
4. 🌐 **Navegador y versión** que estás usando

---

**Última Actualización:** 21 de Enero de 2026  
**Estado:** ✅ Listo para pruebas
