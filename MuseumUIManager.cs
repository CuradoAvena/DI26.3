using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MuseumUIManager : MonoBehaviour
{
    public static MuseumUIManager Instancia { get; private set; }

    [Header("Componentes del Panel UI")]
    [SerializeField] private GameObject panelInformacion;
    [SerializeField] private TextMeshProUGUI txtDescripcion;
    [SerializeField] private Button botonCerrar;

    public System.Action OnUIClosed;

    public bool InterfazActiva => panelInformacion != null && panelInformacion.activeSelf;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelInformacion != null) panelInformacion.SetActive(false);
        if (botonCerrar != null) botonCerrar.onClick.AddListener(CerrarPanel);
    }

    // Recibe el cuadro y pinta solo la descripción
    public void MostrarFicha(Cuadro cuadro)
    {
        if (cuadro == null) return;

        if (txtDescripcion != null) txtDescripcion.text = cuadro.Descripcion;
        if (panelInformacion != null) panelInformacion.SetActive(true);
    }

    public void CerrarPanel()
    {
        if (panelInformacion != null) panelInformacion.SetActive(false);
        OnUIClosed?.Invoke();
    }
}
