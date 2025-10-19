using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Transform respawnPoint;
    [SerializeField] private float respawnTime = 1f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;          // Hýz
    [SerializeField] private float moveDistance = 1f;       // Yukarý aþaðý mesafe
    private Vector3 startPos;
    private bool movingUp = true;

    private void Start()
    {
        startPos = transform.position; // Baþlangýç konumunu kaydet
    }

    private void Update()
    {
        MoveUpDown();
    }

    private void MoveUpDown()
    {
        // Yukarý veya aþaðý hareket
        float newY = transform.position.y + (movingUp ? 1 : -1) * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Mesafe sýnýrýna gelince yönü ters çevir
        if (movingUp && transform.position.y >= startPos.y + moveDistance)
        {
            movingUp = false;
        }
        else if (!movingUp && transform.position.y <= startPos.y - moveDistance)
        {
            movingUp = true;
        }
    }

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