using UnityEngine;

public class TriggerTrampaGas : MonoBehaviour
{
    [Header("Configuración de la Trampa")]
    [SerializeField] private float _tiempoLimite = 30f;

    [Header("Efectos Visuales (Gas)")]
    [SerializeField] private ParticleSystem _humoGas; // Referencia al objeto del techo
    [SerializeField] private float _tiempoMaximoEmision = 20f; // Cuánto tiempo tardará en llenar la sala
    [SerializeField] private float _tasaEmisionInicial = 20f;
    [SerializeField] private float _tasaEmisionFinal = 200f;

    private bool _trampaActivada = false;
    private float _tiempoTranscurrido = 0f;
    private void Start()
    {
        // Al inicio, nos aseguramos de que el humo esté apagado
        if (_humoGas != null && _humoGas.isPlaying)
        {
            _humoGas.Stop();
        }
    }

    private void Update()
    {
        if (_trampaActivada)
        {
            _tiempoTranscurrido += Time.deltaTime;

            // Calculamos qué tan llena está la sala (de 0 a 1)
            float progreso = Mathf.Clamp01(_tiempoTranscurrido / _tiempoLimite);

            // Ajustamos la emisión dinámicamente
            if (_humoGas != null)
            {
                var emission = _humoGas.emission;
                emission.rateOverTime = Mathf.Lerp(_tasaEmisionInicial, _tasaEmisionFinal, progreso);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_trampaActivada)
        {
            _trampaActivada = true;

            Debug.Log("<b>[TRAMPA]</b> ¡Puertas bloqueadas! Gas letal liberado.");

            // 1. Cerramos las puertas hidráulicas (Invertimos el Lerp)
            if (PuertaGaleria.Instancia != null)
            {
                PuertaGaleria.Instancia.CerrarCompuertaDeGolpe();
            }

            // 2. Activamos la cuenta regresiva letal en la UI
            ManagerTerminalUI.Instancia.IniciarContrarrelojGas(_tiempoLimite);

            // ==========================================
            // ¡ACTIVAMOS EL HUMO REAL!
            // ==========================================
            if (_humoGas != null)
            {
                // Encendemos el sistema de partículas
                _humoGas.Play();

                // Opcional: Podemos forzar la tasa de emisión para que 
                // parezca que se llena más rápido al principio
                var emission = _humoGas.emission;
                emission.rateOverTime = 150;

                Debug.Log("<b>[FX]</b> Sistema de partículas de humo activado.");
            }
        }
    }

    public void ApagarHumo()
    {
        if (_humoGas != null)
        {
            _humoGas.Stop(); // Detiene la emisión de nuevas partículas inmediatamente
            Debug.Log("<b>[FX]</b> Sistema de partículas detenido. Extrayendo gas...");
        }
    }
}
