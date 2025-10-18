using TMPro;
using UnityEngine;

public class FrequancyClue : Clues
{
    [SerializeField]private TMP_Text frequancyText;

    public void SetupFrequancyClue(float frequancy)
    {
        float rangeWidth = 0.20f;
        float lowestPossibleMin = frequancy - rangeWidth;
        float highestPossibleMin = frequancy;

        lowestPossibleMin = Mathf.Max(0f, lowestPossibleMin);
        highestPossibleMin = Mathf.Min(highestPossibleMin, 1f - rangeWidth);

        float randomMin = Random.Range(lowestPossibleMin, highestPossibleMin);

        float randomMax = randomMin + rangeWidth;

        frequancyText.text = $"Frequancy: {randomMin:F2} - {randomMax:F2}";
    }
}
