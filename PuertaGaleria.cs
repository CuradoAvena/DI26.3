using UnityEngine;

public class PuertaGaleria : MonoBehaviour
{
    public static PuertaGaleria Instancia { get; private set; }

    [Header("Componentes de la Compuerta")]
    [SerializeField] private Transform _hojaIzquierda;
    [SerializeField] private Transform _hojaDerecha;

    [Header("Configuración del Movimiento")]
    [SerializeField] private float _distanciaDesplazamiento = 2.5f;
    [SerializeField] private float _velocidadApertura = 3f;

    private Vector3 _posicionInicialIzquierda;
    private Vector3 _posicionInicialDerecha;
    private Vector3 _posicionObjetivoIzquierda;
    private Vector3 _posicionObjetivoDerecha;

    private bool _enMovimiento = false;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        if (_hojaIzquierda != null && _hojaDerecha != null)
        {
            // Guardamos las posiciones cerradas de fábrica
            _posicionInicialIzquierda = _hojaIzquierda.localPosition;
            _posicionInicialDerecha = _hojaDerecha.localPosition;

            // Calculamos las abiertas
            _posicionObjetivoIzquierda = _posicionInicialIzquierda - new Vector3(_distanciaDesplazamiento, 0, 0);
            _posicionObjetivoDerecha = _posicionInicialDerecha + new Vector3(_distanciaDesplazamiento, 0, 0);
        }
    }

    private void Update()
    {
        if (_enMovimiento)
        {
            _hojaIzquierda.localPosition = Vector3.Lerp(_hojaIzquierda.localPosition, _posicionObjetivoIzquierda, Time.deltaTime * _velocidadApertura);
            _hojaDerecha.localPosition = Vector3.Lerp(_hojaDerecha.localPosition, _posicionObjetivoDerecha, Time.deltaTime * _velocidadApertura);

            if (Vector3.Distance(_hojaIzquierda.localPosition, _posicionObjetivoIzquierda) < 0.01f)
            {
                _enMovimiento = false;
            }
        }
    }

    public void AbrirCompuerta()
    {
        _posicionObjetivoIzquierda = _posicionInicialIzquierda - new Vector3(_distanciaDesplazamiento, 0, 0);
        _posicionObjetivoDerecha = _posicionInicialDerecha + new Vector3(_distanciaDesplazamiento, 0, 0);
        _enMovimiento = true;
    }

    public void CerrarCompuertaDeGolpe()
    {
        // Revertimos las posiciones objetivo a sus puntos cerrados originales
        _posicionObjetivoIzquierda = _posicionInicialIzquierda;
        _posicionObjetivoDerecha = _posicionInicialDerecha;
        _enMovimiento = true;

        // Reactivamos el colisionador para asegurar que el jugador quede atrapado
        if (TryGetComponent<BoxCollider>(out var colisionador))
        {
            colisionador.enabled = true;
        }
    }
}
