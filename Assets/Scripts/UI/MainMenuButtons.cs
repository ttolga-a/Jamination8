using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditPanel;

    private void Start()
    {

    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
    }

    public void StartButton()
    {
        SceneManager.LoadScene("Map");
    }

    public void CreditOn()
    {
        creditPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
    public void CreditOff()
    {
        creditPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
