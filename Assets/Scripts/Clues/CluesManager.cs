using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CluesManager : MonoBehaviour
{
    public static CluesManager instance { get; private set; }

    [Header("WireCLue")]
    [SerializeField] private Image clueShowLoc;
    [SerializeField] private Sprite[] wireClueSprites;
    public bool hasFoundWireClue { get; private set; }

    [Header("SymbolClue")]
    [SerializeField] private Image[] symbolLocations;
    [SerializeField] private Sprite[] symbolSprites;
    public bool hasFoundSymbolClue { get; private set; }

    [Header("SequenceClue")]
    [SerializeField] private Image[] seqLocations;
    [SerializeField] private Sprite[] seqSprites;
    public bool hasFoundSequenceClue { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SetupWireClueUI(int id)
    {
        clueShowLoc.sprite = wireClueSprites[id];
    }



    public void MarkWireClueAsFound()
    {
        if (!hasFoundWireClue)
            hasFoundWireClue = true;
    }

    public void MarkSymbolClueAsFound()
    {
        if (!hasFoundSymbolClue)
            hasFoundSymbolClue = true;
    }

    public void MarkSequenceClueAsFound()
    {
        if (!hasFoundSequenceClue)
            hasFoundSequenceClue = true;
    }
}
