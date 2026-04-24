using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMenuButtons : MonoBehaviour
{
    public void handleNewGameBtn()
    {
        // Load the first night scene
        SaveManager.instance.CurrentNight = 1;
        NightData.Instance.setCurrentNight(1);

        UnityEngine.SceneManagement.SceneManager.LoadScene("NightIntroScene");
    }

    public void handleContinueBtn()
    {
        // Load the scene based on saved data
        NightData.Instance.setCurrentNight(NightData.Instance.getCurrentNight());
        UnityEngine.SceneManagement.SceneManager.LoadScene("NightIntroScene");
    }
}
