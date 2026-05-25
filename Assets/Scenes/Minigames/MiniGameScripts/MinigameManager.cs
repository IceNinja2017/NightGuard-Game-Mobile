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

    // It returns true if a minigame is currently running
    public bool IsMinigameActive => currentCallback != null;

    [Header("Main UI")]
    public GameObject checklistUI;
    public GameObject minigameList;

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
    }

    private void Update()
    {
        if (IsMinigameActive) checklistUI.SetActive(false);
        else checklistUI.SetActive(true);
    }

    public void LoadMinigame(string sceneName, Action<bool> onFinished)
    {
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
            additionalPowerReward += powerChange; // Example: Each completed minigame adds 5 to the additional power reward
        }
        else
        {
            Debug.Log("Player LOST the minigame");
            powerChange = -3;
            additionalPowerReward -= powerChange; // Example: Each failed minigame subtracts 2 from the additional power reward
        }

        ShowPowerPopup(powerChange);

        currentCallback?.Invoke(success);

        Debug.Log("Unloading Minigame Scene: " + sceneName);

        SceneManager.UnloadSceneAsync(sceneName);

        currentCallback = null;
    }

    private void OnDestroy()
    {
        additionalPowerReward = Mathf.Clamp(additionalPowerReward, -8, 8); // Add the accumulated additional power reward to NightData when the manager is destroyed
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
}