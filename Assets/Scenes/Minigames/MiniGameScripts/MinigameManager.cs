using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    enum Minigames
    {
        None,
        Matching,
        ColorConnect,
        Minigame3
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Selected = Minigames.Matching;
            LoadMinigame(Selected.ToString());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Selected = Minigames.ColorConnect;
            LoadMinigame(Selected.ToString());
        }
    }

    Minigames Selected;
    public void LoadMinigame(string Minigame)
    {
        switch (Selected)
        {
            case Minigames.Matching:
                UnityEngine.SceneManagement.SceneManager.LoadScene("Matching", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                break;
            case Minigames.ColorConnect:
                UnityEngine.SceneManagement.SceneManager.LoadScene("ColorConnect", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                break;
        }
    }
}
