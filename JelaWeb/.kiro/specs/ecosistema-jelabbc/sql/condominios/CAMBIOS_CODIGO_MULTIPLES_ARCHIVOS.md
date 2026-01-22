# Cambios de Código - Sistema de Múltiples Archivos

## Resumen

Este documento describe todos los cambios necesarios en JavaScript y VB.NET para implementar el sistema de múltiples archivos por registro.

---

## Cambios en JavaScript (unidades.js)

### 1. Variables Globales - Reemplazar variables únicas por arrays

**ANTES:**
```javascript
var ineImageBase64 = null;
var tarjetaImageBase64 = null;
var documentoArchivoBase64 = null;
```

**DESPUÉS:**
```javascript
// Arrays para almacenar múltiples archivos
var ineArchivosBase64 = []; // Array de objetos: { nombre, base64, tipoMime, tamanio }
var tarjetaArchivosBase64 = [];
var documentoArchivosBase64 = [];
```

### 2. Función para manejar múltiples archivos INE

**AGREGAR nuevas funciones:**

```javascript
// Función para manejar selección múltiple de archivos INE
function onINEFileInputChange(e) {
    var files = e.target.files;
    if (!files || files.length === 0) return;
    
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if (file.size > 10 * 1024 * 1024) { // 10MB
            showToast('warning', 'El archivo ' + file.name + ' excede 10MB');
            continue;
        }
        
        var reader = new FileReader();
        reader.onload = function(file, result) {
            return function(e) {
                var base64Completo = e.target.result;
                var base64SinPrefijo = base64Completo;
                if (base64SinPrefijo.indexOf(',') > -1) {
                    base64SinPrefijo = base64SinPrefijo.split(',')[1];
                }
                
                ineArchivosBase64.push({
                    nombre: file.name,
                    base64: base64SinPrefijo,
                    base64Completo: base64Completo,
                    tipoMime: file.type || 'application/octet-stream',
                    tamanio: file.size
                });
                
                actualizarPreviewINE();
            };
        }(file, null);
        reader.readAsDataURL(file);
    }
}

// Función para actualizar preview de archivos INE
function actualizarPreviewINE() {
    var container = document.getElementById('ineFilesPreview');
    if (!container) return;
    
    container.innerHTML = '';
    
    ineArchivosBase64.forEach(function(archivo, index) {
        var div = document.createElement('div');
        div.style.cssText = 'position: relative; display: inline-block; margin: 5px;';
        
        if (archivo.base64Completo.indexOf('data:image/') === 0) {
            var img = document.createElement('img');
            img.src = archivo.base64Completo;
            img.style.cssText = 'max-width: 150px; max-height: 150px; border: 1px solid #ccc; border-radius: 4px;';
            div.appendChild(img);
        } else {
            var span = document.createElement('span');
            span.textContent = archivo.nombre;
            span.style.cssText = 'padding: 10px; border: 1px solid #ccc; border-radius: 4px; display: block;';
            div.appendChild(span);
        }
        
        var btn = document.createElement('button');
        btn.textContent = '×';
        btn.style.cssText = 'position: absolute; top: 5px; right: 5px; background: red; color: white; border: none; border-radius: 50%; width: 25px; height: 25px; cursor: pointer;';
        btn.onclick = function(idx) {
            return function() {
                ineArchivosBase64.splice(idx, 1);
                actualizarPreviewINE();
            };
        }(index);
        div.appendChild(btn);
        
        container.appendChild(div);
    });
    
    var filesList = document.getElementById('ineFilesList');
    if (ineArchivosBase64.length > 0) {
        filesList.style.display = 'block';
    } else {
        filesList.style.display = 'none';
    }
}

// Función para cargar archivos existentes de residente
function cargarArchivosResidente(residenteId) {
    if (!residenteId || residenteId === 0) return;
    
    ajaxCall('ObtenerArchivosResidente', { residenteId: residenteId }, function(r) {
        if (r.success && r.data) {
            gridArchivosResidente.PerformCallback('cargar|' + residenteId);
        }
    });
}

// Función para ver archivo de residente
function verArchivoResidente(container, archivoId) {
    var residenteId = parseInt(document.getElementById('hfResidenteId').value) || 0;
    window.open('/Views/Consultas/VisorArchivo.aspx?tipo=residente&id=' + residenteId + '&archivoId=' + archivoId, '_blank', 'width=1024,height=768');
}

// Función para eliminar archivo de residente
function eliminarArchivoResidente(container, archivoId) {
    if (!confirm('¿Eliminar este archivo?')) return;
    
    ajaxCall('EliminarArchivoResidente', { id: archivoId }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            var residenteId = parseInt(document.getElementById('hfResidenteId').value) || 0;
            cargarArchivosResidente(residenteId);
        } else {
            showToast('error', r.message);
        }
    });
}
```

