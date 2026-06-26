using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTouch2D : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private string requiredTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag))
            return;

        SceneManager.LoadScene(sceneName);
    }
}