using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TimedTutorialUI : MonoBehaviour
{
    [Header("Target Elements")]
    [Tooltip("The TextMeshPro component that will fade out.")]
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Tooltip("The object to deactivate when finished (e.g., the specific panel/parent). If left empty, it deactivates this GameObject.")]
    [SerializeField] private GameObject objectToDeactivate;

    [Header("Night Settings")]
    [Tooltip("Which night should this tutorial appear on?")]
    [SerializeField] private int targetNight = 1;

    [Header("Timer Settings")]
    [Tooltip("How long the text stays visible before starting to fade.")]
    [SerializeField] private float displayDuration = 60f;

    [Tooltip("How long the fade-out animation takes.")]
    [SerializeField] private float fadeDuration = 1.5f;

    void Start()
    {
        // Fallback: If no specific object to deactivate is assigned, default to this one
        if (objectToDeactivate == null)
        {
            objectToDeactivate = gameObject;
        }

        // Check if the current night matches the specified target night
        if (NightData.Instance.getCurrentNight() != targetNight)
        {
            objectToDeactivate.SetActive(false);
            return;
        }

        // Trigger the fade sequence
        if (tutorialText != null)
        {
            tutorialText.DOFade(0f, fadeDuration)
                .SetDelay(displayDuration)
                .OnComplete(() => {
                    objectToDeactivate.SetActive(false);
                });
        }
        else
        {
            Debug.LogWarning($"Tutorial Text reference is missing on {gameObject.name}", this);
        }
    }
}