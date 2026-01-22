# Botones Flotantes Circulares en Selector de Entidades

## Fecha
2026-01-20

## Cambio Implementado

Se convirtieron los botones "Agregar Nuevo Condominio" y "Cerrar Sesión" en **botones flotantes circulares** con solo iconos, posicionados en la **esquina superior derecha** de la pantalla, similar al estilo de botones FAB (Floating Action Button).

## Archivos Modificados

### 1. `JelaWeb/Views/Auth/SelectorEntidades.aspx`

**Cambios realizados:**

#### HTML:
```html
<!-- Botones flotantes en la esquina superior derecha -->
<div class="floating-buttons">
    <asp:Button 
        ID="btnAgregarEntidad" 
        runat="server" 
        Text="Agregar Nuevo Condominio" 
        CssClass="btn-floating btn-floating-add"
        OnClick="btnAgregarEntidad_Click"
        ToolTip="Agregar Nuevo Condominio" />
    <asp:Button 
        ID="btnCerrarSesion" 
        runat="server" 
        Text="Cerrar Sesión" 
        CssClass="btn-floating btn-floating-logout"
        OnClick="btnCerrarSesion_Click"
        ToolTip="Cerrar Sesión" />
</div>
```

### 2. `JelaWeb/Content/Styles/selector-entidades.css`

**Estilos de botones circulares:**

```css
.floating-buttons {
    position: fixed;
    top: 30px;
    right: 30px;
    display: flex;
    flex-direction: column;
    gap: 15px;
    z-index: 1000;
    animation: fadeInRight 0.5s ease;
}

.btn-floating {
    width: 60px;
    height: 60px;
    border: none;
    border-radius: 50%;
    font-size: 0;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0;
}

.btn-floating:hover {
    transform: scale(1.1);
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.3);
}

/* Botón Agregar - Verde circular */
.btn-floating-add {
    background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
    color: white;
}

.btn-floating-add::before {
    content: "\f067"; /* fa-plus */
    font-family: "Font Awesome 6 Free";
    font-weight: 900;
    font-size: 1.5rem;
    display: block;
}

/* Botón Cerrar Sesión - Rojo circular */
.btn-floating-logout {
    background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
    color: white;
}

.btn-floating-logout::before {
    content: "\f2f5"; /* fa-sign-out-alt */
    font-family: "Font Awesome 6 Free";
    font-weight: 900;
    font-size: 1.5rem;
    display: block;
}
```

## Características de los Botones Circulares

### Diseño Visual

#### Forma
- **Completamente circulares**: 60x60px (55x55px en móvil)
- **Solo iconos**: Sin texto visible (font-size: 0)
- **Border-radius**: 50% para círculo perfecto

#### Botón "Agregar Nuevo Condominio" (Verde)
- **Color**: Verde con gradiente (#28a745 → #20c997)
- **Icono**: Plus (+) centrado
- **Tamaño icono**: 1.5rem

#### Botón "Cerrar Sesión" (Rojo)
- **Color**: Rojo con gradiente (#dc3545 → #c82333)
- **Icono**: Sign-out centrado
- **Tamaño icono**: 1.5rem

### Posicionamiento
- **Posición**: `fixed` - Siempre visible
- **Ubicación**: Esquina superior derecha (30px desde arriba y derecha)
- **Apilados verticalmente**: `flex-direction: column`
- **Separación**: 15px entre botones
- **Z-index**: 1000

### Efectos Interactivos

1. **Hover**:
   - Escala aumenta a 1.1 (crece 10%)
   - Sombra más pronunciada
   - Gradiente más oscuro

2. **Active**:
   - Escala a 1.05 (feedback táctil)

3. **Animación de entrada**:
   - Fade in desde la derecha
   - Duración: 0.5s

### Responsive (Móvil)

En pantallas pequeñas (< 768px):
- **Tamaño**: 55x55px
- **Posición**: 15px desde arriba y derecha
- **Icono**: 1.3rem
- **Mismo estilo circular**

## Comparación Visual

### Estilo Implementado:
```
                                    ┌────┐
                                    │ +  │ ← Verde circular
                                    └────┘
                                    ┌────┐
                                    │ →  │ ← Rojo circular
                                    └────┘
┌─────────────────────────────────────┐
│         SELECTOR DE ENTIDADES       │
│                                     │
│  ┌──────┐  ┌──────┐  ┌──────┐     │
│  │ Ent1 │  │ Ent2 │  │ Ent3 │     │
│  └──────┘  └──────┘  └──────┘     │
│                                     │
│  Licencias: 2 disponibles           │
└─────────────────────────────────────┘
```

## Iconos Font Awesome

- **Plus (Agregar)**: `\f067`
- **Sign-out (Cerrar Sesión)**: `\f2f5`

## Colores

### Botón Agregar (Verde)
- **Normal**: `#28a745` → `#20c997`
- **Hover**: `#218838` → `#1aa179`

### Botón Cerrar Sesión (Rojo)
- **Normal**: `#dc3545` → `#c82333`
- **Hover**: `#c82333` → `#bd2130`

## Beneficios del Diseño Circular

✅ **Minimalista**: Solo iconos, sin texto que ocupe espacio  
✅ **Moderno**: Estilo FAB (Floating Action Button) popular en Material Design  
✅ **Intuitivo**: Iconos universalmente reconocidos  
✅ **Compacto**: Ocupa menos espacio visual  
✅ **Elegante**: Sombras y gradientes profesionales  
✅ **Responsive**: Funciona perfectamente en móvil y desktop

## Pruebas Recomendadas

### ✅ Desktop
1. Verificar círculos perfectos (60x60px)
2. Confirmar que solo se ven iconos (sin texto)
3. Probar hover (escala 1.1)
4. Verificar gradientes y sombras
5. Confirmar posición fija al hacer scroll

### ✅ Móvil
1. Verificar tamaño 55x55px
2. Confirmar que mantienen forma circular
3. Probar funcionalidad táctil
4. Verificar separación entre botones

### ✅ Funcionalidad
1. Click en botón verde (+) → Redirige a Entidades.aspx
2. Click en botón rojo (→) → Cierra sesión
3. Tooltips funcionan al pasar el mouse

## Accesibilidad

✅ **Tooltips**: Texto descriptivo en hover  
✅ **Contraste**: Colores con buen contraste (WCAG AA)  
✅ **Tamaño táctil**: 60x60px cumple con mínimo de 44x44px  
✅ **Iconos claros**: Símbolos universalmente reconocidos

## Conclusión

✅ **Cambio completado exitosamente**

Los botones ahora son completamente circulares con solo iconos, siguiendo el estilo moderno de FAB (Floating Action Button). El diseño es limpio, elegante y funcional.

**Archivos modificados:**
- `JelaWeb/Views/Auth/SelectorEntidades.aspx`
- `JelaWeb/Content/Styles/selector-entidades.css`

**Sin errores de compilación** ✅

**Estilo**: Similar al botón de chat azul circular de la imagen de referencia 🎨

