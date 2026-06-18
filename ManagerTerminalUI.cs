using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ManagerTerminalUI : MonoBehaviour
{
    public static ManagerTerminalUI Instancia { get; private set; }

    [Header("Componentes de Pantalla Mínimos")]
    [SerializeField] private GameObject _panelConsola;
    [SerializeField] private TextMeshProUGUI _txtEstatus;
    [SerializeField] private TMP_InputField _inputContraseña;

    [Header("Lógica del Puzzle Global")]
    [SerializeField] private float _voltajeRequerido = 1000f;
    private float _voltajeActualAcumulado = 0f;

    [Header("Lógica de la Trampa de Gas")]
    [SerializeField] private GameObject _panelTimerMuerte; // Un texto grande en la pantalla del jugador
    [SerializeField] private TextMeshProUGUI _txtTimer;


    [Header("Paneles de Estado Final")]
    [SerializeField] private GameObject _panelGameOver;
    [SerializeField] private GameObject _panelVictoria;

    private float _tiempoRestanteGas;
    private bool _cronometroActivo = false;
    private Consola _consolaActualActiva;
    private bool _pantallaAbierta = false;

    public bool PantallaAbierta => _pantallaAbierta;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (_cronometroActivo)
        {
            _tiempoRestanteGas -= Time.deltaTime;

            // Formateamos el texto a un decimal
            _txtTimer.text = $"ALERTA: GAS EN {_tiempoRestanteGas:F1}s";

           
            if (_tiempoRestanteGas <= 10f)
            {
                // Hace que el texto parpadee usando una onda seno matemática
                float parpadeo = Mathf.Abs(Mathf.Sin(Time.time * 10f));
                _txtTimer.color = Color.Lerp(Color.yellow, Color.red, parpadeo);

                // Incrementa la escala ligeramente para que "late" con urgencia
                _txtTimer.transform.localScale = Vector3.one * (1f + (parpadeo * 0.1f));
            }
            else
            {
                _txtTimer.color = Color.white;
                _txtTimer.transform.localScale = Vector3.one;
            }

            // Si el tiempo muere, se acaba el juego
            if (_tiempoRestanteGas <= 0f)
            {
                ProcesarMuertePorGas();
            }
        }
    }

    public void IniciarContrarrelojGas(float tiempo)
    {
        _tiempoRestanteGas = tiempo;
        _cronometroActivo = true;
        _panelTimerMuerte.SetActive(true); // Encendemos el reloj en el HUD del jugador
    }

    public void DetenerTemporizador()
    {
        _cronometroActivo = false;
        _panelTimerMuerte.SetActive(false);

        // Apagamos el humo
        TriggerTrampaGas trampa = UnityEngine.Object.FindFirstObjectByType<TriggerTrampaGas>();
        if (trampa != null)
        {
            trampa.ApagarHumo();
        }

        Debug.Log("<b>[SISTEMA]</b> Temporizador detenido con éxito.");

        // ACTIVACIÓN DEL PANEL DE VICTORIA
        if (_panelVictoria != null)
        {
            _panelVictoria.SetActive(true);
        }

        // Congelamos el mundo y liberamos el mouse
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ProcesarMuertePorGas()
    {
        _cronometroActivo = false;
        _panelTimerMuerte.SetActive(false); // Apagamos el reloj parpadeante

        Debug.LogError("<b>[GAME OVER]</b> El jugador se quedó sin aire.");

        // ACTIVACIÓN DEL PANEL DE MUERTE
        if (_panelGameOver != null)
        {
            _panelGameOver.SetActive(true);
        }

        // Congelamos el juego y liberamos el cursor para que puedan picar "Reintentar"
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- LÓGICA DE HACKEO ANTERIOR ---
    public void AbrirTerminal(Consola consola)
    {
        _consolaActualActiva = consola;
        _pantallaAbierta = true;
        _inputContraseña.text = "";

        if (consola.YaHackeada)
        {
            _txtEstatus.text = $"SISTEMA ONLINE: {consola.Voltaje}V DISPARADOS.";
            _inputContraseña.interactable = false;
        }
        else
        {
            _txtEstatus.text = "INGRESE CÓDIGO DE ACCESO:";
            _inputContraseña.interactable = true;
        }

        _panelConsola.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inputContraseña.ActivateInputField();
    }

    public void ValidarCodigoIntroducido()
    {
        if (_consolaActualActiva == null || _consolaActualActiva.YaHackeada) return;

        if (_inputContraseña.text == _consolaActualActiva.ContraseñaCorrecta)
        {
            _voltajeActualAcumulado += _consolaActualActiva.Voltaje;
            _consolaActualActiva.MarcarComoHackeada();

            // Si es la consola maestra y metió la clave correcta, desactivamos el gas
            if (_consolaActualActiva.IdTerminal == "CONSOLA_MAESTRA")
            {
                DetenerTemporizador();
            }

            if (_voltajeActualAcumulado >= _voltajeRequerido)
            {
                PuertaGaleria puerta = UnityEngine.Object.FindFirstObjectByType<PuertaGaleria>();
                if (puerta != null) puerta.AbrirCompuerta();
            }

            CerrarTerminal();
        }
        else
        {
            _txtEstatus.text = "❌ CÓDIGO ERRONEO. INTENTE DE NUEVO.";
            _inputContraseña.text = "";
            _inputContraseña.ActivateInputField();
        }
    }

    public void CerrarTerminal()
    {
        _pantallaAbierta = false;
        _panelConsola.SetActive(false);
        _consolaActualActiva = null;
        _inputContraseña.gameObject.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- MÉTODO DEL TUTORIAL AUTOMÁTICO ANTERIOR ---
    public void MostrarInstruccionesLobby(string mensaje)
    {
        _pantallaAbierta = true;
        _txtEstatus.text = mensaje;
        _inputContraseña.gameObject.SetActive(false);
        _panelConsola.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void BotonReintentar()
    {
        Time.timeScale = 1f; // Devuelve el tiempo a la normalidad antes de reiniciar
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaActual);
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
