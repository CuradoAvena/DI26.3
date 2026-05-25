using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoPlayer : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float _velocidadCaminar = 5f;

    private Rigidbody _rb;
    private Vector2 _inputMovimiento;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        // El cuerpo maneja el bloqueo del cursor del mouse al iniciar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        LeerTeclado();
    }

    private void FixedUpdate()
    {
        MoverCuerpo();
    }

    private void LeerTeclado()
    {
        _inputMovimiento = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) _inputMovimiento.y = 1;
        if (Keyboard.current.sKey.isPressed) _inputMovimiento.y = -1;
        if (Keyboard.current.aKey.isPressed) _inputMovimiento.x = -1;
        if (Keyboard.current.dKey.isPressed) _inputMovimiento.x = 1;
    }

    private void MoverCuerpo()
    {
        // Movimiento relativo a la orientación actual del objeto padre (la cápsula)
        Vector3 direccionFrente = transform.forward * _inputMovimiento.y;
        Vector3 direccionLado = transform.right * _inputMovimiento.x;
        Vector3 velocidadFinal = (direccionFrente + direccionLado).normalized * _velocidadCaminar;

        // Modificamos la velocidad manteniendo la gravedad intacta
        _rb.linearVelocity = new Vector3(velocidadFinal.x, _rb.linearVelocity.y, velocidadFinal.z);
    }
}
