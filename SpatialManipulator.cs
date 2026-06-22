using UnityEngine;

public class SpatialManipulator : MonoBehaviour
{
    private InputReader _inputReader;

    // El Raycaster de la Práctica 2 controlará esto. 
    // Para la tarea, como está solo en la estación de prueba, se queda en 'true'.
    [Header("Estado de Control")]
    [SerializeField] private bool estaActivo = true;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float scaleSpeed = 0.05f;
    [SerializeField] private float rotationSpeed = 0.4f;

    private bool _isRotating = false;
    private float _lastMouseX;

    // Método público para que la próxima semana el Raycast Manager tome el control
    public void SetActiveState(bool active)
    {
        estaActivo = active;
        if (!active) _isRotating = false;
    }

    private void Start()
    {
        // Buscamos el InputReader central de la escena de forma segura
        _inputReader = Object.FindFirstObjectByType<InputReader>();

        if (_inputReader != null)
        {
            _inputReader.OnClickStateChanged += ToggleRotationState;
            // Usamos FixedUpdate o eventos directos para evitar saltos de frames
            _inputReader.OnMouseMoved += HandleRotation;
            _inputReader.OnMouseScroll += HandleScale;
        }
    }

    private void OnDestroy()
    {
        if (_inputReader == null) return;
        _inputReader.OnClickStateChanged -= ToggleRotationState;
        _inputReader.OnMouseMoved -= HandleRotation;
        _inputReader.OnMouseScroll -= HandleScale;
    }

    private void ToggleRotationState(bool isPressed)
    {
        _isRotating = isPressed && estaActivo;
    }

    private void HandleRotation(Vector2 mousePosition)
    {
        if (!estaActivo) return;

        if (!_isRotating)
        {
            _lastMouseX = mousePosition.x;
            return;
        }

        float deltaX = mousePosition.x - _lastMouseX;
        // Rotación fluida en el eje Y
        transform.Rotate(Vector3.up, -deltaX * rotationSpeed, Space.World);
        _lastMouseX = mousePosition.x;
    }

    private void HandleScale(Vector2 scrollDelta)
    {
        if (!estaActivo || scrollDelta.y == 0) return;

        float scaleModifier = scrollDelta.y > 0 ? scaleSpeed : -scaleSpeed;
        Vector3 newScale = transform.localScale + Vector3.one * scaleModifier;

        // Límites duros para evitar deformaciones absurdas
        newScale.x = Mathf.Clamp(newScale.x, 0.3f, 4.0f);
        newScale.y = Mathf.Clamp(newScale.y, 0.3f, 4.0f);
        newScale.z = Mathf.Clamp(newScale.z, 0.3f, 4.0f);

        transform.localScale = newScale;
    }
}
