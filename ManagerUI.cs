using TMPro;
using UnityEngine;

public class ManagerUI : MonoBehaviour
{
    // Singleton simple para acceder al manager desde otros scripts sin acoplar
    public static ManagerUI Instancia { get; private set; }

    [Header("Componentes de la Interfaz")]
    [SerializeField] private GameObject _panelFicha;
    [SerializeField] private TextMeshProUGUI _txtTitulo;
    [SerializeField] private TextMeshProUGUI _txtAutorAño;
    [SerializeField] private TextMeshProUGUI _txtDescripcion;

    private bool _interfazActiva = false;
    public bool InterfazActiva => _interfazActiva;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void MostrarFicha(string titulo, string autor, int año, string descripcion)
    {
        _interfazActiva = true;

        // 1. Inyectar los datos del cuadro en los textos de la UI
        _txtTitulo.text = titulo;
        _txtAutorAño.text = $"{autor} ({año})";
        _txtDescripcion.text = descripcion;

        // 2. Mostrar el panel y liberar el mouse para interactuar si fuera necesario
        _panelFicha.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Pausar el tiempo del juego (Físicas del Rigidbody se congelan)
        Time.timeScale = 0f;
    }

    public void CerrarFicha()
    {
        _interfazActiva = false;
        _panelFicha.SetActive(false);

        // Volver a bloquear el mouse para la cámara en primera persona
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reanudar el tiempo del juego
        Time.timeScale = 1f;
    }
}
