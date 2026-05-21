using UnityEngine;

public class MinigameInteractable : MonoBehaviour, IInteractable
{
    public string minigameScene;

    private bool completed = false;

    private void Start()
    {
        if(NightData.Instance.getCurrentNight() <= 1)
        {
            completed = true; // Mark as completed to prevent interaction before Night 2
        }
    }

    public void Interact()
    {
        //if (NightData.Instance.getCurrentNight() <= 1) return; // Only allow interaction after Night 1

        if (completed)
        {
            Debug.Log("Already completed!");
            return;
        }

        MinigameManager.Instance.LoadMinigame(minigameScene, OnMinigameFinished);
    }

    private void OnMinigameFinished(bool success)
    {
        if (success)
        {
            completed = true;

            Debug.Log("Minigame completed!");
        }
        else
        {
            Debug.Log("Failed minigame");
        }
    }

    public string GetDescription()
    {
        return completed ? "Completed" : "Play Minigame";
    }

    public bool isComplete()
    {
        return completed;
    }
}