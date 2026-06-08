using System;
using TMPro;
using UnityEngine;

public class ManagerTerminalUI : MonoBehaviour
{

    public static ManagerTerminalUI Instancia { get; private set; }

    [Header("Componentes de Pantalla Mínimos")]
    [SerializeField] private GameObject _panelConsola;
    [SerializeField] private TextMeshProUGUI _txtEstatus;      // Único texto: para decir "Error", "Acceso Concedido" o "Escribe clave"
    [SerializeField] private TMP_InputField _inputContraseña; // El campo donde se digita

    [Header("Lógica del Puzzle Global")]
    [SerializeField] private float _voltajeRequerido = 1000f;
    private float _voltajeActualAcumulado = 0f;

    private Consola _consolaActualActiva;
    private bool _pantallaAbierta = false;
    public bool PantallaAbierta => _pantallaAbierta;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void AbrirTerminal(Consola consola)
    {
        _consolaActualActiva = consola;
        _pantallaAbierta = true;

        _inputContraseña.text = ""; // Limpiar lo que se escribió antes

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

        // Congelar juego y liberar mouse
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _inputContraseña.ActivateInputField(); // Enfocar para escribir directo
    }

    public void ValidarCodigoIntroducido()
    {
        if (_consolaActualActiva == null || _consolaActualActiva.YaHackeada) return;

        // Validar contraseña
        if (_inputContraseña.text == _consolaActualActiva.ContraseñaCorrecta)
        {
            _voltajeActualAcumulado += _consolaActualActiva.Voltaje;
            _consolaActualActiva.MarcarComoHackeada();

            _txtEstatus.text = $"CORRECTO. +{_consolaActualActiva.Voltaje}V INYECTADOS.";
            _inputContraseña.interactable = false;

            Debug.Log($"<b>[SISTEMA]</b> Energía: {_voltajeActualAcumulado} / {_voltajeRequerido}V");

            if (_voltajeActualAcumulado >= _voltajeRequerido)
            {
                // Especificamos el namespace completo (UnityEngine.Object) para quitar la ambigüedad
                PuertaGaleria puerta = UnityEngine.Object.FindFirstObjectByType<PuertaGaleria>();

                if (puerta != null)
                {
                    puerta.AbrirCompuerta();
                }
                else
                {
                    Debug.LogError("❌ <b>[ERROR]</b> No se encontró ningún objeto con el script PuertaGaleria en la escena.");
                }
            }

            CerrarTerminal();
        }
        else
        {
            _txtEstatus.text = "CÓDIGO ERRONEO. INTENTE DE NUEVO.";
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

    public void MostrarInstruccionesLobby(string mensaje)
    {
        _pantallaAbierta = true;

        // Usamos el mismo texto de estatus que ya teníamos para mostrar las instrucciones
        _txtEstatus.text = mensaje;

        // Apagamos el campo de contraseña y el botón porque aquí no los necesita, solo es leer
        _inputContraseña.gameObject.SetActive(false);

        _panelConsola.SetActive(true);

        // Congelamos el juego para que lea con calma
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
