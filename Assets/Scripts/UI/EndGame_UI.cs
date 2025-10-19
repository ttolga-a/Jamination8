using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame_UI : MonoBehaviour
{
    public Player player;

    public void PlayAgainButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
