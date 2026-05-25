using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class CuadroMuseo : MonoBehaviour
{
    [Header("Información de la Obra")]
    [SerializeField] private string _nombreObra = "Sin Título";
    [SerializeField] private string _autor = "Anónimo";
    [SerializeField] private int _año = 2026;

    [Header("Efecto Visual (Feedback)")]
    [SerializeField] private Color _colorSeleccionado = Color.yellow;

  
    [Header("Efecto de Exposición (Feedback)")]
    [SerializeField] private float _factorEscala = 1.3f; // Qué tanto crece (Zoom)
    [SerializeField] private float _velocidadRotacion = 45f; // Grados por segundo

    private Vector3 _escalaOriginal;
    private Vector3 _escalaObjetivo;
    private bool _siendoMirado;
    private MeshRenderer _meshRenderer;
    private Color _colorOriginal;
    private void Awake()
    {
        // 1. Inicializar componentes físicos/visuales
        _meshRenderer = GetComponent<MeshRenderer>();
        _colorOriginal = _meshRenderer.material.color;

        // 2. Guardar dimensiones originales de la forma
        _escalaOriginal = transform.localScale;
        _escalaObjetivo = _escalaOriginal;
    }
    private void Update()
    {
        // 1. Transición suave de tamaño (Zoom)
        transform.localScale = Vector3.Lerp(transform.localScale, _escalaObjetivo, Time.deltaTime * 8f);

        // 2. Si el jugador lo está viendo, el cuadro rota sobre su eje Y
        if (_siendoMirado)
        {
            transform.Rotate(Vector3.up * _velocidadRotacion * Time.deltaTime, Space.World);
        }
    }

    public void MirarCuadro()
    {
        _siendoMirado = true;
        _escalaObjetivo = _escalaOriginal * _factorEscala; // Activa el Zoom
        _meshRenderer.material.color = _colorSeleccionado;
        Debug.Log($"<b>[EXPOSICIÓN]</b> Obra: {_nombreObra} | Autor: {_autor} ({_año})");
    }

    public void QuitarMirada()
    {
        _siendoMirado = false;
        _escalaObjetivo = _escalaOriginal; // Regresa a su tamaño normal
        _meshRenderer.material.color = _colorOriginal;

    }
}
