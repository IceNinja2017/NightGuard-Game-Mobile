using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BathroomGameManager : MonoBehaviour
{
    [Header("Game Entities")]
    [SerializeField] private List<HeadPop> heads;

    [Header("UI Text Fields")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("UI Windows / Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    public float startingTime = 30f;
    private float timeRemaining;

    private HashSet<HeadPop> currentHeads = new HashSet<HeadPop>();
    private int score = 0;
    private bool playing = false;
    private int targetScore;

    private float spawnTimer;
    private float spawnInterval = 0.5f;

    public void Start()
    {
        if (NightData.Instance.getCurrentNight() <= 2)
        {
            targetScore = 10;
        }
        else if (NightData.Instance.getCurrentNight() == 3)
        {
            targetScore = 15;
        }
        else if (NightData.Instance.getCurrentNight() == 4)
        {
            targetScore = 20;
        }
        else
        {
            targetScore = 25;
        }

            Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ResetAndStartGame();
    }

    private void ResetAndStartGame()
    {
        timeRemaining = startingTime;
        score = 0;
        playing = true;
        spawnTimer = 0f;
        currentHeads.Clear();

        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        UpdateUIFields();

        if (heads != null && heads.Count > 0)
        {
            int initialPop = Random.Range(0, heads.Count);
            heads[initialPop].Activate(1);
            currentHeads.Add(heads[initialPop]);
        }
    }

    private void Update()
    {
        if (!playing) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            UpdateUIFields();
            CheckEndGameConditions();
            return;
        }

        UpdateUIFields();

        if (score >= targetScore)
        {
            CheckEndGameConditions();
            return;
        }

        int currentLevel = score / 10;
        int maxActiveHeads = Mathf.Clamp(1 + currentLevel, 1, 4);

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;

            if (currentHeads.Count < maxActiveHeads)
            {
                int index = Random.Range(0, heads.Count);

                if (!currentHeads.Contains(heads[index]))
                {
                    currentHeads.Add(heads[index]);
                    heads[index].Activate(currentLevel + 1);
                }
            }
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateUIFields()
    {
        if (scoreText != null) scoreText.text = $"Score: {score} / {targetScore}";
        if (timerText != null) timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s";
    }

    private void CheckEndGameConditions()
    {
        playing = false;

        foreach (HeadPop head in heads)
        {
            if (head != null)
            {
                head.StopAllCoroutines();
                head.Hide();
            }
        }

        // Determine if the game was a success or failure
        bool isWin = score >= targetScore;

        if (isWin)
        {
            if (winPanel != null) winPanel.SetActive(true);
        }
        else
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }

        // START THE COROUTINE HERE and pass the result (e.g. 2 second delay)
        StartCoroutine(UnloadAfterDelay(2f, isWin));
    }

    public void AddScore(int headIndex)
    {
        if (!playing) return;

        score += 1;
        currentHeads.Remove(heads[headIndex]);
    }

    public void Missed(int headIndex, bool isHead)
    {
        if (!playing) return;

        currentHeads.Remove(heads[headIndex]);
    }

    // Adjusted to accept whether it's a win or lose condition dynamically
    private IEnumerator UnloadAfterDelay(float delay, bool dynamicWinStatus)
    {
        yield return new WaitForSeconds(delay);

        if (MinigameManager.Instance != null)
        {
            MinigameManager.Instance.CompleteMinigame(dynamicWinStatus, "WackaAnimatronic");
        }
        else
        {
            Debug.LogWarning("MinigameManager Instance is missing in the scene!");
        }
    }
}