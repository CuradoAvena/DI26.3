using UnityEngine;

public class InspectionManager : MonoBehaviour
{
    public static InspectionManager Instancia { get; private set; }

    [Header("Referencias Centrales")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform puntoInspeccion;

    [Header("Aislamiento del Entorno")]
    [SerializeField] private GameObject estructuraVisualHabitacion; // Aquí va _DioramaRoot

    [Header("Configuración de Manipulación")]
    [SerializeField] private float velocidadRotacion = 0.25f;
    [SerializeField] private float velocidadEscala = 0.05f;

    private Cuadro _objetoActual;
    private bool _enModoInspeccion = false;
    private bool _mouseClickeado = false;
    private Vector2 _lastMousePosition;
    private Vector3 _escalaObjetivo;
    private Camera _mainCamera;

    public bool EnModoInspeccion => _enModoInspeccion;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            _mainCamera = Camera.main;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.OnClickStateChanged += ActivarRotacionMouse;
            inputReader.OnMouseMoved += RotarObjeto;
            inputReader.OnMouseScroll += EscalarObjeto;
            inputReader.OnInteractPressed += EscucharTeclasInspeccion; // <--- CABLE RE-CONECTADO
        }

        if (MuseumUIManager.Instancia != null)
        {
            MuseumUIManager.Instancia.OnUIClosed += TerminarInspeccion;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.OnClickStateChanged -= ActivarRotacionMouse;
            inputReader.OnMouseMoved -= RotarObjeto;
            inputReader.OnMouseScroll -= EscalarObjeto;
            inputReader.OnInteractPressed -= EscucharTeclasInspeccion; // <--- LIMPIEZA DE EVENTO
        }

        if (MuseumUIManager.Instancia != null)
        {
            MuseumUIManager.Instancia.OnUIClosed -= TerminarInspeccion;
        }
    }

    private void ActivarRotacionMouse(bool estaPresionado) => _mouseClickeado = estaPresionado;

    public void IniciarInspeccion(Cuadro objetoTocado)
    {
        if (_enModoInspeccion) return;

        _objetoActual = objetoTocado;
        _enModoInspeccion = true;
        _escalaObjetivo = _objetoActual.transform.localScale;

        if (estructuraVisualHabitacion != null)
        {
            estructuraVisualHabitacion.SetActive(false);
        }

        if (_objetoActual.TryGetComponent<Collider>(out var col)) col.enabled = false;
    }

    private void Update()
    {
        if (!_enModoInspeccion || _objetoActual == null) return;

        if (puntoInspeccion != null)
        {
            _objetoActual.transform.position = Vector3.Lerp(_objetoActual.transform.position, puntoInspeccion.position, Time.deltaTime * 8f);
        }
        _objetoActual.transform.localScale = Vector3.Lerp(_objetoActual.transform.localScale, _escalaObjetivo, Time.deltaTime * 8f);
    }

    private void RotarObjeto(Vector2 mousePosition)
    {
        if (!_enModoInspeccion || !_mouseClickeado || _objetoActual == null)
        {
            _lastMousePosition = mousePosition;
            return;
        }

        float deltaX = mousePosition.x - _lastMousePosition.x;
        float deltaY = mousePosition.y - _lastMousePosition.y;

        if (_mainCamera != null)
        {
            _objetoActual.transform.Rotate(_mainCamera.transform.up, -deltaX * velocidadRotacion, Space.World);
            _objetoActual.transform.Rotate(_mainCamera.transform.right, deltaY * velocidadRotacion, Space.World);
        }

        _lastMousePosition = mousePosition;
    }

    private void EscalarObjeto(Vector2 scrollDelta)
    {
        if (!_enModoInspeccion || _objetoActual == null || scrollDelta.y == 0) return;

        float modificador = scrollDelta.y > 0 ? velocidadEscala : -velocidadEscala;
        Vector3 nuevaEscala = _escalaObjetivo + Vector3.one * modificador;

        float min = _objetoActual.EscalaOriginal.x * 0.5f;
        float max = _objetoActual.EscalaOriginal.x * 2.5f;
        nuevaEscala.x = Mathf.Clamp(nuevaEscala.x, min, max);
        nuevaEscala.y = Mathf.Clamp(nuevaEscala.y, min, max);
        nuevaEscala.z = Mathf.Clamp(nuevaEscala.z, min, max);

        _escalaObjetivo = nuevaEscala;
    }

    
    private void EscucharTeclasInspeccion()
    {
        if (_enModoInspeccion && _objetoActual != null && MuseumUIManager.Instancia != null)
        {
            Debug.Log("<color=green>[Mánager]</color> Tecla E detectada. Abriendo ficha técnica.");
            MuseumUIManager.Instancia.MostrarFicha(_objetoActual);
        }
    }

    private void TerminarInspeccion()
    {
        if (_objetoActual == null) return;

        // Encendemos el entorno al salir
        if (estructuraVisualHabitacion != null)
        {
            estructuraVisualHabitacion.SetActive(true);
        }

        // Retorno limpio a las variables nativas de tu componente
        _objetoActual.transform.position = _objetoActual.PosicionOriginal;
        _objetoActual.transform.rotation = _objetoActual.RotacionOriginal;
        _objetoActual.transform.localScale = _objetoActual.EscalaOriginal;

        if (_objetoActual.TryGetComponent<Collider>(out var col)) col.enabled = true;

        _objetoActual = null;
        _enModoInspeccion = false;
    }
}
