using UnityEngine;
using UnityEngine.InputSystem;
public class DetectorInteraccion : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask capasInteractuables;
    [SerializeField] private float distanciaMaxima = 100f;

    private Cuadro _cuadroActual;

    void Update()
    {
        // === SHORTCUT DE RETORNO (CLICK DERECHO) ===
        // Si el mánager está inspeccionando un objeto en el centro de la pantalla...
        if (InspectionManager.Instancia != null && InspectionManager.Instancia.EnModoInspeccion)
        {
            // ...y el jugador presiona Click Derecho, cerramos la UI y devolvemos el cubo de golpe
            if (UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame)
            {
                Debug.Log("<color=cyan>[Inspección]</color> Retorno rápido activado por Click Derecho.");
                MuseumUIManager.Instancia.CerrarPanel(); // Esto apaga el panel e invoca el regreso automático
            }
            return; // Mantiene el escudo activo para que el Raycast no ensucie la selección
        }

        // Si la UI de la ficha está abierta por el método tradicional, congelamos el resto del Raycast
        if (MuseumUIManager.Instancia != null && MuseumUIManager.Instancia.InterfazActiva) return;

        // Si no estamos inspeccionando, el buscador láser opera de forma normal (Hover)
        LanzarRaycast();

        // Detección de la tecla E para abrir la ficha técnica
        if (_cuadroActual != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Objeto interactivo detectado en el museo.");
            MuseumUIManager.Instancia.MostrarFicha(_cuadroActual);
        }
    }

    void LanzarRaycast()
    {
        if (mainCamera == null) return;

        // Proyectamos el rayo desde la posición del mouse
        Ray rayo = mainCamera.ScreenPointToRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        RaycastHit golpe;

        if (Physics.Raycast(rayo, out golpe, distanciaMaxima, capasInteractuables))
        {
            // Buscamos el componente correcto: Cuadro
            Cuadro cuadro = golpe.collider.GetComponent<Cuadro>();

            if (cuadro != null)
            {
                if (_cuadroActual != cuadro)
                {
                    if (_cuadroActual != null) _cuadroActual.QuitarMirada();
                    _cuadroActual = cuadro;
                    _cuadroActual.MirarCuadro();
                    Debug.Log($"<color=yellow>[Raycast]</color> Objeto seleccionado: {golpe.collider.name}");
                }

                // Si el usuario hace click izquierdo, vuela al centro de la pantalla
                if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame && InspectionManager.Instancia != null)
                {
                    MuseumUIManager.Instancia.CerrarPanel();
                    InspectionManager.Instancia.IniciarInspeccion(_cuadroActual);
                }
                return;
            }
        }

        // Si el rayo toca el vacío u otra cosa, limpiamos el rastro
        if (_cuadroActual != null)
        {
            _cuadroActual.QuitarMirada();
            _cuadroActual = null;
            Debug.Log("<color=red>[Raycast]</color> Selección limpia. Ningún objeto activo.");
        }
    }
}
