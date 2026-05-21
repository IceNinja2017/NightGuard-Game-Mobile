using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNightScene : MonoBehaviour, IInteractable
{
    string IInteractable.GetDescription()
    {
        return "Start Shift";
    }

    void IInteractable.Interact()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("NightIntroScene");
    }

    bool IInteractable.isComplete()
    {
        return false;
    }
}
