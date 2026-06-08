using UnityEngine;

public class PuertaGaleria : MonoBehaviour
{
    public static PuertaGaleria Instancia { get; private set; }

    private void Awake()
    {
        // Singleton para que el Manager la encuentre al instante sin consumir recursos
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void AbrirCompuerta()
    {
        Debug.Log("<b>[MECATRÓNICA]</b> Energía restablecida. Abriendo compuerta hidráulica...");

        // Acción directa: Desactivamos el objeto en el mapa para que el jugador pueda pasar
        gameObject.SetActive(false);

        // NOTA PARA LA CLASE 6: Aquí, en vez de desaparecerla, podremos meter la animación de Maya
    }
}
