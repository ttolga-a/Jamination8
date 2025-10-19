using UnityEngine;

public class MiniGameTargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;
    public RectTransform canvasTransform; // Canvas parent
    public float spawnRangeX = 400f;
    public float spawnRangeY = 300f;
    public float baseSpawnInterval = 0.7f;
    public float minSpawnInterval = 0.3f;

    private float timer = 0f;
    private float spawnInterval;

    private void Start()
    {
        spawnInterval = baseSpawnInterval;
    }

    private void Update()
    {
        if (!MiniGameManager.Instance || !MiniGameManager.Instance.gameActive) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnTarget();
        }

        // Spawn interval’i sürekli güncelle
        float progress = Mathf.Clamp01((float)MiniGameManager.Instance.currentScore / MiniGameManager.Instance.targetScore);
        spawnInterval = Mathf.Lerp(baseSpawnInterval, minSpawnInterval, progress);
    }

    private void SpawnTarget()
    {
        float x = Random.Range(-spawnRangeX, spawnRangeX);
        float y = Random.Range(-spawnRangeY, spawnRangeY);

        GameObject target = Instantiate(targetPrefab, canvasTransform);
        RectTransform rt = target.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        rt.localScale = Vector3.one;
    }
}
