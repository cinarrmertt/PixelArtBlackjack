using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Animasyonları")]
    public Animator playButtonAnimator; 

    [Header("Ses Ayarları")]
    public AudioSource buttonSound; 

    public void PlayButton()
    {
        if (buttonSound != null)
        {
            buttonSound.Play();
        }

        StartCoroutine(PlayWithDelay());
    }

    IEnumerator PlayWithDelay()
    {
        if (playButtonAnimator != null)
        {
            playButtonAnimator.SetTrigger("PlayClick");
        }

        yield return new WaitForSeconds(.5f); 

        SceneManager.LoadScene("GameScene");
    }
    
    public void QuitButton()
    {
        if (buttonSound != null) buttonSound.Play();

        Debug.Log("Oyundan Çıkılıyor..."); 
        
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            // Eğer oyun derlenmişse (.exe ise) uygulamayı kapatır
            Application.Quit();
    #endif
    }
}