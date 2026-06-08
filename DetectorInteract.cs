using UnityEngine;
using UnityEngine.InputSystem;

public class DetectorInteract : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    [SerializeField] private float _distanciaMaxima = 3.5f;
    [SerializeField] private LayerMask _capaInteractuable;

    private Consola _consolaActual;

    private void Update()
    {
        if (ManagerTerminalUI.Instancia.PantallaAbierta && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ManagerTerminalUI.Instancia.CerrarTerminal();
            return;
        }

        if (ManagerTerminalUI.Instancia.PantallaAbierta) return;

        LanzarRaycast();

        if (_consolaActual != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ManagerTerminalUI.Instancia.AbrirTerminal(_consolaActual);
        }
    }

    private void LanzarRaycast()
    {
        Ray rayo = new Ray(transform.position, transform.forward);
        RaycastHit infoChoque;

        if (Physics.Raycast(rayo, out infoChoque, _distanciaMaxima, _capaInteractuable))
        {
            Consola nuevaConsola = infoChoque.collider.GetComponent<Consola>();

            if (nuevaConsola != null && nuevaConsola != _consolaActual)
            {
                if (_consolaActual != null) _consolaActual.PerderConsola();

                _consolaActual = nuevaConsola;
                _consolaActual.DetectarConsola();
            }
        }
        else
        {
            if (_consolaActual != null)
            {
                _consolaActual.PerderConsola();
                _consolaActual = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (_consolaActual != null) ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * _distanciaMaxima);
    }
}