### 3. Función guardarResidenteContinuar - Modificar para enviar múltiples archivos

**ANTES:**
```javascript
function guardarResidenteContinuar(unidadId) {
    var datos = {
        id: parseInt(document.getElementById('hfResidenteId').value) || 0,
        unidadId: unidadId,
        // ... otros campos ...
        imagenINEBase64: ineImageBase64 || null
    };
    // ...
}
```

**DESPUÉS:**
```javascript
function guardarResidenteContinuar(unidadId) {
    var datos = {
        id: parseInt(document.getElementById('hfResidenteId').value) || 0,
        unidadId: unidadId,
        tipoResidente: cboTipoResidente.GetValue(),
        esPrincipal: chkResPrincipal.GetChecked(),
        nombre: txtResNombre.GetValue(),
        apellidoPaterno: txtResApPaterno.GetValue(),
        apellidoMaterno: txtResApMaterno.GetValue(),
        email: txtResEmail.GetValue(),
        telefono: txtResTelefono.GetValue(),
        celular: txtResCelular.GetValue(),
        curp: txtResCURP.GetValue(),
        activo: chkResActivo.GetChecked(),
        archivos: ineArchivosBase64.map(function(a) {
            return {
                nombreArchivo: a.nombre,
                archivoBase64: a.base64,
                tipoMime: a.tipoMime,
                tamanioBytes: a.tamanio,
                tipoArchivo: 'INE' // Por defecto INE, pero se puede cambiar
            };
        })
    };
    
    ajaxCall('GuardarResidente', { datos: datos }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            var residenteId = r.data && r.data.id ? r.data.id : datos.id;
            if (residenteId > 0) {
                // Si hay archivos nuevos, guardarlos
                if (ineArchivosBase64.length > 0) {
                    guardarArchivosResidente(residenteId, ineArchivosBase64);
                }
                // Recargar grid de archivos
                cargarArchivosResidente(residenteId);
            }
            popupResidente.Hide();
            if (currentUnidadId) {
                cargarResidentesGrid(currentUnidadId);
            }
        } else {
            showToast('error', r.message);
        }
    });
}

// Nueva función para guardar archivos después de guardar el residente
function guardarArchivosResidente(residenteId, archivos) {
    archivos.forEach(function(archivo) {
        var datosArchivo = {
            residenteId: residenteId,
            tipoArchivo: 'INE',
            nombreArchivo: archivo.nombre,
            archivoBase64: archivo.base64,
            tipoMime: archivo.tipoMime,
            tamanioBytes: archivo.tamanio
        };
        
        ajaxCall('GuardarArchivoResidente', { datos: datosArchivo }, function(r) {
            if (!r.success) {
                console.error('Error al guardar archivo:', r.message);
            }
        });
    });
    
    // Limpiar array después de guardar
    ineArchivosBase64 = [];
    actualizarPreviewINE();
}
```

### 4. Función limpiarFormularioResidente - Modificar

**AGREGAR:**
```javascript
function limpiarFormularioResidente() {
    // ... código existente ...
    
    // Limpiar archivos
    ineArchivosBase64 = [];
    actualizarPreviewINE();
    var residenteId = parseInt(document.getElementById('hfResidenteId').value) || 0;
    if (residenteId > 0) {
        gridArchivosResidente.PerformCallback('cargar|' + residenteId);
    }
}
```

### 5. Función cargarDatosResidente - Modificar para cargar archivos

**AGREGAR al final:**
```javascript
function cargarDatosResidente(d) {
    // ... código existente ...
    
    // Cargar archivos del residente
    if (d.Id && d.Id > 0) {
        cargarArchivosResidente(d.Id);
    }
}
```

### 6. Inicialización - Agregar event listeners

**AGREGAR en función de inicialización:**
```javascript
// Inicializar drag & drop y file input para INE
(function() {
    var ineDropZone = document.getElementById('ineDropZone');
    var ineFileInput = document.getElementById('ineFileInput');
    
    if (ineDropZone && ineFileInput) {
        ineDropZone.addEventListener('click', function() {
            ineFileInput.click();
        });
        
        ineDropZone.addEventListener('dragover', function(e) {
            e.preventDefault();
            ineDropZone.style.backgroundColor = '#e0e0e0';
        });
        
        ineDropZone.addEventListener('dragleave', function(e) {
            e.preventDefault();
            ineDropZone.style.backgroundColor = '#f9f9f9';
        });
        
        ineDropZone.addEventListener('drop', function(e) {
            e.preventDefault();
            ineDropZone.style.backgroundColor = '#f9f9f9';
            var files = e.dataTransfer.files;
            if (files && files.length > 0) {
                ineFileInput.files = files;
                onINEFileInputChange({ target: { files: files } });
            }
        });
        
        ineFileInput.addEventListener('change', onINEFileInputChange);
    }
})();
```

