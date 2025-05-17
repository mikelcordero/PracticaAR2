using UnityEngine;

public class GemaAR : MonoBehaviour
{
    public GameManagerAR gameManager;

    void OnMouseDown()
    {
        gameManager.RecolectarGema(gameObject);
    }
}
