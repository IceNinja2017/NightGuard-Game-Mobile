using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryMatchManager : MonoBehaviour
{
    public Button redButton;
    public Button blueButton;
    public Button yellowButton;
    public Button greenButton;

    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();

    private int sequenceLength = 3;

    void Start()
    {
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
        yield return new WaitForSeconds(1f);

        foreach (int color in sequence)
        {
            Button currentButton = GetButton(color);

            currentButton.transform.localScale = Vector3.one * 1.2f;

            yield return new WaitForSeconds(0.5f);

            currentButton.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(0.3f);
        }
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
        playerInput.Add(value);

        int currentIndex = playerInput.Count - 1;

        if (playerInput[currentIndex] != sequence[currentIndex])
        {
            Debug.Log("WRONG!");
            return;
        }

        if (playerInput.Count == sequence.Count)
        {
            Debug.Log("SUCCESS!");
        }
    }
}
