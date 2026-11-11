using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string spawnPointTag = "Respawn";

    private void Start()
    {
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
            return;

        GameObject spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);
        if (spawnPoint == null)
        {
            return;
        }

        Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }
}