### 7. Repetir cambios similares para Vehículos y Documentos

**Para vehículos:**
- Crear funciones similares pero con prefijo `tarjeta`
- Variables: `tarjetaArchivosBase64`
- Funciones: `onTarjetaFileInputChange`, `actualizarPreviewTarjeta`, `guardarArchivosVehiculo`, etc.

**Para documentos:**
- Crear funciones similares pero con prefijo `documento`
- Variables: `documentoArchivosBase64`
- Funciones: `onDocumentoFileInputChange`, `actualizarPreviewDocumento`, `guardarArchivosDocumento`, etc.

---

## Cambios en VB.NET (Unidades.aspx.vb)

### 1. WebMethod para obtener archivos de residente

**AGREGAR:**
```vb
<System.Web.Services.WebMethod()>
Public Shared Function ObtenerArchivosResidente(residenteId As Integer) As Object
    Try
        Dim query As String = "SELECT * FROM cat_residente_archivos WHERE ResidenteId = " & residenteId & " AND Activo = 1 ORDER BY FechaCreacion DESC"
        Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
        Return New With {.success = True, .data = ConvertDataTableToList(dt)}
    Catch ex As Exception
        Return New With {.success = False, .message = "Error: " & ex.Message}
    End Try
End Function
```

### 2. WebMethod para guardar archivo de residente

**AGREGAR:**
```vb
<System.Web.Services.WebMethod()>
Public Shared Function GuardarArchivoResidente(datos As Dictionary(Of String, Object)) As Object
    Try
        Dim datosGuardar As New Dictionary(Of String, Object) From {
            {"ResidenteId", Convert.ToInt32(datos("residenteId"))},
            {"TipoArchivo", datos("tipoArchivo")?.ToString()},
            {"NombreArchivo", datos("nombreArchivo")?.ToString()},
            {"ArchivoBase64", datos("archivoBase64")?.ToString()},
            {"TipoMime", datos("tipoMime")?.ToString()},
            {"TamanioBytes", If(datos.ContainsKey("tamanioBytes"), Convert.ToInt64(datos("tamanioBytes")), DBNull.Value)},
            {"Activo", 1}
        }
        
        Dim resultado = DynamicCrudService.Insertar("cat_residente_archivos", datosGuardar)
        Return New With {.success = resultado, .message = If(resultado, "Archivo guardado correctamente", "Error al guardar")}
    Catch ex As Exception
        Return New With {.success = False, .message = "Error: " & ex.Message}
    End Try
End Function
```

### 3. WebMethod para eliminar archivo de residente

**AGREGAR:**
```vb
<System.Web.Services.WebMethod()>
Public Shared Function EliminarArchivoResidente(id As Integer) As Object
    Try
        ' Eliminar físicamente o marcar como inactivo
        Dim datosActualizar As New Dictionary(Of String, Object) From {
            {"Activo", 0}
        }
        Dim resultado = DynamicCrudService.Actualizar("cat_residente_archivos", id, datosActualizar)
        Return New With {.success = resultado, .message = If(resultado, "Archivo eliminado", "Error")}
    Catch ex As Exception
        Return New With {.success = False, .message = "Error: " & ex.Message}
    End Try
End Function
```

### 4. Modificar GuardarResidente para manejar múltiples archivos

**MODIFICAR:**
```vb
<System.Web.Services.WebMethod()>
Public Shared Function GuardarResidente(datos As Dictionary(Of String, Object)) As Object
    Try
        Dim id As Integer = Convert.ToInt32(datos("id"))
        Dim datosGuardar As New Dictionary(Of String, Object) From {
            {"UnidadId", Convert.ToInt32(datos("unidadId"))},
            {"TipoResidente", datos("tipoResidente")?.ToString()},
            {"EsPrincipal", If(Convert.ToBoolean(datos("esPrincipal")), 1, 0)},
            {"Nombre", datos("nombre")?.ToString()},
            {"ApellidoPaterno", datos("apellidoPaterno")?.ToString()},
            {"ApellidoMaterno", datos("apellidoMaterno")?.ToString()},
            {"Email", datos("email")?.ToString()},
            {"Telefono", datos("telefono")?.ToString()},
            {"TelefonoCelular", datos("celular")?.ToString()},
            {"CURP", datos("curp")?.ToString()},
            {"Activo", If(Convert.ToBoolean(datos("activo")), 1, 0)}
        }
        
        Dim resultado As Boolean
        Dim nuevoId As Integer = id
        
        If id = 0 Then
            resultado = DynamicCrudService.Insertar("cat_residentes", datosGuardar)
            ' Obtener el ID del registro insertado
            ' (Esto depende de cómo funcione DynamicCrudService.Insertar - puede retornar el ID o necesitar una consulta adicional)
        Else
            resultado = DynamicCrudService.Actualizar("cat_residentes", id, datosGuardar)
            nuevoId = id
        End If
        
        ' Los archivos se guardan por separado desde JavaScript después de que se guarda el residente
        ' No es necesario guardarlos aquí ya que el JavaScript manejará el guardado de archivos
        
        Return New With {
            .success = resultado, 
            .message = If(resultado, "Guardado correctamente", "Error al guardar"),
            .data = New With {.id = nuevoId}
        }
    Catch ex As Exception
        Return New With {.success = False, .message = "Error: " & ex.Message}
    End Try
End Function
```

