using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Cuadro : MonoBehaviour
{
    [Header("Información de la Obra")]
    [SerializeField, TextArea(3, 5)] private string _descripcionObra = "Escribe aquí la historia o datos de la obra...";

    // Propiedad pública para que los mánagers lean la descripción
    public string Descripcion => _descripcionObra;

    // Almacén de transformación para el retorno automático
    public Vector3 PosicionOriginal { get; private set; }
    public Quaternion RotacionOriginal { get; private set; }
    public Vector3 EscalaOriginal { get; private set; }

    private MeshRenderer _meshRenderer;
    private Color _colorOriginal;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _colorOriginal = _meshRenderer.material.color;

        // Guardamos su estado inicial en el espacio
        PosicionOriginal = transform.position;
        RotacionOriginal = transform.rotation;
        EscalaOriginal = transform.localScale;
    }

    public void MirarCuadro()
    {
        _meshRenderer.material.color = Color.yellow;
        Debug.Log("<b>[EXPOSICIÓN]</b> Viendo un objeto interactivo.");
    }

    public void QuitarMirada()
    {
        _meshRenderer.material.color = _colorOriginal;
    }
}