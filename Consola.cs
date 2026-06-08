using UnityEngine;

public class Consola : MonoBehaviour
{
    [Header("Configuración de Seguridad")]
    [SerializeField] private string _idTerminal = "CONSOLA_ALFA";
    [SerializeField] private float _voltajeActual = 300f;
    [SerializeField] private string _contraseñaCorrecta = "1234"; // Nueva variable de clave
    [SerializeField, TextArea(2, 4)] private string _diagnosticoSistema = "SISTEMA BLOQUEADO. INGRESE CÓDIGO DE ACCESO.";

    [Header("Efecto Visual (Transform)")]
    [SerializeField] private float _factorEscala = 1.2f;

    private Vector3 _escalaOriginal;
    private Vector3 _escalaObjetivo;
    private MeshRenderer _meshRenderer;
    private bool _yaHackeada = false;

    // Getters
    public string IdTerminal => _idTerminal;
    public float Voltaje => _voltajeActual;
    public string ContraseñaCorrecta => _contraseñaCorrecta;
    public string Diagnostico => _diagnosticoSistema;
    public bool YaHackeada => _yaHackeada;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _escalaOriginal = transform.localScale;
        _escalaObjetivo = _escalaOriginal;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _escalaObjetivo, Time.deltaTime * 8f);
    }

    public void DetectarConsola() => _escalaObjetivo = _escalaOriginal * _factorEscala;
    public void PerderConsola() => _escalaObjetivo = _escalaOriginal;

    public void MarcarComoHackeada()
    {
        _yaHackeada = true;
        _meshRenderer.material.color = Color.green; // Feedback visual
    }
}

