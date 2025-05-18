using UnityEngine;

public class GemaAR : MonoBehaviour
{
    public GameManagerAR gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("Gema recogida por la cámara");
            gameManager.RecolectarGema(gameObject);
        }
    }
}
