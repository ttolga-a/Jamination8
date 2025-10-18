using UnityEngine;

public class Clues : MonoBehaviour
{
    public GameObject pressFText;
    public GameObject cluePanel;

    private bool playerInRange = false;

    private void Start()
    {
        if (pressFText)
            pressFText.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            cluePanel.SetActive(!cluePanel.activeSelf);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressFText)
                pressFText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressFText)
                pressFText.SetActive(false);
        }
    }
}
