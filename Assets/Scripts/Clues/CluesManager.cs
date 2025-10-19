using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CluesManager : MonoBehaviour
{
    public static CluesManager instance;

    [Header("WireCLue")]
    [SerializeField] private Image clueShowLoc;
    [SerializeField] private Sprite[] wireClueSprites;
    public bool isWireKnown = false;

    private void Awake()
    {
        instance = this;
    }

    public void SetupWireClueUI(int id)
    {
        clueShowLoc.sprite = wireClueSprites[id];
    }
}
