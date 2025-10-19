using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BombDefuseManager : MonoBehaviour
{
    public static BombDefuseManager instance;
    public WireClue wireClue;
    public SymbolClue symbolClue;
    public SequanceClue seqClue;
    public FrequancyClue freqClue;
    public Player player;

    [SerializeField] private Image[] activeLights;
    public GameObject bombUI;

    [Header("Wire Quiz")]
    [SerializeField] private Button[] wires;
    public int wiresCorrectIndex;
    private int correctAnswerNeeded = 4;

    [Header("SymbolQuiz")]
    [SerializeField] private Sprite[] allShapes;
    [SerializeField] private Image[] displayShapes;
    [SerializeField] private Button[] symbolQuizButtons;
    public int[] correctShapeIndices = new int[3];
    private int[] playerSelectedIndices = new int[3] { 0, 0, 0 }; // Baþlangýçta hepsi ilk þekil (index 0)

    [Header("Sequance Puzzle")]
    [SerializeField] private Button[] sequanceButtons;
    public List<int> correctSequence = new List<int>();
    private List<int> playerInputSequence = new List<int>();

    [Header("FrequencyQuiz")]
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text valueText;
    [Tooltip("Doðru kabul edilecek hata payý (+/-). Örn: 0.04")]
    [SerializeField] private float tolerance = 0.04f;
    public float targetValue;
    [SerializeField] private RectTransform waveVisualTransform;
    [SerializeField] private float maxScaleX = 1.5f;
    [SerializeField] private float minScaleX = 0.2f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetupWireQuiz();
        SetupSymbolQuiz();
        SetupSequanceQuiz();
        SetupFrequencyQuiz();
    }

    private void Update()
    {
        if (correctAnswerNeeded == 0)
        {
            BombManager.instance.OpenEndScreen(true);
        }
    }

    public void CloseBombUI()
    {
        bombUI.SetActive(!bombUI.activeSelf);
        player.UnlockPlayer();
    }

    private void SetupWireQuiz()
    {
        SetupWireQuizCorrectAnswer();
        for (int i = 0; i < wires.Length; i++)
        {
            int wireIndex = i;
            wires[i].onClick.AddListener(() => WireClicked(wireIndex));
        }
    }

    private void SetupWireQuizCorrectAnswer()
    {
        wiresCorrectIndex = UnityEngine.Random.Range(0, wires.Length);
        wireClue.SetupWireClue(wiresCorrectIndex);
    }
    private void WireClicked(int clickedWireIndex)
    {
        wires[clickedWireIndex].interactable = false;
        if (clickedWireIndex == wiresCorrectIndex)
        {
            correctAnswerNeeded--;
            activeLights[1].color = Color.green;
            foreach (Button wireButton in wires)
            {
                wireButton.interactable = false;
            }
        }
        else
        {
            BombManager.instance.BombTimeChanger(UnityEngine.Random.Range(-25, -10));
        }
    }
    private void SetupSymbolQuiz()
    {
        for (int i = 0; i < correctShapeIndices.Length; i++)
        {
            correctShapeIndices[i] = UnityEngine.Random.Range(0, allShapes.Length);
        }

        Debug.Log("Doðru Kombinasyon: " + correctShapeIndices[0] + ", " + correctShapeIndices[1] + ", " + correctShapeIndices[2]);
        symbolClue.SetupSymbolClue();
        

        UpdateAllDisplayShapes();
    }

    private void UpdateAllDisplayShapes()
    {
        for (int i = 0; i < displayShapes.Length; i++)
        {
            displayShapes[i].sprite = allShapes[playerSelectedIndices[i]];
        }
    }

    public void GoToNextShape(int slotIndex)
    {
        playerSelectedIndices[slotIndex]++;

        if (playerSelectedIndices[slotIndex] >= allShapes.Length)
        {
            playerSelectedIndices[slotIndex] = 0;
        }

        displayShapes[slotIndex].sprite = allShapes[playerSelectedIndices[slotIndex]];
    }

    public void GoToPreviousShape(int slotIndex)
    {

        playerSelectedIndices[slotIndex]--;


        if (playerSelectedIndices[slotIndex] < 0)
        {
            playerSelectedIndices[slotIndex] = allShapes.Length - 1;
        }

        displayShapes[slotIndex].sprite = allShapes[playerSelectedIndices[slotIndex]];
    }

    public void CheckSymbolTestAnswer()
    {
        if (playerSelectedIndices.SequenceEqual(correctShapeIndices))
        {
            correctAnswerNeeded--;
            activeLights[0].color = Color.green;

            foreach (Button btn in symbolQuizButtons)
            {
                btn.interactable = false;
            }
        }
        else
        {
            BombManager.instance.BombTimeChanger(UnityEngine.Random.Range(-25, -10));
        }
    }

    private void SetupSequanceQuiz()
    {
        for (int i = 0; i < sequanceButtons.Length; i++)
        {
            int buttonIndex = i; 
            sequanceButtons[i].onClick.AddListener(() => ButtonPressed(buttonIndex));
        }

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < sequanceButtons.Length; i++)
        {
            availableIndices.Add(i);
        }

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count);
            correctSequence.Add(availableIndices[randomIndex]);
            availableIndices.RemoveAt(randomIndex);
        }

        seqClue.SetupSymbolClue();
        Debug.Log($"Doðru Sýralama: {correctSequence[0]}, {correctSequence[1]}, {correctSequence[2]}");
    }

    private void ButtonPressed(int buttonIndex)
    {
        playerInputSequence.Add(buttonIndex);

        for (int i = 0; i < playerInputSequence.Count; i++)
        {
            if (playerInputSequence[i] != correctSequence[i])
            {
                BombManager.instance.BombTimeChanger(UnityEngine.Random.Range(-25, -10));
                playerInputSequence.Clear();
                return;
            }
        }

        if (playerInputSequence.Count == correctSequence.Count)
        {
            correctAnswerNeeded--;
            activeLights[2].color = Color.green;

            foreach (Button btn in sequanceButtons)
            {
                btn.interactable = false;
            }
        }
    }

    private void SetupFrequencyQuiz()
    {
        targetValue = Mathf.Round(UnityEngine.Random.Range(0.0f, 1.0f) * 100f) / 100f;
        Debug.Log($"Hedef Frekans: {targetValue} (Bu ipucu oyun içinde baþka bir yerden bulunmalý)");
        //freqClue.SetupFrequancyClue(targetValue);

        frequencySlider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(frequencySlider.value);

        confirmButton.onClick.AddListener(CheckFinalAnswer);
    }

    private void OnSliderValueChanged(float value)
    {
        if (valueText != null)
        {
            valueText.text = value.ToString("F2");
        }

        if (waveVisualTransform != null)
        {
            // Slider'ýn deðerini (0-1), maxScale ve minScale aralýðýna çeviriyoruz.
            float newScaleX = Mathf.Lerp(maxScaleX, minScaleX, value);

            // Görselin mevcut ölçeðini alýp sadece x deðerini güncelliyoruz.
            Vector3 currentScale = waveVisualTransform.localScale;
            waveVisualTransform.localScale = new Vector3(newScaleX, currentScale.y, currentScale.z);
        }
    }

    public void CheckFinalAnswer()
    {
        if (Mathf.Abs(frequencySlider.value - targetValue) <= tolerance)
        {
            correctAnswerNeeded--;
            activeLights[3].color = Color.green;

            frequencySlider.interactable = false;
            confirmButton.interactable = false;
        }
        else
        {
            BombManager.instance.BombTimeChanger(UnityEngine.Random.Range(-25, -10));
        }
    }
}