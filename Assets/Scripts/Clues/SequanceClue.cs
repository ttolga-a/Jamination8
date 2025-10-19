using UnityEngine;
using UnityEngine.UI;

public class SequanceClue : Clues
{
    [SerializeField] private Image[] displayImages;
    [SerializeField] private Sprite[] colorSprites;

    public void SetupSequenceClue()
    {
        for (int i = 0; i < 3; i++)
        {
                displayImages[i].sprite = colorSprites[BombDefuseManager.instance.correctSequence[i]];
        }
    }
}
