# Guía para Limpieza de UI - Eliminación de Campos de Entidad

**Fecha:** 20 de Enero de 2026  
**Estado:** Guía para tareas opcionales  
**Prioridad:** Baja (El sistema ya funciona correctamente)

---

## 📋 Resumen

Esta guía describe cómo eliminar los campos de "Entidad" de las páginas existentes. **Estas tareas son opcionales** ya que el sistema ya funciona correctamente con filtrado automático implementado en `DynamicCrudService` y `ApiConsumerCRUD`.

---

## ✅ ¿Por qué son opcionales?

El sistema multi-entidad está **100% funcional** sin necesidad de modificar estas páginas porque:

1. **DynamicCrudService** ya filtra automáticamente por `IdEntidad` en:
   - `ObtenerTodos()` - Agrega `WHERE IdEntidad = X`
   - `ObtenerTodosConFiltro()` - Combina filtro de entidad con filtros adicionales
   - `Insertar()` - Agrega `IdEntidad` automáticamente
   - `Actualizar()` - Valida pertenencia antes de actualizar
   - `Eliminar()` - Valida pertenencia antes de eliminar

2. **ApiConsumerCRUD** hereda el mismo comportamiento

3. **SessionHelper** mantiene `IdEntidadActual` correctamente

4. **Jela.Master** muestra el dropdown para cambiar de entidad

**Resultado:** Los usuarios ya no pueden ver ni modificar datos de otras entidades, incluso si los formularios todavía muestran un campo de "Entidad".

---

## 🎯 Beneficios de Completar Estas Tareas

Aunque opcionales, completar estas tareas proporciona:

1. **UI más limpia** - Elimina campos innecesarios
2. **Menos confusión** - Los usuarios no ven campos que no pueden cambiar
3. **Mejor UX** - Interfaz más simple y directa
4. **Consistencia** - Todas las páginas siguen el mismo patrón

---

## 📝 Pasos para Cada Página

### Paso 1: Identificar Controles de Entidad

Buscar en el archivo `.aspx`:
```aspx
<!-- Ejemplos de controles a eliminar -->
<dx:ASPxComboBox ID="cmbEntidad" ...>
<dx:ASPxGridLookup ID="glEntidad" ...>
<asp:DropDownList ID="ddlEntidad" ...>
```

### Paso 2: Eliminar del ASPX

Eliminar completamente el control y su label asociado:
```aspx
<!-- ANTES -->
<div class="form-group">
    <label>Entidad:</label>
    <dx:ASPxComboBox ID="cmbEntidad" runat="server" ...>
    </dx:ASPxComboBox>
</div>

<!-- DESPUÉS -->
<!-- Eliminado - El sistema usa IdEntidadActual automáticamente -->
```

### Paso 3: Eliminar del Designer (.aspx.designer.vb)

Eliminar la declaración del control:
```vb
' ANTES
Protected WithEvents cmbEntidad As Global.DevExpress.Web.ASPxComboBox

' DESPUÉS
' Eliminado
```

### Paso 4: Limpiar Code-Behind (.aspx.vb)

Eliminar código que:
- Carga el combo de entidades
- Obtiene el valor seleccionado
- Valida la entidad

```vb
' ANTES
Private Sub CargarEntidades()
    ' ... código para cargar combo
End Sub

Protected Sub btnGuardar_Click(...)
    Dim idEntidad As Integer = Convert.ToInt32(cmbEntidad.Value)
    campos.Add("IdEntidad", idEntidad)
    ' ...
End Sub

' DESPUÉS
Protected Sub btnGuardar_Click(...)
    ' IdEntidad se agrega automáticamente por DynamicCrudService
    ' ...
End Sub
```

### Paso 5: Verificar Funcionamiento

1. Compilar el proyecto
2. Probar la página:
   - Crear nuevo registro
   - Editar registro existente
   - Eliminar registro
3. Verificar que solo se ven datos de la entidad actual
4. Cambiar de entidad con el dropdown del master
5. Verificar que ahora se ven datos de la nueva entidad

---

## 📄 Lista de Páginas a Actualizar

### Prioridad Alta (Páginas más usadas)

1. **Tickets.aspx** - Sistema de tickets
   - Ubicación: `JelaWeb/Views/Operacion/Tickets/`
   - Nota: Ya usa `ApiConsumerCRUD`

2. **Residentes.aspx** - Gestión de residentes
   - Ubicación: `JelaWeb/Views/Catalogos/`

3. **Unidades.aspx** - Gestión de unidades
   - Ubicación: `JelaWeb/Views/Catalogos/`

### Prioridad Media

4. **Cuotas.aspx** - Gestión de cuotas
   - Ubicación: `JelaWeb/Views/Operacion/Condominios/`

5. **Conceptos.aspx** - Catálogo de conceptos
   - Ubicación: `JelaWeb/Views/Catalogos/`

