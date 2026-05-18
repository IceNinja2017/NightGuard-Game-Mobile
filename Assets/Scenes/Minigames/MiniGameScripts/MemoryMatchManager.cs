using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MemoryMatchManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button redButton;
    public Button blueButton;
    public Button yellowButton;
    public Button greenButton;

    [Header("Dot Manager")]
    public DotManager dotManager;

    [Header("UI Instruction")]
    public TextMeshProUGUI instructionText;

    [Header("Popup UI")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;

    [Header("Settings")]
    public int sequenceLength = 5;

    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();

    private bool canInput = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartGame();
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void StartGame()
    {
        canInput = false;
        playerInput.Clear();

        // SHOW instruction at start
        if (instructionText != null)
            instructionText.gameObject.SetActive(true);

        // RESET DOTS
        if (dotManager != null)
            dotManager.ResetDots();

        // HIDE popup at start
        if (popupPanel != null)
            popupPanel.SetActive(false);

        GenerateSequence();
        StartCoroutine(ShowSequence());
    }

    void GenerateSequence()
    {
        sequence.Clear();

        for (int i = 0; i < 5; i++)
        {
            sequence.Add(Random.Range(0, 4));
        }
    }

    IEnumerator ShowSequence()
    {
        canInput = false;

        yield return new WaitForSeconds(1f);

        // HIDE instruction when game starts
        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        foreach (int color in sequence)
        {
            Button btn = GetButton(color);

            HighlightButton(btn, true);
            yield return new WaitForSeconds(0.5f);

            HighlightButton(btn, false);
            yield return new WaitForSeconds(0.3f);
        }

        canInput = true;
    }

    void HighlightButton(Button btn, bool on)
    {
        if (btn == null) return;

        if (on)
            btn.image.transform.localScale = Vector3.one * 1.15f;
        else
            btn.image.transform.localScale = Vector3.one;
    }

    Button GetButton(int index)
    {
        switch (index)
        {
            case 0: return redButton;
            case 1: return blueButton;
            case 2: return yellowButton;
            case 3: return greenButton;
        }
        return null;
    }

    public void PressButton(int value)
    {
        if (!canInput) return;

        Button clickedBtn = GetButton(value);
        StartCoroutine(ClickEffect(clickedBtn));

        playerInput.Add(value);

        int currentIndex = playerInput.Count - 1;

        bool isCorrect = playerInput[currentIndex] == sequence[currentIndex];

        if (!isCorrect)
        {
            ShowFail();

            if (dotManager != null)
                dotManager.SetWrong();

            StartCoroutine(RestartSequence());
            return;
        }

        if (dotManager != null)
            dotManager.SetCorrect();

        if (playerInput.Count >= 5)
        {
            ShowSuccess();
            StartCoroutine(UnloadAfterDelay(2f));
        }
    }

    IEnumerator ClickEffect(Button btn)
    {
        if (btn == null) yield break;

        btn.image.transform.localScale = Vector3.one * 1.2f;
        yield return new WaitForSeconds(0.15f);
        btn.image.transform.localScale = Vector3.one;
    }

    // 🎉 SUCCESS UI
    void ShowSuccess()
    {
        if (popupPanel == null || popupText == null) return;

        popupPanel.SetActive(true);
        popupText.text = "SUCCESS!";
        popupText.color = Color.green;
    }

    // ❌ FAIL UI
    void ShowFail()
    {
        if (popupPanel == null || popupText == null) return;

        popupPanel.SetActive(true);
        popupText.text = "FAILED!";
        popupText.color = Color.red;

        StartCoroutine(HidePopup());
    }

    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(1.5f);

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    IEnumerator RestartSequence()
    {
        canInput = false;

        yield return new WaitForSeconds(1f);

        StartGame();
    }

    IEnumerator UnloadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.UnloadSceneAsync("Matching");
    }
}