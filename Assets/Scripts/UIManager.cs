using System.Collections;
using TMPro; 
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    [Header("Referances")]
    public BlackjackManager blackjackManager; 
    public Animator animator; 

    [Header("Wallet UI")]
    [SerializeField] private TextMeshProUGUI walletText; 
    private int totalMoney = 10000; 

    [Header("Bet UI")]
    [SerializeField] private TextMeshProUGUI betAmountText; 
    [SerializeField] private GameObject betChipsPanel; 
    public Button dealButton; 
    
    
    [SerializeField] private GameObject clearButtonObj; 

    [Header("Error UI")]
    [SerializeField] private GameObject maxBetError; 
    [SerializeField] private GameObject zeroBetWarning; 
    [SerializeField] private GameObject notEnoughMoneyWarning; 

    [Header("Game Text UI")]
    public TextMeshProUGUI playerScoreText; 
    public TextMeshProUGUI dealerScoreText; 
    public GameObject actionButtonsPanel; 

    [Header("GameOver UI")]
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
        walletText.text = totalMoney.ToString();
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

    public void DecreaseBet(int amount)
    {
        if(maxBetError) maxBetError.SetActive(false);
        if(notEnoughMoneyWarning) notEnoughMoneyWarning.SetActive(false);

        betAmount -= amount;
        if (betAmount < 0) betAmount = 0;

        betAmountText.text = betAmount.ToString();
    }

    public void ClearBet()
    {
        betAmount = 0;
        betAmountText.text = "0";
        
        if(maxBetError) maxBetError.SetActive(false);
        if(notEnoughMoneyWarning) notEnoughMoneyWarning.SetActive(false);
        if(zeroBetWarning) zeroBetWarning.SetActive(false);
    }
    
    public void Bet1() => IncreaseBet(1);
    public void Bet5() => IncreaseBet(5);
    public void Bet10() => IncreaseBet(10);
    public void Bet50() => IncreaseBet(50);
    public void Bet100() => IncreaseBet(100);

    public void OnDealButtonPressed()
    {
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

        dealButton.interactable = false; 
        
        if(maxBetError) maxBetError.SetActive(false);

        playerScoreText.gameObject.SetActive(true);
        dealerScoreText.gameObject.SetActive(true);

        blackjackManager.StartGame();
    }
    
    public void OnHitButtonPressed()
    {
        blackjackManager.PlayerHit();
    }

    public void OnStandButtonPressed()
    {
        if(actionButtonsPanel) actionButtonsPanel.SetActive(false);
        blackjackManager.PlayerStand();
    }

    public void UpdateScoreUI(bool isPlayer, int score)
    {
        if (isPlayer) playerScoreText.text = score.ToString();
        else dealerScoreText.text = score.ToString();
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

        dealButton.interactable = true; 
        
        if(maxBetError) maxBetError.SetActive(false);
        if(zeroBetWarning) zeroBetWarning.SetActive(false);
        if(notEnoughMoneyWarning) notEnoughMoneyWarning.SetActive(false);
        
        betAmount = 0;
        betAmountText.text = "0";
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}