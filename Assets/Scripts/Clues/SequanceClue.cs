using UnityEngine;
using UnityEngine.UI;

public class SequanceClue : Clues
{
    [SerializeField] private Image[] displayImages;
    Color myColor;

    public void SetupSymbolClue()
    {
        for (int i = 0; i < 3; i++)
        {
            switch (BombDefuseManager.instance.correctSequence[i])
            {
                case 0:
                    displayImages[i].color = Color.red;
                    break;
                case 1:
                    if (ColorUtility.TryParseHtmlString("#FF008A", out myColor))
                    {
                        displayImages[i].color = myColor;
                    }
                    break;
                case 2:
                    if (ColorUtility.TryParseHtmlString("#C200FF", out myColor))
                    {
                        displayImages[i].color = myColor;
                    }
                    break;
                case 3:
                    displayImages[i].color = Color.blue;
                    break;
                case 4:
                    displayImages[i].color = Color.cyan;
                    break;
                case 5:
                    displayImages[i].color = Color.green;
                    break;
                case 6:
                    displayImages[i].color = Color.yellow;
                    break;
                case 7:
                    displayImages[i].color = Color.white;
                    break;
                case 8:
                    displayImages[i].color = Color.black;
                    break;
                default:
                    break;
            }
        }
    }
}
