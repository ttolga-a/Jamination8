using UnityEngine;
using UnityEngine.UI;

public class WireClue : Clues
{
    [SerializeField] private Image wiresAnswer;
    [SerializeField] private Sprite[] wireSprites;

    public void SetupWireClue(int clueID)
    {
        switch (clueID)
        {
            case 0:
                wiresAnswer.sprite = wireSprites[0];
                break;
            case 1:
                wiresAnswer.sprite = wireSprites[1];
                break;
            case 2:
                wiresAnswer.sprite = wireSprites[2];
                break;
            case 3:
                wiresAnswer.sprite = wireSprites[3];
                break;
            default:
                break;
        }
    }
}
