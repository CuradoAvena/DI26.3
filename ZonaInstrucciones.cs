using UnityEngine;

public class ZonaInstrucciones : MonoBehaviour
{
    [Header("Configuración de la Misión")]
    [SerializeField] private string _mensajeMision = "ALERTA DEL COMPLEJO:\n\nCompuerta hidráulica bloqueada. Localiza las 3 terminales mecatrónicas en el laberinto. Digita los códigos correctos para inyectar un total de 1000V y liberar el Núcleo final.";

    private bool _yaActivado = false;

    // Este método de Unity se ejecuta automáticamente cuando el jugador atraviesa el cubo invisible
    private void OnTriggerEnter(Collider other)
    {
        // Validamos que el objeto que cruzó sea el jugador y que no se haya activado antes
        if (other.CompareTag("Player") && !_yaActivado)
        {
            _yaActivado = true; // Candado para que solo salga una vez

            // Le mandamos el texto directo al Manager de UI que ya programamos
            ManagerTerminalUI.Instancia.MostrarInstruccionesLobby(_mensajeMision);
        }
    }
}
