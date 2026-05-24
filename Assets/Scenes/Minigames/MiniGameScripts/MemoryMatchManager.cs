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

    [Header("UI")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI triesLeftUI;


    [Header("Popup UI")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;

    [Header("Settings")]
    public int sequenceLength = 5;

    [Header("Lives")]
    public int maxLives = 3;
    private int currentLives;

    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();

    private bool canInput = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartGame();
    }

    private void Update()
    {
        triesLeftUI.text = $"Tries Left: {currentLives}";
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

        currentLives = maxLives;

        if (instructionText != null)
            instructionText.gameObject.SetActive(true);

        if (dotManager != null)
            dotManager.ResetDots();

        if (popupPanel != null)
            popupPanel.SetActive(false);

        GenerateSequence();
        StartCoroutine(ShowSequence());
    }

    void GenerateSequence()
    {
        sequence.Clear();

        for (int i = 0; i < sequenceLength; i++)
        {
            sequence.Add(Random.Range(0, 4));
        }
    }

    IEnumerator ShowSequence()
    {
        canInput = false;

        yield return new WaitForSeconds(1f);

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

        btn.image.transform.localScale = on
            ? Vector3.one * 1.15f
            : Vector3.one;
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
            currentLives--;

            ShowFail();

            if (dotManager != null)
                dotManager.SetWrong();

            if (currentLives <= 0)
            {
                StartCoroutine(UnloadAfterDelay(1.5f, false));
                return;
            }

            StartCoroutine(RestartSequence());
            return;
        }

        if (dotManager != null)
            dotManager.SetCorrect();

        if (playerInput.Count >= sequenceLength)
        {
            ShowSuccess();
            StartCoroutine(UnloadAfterDelay(2f, true));
        }
    }

    IEnumerator ClickEffect(Button btn)
    {
        if (btn == null) yield break;

        btn.image.transform.localScale = Vector3.one * 1.2f;
        yield return new WaitForSeconds(0.15f);
        btn.image.transform.localScale = Vector3.one;
    }

    void ShowSuccess()
    {
        if (popupPanel == null || popupText == null) return;

        popupPanel.SetActive(true);
        popupText.text = "SUCCESS!";
        popupText.color = Color.green;
    }

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

        playerInput.Clear();

        if (dotManager != null)
            dotManager.ResetDots();

        StartCoroutine(ShowSequence());
    }

    IEnumerator UnloadAfterDelay(float delay, bool won)
    {
        yield return new WaitForSeconds(delay);
        MinigameManager.Instance.CompleteMinigame(won, "Matching");
    }
}