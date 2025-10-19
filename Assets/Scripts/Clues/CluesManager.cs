using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CluesManager : MonoBehaviour
{
    public static CluesManager instance { get; private set; }

    [Header("WireCLue")]
    [SerializeField] private Image clueShowLoc;
    [SerializeField] private Sprite[] wireClueSprites;
    [SerializeField] private GameObject hasWireClueUI;
    public bool hasFoundWireClue { get; private set; }

    [Header("SymbolClue")]
    [SerializeField] private Image[] symbolLocations;
    [SerializeField] private Sprite[] symbolSprites;
    [SerializeField] private GameObject hasSymbomClueUI;
    public bool hasFoundSymbolClue { get; private set; }

    [Header("SequenceClue")]
    [SerializeField] private Image[] seqLocations;
    [SerializeField] private Sprite[] seqSprites;
    [SerializeField] private GameObject hasSeqClueUI;
    public bool hasFoundSequenceClue { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }

    public void SetupWireClueUI(int id)
    {
        clueShowLoc.sprite = wireClueSprites[id];
    }

    public void SetupSymbolClueUI()
    {
        for (int i = 0; i < 3; i++)
        {
            symbolLocations[i].sprite = symbolSprites[BombDefuseManager.instance.correctShapeIndices[i]];
        }
    }

    public void MarkWireClueAsFound()
    {
        if (!hasFoundWireClue)
        {
            hasFoundWireClue = true;
            hasWireClueUI.SetActive(true);
        }
    }

    public void MarkSymbolClueAsFound()
    {
        if (!hasFoundSymbolClue)
        {
            hasFoundSymbolClue = true;
            hasSymbomClueUI.SetActive(true);
        }
    }

    public void MarkSequenceClueAsFound()
    {
        if (!hasFoundSequenceClue)
        {
            hasFoundSequenceClue = true;
            hasSeqClueUI.SetActive(true);
        }
    }

    public void SetupSequenceClueUI()
    {
        for (int i = 0; i < 3; i++)
        {
            seqLocations[i].sprite = seqSprites[BombDefuseManager.instance.correctSequence[i]];   
        }
    }
}
