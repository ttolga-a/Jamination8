using UnityEngine;
using System.Collections.Generic;
using System.Collections; // TextMeshPro kullanýyorsan bu satýr da lazým

public class ClueUIController : MonoBehaviour
{
    [Header("Clue UI Panels")]
    [SerializeField] private GameObject wireClueUI;  // 1 Tuþu için
    [SerializeField] private GameObject symbolClueUI;  // 2 Tuþu için
    [SerializeField] private GameObject sequenceClueUI; // 3 Tuþu için
    // Diðerleri için de buraya ekleyebilirsin...

    // Hangi ipucunun o anda ekranda olduðunu takip etmek için bir deðiþken
    private GameObject currentlyShowingClue = null;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1) && CluesManager.instance.hasFoundWireClue)
        {
            SwitchActiveClue(wireClueUI);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && CluesManager.instance.hasFoundSymbolClue)
        {
            SwitchActiveClue(symbolClueUI);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && CluesManager.instance.hasFoundSequenceClue)
        {
            SwitchActiveClue(sequenceClueUI);
        }

        if (Input.GetKeyUp(KeyCode.Alpha1) && currentlyShowingClue == wireClueUI)
        {
            currentlyShowingClue.SetActive(false);
            currentlyShowingClue = null;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2) && currentlyShowingClue == symbolClueUI)
        {
            currentlyShowingClue.SetActive(false);
            currentlyShowingClue = null;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3) && currentlyShowingClue == sequenceClueUI)
        {
            currentlyShowingClue.SetActive(false);
            currentlyShowingClue = null;
        }
    }

    private void SwitchActiveClue(GameObject newClueToShow)
    {
        if (currentlyShowingClue != null)
        {
            currentlyShowingClue.SetActive(false);
        }

        currentlyShowingClue = newClueToShow;
        currentlyShowingClue.SetActive(true);
    }
}