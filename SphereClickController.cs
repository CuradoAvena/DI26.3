using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SphereClickController : MonoBehaviour
{
    [Header("Configuración de Empuje")]
    [SerializeField] private float _fuerzaEmpuje = 10f;

    private Rigidbody _rigidbody;
    private bool _estaEmpujando;

    private void Awake()
    {
        // Inicialización del componente (Single Responsibility)
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Uso del nuevo Input System de Unity 6
        // Detectamos si la tecla Espacio o la Flecha Arriba están presionadas
        _estaEmpujando = Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed;
    }

    private void FixedUpdate()
    {
        if (_estaEmpujando)
        {
            AplicarFuerzaHaciaAdelante();
        }
    }

    private void AplicarFuerzaHaciaAdelante()
    {
        // Aplicamos la fuerza en el eje Z (hacia adelante)
        // ForceMode.Acceleration ignora la masa para un movimiento más predecible para principiantes
        _rigidbody.AddForce(Vector3.forward * _fuerzaEmpuje, ForceMode.Force);
    }
}
