using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, DioramaInputActions.IMouseActionsActions
{
    private DioramaInputActions _inputActions;

    // Eventos públicos para que otros componentes se suscriban libremente
    public event Action<bool> OnClickStateChanged;
    public event Action<Vector2> OnMouseMoved;
    public event Action<Vector2> OnMouseScroll;

    private void Awake()
    {
        if (_inputActions == null)
        {
            _inputActions = new DioramaInputActions();
            // Asignamos esta clase como la responsable de recibir los callbacks
            _inputActions.MouseActions.SetCallbacks(this);
        }
    }

    private void OnEnable()
    {
        _inputActions.MouseActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.MouseActions.Disable();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        // Gracias a 'Press and Release', esto es 100% preciso
        if (context.started)
            OnClickStateChanged?.Invoke(true);

        if (context.canceled)
            OnClickStateChanged?.Invoke(false);
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        if (context.performed) OnMouseMoved?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (context.performed) OnMouseScroll?.Invoke(context.ReadValue<Vector2>());
    }
}