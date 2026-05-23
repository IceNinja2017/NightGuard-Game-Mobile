using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    // ADD THIS LINE: It returns true if a minigame is currently running
    public bool IsMinigameActive => currentCallback != null;

    public GameObject minigameList; // Reference to the minigame list UI

    private Action<bool> currentCallback;

    private int additionalPowerReward; // This will hold the additional power reward for the current minigame

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

        if (NightData.Instance.getCurrentNight() == 1)
        {
            minigameList.SetActive(false); // Hide the minigame list when night 1
        }
    }

    public void LoadMinigame(string sceneName, Action<bool> onFinished)
    {
        currentCallback = onFinished;

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void CompleteMinigame(bool success, string sceneName)
    {
        Debug.Log("Minigame Finished: " + sceneName);

        if (success)
        {
            Debug.Log("Player WON the minigame");
        }
        else
        {
            Debug.Log("Player LOST the minigame");
        }

        currentCallback?.Invoke(success);

        Debug.Log("Unloading Minigame Scene: " + sceneName);

        SceneManager.UnloadSceneAsync(sceneName);

        currentCallback = null;

        additionalPowerReward += 5; // Example: Each completed minigame adds 5 to the additional power reward
    }

    private void OnDestroy()
    {
        NightData.Instance.additionalPower = additionalPowerReward; // Add the accumulated additional power reward to NightData when the manager is destroyed
    }
}