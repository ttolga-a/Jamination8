using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Transform respawnPoint;
    public Player playerx;
    [SerializeField] private float respawnTime = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BombManager.instance.BombTimeChanger(Random.Range(-25, -10));
            StartCoroutine(RespawnPlayer(other.gameObject));
        }
    }

    private IEnumerator RespawnPlayer(GameObject player)
    {
        player.SetActive(false);
        playerx.LockPlayer();

        yield return new WaitForSeconds(respawnTime);

        player.transform.position = respawnPoint.position;

        player.SetActive(true);
        playerx.UnlockPlayer();
    }
}
