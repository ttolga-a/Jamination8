using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider musicSlider;

    private const string VOLUME_KEY = "MusicVolume"; // AudioManager'daki ile ayný olmalý

    void Start()
    {
        // Slider'ýn baþlangýç deðerini kayýtlý sese ayarla
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.75f);
        musicSlider.value = savedVolume;

        // Slider deðeri deðiþtiðinde AudioManager'daki fonksiyonu çaðýr
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
    }

    public void OnMusicSliderChanged(float value)
    {
        // AudioManager'ý bul ve ses ayarýný güncelle
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(value);
        }
    }
}