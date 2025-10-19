using UnityEngine;

// Bu satýr, script'i eklediðin objeye otomatik olarak bir AudioSource bileþeni ekler.
// Eðer zaten varsa eklemez, böylece hata almayý önler.
[RequireComponent(typeof(AudioSource))]
public class UISoundPlayer : MonoBehaviour
{
    // Bütün sesleri çalmak için kullanacaðýmýz merkezi ses kaynaðý
    private AudioSource audioSource;

    // Oyun baþladýðýnda bir kere çalýþýr
    void Awake()
    {
        // Objeye eklediðimiz AudioSource bileþenine ulaþýp onu deðiþkene atýyoruz
        audioSource = GetComponent<AudioSource>();
    }

    // Butonlarýn OnClick event'inden çaðýracaðýmýz ana fonksiyon bu.
    // Dýþarýdan bir ses klibi (AudioClip) alacak þekilde tasarlanmýþtýr.
    public void PlaySound(AudioClip soundClip)
    {
        // Eðer bir ses klibi atanmýþsa
        if (soundClip != null)
        {
            // PlayOneShot, mevcut sesi kesmeden yeni sesi çalar.
            // Butonlara hýzlý hýzlý bassan bile sesler düzgün çalýþýr.
            audioSource.PlayOneShot(soundClip);
        }
        else
        {
            // Hata olmasýn diye uyarý verelim
            Debug.LogWarning("PlaySound fonksiyonuna bir ses klibi atanmamýþ!");
        }
    }
}