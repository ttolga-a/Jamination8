using System;
using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager instance;
    public Player player;
    private Animator anim;

    [SerializeField] private GameObject endUI;
    [SerializeField] private int bombFullTime = 300;
    public GameObject pressFText;
    private bool playerInRange = false;
    public float bombRemainingTime;
    public bool isBlowing = false;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Time.timeScale = 1;
        player.UnlockPlayer();

        bombRemainingTime = bombFullTime;
        StartCoroutine(StartCountdown());
        HandleUnstableBomb();
        if (pressFText)
            pressFText.SetActive(false);
    }

    private void Update()
    {
        bool shouldBeBlowing = (bombRemainingTime <= 5);
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !shouldBeBlowing)
        {
            BombDefuseManager.instance.bombUI.SetActive(!BombDefuseManager.instance.bombUI.activeSelf);
            if (BombDefuseManager.instance.bombUI.activeSelf)
                player.LockPlayer();
            else
                player.UnlockPlayer();
        }
        if (shouldBeBlowing && BombDefuseManager.instance.bombUI.activeSelf)
        {
            BombDefuseManager.instance.bombUI.SetActive(false);
            player.UnlockPlayer();
        }

        CheckBombAnimation();
    }

    private void CheckBombAnimation()
    {
        bool shouldBeBlowing = (bombRemainingTime <= 5);

        anim.SetBool("blowing", shouldBeBlowing);
    }

    private IEnumerator StartCountdown()
    {
        while (bombRemainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            bombRemainingTime--;
        }
        OpenEndScreen(false);
        
    }

    public void OpenEndScreen(bool IsWin)
    {
        if (IsWin)
        {
            Debug.Log("win");
        }
        else
        {
            Ingame_UI.instance.endGameText.text = "BOMBA PATLADI";
        }
        endUI.SetActive(true);
        Time.timeScale = 0;
        player.LockPlayer();
    }

    public void BombTimeChanger(int TimeValue)
    {
        bombRemainingTime += TimeValue;
        var unstableText = Ingame_UI.instance.bombUnstableText;

        if (TimeValue > 0)
        {
            unstableText.color = Color.green;
            unstableText.text = "+" + TimeValue.ToString();
            StartCoroutine(BombUnstableTextCo());
        }
        else if (TimeValue < 0)
        {
            unstableText.color = Color.red;
            unstableText.text = TimeValue.ToString();
            StartCoroutine(BombUnstableTextCo());
        }
        else 
            unstableText.text = "";
    }

    private void BombTimeChangerForUnstable(int TimeValue)
    {
        bombRemainingTime += TimeValue;
    }

    private void RandomTimeAdder()
    {
        int addingValue = UnityEngine.Random.Range(-45, 45);

        BombTimeChangerForUnstable(addingValue);

        if (addingValue > 0)
        {
            Ingame_UI.instance.bombUnstableText.color = Color.green;
            Ingame_UI.instance.bombUnstableText.text = "Bomba dengesizlesti, +" + addingValue + " saniye eklendi!!!";
        }
        else if(addingValue < 0)
        {
            Ingame_UI.instance.bombUnstableText.color = Color.red;
            Ingame_UI.instance.bombUnstableText.text = "Bomba dengesizlesti, " + addingValue + " saniye azaldi!!!!";
        }

        StartCoroutine(BombUnstableTextCo());
    }

    IEnumerator BombUnstableTextCo()
    {
        Ingame_UI.instance.bombUnstableText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        Ingame_UI.instance.bombUnstableText.gameObject.SetActive(false);
    }

    private void HandleUnstableBomb()
    {
        StartCoroutine(BombUnstableCo());
    }

    IEnumerator BombUnstableCo()
    {
        while (bombRemainingTime > 0)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0, 60));
            RandomTimeAdder();
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
