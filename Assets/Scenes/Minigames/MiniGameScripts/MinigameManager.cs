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
    }
}