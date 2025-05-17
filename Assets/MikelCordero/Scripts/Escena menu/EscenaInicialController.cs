using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EscenaInicialController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderTiempo;
    public Slider sliderHorizontales;
    public Slider sliderVerticales;

    [Header("Textos")]
    public TMP_Text textoTiempo;
    public TMP_Text textoHorizontales;
    public TMP_Text textoVerticales;

    [Header("Oclusión")]
    public Toggle toggleOclusion;

    public static float tiempoLimite;
    public static int planosHorizontales;
    public static int planosVerticales;
    public static bool oclusionActiva;

    public static int numGemas;


    void Start()
    {
        ActualizarTextos();
    }

    void Update()
    {
        ActualizarTextos();
    }

    void ActualizarTextos()
    {
        int tiempo = Mathf.RoundToInt(sliderTiempo.value);
        int hor = Mathf.RoundToInt(sliderHorizontales.value);
        int ver = Mathf.RoundToInt(sliderVerticales.value);

        textoTiempo.text = "Tiempo: " + tiempo + " s";
        textoHorizontales.text = "Planos Horizontales: " + hor;
        textoVerticales.text = "Planos Verticales: " + ver;

        Debug.Log("Tiempo: " + tiempo + " | H: " + hor + " | V: " + ver);
    }

    public void EmpezarJuego()
    {
        tiempoLimite = Mathf.RoundToInt(sliderTiempo.value);
        planosHorizontales = Mathf.RoundToInt(sliderHorizontales.value);
        planosVerticales = Mathf.RoundToInt(sliderVerticales.value);
        oclusionActiva = toggleOclusion.isOn;

        Debug.Log("EmpezarJuego → Tiempo: " + tiempoLimite + " | H: " + planosHorizontales + " | V: " + planosVerticales + " | Oclusión: " + oclusionActiva);

        SceneManager.LoadScene("EscenaJuego");
    }
}
