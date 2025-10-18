using UnityEngine;

public class Clues : MonoBehaviour
{
    private Animator anim;

    public GameObject pressFText;
    public GameObject cluePanel;
    public Player player;
    private bool playerInRange = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

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
            if (cluePanel.activeSelf)
                player.LockPlayer();
            else
                player.UnlockPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            anim.SetBool("isPlayerOn", playerInRange);
            if (pressFText)
                pressFText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            anim.SetBool("isPlayerOn", playerInRange);
            if (pressFText)
                pressFText.SetActive(false);
        }
    }
}
