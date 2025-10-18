using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuPanel;

    public void OpenSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
    }

    public void StartButton()
    {
        SceneManager.LoadScene("TolgaTestScene");
    }
}
