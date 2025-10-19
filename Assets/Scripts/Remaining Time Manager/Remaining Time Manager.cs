using UnityEngine;

public class RemainingTimeManager : MonoBehaviour
{
    [SerializeField] BombManager bombManager;
    [SerializeField] bool clampToNonNegative = false;

    void Awake()
    {
        bombManager = FindAnyObjectByType<BombManager>();
    }

    public void AddRandomToRemainingTime(float aValue, float bValue)
    {
        float min = Mathf.Min(aValue, bValue);
        float max = Mathf.Max(aValue, bValue);
        float randomDelta = Random.Range(min, max);

        bombManager.bombRemainingTime += randomDelta;

        if(clampToNonNegative && bombManager.bombRemainingTime < 0)
            bombManager.bombRemainingTime = 0;
    }
}
