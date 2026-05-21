using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MaIn : MonoBehaviour
{
    static public MaIn Instance;


    public int switchCount;
    public GameObject winText;
    private int onCount = 0;


    public void Awake()
    {
        winText.SetActive(false);
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SwitchChange(int points)
    {
        onCount = onCount + points;

        if (onCount == switchCount)
        {
            winText.SetActive(true);
            StartCoroutine(UnloadAfterDelay(2f)); 
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator UnloadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        MinigameManager.Instance.CompleteMinigame(true, "ColorConnect");
    }
}
