using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDemoScene : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "DemoScene";
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}