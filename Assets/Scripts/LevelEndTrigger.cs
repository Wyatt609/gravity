using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    public string nextScene;
    public bool useNextBuildIndex = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() == null && !other.CompareTag("Player")) return;
        LoadNext();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CharacterController>() == null && !other.CompareTag("Player")) return;
        LoadNext();
    }

    void LoadNext()
    {
        if (useNextBuildIndex)
        {
            int i = SceneManager.GetActiveScene().buildIndex + 1;
            if (i < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(i);
        }
        else
        {
            if (!string.IsNullOrEmpty(nextScene)) SceneManager.LoadScene(nextScene);
        }
    }
}
