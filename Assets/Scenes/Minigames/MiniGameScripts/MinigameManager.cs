using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    public bool IsMinigameActive => currentCallback != null; // returns true if a minigame is currently running

    [Header("Main UI")]
    public GameObject checklistUI;
    public GameObject minigameList;

    [System.Serializable]
    public struct MinigameTextMapping
    {
        [Tooltip("The exact name of the minigame scene (e.g., CardFlipScene)")]
        public string sceneName;
        [Tooltip("The text GameObject from your hierarchy (e.g., CardFlip, ColorConnect)")]
        public TMP_Text checklistText;
    }
    [Header("Checklist Text Elements")]
    [SerializeField] private List<MinigameTextMapping> minigameChecklist;

    [Header("Game Limit Settings")]
    [SerializeField] private int maxSuccessfulGames = 3;
    private int successfulGamesCount = 0;

    [Header("Global Timer Settings")]
    [Tooltip("Total time in seconds the player has to play minigames (e.g., 180 seconds = 3 minutes)")]
    [SerializeField] private float timeRemaining = 180f;
    [Tooltip("Drag the TimerText UI object you just created here")]
    [SerializeField] private TMP_Text timerText;
    private bool isTimerRunning = true;

    [Header("Popup UI")]
    [SerializeField] private GameObject powerPopup;
    [SerializeField] private TMP_Text powerPopupText;
    [SerializeField] private float popupDuration = 2f;

    [Header("Reward Settings")]
    private int additionalPowerReward;

    private Action<bool> currentCallback;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NightData.Instance.additionalPower = 0; // Reset additional power when starting prologue
        additionalPowerReward = 0; // Initialize the additional power reward
        powerPopup.SetActive(true);
        powerPopup.transform.localScale = Vector3.zero;
        powerPopup.SetActive(false);

        if (NightData.Instance.getCurrentNight() == 1)
        {
            minigameList.SetActive(false); // Hide the minigame list when night 1
        }

        PlayerDialogueManager.Instance.PlayDialogue(DialogueEventType.NightStart, 1f); // Play night start dialogue with a delay
    }

    private void Update()
    {
        if (IsMinigameActive) checklistUI.SetActive(false);
        else checklistUI.SetActive(true);

        // --- RUNS THE TIMER LOGIC EVERY FRAME ---
        UpdateGlobalTimer();
    }

    // --- METHOD: HANDLES TIMER COUNTDOWN AND DISPLAY FORMATTING ---
    private void UpdateGlobalTimer()
    {
        if (!isTimerRunning) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
        {
            Debug.Log("Time ran out!");
            timeRemaining = 0;
            isTimerRunning = false;
            OnTimeRanOut();
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;

        // Calculates minutes and seconds structural presentation
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Updates the text block directly into a "00:00" string structure
        if (timeRemaining <= 11)
        {
            PlayerDialogueManager.Instance.PlayDialogue(DialogueEventType.TimerEnd);
        }

        if (timeToDisplay <= 11)
        {
            timerText.text = string.Format("<color=red>{0:00}:{1:00}</color>", minutes, seconds);
        }
        else if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void OnTimeRanOut()
    {
        if (timerText != null)
        {
            timerText.text = "<color=red>00:00</color>";
        }

        // Force-closes active additive minigames immediately on timeout failure
        if (IsMinigameActive)
        {
            string activeMinigameScene = "";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.name != gameObject.scene.name)
                {
                    activeMinigameScene = s.name;
                    break;
                }
            }
        }

        // Hide menus completely since time is up
        minigameList.SetActive(false);
        checklistUI.SetActive(false);

        // ----NOTE: Code After Timer Runsout----
        Debug.LogWarning("Time has expired entirely. Restricting menu access.");
        SceneManager.LoadScene("Freeroam_Jumpscare");
    }

    public void LoadMinigame(string sceneName, Action<bool> onFinished)
    {
        // --- SAFEGUARD CHECK: Blocks loading if 3 wins are achieved OR if timer hits 00:00 ---
        if (successfulGamesCount >= maxSuccessfulGames || timeRemaining <= 0)
        {
            Debug.LogWarning($"Cannot load {sceneName}. Maximum games cleared or timeline expired.");
            return;
        }

        currentCallback = onFinished;

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void CompleteMinigame(bool success, string sceneName)
    {
        Debug.Log("Minigame Finished: " + sceneName);

        int powerChange = 0;

        if (success)
        {
            Debug.Log("Player WON the minigame");
            powerChange = 3;
            additionalPowerReward += powerChange; // Example: Each completed minigame adds 3 to the additional power reward

            // --- TRACK PROGRESS AND STRIKE OUT TEXT IN RED ---
            successfulGamesCount++;
            StrikethroughCompletedMinigame(sceneName);

            // Shuts off selection panel and stops timer immediately when reaching the 3 game limit
            if (successfulGamesCount >= maxSuccessfulGames)
            {
                minigameList.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Player LOST the minigame");
            powerChange = -2;
            additionalPowerReward += powerChange; // Example: Each failed minigame subtracts 2 from the additional power reward
        }

        ShowPowerPopup(powerChange);

        currentCallback?.Invoke(success);

        Debug.Log("Unloading Minigame Scene: " + sceneName);

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        currentCallback = null;
    }

    private void OnDestroy()
    {
        additionalPowerReward = Mathf.Clamp(additionalPowerReward, -9, 9); // Add the accumulated additional power reward to NightData when the manager is destroyed
        NightData.Instance.additionalPower = additionalPowerReward;
    }

    private void ShowPowerPopup(int amount)
    {
        powerPopup.SetActive(true);

        if (amount >= 0)
        {
            powerPopupText.text = "+" + amount + " Power";
            powerPopupText.color = Color.green;
        }
        else
        {
            powerPopupText.text = amount + " Power";
            powerPopupText.color = Color.red;
        }

        // reset any previous tweens
        powerPopup.transform.DOKill();

        // POP IN animation
        powerPopup.transform.localScale = Vector3.zero;

        powerPopup.transform
            .DOScale(1f, 0.25f)
            .SetEase(Ease.OutBack);

        StopAllCoroutines();
        StartCoroutine(HidePopupCoroutine());
    }

    private IEnumerator HidePopupCoroutine()
    {
        yield return new WaitForSeconds(popupDuration);

        powerPopup.transform.DOKill();

        powerPopup.transform
            .DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                powerPopup.SetActive(false);
            });
    }

    private void StrikethroughCompletedMinigame(string sceneName)
    {
        foreach (var mapping in minigameChecklist)
        {
            if (mapping.sceneName == sceneName && mapping.checklistText != null)
            {
                if (!mapping.checklistText.text.StartsWith("<s>"))
                {
                    mapping.checklistText.text = $"<s><color=red>{mapping.checklistText.text}</color></s>";
                }
                break;
            }
        }
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}