using System;
using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager instance;

    [SerializeField] private int bombFullTime = 300;
    public float bombRemainingTime;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        bombRemainingTime = bombFullTime;
        StartCoroutine(StartCountdown());
        HandleUnstableBomb();
    }

    private void Update()
    {
    }

    private IEnumerator StartCountdown()
    {
        while (bombRemainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            bombRemainingTime--;
        }
        Debug.Log("Süre Bitti! Bomba patladý!");
    }

    public void BombTimeChanger(int TimeValue)
    {
        bombRemainingTime += TimeValue;
    }

    private void RandomTimeAdder()
    {
        int addingValue = UnityEngine.Random.Range(-45, 45);

        BombTimeChanger(addingValue);

        if (addingValue > 0)
        {
            Ingame_UI.instance.bombUnstableText.color = Color.green;
            Ingame_UI.instance.bombUnstableText.text = "Bomb is so unstable and it gives +" + addingValue + " seconds!!!!";
        }
        else if(addingValue < 0)
        {
            Ingame_UI.instance.bombUnstableText.color = Color.red;
            Ingame_UI.instance.bombUnstableText.text = "Bomb is so unstable and it gives " + addingValue + " seconds!!!!";
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
}