### 5. CustomCallback para gridArchivosResidente

**AGREGAR en code-behind:**
```vb
Protected Sub gridArchivosResidente_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)
    Try
        Dim partes = e.Parameters.Split("|"c)
        If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
            Dim residenteId As Integer = Integer.Parse(partes(1))
            Dim query As String = "SELECT * FROM cat_residente_archivos WHERE ResidenteId = " & residenteId & " AND Activo = 1 ORDER BY FechaCreacion DESC"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
            
            GenerarColumnasDinamicas(gridArchivosResidente, dt)
            
            Session("dtArchivosResidente") = dt
            gridArchivosResidente.DataSource = dt
            gridArchivosResidente.DataBind()
        End If
    Catch ex As Exception
        Logger.LogError("Unidades.gridArchivosResidente_CustomCallback", ex)
    End Try
End Sub
```

### 6. Repetir cambios similares para Vehículos y Documentos

**Para vehículos:**
- `ObtenerArchivosVehiculo(vehiculoId)`
- `GuardarArchivoVehiculo(datos)`
- `EliminarArchivoVehiculo(id)`
- `gridArchivosVehiculo_CustomCallback`

**Para documentos:**
- `ObtenerArchivosDocumento(documentoId)`
- `GuardarArchivoDocumento(datos)`
- `EliminarArchivoDocumento(id)`
- `gridArchivosDocumento_CustomCallback`

---

## Cambios en VisorArchivo.aspx.vb

### Modificar para soportar archivoId

**ANTES:**
```vb
Case "residente"
    archivoBase64 = ObtenerImagenINEResidente(id)
```

**DESPUÉS:**
```vb
Case "residente"
    Dim archivoIdStr As String = Request.QueryString("archivoId")
    If Not String.IsNullOrEmpty(archivoIdStr) AndAlso Integer.TryParse(archivoIdStr, archivoId) Then
        ' Obtener archivo específico desde cat_residente_archivos
        archivoBase64 = ObtenerArchivoResidente(archivoId)
        nombreArchivo = "INE_Residente_" & archivoId.ToString()
    Else
        ' Retrocompatibilidad: buscar en cat_residentes.ImagenINE si existe
        archivoBase64 = ObtenerImagenINEResidente(id)
        nombreArchivo = "INE_Residente_" & id.ToString()
    End If
```

**AGREGAR nueva función:**
```vb
Private Function ObtenerArchivoResidente(archivoId As Integer) As String
    Try
        Dim query As String = "SELECT ArchivoBase64, NombreArchivo FROM cat_residente_archivos WHERE Id = " & archivoId & " AND Activo = 1"
        Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
        
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso dt.Rows(0)("ArchivoBase64") IsNot Nothing AndAlso Not IsDBNull(dt.Rows(0)("ArchivoBase64")) Then
            Return dt.Rows(0)("ArchivoBase64").ToString()
        End If
        
        Return Nothing
    Catch ex As Exception
        Logger.LogError("VisorArchivo.ObtenerArchivoResidente", ex)
        Return Nothing
    End Try
End Function
```

**Repetir cambios similares para "vehiculo" y "documento"**

---

## Notas Importantes

1. **Retrocompatibilidad:** El sistema mantiene soporte para archivos en campos únicos mientras se migran los datos.

2. **Performance:** Considerar paginación en grids de archivos si hay muchos archivos por registro.

3. **Validaciones:** Agregar validaciones de tamaño máximo y tipos de archivo permitidos en el servidor también.

4. **Limpieza:** Los archivos eliminados se marcan como `Activo = 0`, no se eliminan físicamente. Considerar implementar limpieza periódica si es necesario.

5. **Migración:** Después de verificar que todo funciona, considerar eliminar los campos antiguos (`ImagenINE`, `TarjetaCirculacionBase64`, `ArchivoBase64`).
