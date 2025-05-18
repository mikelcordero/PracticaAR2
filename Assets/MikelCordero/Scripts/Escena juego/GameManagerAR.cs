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

    [Header("Audios adicionales")]
    public AudioClip sonidoGema;
    public AudioClip sonidoGanar;
    public AudioClip sonidoPerder;

    [Header("Configuración gemas")]
    public GameObject gemaPrefab;

    private int horizontalesDetectados;
    private int verticalesDetectados;
    private int gemasRecogidas;
    private int totalGemasARecoger;
    private float tiempoRestante;

    private bool juegoIniciado = false;
    private bool juegoTerminado = false;

    private AudioSource audioEfectos;

    void Start()
    {
        tiempoRestante = EscenaInicialController.tiempoLimite;
        textoTemporizador.gameObject.SetActive(false);
        textoGemas.gameObject.SetActive(false);
        textoResultado.gameObject.SetActive(false);
        botonEmpezar.SetActive(false);

        audioEfectos = gameObject.AddComponent<AudioSource>();
        audioEfectos.playOnAwake = false;

        if (audioFondo != null)
        {
            audioFondo.Stop();
            audioFondo.time = 0f;
        }
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
            else
            {
                botonEmpezar.SetActive(false);
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
        if (juegoIniciado) return; // ✅ Protección contra ejecución doble

        juegoIniciado = true;

        if (audioFondo != null)
        {
            audioFondo.time = 0f;
            audioFondo.Play();
        }

        totalGemasARecoger = EscenaInicialController.planosHorizontales + EscenaInicialController.planosVerticales;

        textoHorizontales.gameObject.SetActive(false);
        textoVerticales.gameObject.SetActive(false);
        textoTemporizador.gameObject.SetActive(true);
        textoGemas.gameObject.SetActive(true);
        botonEmpezar.SetActive(false);

        InstanciarGemas();
        ActualizarUIGemas();
    }

    void InstanciarGemas()
    {
        // ✅ Eliminar gemas antiguas si existen
        foreach (var gema in GameObject.FindGameObjectsWithTag("Gema"))
        {
            Destroy(gema);
        }

        int gemasHorizontalesRestantes = EscenaInicialController.planosHorizontales;
        int gemasVerticalesRestantes = EscenaInicialController.planosVerticales;

        foreach (var plane in planeManager.trackables)
        {
            if (gemasHorizontalesRestantes == 0 && gemasVerticalesRestantes == 0)
                break;

            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp && gemasHorizontalesRestantes > 0)
            {
                Vector3 pos = plane.transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 0.15f, Random.Range(-0.3f, 0.3f));
                GameObject gema = Instantiate(gemaPrefab, pos, Quaternion.identity);
                gema.tag = "Gema"; // Asegúrate de que el prefab tenga este tag también
                gema.AddComponent<GemaAR>().gameManager = this;
                gemasHorizontalesRestantes--;
            }
            else if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical && gemasVerticalesRestantes > 0)
            {
                Vector3 pos = plane.transform.position + new Vector3(0, 0, Random.Range(-0.3f, 0.3f));
                GameObject gema = Instantiate(gemaPrefab, pos, Quaternion.identity);
                gema.tag = "Gema";
                gema.AddComponent<GemaAR>().gameManager = this;
                gemasVerticalesRestantes--;
            }
        }
    }

    public void RecolectarGema(GameObject gema)
    {
        Destroy(gema);
        gemasRecogidas++;
        ActualizarUIGemas();

        if (sonidoGema != null)
            audioEfectos.PlayOneShot(sonidoGema);

        if (gemasRecogidas >= totalGemasARecoger)
        {
            FinDelJuego(true);
        }
    }

    void ActualizarUIGemas()
    {
        textoGemas.text = $"Gemas recogidas: {gemasRecogidas} / {totalGemasARecoger}";
    }

    void FinDelJuego(bool victoria)
    {
        juegoTerminado = true;

        if (audioFondo != null && audioFondo.isPlaying)
        {
            audioFondo.Stop();
        }

        textoTemporizador.gameObject.SetActive(false);
        textoGemas.gameObject.SetActive(false);
        textoResultado.gameObject.SetActive(true);
        textoResultado.text = victoria ? "¡Has ganado!" : "Has perdido";

        if (victoria && sonidoGanar != null)
            audioEfectos.PlayOneShot(sonidoGanar);
        else if (!victoria && sonidoPerder != null)
            audioEfectos.PlayOneShot(sonidoPerder);
    }

    public void SalirDelJuego()
    {
        SceneManager.LoadScene("Scene1");
    }
}
