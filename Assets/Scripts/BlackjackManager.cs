using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CardData
{
    public string cardName; 
    public Sprite sprite;    
    public int point;        
}

public class BlackjackManager : MonoBehaviour
{
    [Header("References")]
    public UIManager uiManager;

    [Header("Game Objects")]
    public GameObject cardPrefab;      
    public Transform deckPosition;     
    public Transform playerHandPos;    
    public Transform dealerHandPos;    
    public Sprite cardBackSprite;      

    [Header("Decks of Cards")]
    public List<CardData> deckOfCards; 
    
    private List<int> playerHandValues = new List<int>(); 
    private List<int> dealerHandValues = new List<int>();
    
    private List<GameObject> activeCards = new List<GameObject>();
    
    private int currentCardIndex = 0;   
    private int currentSortOrder = 10;  
    private CardDisplay dealerHiddenCardDisplay; 

    void Start()
    {
        ShuffleDeck();
    }
    
    public void StartGame()
    {
        playerHandValues.Clear();
        dealerHandValues.Clear();
        dealerHiddenCardDisplay = null;
        currentSortOrder = 10;
        
        ShuffleDeck(); 
        
        StartCoroutine(StartGameDeal());
    }
    
    public void CleanTable()
    {
        foreach (GameObject card in activeCards)
        {
            if (card != null) Destroy(card);
        }
        activeCards.Clear();
    }
    
    public void PlayerHit()
    {
        StartCoroutine(DealSingleCard(playerHandPos, true, true));
    }

    public void PlayerStand()
    {
        StartCoroutine(DealerTurn());
    }
    
    IEnumerator DealerTurn() //kurpiyer yapay zeka
    {
        if (dealerHiddenCardDisplay != null)
        {
            dealerHiddenCardDisplay.FlipCard(); 
        }
        
        NotifyUI(false); 
        yield return new WaitForSeconds(1f);

        while (CalculateScore(dealerHandValues) < 17)
        {
            yield return StartCoroutine(DealSingleCard(dealerHandPos, true, false));
            if(CalculateScore(dealerHandValues) > 21) break; 
        }

        DetermineWinner();
    }
    
    void DetermineWinner() 
    {
        int playerScore = CalculateScore(playerHandValues);
        int dealerScore = CalculateScore(dealerHandValues);

        string resultMessage = "";
        bool playerWins = false;
        bool isPush = false;

        if (playerScore > 21)
        {
            playerWins = false;
            resultMessage = "BUST! (21'i Geçtin)";
        }
        else if (dealerScore > 21)
        {
            playerWins = true;
            resultMessage = "Kurpiyer Battı! KAZANDIN!";
        }
        else if (playerScore > dealerScore)
        {
            playerWins = true;
            resultMessage = "Tebrikler! KAZANDIN!";
        }
        else if (playerScore < dealerScore)
        {
            playerWins = false;
            resultMessage = "Kurpiyer Kazandı.";
        }
        else
        {
            playerWins = false;
            isPush = true;
            resultMessage = "BERABERE";
        }

        uiManager.GameResult(playerWins, resultMessage, isPush);
    }
    
    IEnumerator StartGameDeal()
    {
        yield return StartCoroutine(DealSingleCard(playerHandPos, true, true));
        yield return StartCoroutine(DealSingleCard(dealerHandPos, true, false));
        yield return StartCoroutine(DealSingleCard(playerHandPos, true, true));
        yield return StartCoroutine(DealSingleCard(dealerHandPos, false, false));
        
        Debug.Log("Sıra Oyuncuda.");
    }

    IEnumerator DealSingleCard(Transform handPos, bool isFaceUp, bool isPlayer)
    {
        GameObject newCardObj = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity);
        CardDisplay cardScript = newCardObj.GetComponent<CardDisplay>();
        
        activeCards.Add(newCardObj);

        CardData cekilenKart = deckOfCards[currentCardIndex];
        
        cardScript.Setup(cekilenKart.sprite, cardBackSprite);
        cardScript.render.sortingOrder = currentSortOrder;
        currentSortOrder++;
        currentCardIndex++; 

        if (isPlayer) playerHandValues.Add(cekilenKart.point);
        else dealerHandValues.Add(cekilenKart.point);

        if (!isPlayer && !isFaceUp) dealerHiddenCardDisplay = cardScript;

        float xOffset = 0.5f; 
        int cardCount = isPlayer ? playerHandValues.Count : dealerHandValues.Count;
        Vector3 targetPos = handPos.position + new Vector3(xOffset * (cardCount - 1), 0, 0);
        
        float speed = 15f; 
        while (Vector3.Distance(newCardObj.transform.position, targetPos) > 0.05f)
        {
            newCardObj.transform.position = Vector3.MoveTowards(newCardObj.transform.position, targetPos, speed * Time.deltaTime);
            yield return null; 
        }
        newCardObj.transform.position = targetPos;

        if (isFaceUp) cardScript.FlipCard();

        NotifyUI(isPlayer); 

        if(isPlayer && CalculateScore(playerHandValues) > 21)
        {
            yield return new WaitForSeconds(0.5f); 
            uiManager.GameResult(false, "BUST! (21'i Geçtin)"); 
        }
        
        yield return new WaitForSeconds(0.2f); 
    }

    void NotifyUI(bool isPlayer)
    {
        if (isPlayer)
        {
            int score = CalculateScore(playerHandValues);
            uiManager.UpdateScoreUI(true, score);
        }
        else
        {
            bool hiddenCardIsClosed = (dealerHiddenCardDisplay != null && dealerHiddenCardDisplay.render.sprite == 
                cardBackSprite);

            if (dealerHandValues.Count > 1 && hiddenCardIsClosed)
            {
                 int visibleScore = dealerHandValues[0];
                 if(visibleScore == 11) visibleScore = 11;
                 uiManager.UpdateScoreUI(false, visibleScore);
            }
            else
            {
                int score = CalculateScore(dealerHandValues);
                uiManager.UpdateScoreUI(false, score);
            }
        }
    }

    int CalculateScore(List<int> hand)
    {
        int total = 0;
        int aceCount = 0;
        foreach (int cardVal in hand)
        {
            total += cardVal;
            if (cardVal == 11) aceCount++;
        }
        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }
        return total;
    }

    void ShuffleDeck()
    {
        currentCardIndex = 0;
        for (int i = 0; i < deckOfCards.Count; i++)
        {
            CardData temp = deckOfCards[i]; 
            int randomIndex = Random.Range(i, deckOfCards.Count); 
            deckOfCards[i] = deckOfCards[randomIndex];
            deckOfCards[randomIndex] = temp;
        }
    }
}