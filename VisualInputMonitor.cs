using UnityEngine;
using TMPro; // Necesario para controlar TextMeshPro

// Este script depende completamente de que exista un InputReader
[RequireComponent(typeof(InputReader))]
public class VisualInputMonitor : MonoBehaviour
{
    [Header("Referencias de UI (TMP)")]
    [SerializeField] private TextMeshProUGUI posicionText;
    [SerializeField] private TextMeshProUGUI scrollText;
    [SerializeField] private TextMeshProUGUI clickStateText;

    [Header("Configuración de Colores")]
    [SerializeField] private Color colorNormal = Color.gray;
    [SerializeField] private Color colorPresionado = Color.green;

    private InputReader _inputReader;

    private void Awake() => _inputReader = GetComponent<InputReader>();

    private void OnEnable()
    {
        _inputReader.OnMouseMoved += ActualizarPosicionVisual;
        _inputReader.OnMouseScroll += ActualizarScrollVisual;
        _inputReader.OnClickStateChanged += ActualizarClicVisual;
    }

    private void OnDisable()
    {
        _inputReader.OnMouseMoved -= ActualizarPosicionVisual;
        _inputReader.OnMouseScroll -= ActualizarScrollVisual;
        _inputReader.OnClickStateChanged -= ActualizarClicVisual;
    }

    private void ActualizarPosicionVisual(Vector2 posicionActual)
    {
        if (posicionText != null)
            posicionText.text = $"Pos Mouse: ({posicionActual.x:F0}, {posicionActual.y:F0})";
    }

    private void ActualizarScrollVisual(Vector2 scrollDelta)
    {
        if (scrollText != null)
            scrollText.text = $"Scroll Delta Y: {scrollDelta.y:F1}";
    }

    private void ActualizarClicVisual(bool estaPresionado)
    {
        if (clickStateText == null) return;

        if (estaPresionado)
        {
            clickStateText.text = "Estado Clic: [PRESIONANDO]";
            clickStateText.color = colorPresionado;
        }
        else
        {
            clickStateText.text = "Estado Clic: [LIBERADO/CLICK]";
            clickStateText.color = colorNormal;
        }
    }
}