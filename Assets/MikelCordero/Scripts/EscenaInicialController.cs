using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EscenaInicialController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderTiempo;
    public Slider sliderHorizontales;
    public Slider sliderVerticales;

    [Header("Textos")]
    public TextMeshPro textoTiempo;
    public TextMeshPro textoHorizontales;
    public TextMeshPro textoVerticales;

    [Header("Opción de oclusión")]
    public Toggle toggleOclusion;

    // Variables globales que se usarán en la siguiente escena
    public static float tiempoLimite;
    public static int planosHorizontales;
    public static int planosVerticales;
    public static bool oclusionActiva;

    void Start()
    {
        // Inicializamos los textos con el valor inicial de los sliders
        ActualizarTextos();
    }

    void Update()
    {
        // Actualizamos los textos en tiempo real
        ActualizarTextos();
    }

    void ActualizarTextos()
    {
        textoTiempo.text = sliderTiempo.value.ToString("0") + " s";
        textoHorizontales.text = sliderHorizontales.value.ToString("0");
        textoVerticales.text = sliderVerticales.value.ToString("0");
    }

    public void EmpezarJuego()
    {
        tiempoLimite = sliderTiempo.value;
        planosHorizontales = Mathf.RoundToInt(sliderHorizontales.value);
        planosVerticales = Mathf.RoundToInt(sliderVerticales.value);
        oclusionActiva = toggleOclusion.isOn;

        SceneManager.LoadScene("EscenaJuego");
    }
}
