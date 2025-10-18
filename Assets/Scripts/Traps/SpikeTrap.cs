using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Transform respawnPoint;
    [SerializeField] private float respawnTime = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPlayer(other.gameObject));
        }
    }

    private IEnumerator RespawnPlayer(GameObject player)
    {
        player.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        player.transform.position = respawnPoint.position;

        player.SetActive(true);
    }
}
