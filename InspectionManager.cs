using UnityEngine;

public class InspectionManager : MonoBehaviour
{
    // SINGLETON: Para que DetectorInteraccion pueda saber el estado del mánager
    public static InspectionManager Instancia { get; private set; }

    [Header("Referencias Centrales")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform puntoInspeccion;

    [Header("Configuración de Manipulación")]
    [SerializeField] private float velocidadRotacion = 0.4f;
    [SerializeField] private float velocidadEscala = 0.05f;

    private Cuadro _objetoActual;
    private bool _enModoInspeccion = false;
    private bool _mouseClickeado = false;
    private float _lastMouseX;
    private Vector3 _escalaObjetivo;

    // Propiedad pública para bloquear el Raycast externo
    public bool EnModoInspeccion => _enModoInspeccion;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (inputReader != null)
        {
            inputReader.OnClickStateChanged += ActivarRotacionMouse;
            inputReader.OnMouseMoved += RotarObjeto;
            inputReader.OnMouseScroll += EscalarObjeto;
            inputReader.OnInteractPressed += EscucharTeclasInpseccion;
        }

        if (MuseumUIManager.Instancia != null)
        {
            MuseumUIManager.Instancia.OnUIClosed += TerminarInspeccion;
        }
    }
    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.OnClickStateChanged += ActivarRotacionMouse;
            inputReader.OnMouseMoved += RotarObjeto;
            inputReader.OnMouseScroll += EscalarObjeto;
            inputReader.OnInteractPressed += EscucharTeclasInpseccion;
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
            inputReader.OnInteractPressed -= EscucharTeclasInpseccion;
        }

        if (MuseumUIManager.Instancia != null)
        {
            MuseumUIManager.Instancia.OnUIClosed -= TerminarInspeccion;
        }
    }

    public void IniciarInspeccion(Cuadro objetoTocado)
    {
        if (_enModoInspeccion) return;

        _objetoActual = objetoTocado;
        _enModoInspeccion = true;
        _escalaObjetivo = _objetoActual.transform.localScale;

        // Apagamos colisionador para la manipulación
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

    private void ActivarRotacionMouse(bool estaPresionado) => _mouseClickeado = estaPresionado;

    private void RotarObjeto(Vector2 mousePosition)
    {
        if (!_enModoInspeccion || !_mouseClickeado || _objetoActual == null)
        {
            _lastMouseX = mousePosition.x;
            return;
        }
        float deltaX = mousePosition.x - _lastMouseX;
        _objetoActual.transform.Rotate(Vector3.up, -deltaX * velocidadRotacion, Space.World);
        _lastMouseX = mousePosition.x;
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

    private void EscucharTeclasInpseccion()
    {
        if (_enModoInspeccion && _objetoActual != null)
        {
            MuseumUIManager.Instancia.MostrarFicha(_objetoActual);
        }
    }

    private void TerminarInspeccion()
    {
        if (_objetoActual == null) return;

        // Regresa seguro a sus coordenadas iniciales
        _objetoActual.transform.position = _objetoActual.PosicionOriginal;
        _objetoActual.transform.rotation = _objetoActual.RotacionOriginal;
        _objetoActual.transform.localScale = _objetoActual.EscalaOriginal;

        // Devolvemos el colisionador para futuros clics
        if (_objetoActual.TryGetComponent<Collider>(out var col)) col.enabled = true;

        _objetoActual = null;
        _enModoInspeccion = false;
    }
}
