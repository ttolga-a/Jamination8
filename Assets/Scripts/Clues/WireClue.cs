using UnityEngine;
using UnityEngine.UI;

public class WireClue : Clues
{
    [SerializeField] private Image wiresAnswer;

    public void SetupWireClue(int clueID)
    {
        switch (clueID)
        {
            case 0:
                wiresAnswer.color = Color.red;
                break;
            case 1:
                wiresAnswer.color = Color.green;
                break;
            case 2:
                wiresAnswer.color = Color.blue;
                break;
            case 3:
                wiresAnswer.color = Color.black;
                break;
            default:
                break;
        }
    }
}
