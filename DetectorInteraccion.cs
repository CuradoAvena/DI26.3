using UnityEngine;
using UnityEngine.InputSystem;
public class DetectorInteraccion : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    [SerializeField] private float _distanciaMaxima = 3.5f;
    [SerializeField] private LayerMask _capaInteractuable;

    private CuadroMuseo _cuadroActual;

    private void Update()
    {
        LanzarRaycast();
    }

    private void LanzarRaycast()
    {
        Ray rayo = new Ray(transform.position, transform.forward);
        RaycastHit infoChoque;

        if (Physics.Raycast(rayo, out infoChoque, _distanciaMaxima, _capaInteractuable))
        {
            // Intentamos obtener el componente del cuadro con el que chocamos
            CuadroMuseo nuevoCuadro = infoChoque.collider.GetComponent<CuadroMuseo>();

            if (nuevoCuadro != null && nuevoCuadro != _cuadroActual)
            {
                // Si estábamos viendo otro cuadro antes, le quitamos el color
                if (_cuadroActual != null) _cuadroActual.QuitarMirada();

                // Guardamos el nuevo cuadro y lo encendemos
                _cuadroActual = nuevoCuadro;
                _cuadroActual.MirarCuadro();
            }
        }
        else
        {
            // Si el rayo ya no choca con nada, apagamos el cuadro actual
            if (_cuadroActual != null)
            {
                _cuadroActual.QuitarMirada();
                _cuadroActual = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Si el script está encontrando un cuadro, pinta el láser verde. Si no, rojo.
        Gizmos.color = (_cuadroActual != null) ? Color.green : Color.red;

        // Dibuja la línea desde la cámara hacia adelante con el límite exacto de la distancia
        Gizmos.DrawRay(transform.position, transform.forward * _distanciaMaxima);
    }
}
