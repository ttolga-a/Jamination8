using UnityEngine;
using UnityEngine.UI;

public class SymbolClue : Clues
{
    [SerializeField] private Sprite[] allAnswers;
    [SerializeField] private Image[] displayImages;

    public void SetupSymbolClue()
    {
        for (int i = 0; i < 3; i++)
        {
            displayImages[i].sprite = allAnswers[BombDefuseManager.instance.correctShapeIndices[i]];
        }
    }
}
