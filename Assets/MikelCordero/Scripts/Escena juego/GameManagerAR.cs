using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class GameManagerAR : MonoBehaviour
{
    [Header("AR")]
    public ARPlaneManager planeManager;

    [Header("UI")]
    public TMP_Text textoHorizontales;
    public TMP_Text textoVerticales;
    public TMP_Text textoGemas;
    public TMP_Text textoTemporizador;
    public TMP_Text textoResultado;
    public GameObject botonEmpezar;
    public GameObject botonSalir;

    [Header("Audio")]
    public AudioSource audioFondo;

    [Header("Configuración gemas")]
    public GameObject gemaPrefab;

    private int horizontalesDetectados;
    private int verticalesDetectados;
    private int gemasRecogidas;
    private float tiempoRestante;

    private bool juegoIniciado = false;
    private bool juegoTerminado = false;

    void Start()
    {
        tiempoRestante = EscenaInicialController.tiempoLimite;
        textoTemporizador.gameObject.SetActive(false);
        textoGemas.gameObject.SetActive(false);
        textoResultado.gameObject.SetActive(false);
        botonEmpezar.SetActive(false);

        audioFondo.Play();
    }

    void Update()
    {
        if (!juegoIniciado)
        {
            ContarPlanos();
            ActualizarUIPlanos();

            if (horizontalesDetectados >= EscenaInicialController.planosHorizontales &&
                verticalesDetectados >= EscenaInicialController.planosVerticales)
            {
                botonEmpezar.SetActive(true);
            }
        }
        else if (!juegoTerminado)
        {
            tiempoRestante -= Time.deltaTime;
            textoTemporizador.text = "Tiempo restante: " + Mathf.CeilToInt(tiempoRestante) + " s";

            if (tiempoRestante <= 0)
            {
                FinDelJuego(false);
            }
        }
    }

    void ContarPlanos()
    {
        horizontalesDetectados = 0;
        verticalesDetectados = 0;

        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
                horizontalesDetectados++;
            else if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                verticalesDetectados++;
        }
    }

    void ActualizarUIPlanos()
    {
        textoHorizontales.text = $"Planos Horizontales: {horizontalesDetectados} / {EscenaInicialController.planosHorizontales}";
        textoVerticales.text = $"Planos Verticales: {verticalesDetectados} / {EscenaInicialController.planosVerticales}";
    }

    public void IniciarJuego()
{
    juegoIniciado = true;

    textoHorizontales.gameObject.SetActive(false);
    textoVerticales.gameObject.SetActive(false);
    textoTemporizador.gameObject.SetActive(true);
    textoGemas.gameObject.SetActive(true);
    botonEmpezar.SetActive(false);

    Debug.Log("Iniciando juego..."); // AÑADIDO

    InstanciarGemas();
    ActualizarUIGemas();
}


    void InstanciarGemas()
{
    int cantidad = EscenaInicialController.numGemas;

    foreach (var plane in planeManager.trackables)
    {
        if (cantidad <= 0) break;

        Vector3 pos = plane.transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 0.05f, Random.Range(-0.3f, 0.3f));
        GameObject gema = Instantiate(gemaPrefab, pos, Quaternion.identity);
        gema.AddComponent<GemaAR>().gameManager = this;

        Debug.Log("Gema instanciada en: " + pos); // <-- AÑADE ESTA LÍNEA

        cantidad--;
    }
}


    public void RecolectarGema(GameObject gema)
    {
        Destroy(gema);
        gemasRecogidas++;
        ActualizarUIGemas();

        if (gemasRecogidas >= EscenaInicialController.numGemas)
        {
            FinDelJuego(true);
        }
    }

    void ActualizarUIGemas()
    {
        textoGemas.text = $"Gemas recogidas: {gemasRecogidas} / {EscenaInicialController.numGemas}";
    }

    void FinDelJuego(bool victoria)
{
    juegoTerminado = true;

    // Ocultar UI del juego
    textoTemporizador.gameObject.SetActive(false);
    textoGemas.gameObject.SetActive(false);

    // Mostrar resultado final
    textoResultado.gameObject.SetActive(true);
    textoResultado.text = victoria ? "¡Has ganado!" : "Has perdido";
}


    public void SalirDelJuego()
    {
        SceneManager.LoadScene("Scene1");
    }
}
