using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWallKiller : MonoBehaviour
{
    public Transform respawnPoint;
    public bool reloadScene;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("KillWall")) return;
        if (reloadScene) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else if (respawnPoint != null) transform.position = respawnPoint.position;
    }
}

