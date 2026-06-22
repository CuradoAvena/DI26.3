using UnityEngine;

public class InputTester : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private void OnEnable()
    {
        if (inputReader != null)
        {
            // Suscripción a los eventos del lector de inputs
           
            inputReader.OnMouseMoved += HandleMouseMove;
            inputReader.OnMouseScroll += HandleMouseScroll;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            // Desuscripción obligatoria para evitar fugas de memoria
            
            inputReader.OnMouseMoved -= HandleMouseMove;
            inputReader.OnMouseScroll -= HandleMouseScroll;
        }
    }

    private void HandleClick()
    {
        Debug.Log("<color=green>[Input System]</color> Clic Izquierdo Detectado.");
    }

    private void HandleMouseMove(Vector2 position)
    {
        // Descomentar solo si es estrictamente necesario, genera mucho spam en consola
        // Debug.Log($"Posición del Mouse: {position}");
    }

    private void HandleMouseScroll(Vector2 scrollDelta)
    {
        Debug.Log($"<color=cyan>[Input System]</color> Scroll del Mouse detectado. Delta: {scrollDelta.y}");
    }
}
