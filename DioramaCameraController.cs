using UnityEngine;
using Unity.Cinemachine;

public class DioramaCameraController : MonoBehaviour
{
    [Header("Referencias Centrales")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private Transform rigCentro;

    [Header("Configuración de Órbita")]
    [SerializeField] private float velocidadRotacionX = 0.2f;
    [SerializeField] private float velocidadRotacionY = 0.2f;
    [SerializeField] private float limiteVerticalMin = 10f;  // Vista casi a ras de suelo
    [SerializeField] private float limiteVerticalMax = 80f;  // Vista casi cenital (desde arriba)

    [Header("Configuración de Zoom")]
    [SerializeField] private float velocidadZoom = 0.5f;
    [SerializeField] private float zoomMin = 4f;
    [SerializeField] private float zoomMax = 15f;

    private float _rotacionX; // Giro Horizontal (Yaw)
    private float _rotacionY = 30f; // Giro Vertical (Pitch) inicial seguro
    private float _distanciaObjetivo = 10f;
    private float _distanciaActual = 10f;
    private bool _mouseClickeado = false;
    private Vector2 _lastMousePosition;
    private CinemachineFollow _cameraFollowComponent;

    private void Awake()
    {
        if (virtualCamera != null)
        {
            _cameraFollowComponent = virtualCamera.GetComponent<CinemachineFollow>();
            if (_cameraFollowComponent != null)
            {
                // Calculamos la distancia inicial basándonos en la magnitud del offset actual
                _distanciaObjetivo = _cameraFollowComponent.FollowOffset.magnitude;
                _distanciaActual = _distanciaObjetivo;
            }
        }

        if (rigCentro != null)
        {
            _rotacionX = rigCentro.eulerAngles.y;
        }
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.OnClickStateChanged += SetMouseClickState;
            inputReader.OnMouseMoved += ProcesarOrbita;
            inputReader.OnMouseScroll += ProcesarZoom;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.OnClickStateChanged -= SetMouseClickState;
            inputReader.OnMouseMoved -= ProcesarOrbita;
            inputReader.OnMouseScroll -= ProcesarZoom;
        }
    }

    private void SetMouseClickState(bool estaPresionado) => _mouseClickeado = estaPresionado;

    private void ProcesarOrbita(Vector2 mousePosition)
    {
        // Bloqueo total si estamos inspeccionando un objeto individual
        if (InspectionManager.Instancia != null && InspectionManager.Instancia.EnModoInspeccion) return;
        if (MuseumUIManager.Instancia != null && MuseumUIManager.Instancia.InterfazActiva) return;

        if (!_mouseClickeado)
        {
            _lastMousePosition = mousePosition;
            return;
        }

        float deltaX = mousePosition.x - _lastMousePosition.x;
        float deltaY = mousePosition.y - _lastMousePosition.y;

        // 1. EL SECRETO: El Rig SÓLO rota en el eje Y (Giro horizontal puro, el horizonte jamás se inclinará)
        _rotacionX += deltaX * velocidadRotacionX;
        if (rigCentro != null)
        {
            rigCentro.rotation = Quaternion.Euler(0f, _rotacionX, 0f);
        }

        // 2. Registramos el ángulo vertical (Inclinación) de forma aislada
        _rotacionY += deltaY * velocidadRotacionY;
        _rotacionY = Mathf.Clamp(_rotacionY, limiteVerticalMin, limiteVerticalMax);

        _lastMousePosition = mousePosition;
    }

    private void Update()
    {
        if (_cameraFollowComponent == null) return;

        // Interpolación suave para el zoom por scroll
        _distanciaActual = Mathf.Lerp(_distanciaActual, _distanciaObjetivo, Time.deltaTime * 8f);

        // Convertimos el ángulo de inclinación a radianes para los cálculos matemáticos
        float rad = _rotacionY * Mathf.Deg2Rad;

        // Calculamos los componentes Y (Altura) y Z (Profundidad) del offset de forma limpia
        float targetY = Mathf.Sin(rad) * _distanciaActual;
        float targetZ = -Mathf.Cos(rad) * _distanciaActual;

        // Inyectamos el offset corregido a Cinemachine. X se queda en 0 para mantener el enfoque centrado.
        _cameraFollowComponent.FollowOffset = new Vector3(0f, targetY, targetZ);
    }

    private void ProcesarZoom(Vector2 scrollDelta)
    {
        // Bloqueo total si estamos inspeccionando un objeto individual
        if (InspectionManager.Instancia != null && InspectionManager.Instancia.EnModoInspeccion) return;
        if (MuseumUIManager.Instancia != null && MuseumUIManager.Instancia.InterfazActiva) return;
        if (scrollDelta.y == 0) return;

        // Si el scroll va hacia arriba, reducimos la distancia (Acercamiento)
        float modificador = scrollDelta.y > 0 ? -velocidadZoom : velocidadZoom;
        _distanciaObjetivo += modificador;
        _distanciaObjetivo = Mathf.Clamp(_distanciaObjetivo, zoomMin, zoomMax);
    }
}
