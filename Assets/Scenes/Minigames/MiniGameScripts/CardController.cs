using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [Header("Setup References")]
    [SerializeField] Card cardPrefab;
    [SerializeField] Transform gridTransform;
    [SerializeField] Sprite[] sprites;

    [Header("UI Elements")]
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private float previewDuration = 3.0f; // How long player gets to memorize positions

    private List<Sprite> spritePairs;
    private List<Card> spawnedCards = new List<Card>(); // Keeps track of all created cards for the preview
    private Card firstSelectedCard;
    private Card secondSelectedCard;

    private bool isCheckingMatches = false;
    private int totalMatchesLeft;
    private float timeRemaining;
    private bool isGameActive = false;
    private bool isPreviewing = true; // Blocks interaction during the opening reveal phase

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        timeRemaining = timeLimit;
        PrepareSprites();
        CreateCards();

        totalMatchesLeft = sprites.Length;

        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Run the opening card preview sequence
        StartCoroutine(RevealCardsAtStartCoroutine());
    }

    private void Update()
    {
        if (!isGameActive) return;

        // PRIORITIZE WIN: If there are no matches left, skip the timer countdown entirely
        if (totalMatchesLeft <= 0) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            UpdateTimerUI();
            GameOver(false);
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            spritePairs.Add(sprites[i]);
            spritePairs.Add(sprites[i]);
        }
        ShuffleSprites(spritePairs);
    }

    void CreateCards()
    {
        for (int i = 0; i < spritePairs.Count; i++)
        {
            Card card = Instantiate(cardPrefab, gridTransform);
            card.iconSprite = spritePairs[i];
            card.controller = this;
            card.Hide();
            spawnedCards.Add(card); // Saves references to reveal them in StartCoroutine
        }
    }

    // Opening Sequence: Flips everything face-up, pauses, flips them back down, then starts game loop
    private IEnumerator RevealCardsAtStartCoroutine()
    {
        isPreviewing = true;
        isGameActive = false; // Frozen clock during preview

        // Show every card face
        foreach (Card card in spawnedCards)
        {
            card.Show();
        }

        // Wait for your chosen duration
        yield return new WaitForSeconds(previewDuration);

        // Hide every card face again
        foreach (Card card in spawnedCards)
        {
            card.Hide();
        }

        isPreviewing = false;
        isGameActive = true; // Start the game clock countdown!
    }

    public void SetSelected(Card card)
    {
        // Added '|| isPreviewing' to make sure users can't cheat/click during the preview phase
        if (!isGameActive || isPreviewing || card.isSelected || isCheckingMatches) return;

        card.Show();

        if (firstSelectedCard == null)
        {
            firstSelectedCard = card;
        }
        else
        {
            secondSelectedCard = card;
            StartCoroutine(CheckMatchCoroutine());
        }
    }

    private IEnumerator CheckMatchCoroutine()
    {
        isCheckingMatches = true;

        yield return new WaitForSeconds(0.3f);

        if (firstSelectedCard.iconSprite == secondSelectedCard.iconSprite)
        {
            // Cleaned up to use your proper custom SetColor helper function!
            firstSelectedCard.GetComponent<Image>().color = Color.green;
            secondSelectedCard.GetComponent<Image>().color = Color.green;

            totalMatchesLeft--;

            if (totalMatchesLeft <= 0)
            {
                GameOver(true);
            }
        }
        else
        {
            if (timeRemaining > 0) // Only flip back down if the game hasn't timed out completely
            {
                firstSelectedCard.Hide();
                secondSelectedCard.Hide();
            }
        }

        firstSelectedCard = null;
        secondSelectedCard = null;
        isCheckingMatches = false;
    }

    private void GameOver(bool won)
    {
        isGameActive = false;

        if (won)
        {
            if (winPanel != null) winPanel.SetActive(true);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }
        else
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            if (winPanel != null) winPanel.SetActive(false);
        }

        // START THE UNLOAD TIMEOUT SEQUENCE HERE (e.g., 2 second delay)
        StartCoroutine(UnloadAfterDelay(2f, won));
    }

    void ShuffleSprites(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Now correctly handles passes through the true/false win status dynamically
    private IEnumerator UnloadAfterDelay(float delay, bool dynamicWinStatus)
    {
        yield return new WaitForSeconds(delay);

        if (MinigameManager.Instance != null)
        {
            MinigameManager.Instance.CompleteMinigame(dynamicWinStatus, "CardFlip");
        }
        else
        {
            Debug.LogWarning("MinigameManager Instance is missing in the scene!");
        }
    }
}