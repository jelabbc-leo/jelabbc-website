Imports System.Web
Imports System.Web.Caching

''' <summary>
''' Clase helper para gestión de caché de datos frecuentes
''' Utiliza el caché de ASP.NET para mejorar el rendimiento
''' </summary>
Public NotInheritable Class CacheHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ' Tiempos de expiración por defecto (en minutos)
    Private Const DEFAULT_CACHE_MINUTES As Integer = 30
    Private Const SHORT_CACHE_MINUTES As Integer = 5
    Private Const LONG_CACHE_MINUTES As Integer = 60

    ''' <summary>
    ''' Obtiene un valor del caché
    ''' </summary>
    Public Shared Function GetValue(Of T)(key As String) As T
        Dim cache = HttpContext.Current.Cache

        If cache IsNot Nothing AndAlso cache(key) IsNot Nothing Then
            Return DirectCast(cache(key), T)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Almacena un valor en el caché con expiración por defecto (30 minutos)
    ''' </summary>
    Public Shared Sub SetValue(Of T)(key As String, value As T)
        SetValue(key, value, TimeSpan.FromMinutes(DEFAULT_CACHE_MINUTES))
    End Sub

    ''' <summary>
    ''' Almacena un valor en el caché con tiempo de expiración personalizado
    ''' </summary>
    Public Shared Sub SetValue(Of T)(key As String, value As T, expiration As TimeSpan)
        Dim cache = HttpContext.Current.Cache

        If cache IsNot Nothing Then
            cache.Insert(key, value, Nothing, DateTime.Now.Add(expiration), Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Almacena un valor en el caché con dependencia de archivo
    ''' </summary>
    Public Shared Sub SetWithFileDependency(Of T)(key As String, value As T, filePath As String)
        Dim cache = HttpContext.Current.Cache

        If cache IsNot Nothing Then
            Dim dependency As New CacheDependency(filePath)

            cache.Insert(key, value, dependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Almacena un valor en el caché con expiración deslizante (sliding expiration)
    ''' </summary>
    Public Shared Sub SetWithSlidingExpiration(Of T)(key As String, value As T, slidingExpiration As TimeSpan)
        Dim cache = HttpContext.Current.Cache

        If cache IsNot Nothing Then
            cache.Insert(key, value, Nothing, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Normal, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Verifica si una clave existe en el caché
    ''' </summary>
    Public Shared Function Exists(key As String) As Boolean
        Dim cache = HttpContext.Current.Cache

        Return cache IsNot Nothing AndAlso cache(key) IsNot Nothing
    End Function

    ''' <summary>
    ''' Elimina un valor del caché
    ''' </summary>
    Public Shared Sub Remove(key As String)
        Dim cache = HttpContext.Current.Cache

        If cache IsNot Nothing Then
            cache.Remove(key)
        End If
    End Sub

    ''' <summary>
    ''' Limpia todo el caché
    ''' </summary>
    Public Shared Sub Clear()
        Dim cache = HttpContext.Current.Cache

        If cache IsNot Nothing Then
            Dim enumerator = cache.GetEnumerator()

            While enumerator.MoveNext()

                cache.Remove(enumerator.Key.ToString())
            End While
        End If
    End Sub

    ''' <summary>
    ''' Obtiene o crea un valor en el caché usando una función
    ''' </summary>
    Public Shared Function GetOrSet(Of T)(key As String, factory As Func(Of T), Optional expirationMinutes As Integer = DEFAULT_CACHE_MINUTES) As T
        Dim cached = GetValue(Of T)(key)

        If cached IsNot Nothing Then
            Return cached
        End If

        Dim value = factory()

        SetValue(key, value, TimeSpan.FromMinutes(expirationMinutes))
        Return value
    End Function

    ''' <summary>
    ''' Obtiene o crea un valor en el caché de corta duración (5 minutos)
    ''' </summary>
    Public Shared Function GetOrSetShort(Of T)(key As String, factory As Func(Of T)) As T
        Return GetOrSet(key, factory, SHORT_CACHE_MINUTES)
    End Function

    ''' <summary>
    ''' Obtiene o crea un valor en el caché de larga duración (60 minutos)
    ''' </summary>
    Public Shared Function GetOrSetLong(Of T)(key As String, factory As Func(Of T)) As T
        Return GetOrSet(key, factory, LONG_CACHE_MINUTES)
    End Function

    ' Constantes para claves de caché comunes - Usar Constants.vb en su lugar
    Public Const CACHE_KEY_REGIMEN As String = "Cache_RegimenFiscal"
    Public Const CACHE_KEY_FORMA_PAGO As String = "Cache_FormaPago"
    Public Const CACHE_KEY_METODO_PAGO As String = "Cache_MetodoPago"
    Public Const CACHE_KEY_USOS_CFDI As String = "Cache_UsosCFDI"
    Public Const CACHE_KEY_TIPOS_DOC As String = "Cache_TiposDocumentos"
    Public Const CACHE_KEY_CONCEPTOS As String = "Cache_Conceptos"
    Public Const CACHE_KEY_CONCEPTOS_COMBO As String = "Cache_ConceptosCombo"

End Class
