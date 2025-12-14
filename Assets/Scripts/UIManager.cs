using System.Collections;
using TMPro; 
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    [Header("Yönetici Bağlantıları")]
    public BlackjackManager blackjackManager; 

    [Header("Ses Ayarları (GÜNCELLENDİ)")]
    public AudioSource buttonSound; 
    public AudioSource chipSound;   

    [Header("Ekonomi (Wallet)")]
    [SerializeField] private TextMeshProUGUI walletText; 
    private int totalMoney = 10000; 

    [Header("Bahis UI")]
    [SerializeField] private TextMeshProUGUI betAmountText; 
    [SerializeField] private GameObject betChipsPanel; 
    public Button dealButton; 
    [SerializeField] private GameObject clearButtonObj; 

    [Header("Uyarılar")]
    [SerializeField] private GameObject maxBetError; 
    [SerializeField] private GameObject zeroBetWarning; 
    [SerializeField] private GameObject notEnoughMoneyWarning; 

    [Header("Oyun Metinleri")]
    public TextMeshProUGUI playerScoreText; 
    public TextMeshProUGUI dealerScoreText; 
    public GameObject actionButtonsPanel; 

    [Header("Oyun Sonu")]
    [SerializeField] private TextMeshProUGUI resultText; 
    
    private int maxBetAmount = 1000; 
    private int betAmount = 0; 

    void Start()
    {
        ResetUIForNewRound();
        UpdateWalletUI();
    }

    void UpdateWalletUI()
    {
        if(walletText != null) walletText.text = totalMoney.ToString();
    }
    
    public void PlayClickSound()
    {
        if (buttonSound != null) buttonSound.Play();
    }
    
    public void PlayChipSound()
    {
        if (chipSound != null)
        {
            chipSound.PlayOneShot(chipSound.clip);
        }
    }
    
    public void IncreaseBet(int amount)
    {
        if(zeroBetWarning) zeroBetWarning.SetActive(false);
        if(notEnoughMoneyWarning) notEnoughMoneyWarning.SetActive(false);

        int potansiyelMiktar = betAmount + amount;

        if (potansiyelMiktar > totalMoney)
        {
            betAmount = totalMoney; 
            if(notEnoughMoneyWarning) notEnoughMoneyWarning.SetActive(true);
        }
        else if (potansiyelMiktar >= maxBetAmount)
        {
            betAmount = maxBetAmount; 
            if(maxBetError) maxBetError.SetActive(true);
        }
        else
        {
            betAmount = potansiyelMiktar;
            if(maxBetError) maxBetError.SetActive(false);
        }

        betAmountText.text = betAmount.ToString();
    }

    public void ClearBet()
    {
        PlayClickSound();
        betAmount = 0;
        betAmountText.text = "0";
        
        if(maxBetError) maxBetError.SetActive(false);
        if(notEnoughMoneyWarning) notEnoughMoneyWarning.SetActive(false);
        if(zeroBetWarning) zeroBetWarning.SetActive(false);
    }
    
    public void Bet1() { PlayChipSound(); IncreaseBet(1); }
    public void Bet5() { PlayChipSound(); IncreaseBet(5); }
    public void Bet10() { PlayChipSound(); IncreaseBet(10); }
    public void Bet50() { PlayChipSound(); IncreaseBet(50); }
    public void Bet100() { PlayChipSound(); IncreaseBet(100); }
    
    public void OnDealButtonPressed()
    {
        PlayClickSound();

        if (betAmount <= 0)
        {
            if(zeroBetWarning) zeroBetWarning.SetActive(true);
            return; 
        }
        
        totalMoney -= betAmount; 
        UpdateWalletUI();
        
        if(betChipsPanel) betChipsPanel.SetActive(false);
        if(actionButtonsPanel) actionButtonsPanel.SetActive(true);
        if(clearButtonObj) clearButtonObj.SetActive(false);

        dealButton.gameObject.SetActive(false); 
        
        if(maxBetError) maxBetError.SetActive(false);

        playerScoreText.gameObject.SetActive(true);
        dealerScoreText.gameObject.SetActive(true);

        blackjackManager.StartGame();
    }
    
    public void OnHitButtonPressed()
    {
        PlayClickSound();
        blackjackManager.PlayerHit();
    }

    public void OnStandButtonPressed()
    {
        PlayClickSound();
        if(actionButtonsPanel) actionButtonsPanel.SetActive(false);
        blackjackManager.PlayerStand();
    }

    public void UpdateScoreUI(bool isPlayer, string scoreText)
    {
        if (isPlayer) playerScoreText.text = scoreText;
        else dealerScoreText.text = scoreText;
    }
    
    public void GameResult(bool playerWins, string message, bool isPush = false)
    {
        if(actionButtonsPanel) actionButtonsPanel.SetActive(false);

        if (resultText)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = message;
        }

        if (playerWins)
        {
            int winnings = betAmount * 2;
            totalMoney += winnings;
        }
        else if (isPush)
        {
            totalMoney += betAmount;
        }

        UpdateWalletUI();
        StartCoroutine(AutoResetRoutine());
    }

    IEnumerator AutoResetRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        ResetUIForNewRound();
    }
    
    void ResetUIForNewRound()
    {
        blackjackManager.CleanTable();

        playerScoreText.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        playerScoreText.text = "0";
        dealerScoreText.text = "0";

        if(resultText) resultText.gameObject.SetActive(false);
        
        if(betChipsPanel) betChipsPanel.SetActive(true); 
        if(actionButtonsPanel) actionButtonsPanel.SetActive(false);
        
        if(clearButtonObj) clearButtonObj.SetActive(true);

        dealButton.gameObject.SetActive(true); 
        
        if(maxBetError) maxBetError.SetActive(false);
        if(zeroBetWarning) zeroBetWarning.SetActive(false);
        if(notEnoughMoneyWarning) notEnoughMoneyWarning.SetActive(false);
        
        betAmount = 0;
        betAmountText.text = "0";
    }

    public void MenuButton()
    {
        PlayClickSound();
        SceneManager.LoadScene("MainMenu");
    }
}