6. **AreasComunes.aspx** - Gestión de áreas comunes
   - Ubicación: `JelaWeb/Views/Catalogos/`

### Prioridad Baja

7. **Comunicados.aspx** - Gestión de comunicados
   - Ubicación: `JelaWeb/Views/Catalogos/`

8. **Reservaciones.aspx** - Sistema de reservaciones
   - Ubicación: `JelaWeb/Views/Catalogos/`

9. **Pagos.aspx** - Gestión de pagos
   - Ubicación: `JelaWeb/Views/Operacion/`

10. **EstadoCuenta.aspx** - Estado de cuenta
    - Ubicación: `JelaWeb/Views/Operacion/`

---

## ⚠️ Consideraciones Importantes

### NO Eliminar Si:

1. **La página usa queries personalizadas** que no pasan por DynamicCrudService
   - Solución: Migrar a DynamicCrudService o usar `EntidadHelper.AgregarFiltroEntidad()`

2. **La página necesita mostrar datos de múltiples entidades** (casos especiales)
   - Ejemplo: Reportes consolidados para super-administradores
   - Solución: Agregar lógica condicional basada en tipo de usuario

3. **La página es de administración de entidades** (como Entidades.aspx)
   - Estas páginas necesitan el campo de entidad

### Validar Después de Eliminar:

1. ✅ La página compila sin errores
2. ✅ Los registros se crean con `IdEntidad` correcto
3. ✅ Solo se ven registros de la entidad actual
4. ✅ No se pueden editar registros de otras entidades
5. ✅ No se pueden eliminar registros de otras entidades
6. ✅ El cambio de entidad funciona correctamente

---

## 🔍 Ejemplo Completo: Actualizar Conceptos.aspx

### Antes:

**Conceptos.aspx:**
```aspx
<div class="form-group">
    <label>Entidad:</label>
    <dx:ASPxComboBox ID="cmbEntidad" runat="server" Width="100%">
    </dx:ASPxComboBox>
</div>
<div class="form-group">
    <label>Nombre:</label>
    <dx:ASPxTextBox ID="txtNombre" runat="server" Width="100%">
    </dx:ASPxTextBox>
</div>
```

**Conceptos.aspx.vb:**
```vb
Protected Sub Page_Load(...)
    If Not IsPostBack Then
        CargarEntidades()
        CargarGrid()
    End If
End Sub

Private Sub CargarEntidades()
    Dim dt = DynamicCrudService.ObtenerTodos("cat_entidades")
    cmbEntidad.DataSource = dt
    cmbEntidad.DataBind()
End Sub

Protected Sub btnGuardar_Click(...)
    Dim campos As New Dictionary(Of String, Object)
    campos.Add("IdEntidad", cmbEntidad.Value)
    campos.Add("Nombre", txtNombre.Text)
    DynamicCrudService.Insertar("cat_conceptos", campos)
End Sub
```

### Después:

**Conceptos.aspx:**
```aspx
<!-- Campo de entidad eliminado -->
<div class="form-group">
    <label>Nombre:</label>
    <dx:ASPxTextBox ID="txtNombre" runat="server" Width="100%">
    </dx:ASPxTextBox>
</div>
```

**Conceptos.aspx.vb:**
```vb
Protected Sub Page_Load(...)
    If Not IsPostBack Then
        ' CargarEntidades() eliminado
        CargarGrid()
    End If
End Sub

' Método CargarEntidades() eliminado

Protected Sub btnGuardar_Click(...)
    Dim campos As New Dictionary(Of String, Object)
    ' IdEntidad se agrega automáticamente por DynamicCrudService
    campos.Add("Nombre", txtNombre.Text)
    DynamicCrudService.Insertar("cat_conceptos", campos)
End Sub
```

---

## 📊 Progreso de Limpieza

Marcar con ✅ cuando se complete cada página:

- [ ] 6.1 Cuotas.aspx
- [ ] 6.2 Unidades.aspx
- [ ] 6.3 Residentes.aspx
- [ ] 6.4 Conceptos.aspx
- [ ] 6.5 AreasComunes.aspx
- [ ] 6.6 Tickets.aspx
- [ ] 6.7 Comunicados.aspx
- [ ] 6.8 Reservaciones.aspx
- [ ] 6.9 Pagos.aspx
- [ ] 6.10 EstadoCuenta.aspx

---

## 🎯 Conclusión

Estas tareas son **mejoras cosméticas** que pueden completarse gradualmente sin afectar la funcionalidad del sistema. El sistema multi-entidad está completamente funcional y seguro sin necesidad de realizar estos cambios.

**Recomendación:** Completar estas tareas durante mantenimiento regular o cuando se actualice cada página por otras razones.

---

**Última Actualización:** 20 de Enero de 2026  
**Autor:** Sistema de Especificaciones JELA
